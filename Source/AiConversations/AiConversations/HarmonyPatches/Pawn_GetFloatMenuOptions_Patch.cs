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
    [HarmonyPatch(typeof(ThingWithComps), nameof(Pawn.GetFloatMenuOptions))]
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

            // Check if the instance is a Pawn and add custom float menu option
            if (selPawn != null && selPawn.NonHumanlikeOrWildMan() == false && selPawn != __instance && __instance is Pawn rightClickedPawn == true && rightClickedPawn.NonHumanlikeOrWildMan() == false)
            {
                yield return new FloatMenuOption("Talk to " + rightClickedPawn.LabelShort, () =>
                {
                    // Logic to open the custom chat GUI goes here
                    OpenChatInterface(selPawn, rightClickedPawn);
                });
            }
        }


        private static void OpenChatInterface(Pawn initiator, Pawn recipient)
        {
            var window = ChatWindow.GetSingletonInstance();

            if (Find.WindowStack.IsOpen(window) == true)
            {
                return;
            }

            window.UpdatePawns(initiator, recipient);
            Find.WindowStack.Add(window);
        }
    }
}
