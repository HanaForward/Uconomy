using fr34kyn01535.Uconomy.Model;
using SDG.Unturned;
using Steamworks;
using System;

namespace fr34kyn01535.Uconomy
{
    public class DatabaseManager
    {
        internal DatabaseManager()
        {
            CheckSchema();
        }

        private void CheckSchema()
        {


            if (PlayerLibrary.PlayerLibrary.CheckTable("Uconomys"))
            {
                PlayerLibrary.PlayerLibrary.CreateTables("CREATE TABLE `Uconomys` ( `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, `player` int(10) UNSIGNED NOT NULL, `balance` decimal(15, 2) UNSIGNED DEFAULT NULL, PRIMARY KEY (`id`), KEY `uconomys_player` USING HASH (`player`), CONSTRAINT `uconomys_player` FOREIGN KEY (`player`) REFERENCES `Players` (`id`) ON DELETE CASCADE ON UPDATE CASCADE ) ENGINE = InnoDB CHARSET = utf8;");
            }

            /*
            object parameters = Uconomy.Db.Ado.GetScalar("show tables like 'Uconomys';");
            if (parameters == null)
            {
                Uconomy.Db.Ado.GetScalar("CREATE TABLE `Uconomys` ( `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, `player` int(10) UNSIGNED NOT NULL, `balance` decimal(15, 2) UNSIGNED DEFAULT NULL, PRIMARY KEY (`id`), KEY `uconomys_player` USING HASH (`player`), CONSTRAINT `uconomys_player` FOREIGN KEY (`player`) REFERENCES `Players` (`id`) ON DELETE CASCADE ON UPDATE CASCADE ) ENGINE = InnoDB CHARSET = utf8;");
            }
            */
        }

        /// <summary>
        /// Retrieves the current balance of a specific account.
        /// </summary>
        /// <param name="id">The Steam 64 ID of the account to retrieve the balance from.</param>
        /// <returns>The balance of the account.</returns>
        public decimal GetBalance(string id)
        {
            ulong steamid = Convert.ToUInt64(id);
            decimal output = 0;
            if (Uconomy.Cache.ContainsKey(steamid))
            {
                Uconomys uconomys = Uconomy.Db.Queryable<Uconomys>().InSingle(Uconomy.Cache[steamid]);
                Uconomy.Instance.OnBalanceChecked(id, output);
                output = uconomys.balance;
            }
            return output;
        }


        /// <summary>
        /// Increases the account balance of the specific ID with IncreaseBy.
        /// </summary>
        /// <param name="id">Steam 64 ID of the account.</param>
        /// <param name="increaseBy">The amount that the account should be changed with (can be negative).</param>
        /// <returns>The new balance of the account.</returns>
        public decimal IncreaseBalance(string id, decimal increaseBy)
        {
            ulong steamid = Convert.ToUInt64(id);
            uint Single = Uconomy.Instance.GetSingle(steamid);
            Uconomys uconomys = Uconomy.Db.Queryable<Uconomys>().InSingle(Single);
            uconomys.balance += increaseBy;
            Uconomy.Db.Updateable(uconomys).ExecuteCommand();
            Uconomy.Instance.BalanceUpdated(id, increaseBy);
            EffectManager.sendUIEffect(43005, 1, (CSteamID)steamid, true, uconomys.balance.ToString());
            return uconomys.balance;
        }
    }
}