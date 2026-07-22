using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace givegun;

[HarmonyPatch(typeof(RunManager))]
static class GiveGun_Giver
{
    private const int ITEM_COUNT = 30;
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
            // Do this regardless to make sure no existing loadouts are messed up
            foreach (KeyValuePair<string, Item> p in StatsManager.instance.itemDictionary)
            {
                SetItemMax(p.Key, ITEM_COUNT);
            }
            if (SemiFunc.RunIsLevel() && RunManager.instance.loadLevel == 0)
            {
                List<PlayerAvatar> players = SemiFunc.PlayerGetList();
                int player_count = Math.Max(1, players.Count); // Use max and 1 here to cover offline SP bc idk if GetPlayerList() works there
                foreach (KeyValuePair<string, int> p in GiveGun.loadout)
                {
                    if (p.Value != 0)
                    {
                        int count = p.Value > 0 ? p.Value : player_count;
                        PurchaseItems(p.Key, count);
                    }
                }
            }
        }
    }
}