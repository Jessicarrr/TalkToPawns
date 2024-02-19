using AiConversations.Relationships;
using Mono.Security;
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
        public static string GetIdeoDescription(Pawn pawn)
        {
            if (ModsConfig.IdeologyActive == false)
            {
                return "TtpIdeoDescriptionNone".Translate();
            }

            if (pawn.Ideo == null)
            {
                return "TtpIdeoDescriptionNone".Translate();
            }

            return pawn.Ideo.description;
        }

        public static string GetIdeoMemesDescriptions(Pawn pawn)
        {
            if (ModsConfig.IdeologyActive == false)
            {
                return "TtpIdeoMemesDescriptionNone".Translate();
            }

            if (pawn.Ideo == null)
            {
                return "TtpIdeoMemesDescriptionNone".Translate();
            }

            if (pawn.Ideo.memes.Count() < 1)
            {
                return "TtpIdeoMemesDescriptionNone".Translate();
            }

            string ideologyDescriptions = "";

            foreach(MemeDef meme in pawn.Ideo.memes)
            {
                ideologyDescriptions += meme.description + ", ";
            }

            return ideologyDescriptions.TrimEnd(' ', ',');
        }

        public static float GetIdeoCertainty(Pawn pawn)
        {
            if (ModsConfig.IdeologyActive == false)
            {
                return -1f;
            }

            if (pawn.ideo == null)
            {
                return -1f;
            }

            return pawn.ideo.Certainty;
        }

        public static string GetXenoTypeNameOrHuman(Pawn pawn)
        {
            if(ModsConfig.BiotechActive == false)
            {
                return "TtpXenotypeBaseliner".Translate();
            }

            if (pawn.genes == null)
            {
                return "TtpXenotypeBaseliner".Translate();
            }

            if (pawn.genes.xenotypeName.NullOrEmpty() == true)
            {
                return "TtpXenotypeBaseliner".Translate();
            }

            if (pawn.genes.xenotypeName.ToLower() == "baseliner")
            {
                return "TtpXenotypeBaseliner".Translate();
            }

            return pawn.genes.xenotypeName;
        }

        // Example implementation (you'll need to define this method based on your game's logic)
        public static bool CanTalkToPawn(Pawn pawn, Pawn targetPawn)
        {
            if (targetPawn.Downed || targetPawn.HostileTo(pawn))
            {
                return false;
            }
            if (pawn.Drafted)
            {
                return false;
            }
            if (targetPawn.RaceProps.IsMechanoid || targetPawn.NonHumanlikeOrWildMan() == true)
            {
                return false;
            }
            if (pawn.NonHumanlikeOrWildMan() == true)
            {
                return false;
            }

            // Placeholder: Check conditions such as not being hostile, within talking range, both parties capable of talking, etc.
            return true;
        }

        public static string DescribeRelationship(Pawn perspectivePawn, Pawn otherPawn)
        {
            string directRelation = TryGetDirectRelation(perspectivePawn, otherPawn);

            if (directRelation.NullOrEmpty() == false)
            {
                return "TtpDirectRelation".Translate(otherPawn.Name.ToStringShort, directRelation);
                //return otherPawn.Name.ToStringShort + " is your " + directRelation;
            }

            return GetOtherRelation(perspectivePawn, otherPawn);
        }

        private static string GetOtherRelation(Pawn perspectivePawn, Pawn otherPawn)
        {
            int opinionScore = perspectivePawn.relations.OpinionOf(otherPawn);

            if (opinionScore > 80)
            {
                return "TtpVeryMuchLikes".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
                //return perspectivePawn.Name.ToStringShort + " very much likes " + otherPawn.Name.ToStringShort;
            }
            else if (opinionScore > 50)
            {
                return "TtpLikes".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
                //return perspectivePawn.Name.ToStringShort + " likes " + otherPawn.Name.ToStringShort;
            }
            else if (opinionScore > 20)
            {
                return "TtpFavourable".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
                //return perspectivePawn.Name.ToStringShort + " is favourable towards " + otherPawn.Name.ToStringShort;
            }
            else if (opinionScore > 0)
            {
                return "TtpSlightlyFavourable".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
                //return perspectivePawn.Name.ToStringShort + " is slightly favourable towards " + otherPawn.Name.ToStringShort;
            }
            else if (opinionScore < -80)
            {
                return "TtpDeeplyHates".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
                //return perspectivePawn.Name.ToStringShort + " deeply hates " + otherPawn.Name.ToStringShort;
            }
            else if (opinionScore < -50)
            {
                return "TtpHates".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
                //return perspectivePawn.Name.ToStringShort + " hates " + otherPawn.Name.ToStringShort;
            }
            else if (opinionScore < -20)
            {
                return "TtpDislikes".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
                //return perspectivePawn.Name.ToStringShort + " dislikes " + otherPawn.Name.ToStringShort;
            }
            else if (opinionScore < 0)
            {
                return "TtpUnfavourable".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
                //return perspectivePawn.Name.ToStringShort + " is unfavourable towards " + otherPawn.Name.ToStringShort;
            }
            return "TtpNeutral".Translate(perspectivePawn.Name.ToStringShort, otherPawn.Name.ToStringShort);
            //return perspectivePawn.Name.ToStringShort + " is neutral towards " + otherPawn.Name.ToStringShort;
        }

        private static string TryGetDirectRelation(Pawn perspectivePawn, Pawn otherPawn)
        {
            // Iterate through the direct relations of pawn1
            foreach (var relation in perspectivePawn.relations.DirectRelations)
            {
                // Check if the other pawn in the relation is pawn2
                if (relation.otherPawn == otherPawn)
                {
                    // Return the relation definition (e.g., Parent, Child, Lover, etc.)
                    return relation.def.label;
                }
            }

            // If no direct relation is found, return null or a default indicating no relation
            return null; // Or any suitable default value

        }

        public static string GetBackstory(Pawn pawn)
        {
            string story = "";

            if (pawn.story.Childhood != null)
            {
                story += pawn.story.Childhood.baseDesc.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve();
            }
            if (pawn.story.Adulthood != null)
            {
                if (story.NullOrEmpty() == false)
                {
                    story += " ";
                }
                story += pawn.story.Adulthood.baseDesc.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve();
            }

            return story;
        }

        public static string GetNeedsThatNeedAttending(Pawn pawn)
        {
            string needsNames = "TtpNoLowNeeds".Translate();

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
                returner = "TtpIsTrader".Translate();
            }

            return returner;
        }

        public static string PrintIfPawnIsSlave(Pawn pawn)
        {
            string returner = "";

            if(pawn.IsSlave == true)
            {
                returner = "TtpIsSlave".Translate();
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
            //Log.Message("Getting hediff descriptions for pawn...");
            var hediffDescriptions = "TtpNoHediffs".Translate();

            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.Visible 
                    && (TTPModSettings.GetInstance().onlyShowBadHediffs.Value == true && hediff.def.isBad == true))
                {
                    if (hediffDescriptions == "none")
                    {
                        hediffDescriptions = "";
                    }

                    // Check if hediff.Part is not null before accessing its properties
                    string partLabel = hediff.Part != null ? hediff.Part.Label : "Whole body";
                    hediffDescriptions += $"{partLabel}: {hediff.LabelCap} - {hediff.Description}, ";
                    //Log.Message("Added one. Description: " + hediffDescriptions);
                }
            }

            return hediffDescriptions.ToString().TrimEnd(' ', ','); // Trim the trailing comma and space
        }

        public static string GetCurrentActivityString(Pawn pawn)
        {
            string activity = pawn.GetJobReport();

            if (activity.NullOrEmpty() == true)
            {
                activity = "TtpNoActivity".Translate().ToString();
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
