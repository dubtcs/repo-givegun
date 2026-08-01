using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace loadout;

[BepInPlugin("shaboingboings.loadout", "Loadout", "1.0")]
public class Loadout : BaseUnityPlugin
{
    internal static Loadout Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    public static ConfigEntry<bool> mod_enabled;
    public static ConfigEntry<string> item_list;
    public static ConfigEntry<string> presets;

    public const int MAX_LOADOUTS = 4;
    public static List<string> loadouts = [];

    static public Dictionary<string, int> loadout = [];

    private static void ParsePresets()
    {
        Logger.LogMessage($"Parsing presets string: {presets.Value}");
        List<string> preset_strings = presets.Value.Split("|").ToList();
        for (int i = 0; i < MAX_LOADOUTS; i++)
        {
            if (i >= preset_strings.Count)
            {
                loadouts.Add(string.Empty);
            }
            else
            {
                Logger.LogInfo($"Adding loadout {i}: {preset_strings[i]}");
                loadouts.Add(preset_strings[i]);
            }
        }
    }

    public static void SavePresets()
    {
        string save = "";
        foreach (string s in loadouts)
        {
            save += $"{s}|";
        }
        presets.Value = save;
    }

    public static string GetLoadoutString()
    {
        string new_list = "";
        foreach (KeyValuePair<string, int> p in loadout)
        {
            if (p.Value != 0)
            {
                string quant = p.Value > 0 ? $"#{p.Value}" : "";
                new_list += $"{p.Key}{quant};";
            }
        }
        return new_list;
    }

    public static void SetLoadoutFromString(string l, bool use_stats = false)
    {
        // ONLY USE THIS IF IN A GAME INSTANCE
        if (use_stats)
        {
            if (!StatsManager.instance.IsUnityNull())
            {
                foreach (string s in StatsManager.instance.itemDictionary.Keys)
                {
                    loadout[s] = 0;
                }
            }
            else
            {
                Logger.LogWarning("You just tried to set loadout using the StatsManager, but it isn't available yet. Wait until you're either in a lobby or game.");
            }
        }
        string[] items = l.Split(';');
        foreach (string s in items)
        {
            List<string> i = s.Split('#').ToList();
            string item_name = i[0].TrimStart().TrimEnd();
            int count = (i.Count > 1 && int.TryParse(i[1], out count)) ? count : -1;
            loadout[item_name] = count;
        }
    }

    private void Awake()
    {
        Instance = this;
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        mod_enabled = Config.Bind("General", "Enabled", true, "Enable or disable the mod.");
        item_list = Config.Bind("Items", "ItemList", "Item Gun Handgun", "A list of items you want to start with separated by a semicolon. Use a hash to denote quantity. Items without quantity will be spawned for each player.\nDefault game item list:\nItem Cart Medium\nItem Cart Small\nItem Drone Battery\nItem Drone Feather\nItem Drone Indestructible\nItem Drone Torque\nItem Drone Zero Gravity\nItem Extraction Tracker\nItem Grenade Duct Taped\nItem Grenade Explosive\nItem Grenade Human\nItem Grenade Shockwave\nItem Grenade Stun\nItem Gun Handgun\nItem Gun Shotgun\nItem Gun Tranq\nItem Health Pack Large\nItem Health Pack Medium\nItem Health Pack Small\nItem Melee Baseball Bat\nItem Melee Frying Pan\nItem Melee Inflatable Hammer\nItem Melee Sledge Hammer\nItem Melee Sword\nItem Mine Explosive\nItem Mine Shockwave\nItem Mine Stun\nItem Orb Zero Gravity\nItem Power Crystal\nItem Rubber Duck\nItem Upgrade Map Player Count\nItem Upgrade Player Energy\nItem Upgrade Player Extra Jump\nItem Upgrade Player Grab Range\nItem Upgrade Player Grab Strength\nItem Upgrade Player Health\nItem Upgrade Player Sprint Speed\nItem Upgrade Player Tumble Launch\nItem Valuable Tracker\n");
        presets = Config.Bind("Presets", "Presets", "Item Gun Handgun||||");
        Patch();
        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        ParsePresets();
        LoadoutUI.CreateMenus();
    }
    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }
    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }
}