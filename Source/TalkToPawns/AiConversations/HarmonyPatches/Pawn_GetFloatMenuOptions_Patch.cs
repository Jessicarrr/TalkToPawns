using AiConversations.Controllers;
using AiConversations.GUI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.HarmonyPatches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.GetFloatMenuOptions))]
    public class Pawn_GetFloatMenuOptions_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<FloatMenuOption> AddChatOption(IEnumerable<FloatMenuOption> __result, Pawn selPawn, ThingWithComps __instance)
        {
            // First return all original options
            foreach (var option in __result)
            {
                yield return option;
            }

            if(__instance is Pawn targetPawn == true && selPawn != targetPawn && Helpers.CanTalkToPawn(selPawn, targetPawn))
            {
                yield return new FloatMenuOption("Talk to " + targetPawn.LabelShort, () =>
                {
                    // Logic to open the custom chat GUI goes here
                    OpenChatInterface(selPawn, targetPawn);
                });
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
