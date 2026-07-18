using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace givegun;

[BepInPlugin("plspls.givegun", "givegun", "1.0")]
public class GiveGun : BaseUnityPlugin
{
    internal static GiveGun Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    public static ConfigEntry<string> item_list;
    public static ConfigEntry<int> item_count;

    private void Awake()
    {
        Instance = this;
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        item_list = Config.Bind("Items", "ItemList", "Item Gun Handgun", "A list of items you want to start with separated by a semicolon. Use a hash to denote quantity. Items without quantity will be spawned for each player.");
        item_count = Config.Bind("Items", "ItemMaxOverride", 4, new ConfigDescription("The max amount of each item instance allowed to be spawned.", new AcceptableValueRange<int>(0, int.MaxValue)));
        Patch();
        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
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