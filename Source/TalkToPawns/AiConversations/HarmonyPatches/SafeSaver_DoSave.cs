using HarmonyLib;
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
            Log.Message(Path.GetFileNameWithoutExtension(path));
        }
    }
}
