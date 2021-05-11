using CivilizedConversation.Services;
using HarmonyLib;

namespace CivilizedConversation.Patches
{
    static class Player_Patch
    {
        [HarmonyPatch(typeof(Chat), nameof(Chat.SendText))]
        static class Chat_SendText_Patch
        {
            static void Prefix(ref string text)
            {
                text = Translator.Translate(text);
            }
        }
    }
}