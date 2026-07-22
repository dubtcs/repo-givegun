using System.Collections.Generic;
using System.Linq;
using MenuLib;
using MenuLib.MonoBehaviors;
using UnityEngine;

namespace givegun;

static class GiveGun_UI
{
    static private void FetchItems()
    {
        if (GiveGun.mod_enabled.Value && SemiFunc.IsMasterClientOrSingleplayer())
        {
            if (GiveGun.loadout.Count == 0)
            {
                GiveGun.Logger.LogMessage("Logging items.");
                GiveGun.loadout = [];
                foreach (string s in StatsManager.instance.itemDictionary.Keys)
                {
                    GiveGun.loadout[s] = 0;
                }
            }
            // Updating to match current loadout in config
            string[] items = GiveGun.item_list.Value.Split(';');
            foreach (string s in items)
            {
                List<string> i = s.Split('#').ToList();
                string item_name = i[0].TrimStart().TrimEnd();
                int count = (i.Count > 1 && int.TryParse(i[1], out count)) ? count : -1;
                GiveGun.loadout[i[0]] = count;
            }
        }
    }

    static private void CreateItemEntries(REPOPopupPage menu)
    {
        menu.AddElementToScrollView(view => MenuAPI.CreateREPOSpacer(view, size: new Vector2(0, 20)).rectTransform);
        menu.AddElementToScrollView(view =>
        {
            REPOLabel label = MenuAPI.CreateREPOLabel("Set an item quantity to -1 to spawn one for each player.", view, Vector2.zero);
            label.labelTMP.fontSize = 12;
            return label.rectTransform;
        });
        menu.AddElementToScrollView(view => MenuAPI.CreateREPOSpacer(view, size: new Vector2(0, 20)).rectTransform);
        foreach (string s in StatsManager.instance.itemDictionary.Keys)
        {
            menu.AddElementToScrollView(view =>
            {
                REPOSlider slider = MenuAPI.CreateREPOSlider(s, string.Empty, nv =>
                {
                    int value = GiveGun.loadout[s];
                    if (int.TryParse(nv.ToString(), out value))
                    {
                        GiveGun.loadout[s] = value;
                    }
                }, view, default, -1, 20, GiveGun.loadout[s]);
                return slider.rectTransform;
            });
            menu.AddElementToScrollView(scrollView => MenuAPI.CreateREPOSpacer(scrollView, size: new Vector2(0, 10)).rectTransform);
        }
        menu.AddElementToScrollView(scrollView => MenuAPI.CreateREPOSpacer(scrollView, size: new Vector2(0, 20)).rectTransform);
    }

    static private void Loadout_OpenUI()
    {
        FetchItems();
        REPOPopupPage menu = MenuAPI.CreateREPOPopupPage("Loadout", REPOPopupPage.PresetSide.Right, false, false);
        menu.OpenPage(false);
        menu.onEscapePressed += () =>
        {
            string new_list = "";
            foreach (KeyValuePair<string, int> p in GiveGun.loadout)
            {
                if (p.Value != 0)
                {
                    string quant = p.Value > 0 ? $"#{p.Value}" : "";
                    new_list += $"{p.Key}{quant};";
                }
            }
            GiveGun.item_list.Value = new_list;
            return true;
        };
        CreateItemEntries(menu);
    }

    static public void CreateMenus()
    {
        MenuAPI.AddElementToEscapeMenu(parent => MenuAPI.CreateREPOButton("Loadout", Loadout_OpenUI, parent, new Vector2(350, 50)));
        MenuAPI.AddElementToLobbyMenu(parent => MenuAPI.CreateREPOButton("Loadout", Loadout_OpenUI, parent, new Vector2(350, 50)));
    }
}