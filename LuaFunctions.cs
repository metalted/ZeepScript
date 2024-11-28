using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;

namespace ZeepScript
{
    public static class LuaFunctions
    {
        public static Dictionary<string, Delegate> ZeepScript = new Dictionary<string, Delegate>
        {
            // Leaderboard Functions
            ["ListenTo"] = (Action<DynValue>)((eventNameArg) =>
            {
                string eventName = LuaUtils.ConvertParameter(eventNameArg, DataType.String, "eventName", v => v.String);
                if (eventName == default) return;

                Functions.ZeepScript_SubscribeTo(eventName);
            }),
            ["Log"] = (Action<DynValue>)((messageArg) =>
            {
                string message = LuaUtils.ConvertParameter(messageArg, DataType.String, "message", v => v.String);
                if (message == default) return;

                Functions.ZeepScript_Log(message);
            })
        };

        public static Dictionary<string, Delegate> Leaderboard = new Dictionary<string, Delegate>
        {
            // Leaderboard Functions
            ["ResetChampionshipPoints"] = (Action<DynValue>)((arg) =>
            {
                bool notify = LuaUtils.ConvertParameter(arg, DataType.Boolean, "notify", v => v.Boolean);
                if (notify == default) return;

                Functions.Leaderboard_ResetChampionshipPoints(notify);
            }),

            ["SetPlayerChampionshipPoints"] = (Action<DynValue, DynValue, DynValue, DynValue>)((steamIDArg, pointsArg, changeArg, notifyArg) =>
            {
                ulong steamID = LuaUtils.ConvertParameter(steamIDArg, DataType.Number, "steamID", v => (ulong)v.Number);
                int points = LuaUtils.ConvertParameter(pointsArg, DataType.Number, "points", v => (int)v.Number);
                int change = LuaUtils.ConvertParameter(changeArg, DataType.Number, "change", v => (int)v.Number);
                bool notify = LuaUtils.ConvertParameter(notifyArg, DataType.Boolean, "notify", v => v.Boolean);

                if (steamID == default || points == default || change == default || notify == default) return;

                Functions.Leaderboard_SetPlayerChampionshipPoints(steamID, points, change, notify);
            }),

            ["ResetPointsDistribution"] = (Action)(() =>
            {
                Functions.Leaderboard_ResetPointsDistribution();
            }),

            ["SetPointsDistribution"] = (Action<DynValue, DynValue, DynValue>)((valuesArg, baselineArg, dnfArg) =>
            {
                int[] values = LuaUtils.ConvertLuaTableToIntArray(valuesArg);
                int baseline = LuaUtils.ConvertParameter(baselineArg, DataType.Number, "baseline", v => (int)v.Number);
                int dnf = LuaUtils.ConvertParameter(dnfArg, DataType.Number, "dnf", v => (int)v.Number);

                if (values == null || baseline == default || dnf == default) return;

                Functions.Leaderboard_SetPointsDistribution(values, baseline, dnf);
            }),

            ["RemovePlayerFromLeaderboard"] = (Action<DynValue, DynValue>)((steamIDArg, notifyArg) =>
            {
                ulong steamID = LuaUtils.ConvertParameter(steamIDArg, DataType.Number, "steamID", v => (ulong)v.Number);
                bool notify = LuaUtils.ConvertParameter(notifyArg, DataType.Boolean, "notify", v => v.Boolean);

                if (steamID == default || notify == default) return;

                Functions.Leaderboard_RemovePlayerFromLeaderboard(steamID, notify);
            }),

            ["SetPlayerLeaderboardOverrides"] = (Action<DynValue, DynValue, DynValue, DynValue, DynValue, DynValue>)((steamIDArg, timeArg, nameArg, positionArg, pointsArg, pointsWonArg) =>
            {
                ulong steamID = LuaUtils.ConvertParameter(steamIDArg, DataType.Number, "steamID", v => (ulong)v.Number);
                string time = LuaUtils.ConvertParameter(timeArg, DataType.String, "time", v => v.String);
                string name = LuaUtils.ConvertParameter(nameArg, DataType.String, "name", v => v.String);
                string position = LuaUtils.ConvertParameter(positionArg, DataType.String, "position", v => v.String);
                string points = LuaUtils.ConvertParameter(pointsArg, DataType.String, "points", v => v.String);
                string pointsWon = LuaUtils.ConvertParameter(pointsWonArg, DataType.String, "pointsWon", v => v.String);

                if (steamID == default || string.IsNullOrEmpty(time) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(position) || string.IsNullOrEmpty(points) || string.IsNullOrEmpty(pointsWon)) return;

                Functions.Leaderboard_SetPlayerLeaderboardOverrides(steamID, time, name, position, points, pointsWon);
            }),

            ["SetPlayerTimeOnLeaderboard"] = (Action<DynValue, DynValue, DynValue>)((steamIDArg, timeArg, notifyArg) =>
            {
                ulong steamID = LuaUtils.ConvertParameter(steamIDArg, DataType.Number, "steamID", v => (ulong)v.Number);
                float time = LuaUtils.ConvertParameter(timeArg, DataType.Number, "time", v => (float)v.Number);
                bool notify = LuaUtils.ConvertParameter(notifyArg, DataType.Boolean, "notify", v => v.Boolean);

                if (steamID == default || time == default || notify == default) return;

                Functions.Leaderboard_SetPlayerTimeOnLeaderboard(steamID, time, notify);
            }),

            ["SetSmallLeaderboardSortingMethod"] = (Action<DynValue>)((useChampionshipPointsSortingArg) =>
            {
                bool useChampionshipPointsSorting = LuaUtils.ConvertParameter(useChampionshipPointsSortingArg, DataType.Boolean, "useChampionshipPointsSorting", v => v.Boolean);
                if (useChampionshipPointsSorting == default) return;

                Functions.Leaderboard_SetSmallLeaderboardSortingMethod(useChampionshipPointsSorting);
            }),

            ["BlockPlayerFromSettingTime"] = (Action<DynValue, DynValue>)((steamIDArg, notifyArg) =>
            {
                ulong steamID = LuaUtils.ConvertParameter(steamIDArg, DataType.Number, "steamID", v => (ulong)v.Number);
                bool notify = LuaUtils.ConvertParameter(notifyArg, DataType.Boolean, "notify", v => v.Boolean);

                if (steamID == default || notify == default) return;

                Functions.Leaderboard_BlockPlayerFromSettingTime(steamID, notify);
            }),

            ["UnblockPlayerFromSettingTime"] = (Action<DynValue, DynValue>)((steamIDArg, notifyArg) =>
            {
                ulong steamID = LuaUtils.ConvertParameter(steamIDArg, DataType.Number, "steamID", v => (ulong)v.Number);
                bool notify = LuaUtils.ConvertParameter(notifyArg, DataType.Boolean, "notify", v => v.Boolean);

                if (steamID == default || notify == default) return;

                Functions.Leaderboard_UnblockPlayerFromSettingTime(steamID, notify);
            }),

            ["BlockEveryoneFromSettingTime"] = (Action<DynValue>)((notifyArg) =>
            {
                bool notify = LuaUtils.ConvertParameter(notifyArg, DataType.Boolean, "notify", v => v.Boolean);
                if (notify == default) return;

                Functions.Leaderboard_BlockEveryoneFromSettingTime(notify);
            }),

            ["UnblockEveryoneFromSettingTime"] = (Action<DynValue>)((notifyArg) =>
            {
                bool notify = LuaUtils.ConvertParameter(notifyArg, DataType.Boolean, "notify", v => v.Boolean);
                if (notify == default) return;

                Functions.Leaderboard_UnblockEveryoneFromSettingTime(notify);
            })           
        };

        public static Dictionary<string, Delegate> Lobby = new Dictionary<string, Delegate>
        {
            ["SetRoundLength"] = (Action<DynValue>)((roundTimeArg) =>
            {
                int roundTime = LuaUtils.ConvertParameter(roundTimeArg, DataType.Number, "roundTime", v => (int)v.Number);
                if (roundTime == default) return;

                Functions.Lobby_SetRoundLength(roundTime);
            }),

            ["SetVoteskip"] = (Action<DynValue>)((enabledArg) =>
            {
                bool enabled = LuaUtils.ConvertParameter(enabledArg, DataType.Boolean, "enabled", v => v.Boolean);
                if (enabled == default) return;

                Functions.Lobby_SetVoteskip(enabled);
            }),

            ["SetVoteskipPercentage"] = (Action<DynValue>)((percentageArg) =>
            {
                int percentage = LuaUtils.ConvertParameter(percentageArg, DataType.Number, "percentage", v => (int)v.Number);
                if (percentage == default) return;

                Functions.Lobby_SetVoteskipPercentage(percentage);
            }),

            ["SetLobbyName"] = (Action<DynValue>)((nameArg) =>
            {
                string name = LuaUtils.ConvertParameter(nameArg, DataType.String, "name", v => v.String);
                if (string.IsNullOrEmpty(name)) return;

                Functions.Lobby_SetLobbyName(name);
            }),

            ["SetServerMessage"] = (Action<DynValue, DynValue>)((messageArg, timeArg) =>
            {
                string message = LuaUtils.ConvertParameter(messageArg, DataType.String, "message", v => v.String);
                float time = LuaUtils.ConvertParameter(timeArg, DataType.Number, "time", v => (float)v.Number);

                if (string.IsNullOrEmpty(message) || time == default) return;

                Functions.Lobby_SetServerMessage(message, time);
            }),

            ["RemoveServerMessage"] = (Action)(() =>
            {
                Functions.Lobby_RemoveServerMessage();
            }),

            ["GetPlayerCount"] = (Func<DynValue>)(() =>
            {
                int count = Functions.Lobby_GetPlayerCount();
                return DynValue.NewNumber(count);
            }),

            ["GetPlaylistIndex"] = (Func<DynValue>)(() =>
            {
                int index = Functions.Lobby_GetPlaylistIndex();
                return DynValue.NewNumber(index);
            }),

            ["GetPlaylistLength"] = (Func<DynValue>)(() =>
            {
                int length = Functions.Lobby_GetPlaylistLength();
                return DynValue.NewNumber(length);
            }),

            ["GetLevel"] = (Func<DynValue>)(() =>
            {
                // Get the level object
                LevelScriptableObject level = Functions.Lobby_GetLevel();

                if (level == null)
                {
                    // Return nil if no level is found
                    return DynValue.NewNil();
                }

                // Wrap the LevelScriptableObject as UserData
                return UserData.Create(level);
            }),
        };

        public static Dictionary<string, Delegate> Messaging = new Dictionary<string, Delegate>
        {
            ["ShowScreenMessage"] = (Action<DynValue, DynValue>)((messageArg, timeArg) =>
            {
                string message = LuaUtils.ConvertParameter(messageArg, DataType.String, "message", v => v.String);
                float time = LuaUtils.ConvertParameter(timeArg, DataType.Number, "time", v => (float)v.Number);

                if (string.IsNullOrEmpty(message) || time == default) return;

                Functions.Messaging_ShowFrogMessage(message, time);
            }),

            ["SendChatMessage"] = (Action<DynValue, DynValue>)((prefixArg, messageArg) =>
            {
                string prefix = LuaUtils.ConvertParameter(prefixArg, DataType.String, "prefix", v => v.String);
                string message = LuaUtils.ConvertParameter(messageArg, DataType.String, "message", v => v.String);

                if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(message)) return;

                Functions.Messaging_SendChatMessage(prefix, message);
            }),

            ["SendPrivateChatMessage"] = (Action<DynValue, DynValue, DynValue>)((steamIDArg, prefixArg, messageArg) =>
            {
                ulong steamID = LuaUtils.ConvertParameter(steamIDArg, DataType.Number, "steamID", v => (ulong)v.Number);
                string prefix = LuaUtils.ConvertParameter(prefixArg, DataType.String, "prefix", v => v.String);
                string message = LuaUtils.ConvertParameter(messageArg, DataType.String, "message", v => v.String);

                if (steamID == default || string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(message)) return;

                Functions.Messaging_SendPrivateChatMessage(steamID, prefix, message);
            })
        };
    }
}
