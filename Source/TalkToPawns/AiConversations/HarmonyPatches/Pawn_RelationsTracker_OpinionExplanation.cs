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

            var stringBuilder = new StringBuilder(__result + "\n");

            //Log.Message("Num memories: " + PawnRelationshipTrackerLLM.memories.Count());

            foreach(PawnRelationshipMemoryLLM memory in PawnRelationshipTrackerLLM.memories)
            {
                if(memory.memoryHolderPawnID == pawn.ThingID && memory.thoughtAboutPawnID == other.ThingID)
                {
                    stringBuilder.AppendLine(" - " + memory.description + " " + memory.relationshipImpact);
                }

                
            }

            stringBuilder.AppendLine(" - Had an arbitrary memory -25");
            __result = stringBuilder.ToString();
        }
    }
}
