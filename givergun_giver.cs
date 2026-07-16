using System;
using System.Collections.Generic;
using HarmonyLib;
using SingularityGroup.HotReload;
using UnityEngine;

namespace givegun;

[HarmonyPatch(typeof(RoundDirector))]
static class GiveGun_Giver
{
    private const string ITEM_NAME = "Item Gun Handgun";

    [HarmonyPrefix, HarmonyPatch(nameof(RoundDirector.StartRound))]
    private static void Start_Prefix()
    {
        if (!SemiFunc.IsMainMenu() && SemiFunc.IsMasterClientOrSingleplayer())
        {
            Dictionary<string, int> purchased = StatsManager.instance.itemsPurchased;
            int player_count = Math.Max(1, SemiFunc.PlayerGetList().Count);
            int item_count = purchased.GetValueOrDefault(ITEM_NAME, 0);
            purchased[ITEM_NAME] = Math.Max(player_count, item_count);
            if(player_count < item_count)
            {
                int dif = player_count - item_count;
                GiveGun.Logger.LogInfo($"Adding {dif} guns.");
            }
        }
    }
}