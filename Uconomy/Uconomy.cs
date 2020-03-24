using fr34kyn01535.Uconomy.Model;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SqlSugar;
using Steamworks;
using System;
using System.Collections.Generic;

namespace fr34kyn01535.Uconomy
{
    public class Uconomy : RocketPlugin<UconomyConfiguration>
    {
        public DatabaseManager Database;
        public static Uconomy Instance;

        public static SqlSugarClient Db;

        public static Dictionary<ulong, uint> Cache = new Dictionary<ulong, uint>();

        public static string MessageColor;

        protected override void Load()
        {
            Instance = this;
            Db = PlayerLibrary.DbMySQL.Db;
            Database = new DatabaseManager();

            Configuration.Instance.DatabaseAddress = PlayerLibrary.PlayerLibrary.Instance.Configuration.Instance.DatabaseAddress;
            Configuration.Instance.DatabaseName = PlayerLibrary.PlayerLibrary.Instance.Configuration.Instance.DatabaseName;
            Configuration.Instance.DatabaseUsername = PlayerLibrary.PlayerLibrary.Instance.Configuration.Instance.DatabaseUsername;
            Configuration.Instance.DatabasePassword = PlayerLibrary.PlayerLibrary.Instance.Configuration.Instance.DatabasePassword;
            Configuration.Instance.DatabasePort = PlayerLibrary.PlayerLibrary.Instance.Configuration.Instance.DatabasePort;

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            MessageColor = Configuration.Instance.MessageColor;
        }



        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (Cache.ContainsKey(player.CSteamID.m_SteamID))
            {
                Cache.Remove(player.CSteamID.m_SteamID);
            }
        }

        public delegate void PlayerBalanceUpdate(UnturnedPlayer player, decimal amt);

        public event PlayerBalanceUpdate OnBalanceUpdate;

        public delegate void PlayerBalanceCheck(UnturnedPlayer player, decimal balance);

        public event PlayerBalanceCheck OnBalanceCheck;

        public delegate void PlayerPay(UnturnedPlayer sender, string receiver, decimal amt);

        public event PlayerPay OnPlayerPay;

        public override TranslationList DefaultTranslations =>
            new TranslationList
            {
                {"command_balance_show", "你当前余额为: {0} {1} {2}"},
                {"command_balance_error_player_not_found", "找不到玩家!"},
                {"command_balance_check_noPermissions", "权限不够!"},
                {"command_balance_show_otherPlayer", "{0} 的当前余额为: {0} {1} {2}"},
                {"command_pay_invalid", "无效参数"},
                {"command_pay_error_pay_self", "你不能付钱给自己"},
                {"command_pay_error_invalid_amount", "无效的金额"},
                {"command_pay_error_cant_afford", "没有足够的余额"},
                {"command_pay_error_player_not_found", "找不到这个人"},
                {"command_pay_private", "付款了 {0} {1} {2}"},
                {"command_pay_console", "你收到付款 {0} {1} "},
                {"command_pay_other_private", "你收到付款 {0} {1} 来自 {2}"}
            };

        internal void HasBeenPayed(UnturnedPlayer sender, string receiver, decimal amt)
        {
            OnPlayerPay?.Invoke(sender, receiver, amt);
        }

        internal void BalanceUpdated(string steamId, decimal amt)
        {
            if (OnBalanceUpdate == null) return;
            var player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(steamId)));
            OnBalanceUpdate(player, amt);
        }

        internal void OnBalanceChecked(string steamId, decimal balance)
        {
            if (OnBalanceCheck == null) return;
            var player = UnturnedPlayer.FromCSteamID(new CSteamID(Convert.ToUInt64(steamId)));
            OnBalanceCheck(player, balance);
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            uint user_id = PlayerLibrary.PlayerLibrary.GetPlayerIndexByCSteam(player.CSteamID.m_SteamID);
            Uconomys uconomys = Db.Queryable<Uconomys>().Where(it => it.player == user_id).First();
            if (uconomys == null)
            {
                uconomys = new Uconomys(user_id);
                uconomys = Db.Insertable(uconomys).ExecuteReturnEntity();
            }
            Cache.Add(player.CSteamID.m_SteamID, uconomys.player);
        }
        public uint GetSingle(ulong steamid)
        {
            if (Cache.TryGetValue(steamid, out uint single))
            {
                return single;
            }
            else
            {
                uint user_id = PlayerLibrary.PlayerLibrary.GetPlayerIndexByCSteam(steamid);
                Uconomys uconomys = Db.Queryable<Uconomys>().Where(it => it.player == user_id).First();
                if (uconomys != null)
                {
                    Cache.Add(steamid, uconomys.player);
                    return uconomys.player;
                }
                else
                {
                    uconomys = new Uconomys(user_id);
                    uconomys = Db.Insertable(uconomys).ExecuteReturnEntity();
                    Cache.Add(steamid, uconomys.player);
                    return uconomys.player;
                }
            }
        }
    }
}