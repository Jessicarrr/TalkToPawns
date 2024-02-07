using AiConversations.Relationships;
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
            Pawn pawn = (Pawn)AccessTools.Field(typeof(Pawn_RelationsTracker), "pawn").GetValue(__instance);

            string generatedOpinion = PawnRelationshipTrackerLLM.GetFullOpinionStringFor(pawn.ThingID, other.ThingID);

            __result = __result + "\n" + generatedOpinion;
        }
    }
}
