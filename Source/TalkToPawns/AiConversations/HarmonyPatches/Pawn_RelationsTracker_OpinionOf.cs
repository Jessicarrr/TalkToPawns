using AiConversations.Relationships;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AiConversations.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.OpinionOf))]
    public class Pawn_RelationsTracker_OpinionOf
    {
        public static void Postfix(Pawn_RelationsTracker __instance, ref int __result, Pawn other)
        {
            // Access the 'pawn' private field using reflection
            Pawn pawn = (Pawn)AccessTools.Field(typeof(Pawn_RelationsTracker), "pawn").GetValue(__instance);

            __result = __result - 25; // Arbitrary decrease in relationship score

            foreach (PawnRelationshipMemoryLLM memory in PawnRelationshipTrackerLLM.memories)
            {
                if (memory.memoryHolderPawnID == pawn.ThingID && memory.thoughtAboutPawnID == other.ThingID)
                {
                    __result += memory.relationshipImpact;
                }

                
            }

            // Ensure the result is not positive if the pawn is hostile to the other
            if (__result > 0 && pawn.HostileTo(other))
            {
                __result = 0;
            }

            // Clamp the result to be within the game's expected range for opinion scores
            __result = Mathf.Clamp(__result, -100, 100);
        }
    }
}
