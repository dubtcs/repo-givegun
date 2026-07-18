using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BepInEx.Configuration;
using ExitGames.Client.Photon.StructWrapping;
using HarmonyLib;
using Photon.Realtime;
using SingularityGroup.HotReload;
using Unity.VisualScripting;
using UnityEngine;

namespace givegun;

[HarmonyPatch(typeof(RoundDirector))]
static class GiveGun_Giver
{
    // private const string ITEM_NAME = "Item Gun Handgun";

    // Sets the purchase records of the item to the number passed into as argument
    private static void PurchaseItems(string item_name, int count)
    {
        Dictionary<string, int> purchased = StatsManager.instance.itemsPurchased;
        int item_count = purchased.GetValueOrDefault(item_name, 0);
        purchased[item_name] = Math.Max(count, item_count);
    }

    private static void SetItemMax(string item_name)
    {
        if(GiveGun.item_count.Value == 0)
            return;
        Item[] items = Resources.FindObjectsOfTypeAll<Item>();
        foreach(Item i in items)
        {
            if(i.name == item_name)
            {
                i.maxAmount = Math.Max(i.maxAmount, GiveGun.item_count.Value);
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
            List<string> items = GiveGun.item_list.Value.Split(';').ToList();
            foreach(string s in items)
            {
                List<string> i = s.Split('#').ToList();
                string item_name = i[0].TrimStart().TrimEnd();
                int count = (i.Count > 1 && int.TryParse(i[1], out count)) ? count : player_count;
                GiveGun.Logger.LogMessage($"Giving {count} {item_name}.");
                SetItemMax(item_name);
                PurchaseItems(item_name, count);
            }
        }
    }

}