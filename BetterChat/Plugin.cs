using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using SteamworksNative;
using System.Globalization;

namespace BetterChat
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public sealed class BetterChat : BasePlugin
    {
        internal static BetterChat Instance { get; private set; }

        internal bool formatReturnedUsername = false;
        internal string usernameFormatting;
        internal string aliveText;
        internal string deadText;

        public override void Load()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            Instance = this;

            usernameFormatting = Config.Bind("BetterChat", "UsernameFormatting", "{USERNAME} (#{PLAYER_NUMBER})",
                "The formatting to use for player usernames in chat.\nUSERNAME -> The player's username.\nPLAYER_NUMBER -> The player's unique number.\nCLIENT_ID -> The player's steam id/client id.\nLIVING -> Whether the player is alive or dead.\nOther mods may add more formats.").Value;

            aliveText = Config.Bind("BetterChat", "AliveText", "alive",
                "The text to use for the LIVING username formatting when the player is alive.").Value;
            deadText = Config.Bind("BetterChat", "DeadText", "dead",
                "The text to use for the LIVING username formatting when the player is dead.").Value;

            Api.RegisterFormatting("USERNAME", FormatUsername);
            Api.RegisterFormatting("PLAYER_NUMBER", FormatPlayerNumber);
            Api.RegisterFormatting("CLIENT_ID", FormatClientId);
            Api.RegisterFormatting("LIVING", FormatLiving);

            Harmony harmony = new(MyPluginInfo.PLUGIN_NAME);
            harmony.PatchAll(typeof(Patches));

            Log.LogInfo($"Initialized [{MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION}]");
        }

        internal static string FormatUsername(ulong clientId)
            => clientId == SteamManager.Instance.field_Private_CSteamID_0.m_SteamID ? SteamFriends.GetPersonaName() : SteamFriends.GetFriendPersonaName(new(clientId));
        internal static string FormatPlayerNumber(ulong clientId)
            => LobbyManager.steamIdToUID.ContainsKey(clientId) ? (LobbyManager.steamIdToUID[clientId] + 1).ToString() : "0";
        internal static string FormatClientId(ulong clientId)
            => clientId.ToString();
        internal static string FormatLiving(ulong clientId)
            => GameManager.Instance != null && GameManager.Instance.activePlayers.ContainsKey(clientId) && !GameManager.Instance.activePlayers[clientId].dead
            ? Instance.aliveText 
            : Instance.deadText;
    }
}