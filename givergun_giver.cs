using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BepInEx.Configuration;
using HarmonyLib;
using Photon.Realtime;
using SingularityGroup.HotReload;
using Unity.VisualScripting;
using UnityEngine;

namespace givegun;

[HarmonyPatch(typeof(RoundDirector))]
static class GiveGun_Giver
{
    private const string ITEM_NAME = "Item Gun Handgun";

    // Sets the purchase records of the item to the number passed into as argument
    private static void PurchaseItems(string item_name, int count)
    {
        Dictionary<string, int> purchased = StatsManager.instance.itemsPurchased;
        int item_count = purchased.GetValueOrDefault(item_name, 0);
        purchased[ITEM_NAME] = Math.Max(count, item_count);
    }

    private static void SetItemMax(string item_name, int count)
    {
        Item[] items = Resources.FindObjectsOfTypeAll<Item>();
        foreach(Item i in items)
        {
            if(i.name == ITEM_NAME)
            {
                i.maxAmount = Math.Max(i.maxAmount, count);
            }
        }
    }
    
    [HarmonyPostfix, HarmonyPatch(nameof(RoundDirector.StartRound))]
    private static void Patcher()
    {
        if (SemiFunc.RunIsLevel() && SemiFunc.IsMasterClientOrSingleplayer())
        {
            List<PlayerAvatar> players = SemiFunc.PlayerGetList();
            // Max with 1 accounts for singleplayer
            int player_count = Math.Max(1, players.Count);
            SetItemMax(ITEM_NAME, player_count);
            PurchaseItems(ITEM_NAME, player_count);
        }
    }

}