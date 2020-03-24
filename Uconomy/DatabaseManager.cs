using fr34kyn01535.Uconomy.Model;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;

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
            if (PlayerLibrary.PlayerLibrary.CheckTable(Uconomy.Instance.Configuration.Instance.TableName))
            {
                PlayerLibrary.PlayerLibrary.CreateTables("CREATE TABLE `" + Uconomy.Instance.Configuration.Instance.TableName + "` ( `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, `player` int(10) UNSIGNED NOT NULL, `balance` decimal(15, 2) UNSIGNED DEFAULT NULL, PRIMARY KEY (`id`), KEY `uconomys_player` USING HASH (`player`), CONSTRAINT `uconomys_player` FOREIGN KEY (`player`) REFERENCES `Players` (`id`) ON DELETE CASCADE ON UPDATE CASCADE ) ENGINE = InnoDB CHARSET = utf8;");
            }
        }

        /// <summary>
        /// Retrieves the current balance of a specific account.
        /// </summary>
        /// <param name="id">The Steam 64 ID of the account to retrieve the balance from.</param>
        /// <returns>The balance of the account.</returns>

        public decimal GetBalance(string id)
        {
            ulong steamid = Convert.ToUInt64(id);
            return GetBalance(steamid);
        }

        public decimal GetBalance(ulong steamid)
        {
            uint Single = Uconomy.Instance.GetSingle(steamid);
            Uconomys uconomys = Uconomy.Db.Queryable<Uconomys>().InSingle(Single);
            Uconomy.Instance.OnBalanceChecked(steamid.ToString(), uconomys.balance);
            return uconomys.balance;
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
            return IncreaseBalance(steamid, increaseBy);
        }


        public decimal IncreaseBalance(uint Player_Id, decimal increaseBy)
        {
            Uconomys uconomys = Uconomy.Db.Queryable<Uconomys>().Where(it => it.player == Player_Id).First();
            decimal old_balance = uconomys.balance;
            if (increaseBy < 0)
                uconomys.balance = uconomys.balance <= increaseBy ? 0 : uconomys.balance - increaseBy;
            else
                uconomys.balance += increaseBy;
            if (old_balance == uconomys.balance)
                return uconomys.balance;
            Uconomy.Db.Updateable(uconomys).ExecuteCommand();
            return uconomys.balance;
        }


        public decimal IncreaseBalance(ulong steamid, decimal increaseBy)
        {
            uint Single = Uconomy.Instance.GetSingle(steamid);
            Uconomys uconomys = Uconomy.Db.Queryable<Uconomys>().InSingle(Single);
            uconomys.balance += increaseBy;
            Uconomy.Db.Updateable(uconomys).ExecuteCommand();
            Uconomy.Instance.BalanceUpdated(steamid.ToString(), increaseBy);
            return uconomys.balance;
        }

        public MySqlConnection CreateConnection()
        {
            MySqlConnection mySqlConnection = null;
            try
            {
                if (Uconomy.Instance.Configuration.Instance.DatabasePort == 0)
                {
                    Uconomy.Instance.Configuration.Instance.DatabasePort = 3306;
                }
                mySqlConnection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};Allow User Variables=True;", Uconomy.Instance.Configuration.Instance.DatabaseAddress, Uconomy.Instance.Configuration.Instance.DatabaseName, Uconomy.Instance.Configuration.Instance.DatabaseUsername, Uconomy.Instance.Configuration.Instance.DatabasePassword, Uconomy.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception exception)
            {
                Logger.LogError(exception.Message);
            }
            return mySqlConnection;
        }


        public ushort GetRank(ulong Steamid)
        {
            MySqlConnection mySqlConnection = CreateConnection();
            MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            uint user_id = PlayerLibrary.PlayerLibrary.GetPlayerIndexByCSteam(Steamid);
#if DEBUG
            Logger.Log("user_id : " + user_id);
#endif
            mySqlCommand.CommandText = string.Concat("SET @rank :=0;SELECT rank FROM ( SELECT *,(@rank:=@rank + 1) AS rank FROM " + Uconomy.Instance.Configuration.Instance.TableName + " ORDER BY balance DESC LIMIT 999 ) AS Rank WHERE Rank.player = " + user_id + ";");
            mySqlConnection.Open();
            object obj = mySqlCommand.ExecuteScalar();

            mySqlConnection.Close();
            if (obj == null)
            {
                return 999;
            }
            else
            {
                return Convert.ToUInt16(obj);
            }
        }
    }
}