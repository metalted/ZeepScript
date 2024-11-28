using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeepkistClient;
using ZeepSDK.Chat;
using ZeepSDK.Multiplayer;

namespace ZeepScript
{
    public static class Functions
    {
        public static void ZeepScript_SubscribeTo(string eventName)
        {
            if(Events.Subs.TryGetValue(eventName, out Action subscribe))
            {
                subscribe();
            }
            else
            {
                Plugin.Instance.Log($"Event '{eventName}' is not recognized or supported.");
            }            
        }

        public static void ZeepScript_Log(string message)
        {
            Plugin.Instance.Log(message, true);
        }

        public static bool IsOnlineHost()
        {
            return ZeepkistNetwork.IsConnectedToGame && ZeepkistNetwork.IsMasterClient;
        }

        //Leaderboard functions
        public static void Leaderboard_ResetChampionshipPoints(bool notify)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.ResetChampionshipPoints(notify);
        }

        public static void Leaderboard_SetPlayerChampionshipPoints(ulong steamID, int points, int change, bool notify)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_SetPlayerChampionshipPoints(steamID, points, change, notify);
        }

        public static void Leaderboard_ResetPointsDistribution()
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_ResetPointsDistribution();
        }

        public static void Leaderboard_SetPointsDistribution(int[] values, int baseline, int dnf)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_SetPointsDistribution(values.ToList(), baseline, dnf);
        }

        public static void Leaderboard_RemovePlayerFromLeaderboard(ulong steamID, bool notify)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_RemovePlayerFromLeaderboard(steamID, notify);
        }

        public static void Leaderboard_SetPlayerLeaderboardOverrides(ulong steamID, string time, string name, string position, string points, string pointsWon)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_SetPlayerLeaderboardOverrides(steamID, time, name, position, points, pointsWon);
        }

        public static void Leaderboard_SetPlayerTimeOnLeaderboard(ulong steamID, float time, bool notify)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_SetPlayerTimeOnLeaderboard(steamID, time, notify);
        }

        public static void Leaderboard_SetSmallLeaderboardSortingMethod(bool useChampionshipPointsSorting)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_SetSmallLeaderboardSortingMethod(useChampionshipPointsSorting);
        }

        public static void Leaderboard_BlockPlayerFromSettingTime(ulong steamID, bool notify)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_BlockPlayerFromSettingTime(steamID, notify);
        }

        public static void Leaderboard_UnblockPlayerFromSettingTime(ulong steamID, bool notify)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_UnblockPlayerFromSettingTime(steamID, notify);
        }

        public static void Leaderboard_BlockEveryoneFromSettingTime(bool notify)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_BlockEveryoneFromSettingTime(notify);
        }

        public static void Leaderboard_UnblockEveryoneFromSettingTime(bool notify)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.CustomLeaderBoard_UnblockEveryoneFromSettingTime(notify);
        }


        //Lobby functions
        public static void Lobby_SetRoundLength(int roundTime)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            roundTime = Math.Max(30, roundTime);

            ChatApi.SendMessage("/settime " + roundTime.ToString());
        }

        public static void Lobby_SetVoteskip(bool enabled)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ChatApi.SendMessage($"/vs {(enabled ? "on" : "off")}");
        }

        public static void Lobby_SetVoteskipPercentage(int percentage)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            Math.Min(100, Math.Max(1, percentage));

            ChatApi.SendMessage($"/vs % {percentage.ToString()}");
        }

        public static void Lobby_SetLobbyName(string name)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            if (string.IsNullOrEmpty(name.Trim()))
            {
                return;
            }

            ZeepkistLobby currentLobby = ZeepkistNetwork.CurrentLobby;
            if (currentLobby != null)
            {
                currentLobby.UpdateName(name);
            }
        }

        public static void Lobby_SetServerMessage(string message, float time)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            if (string.IsNullOrEmpty(message.Trim()))
            {
                return;
            }

            ChatApi.SendMessage($"/servermessage white {time.ToString()} {message}");
        }

        public static void Lobby_RemoveServerMessage()
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ChatApi.SendMessage("/servermessage remove");
        }

        public static int Lobby_GetPlayerCount()
        {
            if (!IsOnlineHost())
            {
                return -1;
            }

            return ZeepkistNetwork.CurrentLobby?.PlayerCount ?? -1;
        }

        public static int Lobby_GetPlaylistIndex()
        {
            if (!IsOnlineHost())
            {
                return -1;
            }

            return ZeepkistNetwork.CurrentLobby?.CurrentPlaylistIndex ?? -1;
        }

        public static int Lobby_GetPlaylistLength()
        {
            if (!IsOnlineHost())
            {
                return -1;
            }

            return ZeepkistNetwork.CurrentLobby?.Playlist.Count ?? -1;
        }

        public static LevelScriptableObject Lobby_GetLevel()
        {
            if (!IsOnlineHost())
            {
                return null;
            }

            return ZeepSDK.Level.LevelApi.CurrentLevel;
        }


        //Messaging functions
        public static void Messaging_ShowFrogMessage(string message, float time)
        {
            PlayerManager.Instance.messenger.Log(message, time);
        }

        public static void Messaging_SendChatMessage(string prefix, string message)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.SendCustomChatMessage(true, 0, message, prefix);
        }

        public static void Messaging_SendPrivateChatMessage(ulong steamID, string prefix, string message)
        {
            if (!IsOnlineHost())
            {
                return;
            }

            ZeepkistNetwork.SendCustomChatMessage(false, steamID, message, prefix);
        }
    }
}
