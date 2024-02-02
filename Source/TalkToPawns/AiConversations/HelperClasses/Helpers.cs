using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine.Analytics;
using Verse;

namespace AiConversations
{
    internal class Helpers
    {
        public static string GetNeedsThatNeedAttending(Pawn pawn)
        {
            string needsNames = "None";

            foreach(var need in pawn.needs.AllNeeds)
            {
                if (need.CurLevelPercentage < 0.45 && need.ShowOnNeedList == true)
                {
                    if (needsNames == "None")
                    {
                        needsNames = "";
                    }

                    needsNames += need.LabelCap + ", ";
                }
            }

            return needsNames.TrimEnd(' ', ',');

        }

        public static string GetListOfTraits(Pawn pawn)
        {
            string traits = "";
            int traitNum = 1;

            foreach(Trait trait in pawn.story.traits.allTraits)
            {
                var simpleDescription = trait.CurrentData.description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve();
                // Regular expression to match the color tags
                string pattern = @"<color=#(?:[0-9a-fA-F]{6}|[0-9a-fA-F]{8})>(.*?)</color>";

                // Replace the color tags with just the name
                string descriptionWithoutColorTags = Regex.Replace(simpleDescription, pattern, "$1");

                traits += "Trait " + traitNum + ": " + descriptionWithoutColorTags + " ";
                traitNum++;
            }

            return traits.TrimEnd(' ', ',');
        }

        public static string PrintIfPawnIsTrader(Pawn pawn)
        {
            string returner = "";

            if (pawn.CanTradeNow)
            {
                returner = "You are a trader.";
            }

            return returner;
        }

        public static string PrintIfPawnIsSlave(Pawn pawn)
        {
            string returner = "";

            if(pawn.IsSlave == true)
            {
                returner = "You are a slave.";
            }

            return returner;
        }

        public static string GetRecentMemories(Pawn pawn)
        {
            string memories = "None";

            foreach (Thought_Memory memory in pawn.needs.mood?.thoughts.memories.Memories)
            {
                if (memory.Description.NullOrEmpty() == false)
                {
                    if(memories == "None")
                    {
                        memories = "";
                    }

                    memories += memory.Description + ", ";
                }
            }

            return memories.TrimEnd(' ', ',');
        }

        public static string GetDescriptionsOfHediffs(Pawn pawn)
        {
            Log.Message("Getting hediff descriptions for pawn...");
            var hediffDescriptions = "none";

            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.Visible)
                {
                    if (hediffDescriptions == "none")
                    {
                        hediffDescriptions = "";
                    }

                    // Check if hediff.Part is not null before accessing its properties
                    string partLabel = hediff.Part != null ? hediff.Part.Label : "Whole body";
                    hediffDescriptions += $"{partLabel}: {hediff.LabelCap} - {hediff.Description}, ";
                    Log.Message("Added one. Description: " + hediffDescriptions);
                }
            }

            return hediffDescriptions.TrimEnd(' ', ','); // Trim the trailing comma and space
        }

        public static string GetCurrentActivityString(Pawn pawn)
        {
            string activity = pawn.GetJobReport();

            if (activity.NullOrEmpty() == true)
            {
                activity = "doing nothing";
            }

            return activity;
        }
        
        public static void PrintPawnInfo(Pawn pawn, Pawn initiator)
        {
            string gender = Enum.GetName(typeof(Verse.Gender), pawn.gender);
            string memories = "";
            string needs = GetNeedsThatNeedAttending(pawn);

            foreach (Thought_Memory memory in pawn.needs.mood?.thoughts.memories.Memories)
            {
                memories += memory.Description + ", ";
            }

            Log.Message("Gender: " + gender +
                "\n name: " + pawn.Name +
                "\n age: " + pawn.ageTracker.AgeBiologicalYears +
                "\n pain: " + pawn.health.hediffSet.PainTotal +
                "\n is currently: " + pawn.CurJob.def.reportString +
                "\n relation to initiator: " + pawn.relations.OpinionExplanation(initiator) +
                "\n faction: " + pawn.Faction.Name +
                "\n can trade? " + pawn.CanTradeNow +
                "\n mood CurLevel " + pawn.needs.mood?.CurLevel +
                "\n mood String: " + pawn.needs.mood?.MoodString +
                "\n memories: " + memories +
                "\n needs that need attending: " + needs +
                "\n traits: " + GetListOfTraits(pawn)) ;
        }
    }
}
