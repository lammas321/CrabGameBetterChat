using HarmonyLib;
using SteamworksNative;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterChat
{
    internal static class Patches
    {
        //   Anti Bepinex detection (Thanks o7Moon: https://github.com/o7Moon/CrabGame.AntiAntiBepinex)
        [HarmonyPatch(typeof(EffectManager), nameof(EffectManager.Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0))] // Ensures effectSeed is never set to 4200069 (if it is, modding has been detected)
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.Method_Private_Void_0))] // Ensures connectedToSteam stays false (true means modding has been detected)
        // [HarmonyPatch(typeof(SnowSpeedModdingDetector), nameof(SnowSpeedModdingDetector.Method_Private_Void_0))] // Would ensure snowSpeed is never set to Vector3.zero (though it is immediately set back to Vector3.one due to an accident on Dani's part lol)
        [HarmonyPrefix]
        internal static bool PreBepinexDetection()
            => false;


        // Sends the properly formatted message sent by a client to them (server authoritative)
        [HarmonyPatch(typeof(ServerSend), nameof(ServerSend.SendChatMessage))]
        [HarmonyPrefix]
        internal static bool PreServerSendSendChatMessage(bool __runOriginal, ulong param_0, string param_1)
        {
            if (!__runOriginal)
                return false;

            List<byte> bytes = [];
            bytes.AddRange(BitConverter.GetBytes((int)ServerSendType.sendMessage));
            bytes.AddRange(BitConverter.GetBytes(param_0));

            string formattedUsername = Api.Format(param_0);
            bytes.AddRange(BitConverter.GetBytes(formattedUsername.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(formattedUsername));

            bytes.AddRange(BitConverter.GetBytes(param_1.Length));
            bytes.AddRange(Encoding.ASCII.GetBytes(param_1));

            bytes.InsertRange(0, BitConverter.GetBytes(bytes.Count));

            Packet packet = new();
            packet.field_Private_List_1_Byte_0 = new();
            foreach (byte b in bytes)
                packet.field_Private_List_1_Byte_0.Add(b);

            foreach (ulong clientId in LobbyManager.steamIdToUID.Keys)
                SteamPacketManager.SendPacket(new(clientId), packet, 8, SteamPacketDestination.ToClient);
            return false;
        }

        // Prevents your own messages (client side) from being added to the chat
        [HarmonyPatch(typeof(GameUiChatBox), nameof(GameUiChatBox.AppendMessage))]
        [HarmonyPrefix]
        internal static bool PreGameUiChatBoxAppendMessage(ulong param_1, string param_3)
            => param_1 != 0UL || param_3 != SteamManager.Instance.field_Private_String_0 || SteamMatchmaking.GetLobbyData(SteamManager.Instance.currentLobby, $"lammas123.{MyPluginInfo.PLUGIN_GUID}") != "1";

        // Tells clients with the mod that you have BetterChat as the host
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.StartLobby))]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.StartPracticeLobby))]
        [HarmonyPostfix]
        internal static void PostLobbyManagerStartLobby()
            => SteamMatchmaking.SetLobbyData(SteamManager.Instance.currentLobby, $"lammas123.{MyPluginInfo.PLUGIN_GUID}", "1");
    }
}