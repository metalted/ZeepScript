using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZeepSDK.Multiplayer;
using ZeepSDK.Racing;

namespace ZeepScript
{
    public static class Events
    {
        public static List<Action> SubscribedEvents = new List<Action>();

        public static void Unsubscribe()
        {
            foreach (Action unsubscribe in SubscribedEvents)
            {
                unsubscribe();
            }

            SubscribedEvents.Clear();
        }

        public static Action<int> LobbyTimerAction;

        public static readonly Dictionary<string, Action> Subs = new Dictionary<string, Action>
        {
            {
                "OnPlayerJoined", () =>
                {
                    PlayerJoinedDelegate playerJoinedDelegate = (player) =>
                    {
                        LuaManager.CallFunction("OnPlayerJoined", player);
                    };

                    MultiplayerApi.PlayerJoined += playerJoinedDelegate;
                    SubscribedEvents.Add(() => MultiplayerApi.PlayerJoined -= playerJoinedDelegate);
                }
            },
            {
                "OnPlayerLeft", () =>
                {
                    PlayerLeftDelegate playerLeftDelegate = (player) =>
                    {
                        LuaManager.CallFunction("OnPlayerLeft", player);
                    };

                    MultiplayerApi.PlayerLeft += playerLeftDelegate;
                    SubscribedEvents.Add(() => MultiplayerApi.PlayerLeft -= playerLeftDelegate);
                }
            },
            {
                "OnLevelLoaded", () =>
                {
                    LevelLoadedDelegate levelLoadedDelegate = () =>
                    {
                        LuaManager.CallFunction("OnLevelLoaded");
                    };

                    RacingApi.LevelLoaded += levelLoadedDelegate;
                    SubscribedEvents.Add(() => RacingApi.LevelLoaded -= levelLoadedDelegate);
                }
            },
            {
                "OnRoundStarted", () =>
                {
                    RoundStartedDelegate roundStartedDelegate = () =>
                    {
                        LuaManager.CallFunction("OnRoundStarted");
                    };

                    RacingApi.RoundStarted += roundStartedDelegate;
                    SubscribedEvents.Add(() => RacingApi.RoundStarted -= roundStartedDelegate);
                }
            },
            {
                "OnRoundEnded", () =>
                {
                    RoundStartedDelegate roundEndedDelegate = () =>
                    {
                        LuaManager.CallFunction("OnRoundEnded");
                    };

                    RacingApi.RoundEnded += roundEndedDelegate;
                    SubscribedEvents.Add(() => RacingApi.RoundEnded -= roundEndedDelegate);
                }
            },
            {
                "OnLobbyTimer", () =>
                {
                    Action<int> lobbyTimerAct = (time) =>
                    {
                        LuaManager.CallFunction("OnLobbyTimer", time);
                    };

                    LobbyTimerAction += lobbyTimerAct;
                    SubscribedEvents.Add(() => LobbyTimerAction -= lobbyTimerAct);
                }
            }
        };
    }

    [HarmonyPatch(typeof(OnlineGameplayUI), "Awake")]
    public class OnlineGameplayUIAwakePatch
    {
        public static void Postfix(OnlineGameplayUI __instance)
        {
            if(__instance.gameObject.GetComponent<TickerObserver>() == null)
            {
                __instance.gameObject.AddComponent<TickerObserver>();
            }
        }
    }

    public class TickerObserver : MonoBehaviour
    {
        private string timeString;
        private int time = 0;
        private OnlineGameplayUI onlineUI;

        public void Awake()
        {
            onlineUI = GetComponent<OnlineGameplayUI>();
        }

        public void Update()
        {
            if (onlineUI?.TimeLeftText != null)
            {
                string newTimeString = onlineUI.TimeLeftText.text;

                // Check if the time string has changed
                if (timeString != newTimeString)
                {
                    try
                    {
                        // Parse the time string into a TimeSpan
                        TimeSpan timeSpan;

                        if (TimeSpan.TryParseExact(newTimeString, new[] { @"hh\:mm\:ss", @"mm\:ss" }, null, out timeSpan))
                        {
                            // Convert the TimeSpan into total seconds
                            time = (int)timeSpan.TotalSeconds;

                            // Update the time string
                            timeString = newTimeString;

                            // Invoke the event with the new time
                            Events.LobbyTimerAction?.Invoke(time);
                        }
                        else
                        {
                            Debug.LogError($"Invalid time format: {newTimeString}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error parsing time string '{newTimeString}': {ex.Message}");
                    }
                }
            }
        }
    }
}
