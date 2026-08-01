using System.Collections.Generic;
using System.Linq;
using MenuLib;
using MenuLib.MonoBehaviors;
using UnityEngine;

namespace loadout;

static class LoadoutUI
{
    private static int selected_loadout = 0;

    static private void FetchItems()
    {
        if (Loadout.mod_enabled.Value && SemiFunc.IsMasterClientOrSingleplayer())
        {
            if (Loadout.loadout.Count == 0)
            {
                Loadout.Logger.LogMessage("Logging items.");
                Loadout.loadout = [];
                foreach (string s in StatsManager.instance.itemDictionary.Keys)
                {
                    Loadout.loadout[s] = 0;
                }
            }
            // Updating to match current loadout in config
            string[] items = Loadout.item_list.Value.Split(';');
            foreach (string s in items)
            {
                List<string> i = s.Split('#').ToList();
                string item_name = i[0].TrimStart().TrimEnd();
                int count = (i.Count > 1 && int.TryParse(i[1], out count)) ? count : -1;
                Loadout.loadout[i[0]] = count;
            }
        }
    }

    static private void CreateItemEntries(REPOPopupPage menu)
    {
        menu.AddElementToScrollView(view => MenuAPI.CreateREPOSpacer(view, size: new Vector2(0, 20)).rectTransform);
        menu.AddElementToScrollView(view =>
            {
                REPOSlider slider = MenuAPI.CreateREPOSlider("Preset", string.Empty, nv =>
                {
                    selected_loadout = nv;
                }, view, default, 1, Loadout.MAX_LOADOUTS, selected_loadout);
                return slider.rectTransform;
            });
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
                    int value = Loadout.loadout[s];
                    if (int.TryParse(nv.ToString(), out value))
                    {
                        Loadout.loadout[s] = value;
                    }
                }, view, default, -1, 20, Loadout.loadout[s]);
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
            foreach (KeyValuePair<string, int> p in Loadout.loadout)
            {
                if (p.Value != 0)
                {
                    string quant = p.Value > 0 ? $"#{p.Value}" : "";
                    new_list += $"{p.Key}{quant};";
                }
            }
            Loadout.item_list.Value = new_list;
            return true;
        };
        CreateItemEntries(menu);
        menu.AddElement(parent =>
        {
            MenuAPI.CreateREPOButton("SAVE PRESET", () => { }, parent, new Vector2(370f, 18f));
        });
        menu.AddElement(parent =>
        {
            MenuAPI.CreateREPOButton("LOAD PRESET", () => { }, parent, new Vector2(585f, 18f));
        });
    }

    static public void CreateMenus()
    {
        MenuAPI.AddElementToEscapeMenu(parent => MenuAPI.CreateREPOButton("Loadout", Loadout_OpenUI, parent, new Vector2(350, 50)));
        MenuAPI.AddElementToLobbyMenu(parent => MenuAPI.CreateREPOButton("Loadout", Loadout_OpenUI, parent, new Vector2(350, 50)));
    }
}