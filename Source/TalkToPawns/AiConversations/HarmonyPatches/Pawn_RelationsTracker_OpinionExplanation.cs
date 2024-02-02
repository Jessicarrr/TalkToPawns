using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.OpinionExplanation))]
    public class Pawn_RelationsTracker_OpinionExplanation
    {
        public static void Postfix(Pawn_RelationsTracker __instance, ref string __result, Pawn other)
        {
            var stringBuilder = new StringBuilder(__result + "\n");
            stringBuilder.AppendLine("- Had an arbitrary memory -25");
            __result = stringBuilder.ToString();
        }
    }
}
