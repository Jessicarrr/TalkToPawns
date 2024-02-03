using AiConversations.Parsing;
using AiConversations.Relationships;
using HarmonyLib;
using JsonFx.Json;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.HarmonyPatches
{
    [HarmonyPatch(typeof(SafeSaver), nameof(SafeSaver.Save))]
    public class SafeSaver_DoSave
    {
        public static void Postfix(string path)
        {
            string saveFileName = Path.GetFileNameWithoutExtension(path);
            SaveLoadHandler.TrySaveModData(saveFileName);
        }
    }
}
