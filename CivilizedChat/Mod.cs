// CivilizedConversation
// Make conversations civilized
// 
// File:    CivilizedConversation.cs
// Project: CivilizedConversation

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace CivilizedConversation
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Mod : BaseUnityPlugin
    {
        public const string PluginGUID = "dickdangerjustice.CivilizedConversation";
        public const string PluginName = "CivilizedConversation";
        public const string PluginVersion = "1.0.0";
        public static ConfigEntry<bool> Enabled;

        private readonly Harmony harmony = new Harmony(PluginGUID);

        private void Awake()
        {
            Enabled = Config.Bind("Civilized", "Chat", true, "Enable civilized chat.");
            harmony.PatchAll();
        }
    }
}