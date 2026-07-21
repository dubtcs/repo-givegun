using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace givegun;

[BepInPlugin("plspls.Loadout", "Loadout", "1.3.0")]
public class GiveGun : BaseUnityPlugin
{
    internal static GiveGun Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    public static ConfigEntry<bool> mod_enabled;
    public static ConfigEntry<string> item_list;

    static public Dictionary<string, int> loadout = [];

    private void Awake()
    {
        Instance = this;
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        mod_enabled = Config.Bind("General", "Enabled", true, "Enable or disable the mod.");
        item_list = Config.Bind("Items", "ItemList", "Item Gun Handgun", "A list of items you want to start with separated by a semicolon. Use a hash to denote quantity. Items without quantity will be spawned for each player.\nDefault game item list:\nItem Cart Medium\nItem Cart Small\nItem Drone Battery\nItem Drone Feather\nItem Drone Indestructible\nItem Drone Torque\nItem Drone Zero Gravity\nItem Extraction Tracker\nItem Grenade Duct Taped\nItem Grenade Explosive\nItem Grenade Human\nItem Grenade Shockwave\nItem Grenade Stun\nItem Gun Handgun\nItem Gun Shotgun\nItem Gun Tranq\nItem Health Pack Large\nItem Health Pack Medium\nItem Health Pack Small\nItem Melee Baseball Bat\nItem Melee Frying Pan\nItem Melee Inflatable Hammer\nItem Melee Sledge Hammer\nItem Melee Sword\nItem Mine Explosive\nItem Mine Shockwave\nItem Mine Stun\nItem Orb Zero Gravity\nItem Power Crystal\nItem Rubber Duck\nItem Upgrade Map Player Count\nItem Upgrade Player Energy\nItem Upgrade Player Extra Jump\nItem Upgrade Player Grab Range\nItem Upgrade Player Grab Strength\nItem Upgrade Player Health\nItem Upgrade Player Sprint Speed\nItem Upgrade Player Tumble Launch\nItem Valuable Tracker\n");
        Patch();
        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        GiveGun_UI.CreateMenus();
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
