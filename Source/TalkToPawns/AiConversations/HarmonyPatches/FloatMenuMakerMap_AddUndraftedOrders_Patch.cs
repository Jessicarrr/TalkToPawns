using AiConversations.Controllers;
using HarmonyLib;
using Mono.Unix.Native;
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
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetExtraFloatMenuOptionsFor))]
    public static class FloatMenuMakerMap_AddUndraftedOrders_Patch
    {
        static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> __result, IntVec3 sq, Pawn __instance)
        {
            Pawn pawn = __instance;

            // First return all original options
            foreach (var option in __result)
            {
                yield return option;
            }
            foreach (Thing t in pawn.Map.thingGrid.ThingsAt(sq))
            {
                if (t is Pawn targetPawn && targetPawn != pawn) // Ensure we're not targeting the same pawn
                {
                    // Check if the pawn can be talked to (e.g., targetPawn is not downed, not hostile, etc.)
                    if (Helpers.CanTalkToPawn(pawn, targetPawn)) // Implement this method based on your criteria
                    {
                        yield return new FloatMenuOption("TtpTalkTo".Translate(targetPawn.LabelShort), () =>
                        {
                            // Logic to open the custom chat GUI goes here
                            OpenChatInterface(pawn, targetPawn);
                        });
                    }
                }
            }
        }

        private static void OpenChatInterface(Pawn initiator, Pawn recipient)
        {
            var controller = ChatController.GetSingletonInstance();
            controller.UpdatePawns(initiator, recipient);
            controller.TryOpenWindow();
        }

        
    }
}
