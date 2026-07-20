using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace givegun;

[HarmonyPatch(typeof(RunManager))]
static class GiveGun_Giver
{
    private static void SetItemMax(string item_name, int count)
    {
        Item[] items = Resources.FindObjectsOfTypeAll<Item>();
        foreach (Item i in items)
        {
            if (i.name == item_name)
            {
                i.maxAmount = Math.Max(i.maxAmount, count);
            }
        }
    }

    private static void PurchaseItems(string item_name, int count)
    {
        Dictionary<string, int> purchased = StatsManager.instance.itemsPurchased;
        int item_count = purchased.GetValueOrDefault(item_name, 0);
        purchased[item_name] = Math.Max(count, item_count);
    }

    [HarmonyPostfix, HarmonyPatch(nameof(RunManager.ChangeLevel))]
    private static void Patch()
    {
        if (GiveGun.mod_enabled.Value && SemiFunc.IsMasterClientOrSingleplayer())
        {
            if (SemiFunc.RunIsLevel() && RunManager.instance.loadLevel == 0)
            {
                GiveGun.Logger.LogMessage($"First round, supplying items.");
                List<PlayerAvatar> players = SemiFunc.PlayerGetList();
                int player_count = Math.Max(1, players.Count); // Use max and 1 here to cover offline SP bc idk if GetPlayerList() works there
                List<string> items = GiveGun.item_list.Value.Split(';').ToList();
                foreach (string s in items)
                {
                    List<string> i = s.Split('#').ToList();
                    string item_name = i[0].TrimStart().TrimEnd();
                    int count = (i.Count > 1 && int.TryParse(i[1], out count)) ? count : player_count;
                    GiveGun.Logger.LogMessage($"Giving {count} {item_name}.");
                    SetItemMax(item_name, count);
                    PurchaseItems(item_name, count);
                }
            }
        }
    }
}