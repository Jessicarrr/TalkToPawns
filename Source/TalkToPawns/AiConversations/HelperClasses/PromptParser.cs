using AiConversations.Relationships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.HelperClasses
{
    internal class PromptParser
    {
        public static List<PromptVariable> promptVariables = new List<PromptVariable>
        {
            new PromptVariable("{recipient_age}", "TtpAgeOfAIPawn".Translate(), (aiRecipient, initiator) => aiRecipient.ageTracker.AgeBiologicalYears.ToString()),
            new PromptVariable("{initiator_age}", "TtpAgeOfPlayerPawn".Translate(), (aiRecipient, initiator) => initiator.ageTracker.AgeBiologicalYears.ToString()),
            new PromptVariable("{recipient_gender}", "TtpGenderOfAIPawn".Translate(), (aiRecipient, initiator) => aiRecipient.gender.ToString()),
            new PromptVariable("{initiator_gender}", "TtpGenderOfPlayerPawn".Translate(), (aiRecipient, initiator) => initiator.gender.ToString()),
            new PromptVariable("{initiator_name}", "TtpFullNameOfPlayerPawn".Translate(), (aiRecipient, initiator) => initiator.Name.ToStringFull),
            new PromptVariable("{recipient_name}", "TtpFullNameOfAIPawn".Translate(), (aiRecipient, initiator) => aiRecipient.Name.ToStringFull),
            new PromptVariable("{recipient_name_short}", "TtpShortNameOfAIPawn".Translate(), (aiRecipient, initiator) => aiRecipient.LabelShort),
            new PromptVariable("{initiator_name_short}", "TtpShortNameOfPlayerPawn".Translate(), (aiRecipient, initiator) => initiator.LabelShort),
            new PromptVariable("{recipient_traits_list}", "TtpTraitsOfAIPawn".Translate(), (aiRecipient, initiator) => Helpers.GetListOfTraits(aiRecipient)),
            new PromptVariable("{initiator_traits_list}", "TtpTraitsOfPlayerPawn".Translate(), (initiaiRecipient, initiator) => Helpers.GetListOfTraits(initiator)),
            new PromptVariable("{relation_to_initiator}", "TtpAiRelationToPlayerPawn".Translate(), (aiRecipient, initiator) => Helpers.DescribeRelationship(aiRecipient, initiator)),
            new PromptVariable("{relation_to_recipient}", "TtpPlayerRelationToAiPawn".Translate(), (aiRecipient, initiator) => Helpers.DescribeRelationship(initiator, aiRecipient)),
            new PromptVariable("{recipient_current_action}", "TtpAiCurrentActivity".Translate(), (aiRecipient, initiator) => Helpers.GetCurrentActivityString(aiRecipient)),
            new PromptVariable("{initiator_current_action}", "TtpPlayerCurrentActivity".Translate(), (aiRecipient, initiator) => Helpers.GetCurrentActivityString(initiator)),
            new PromptVariable("{say_if_recipient_is_trader}", "TtpAiPawnTrader".Translate(), (aiRecipient, initiator) => Helpers.PrintIfPawnIsTrader(aiRecipient)),
            new PromptVariable("{say_if_initiator_is_trader}", "TtpPlayerPawnTrader".Translate(), (aiRecipient, initiator) => Helpers.PrintIfPawnIsTrader(initiator)),
            new PromptVariable("{recipient_mood}", "TtpAiMood".Translate(), (aiRecipient, initiator) => aiRecipient.needs.mood?.MoodString),
            new PromptVariable("{initiator_mood}", "TtpPlayerMood".Translate(), (aiRecipient, initiator) => initiator.needs.mood?.MoodString),
            new PromptVariable("{say_recipient_low_needs}", "TtpAiNeeds".Translate(), (aiRecipient, initiator) => Helpers.GetNeedsThatNeedAttending(aiRecipient)),
            new PromptVariable("{say_initiator_low_needs}", "TtpPlayerNeeds".Translate(), (aiRecipient, initiator) => Helpers.GetNeedsThatNeedAttending(initiator)),
            new PromptVariable("{say_if_recipient_is_slave}", "TtpAiIsSlave".Translate(), (aiRecipient, initiator) => Helpers.PrintIfPawnIsSlave(aiRecipient)),
            new PromptVariable("{say_if_initiator_is_slave}", "TtpPlayerIsSlave".Translate(), (aiRecipient, initiator) => Helpers.PrintIfPawnIsSlave(initiator)),
            new PromptVariable("{recipient_recent_memories}", "TtpRecentMemoriesOfAIPawn".Translate(), (aiRecipient, initiator) => Helpers.GetRecentMemories(aiRecipient)),
            new PromptVariable("{initiator_recent_memories}", "TtpRecentMemoriesOfPlayerPawn".Translate(), (aiRecipient, initiator) => Helpers.GetRecentMemories(initiator)),
            new PromptVariable("{recipient_health_conditions}", "TtpHealthConditionsOfAIPawn".Translate(), (aiRecipient, initiator) => Helpers.GetDescriptionsOfHediffs(aiRecipient)),
            new PromptVariable("{initiator_health_conditions}", "TtpHealthConditionsOfPlayerPawn".Translate(), (aiRecipient, initiator) => Helpers.GetDescriptionsOfHediffs(initiator)),
            new PromptVariable("{recipient_backstory}", "TtpBackstoryOfAIPawn".Translate(), (aiRecipient, initiator) => Helpers.GetBackstory(aiRecipient)),
            new PromptVariable("{initiator_backstory}", "TtpBackstoryOfPlayerPawn".Translate(), (aiRecipient, initiator) => Helpers.GetBackstory(initiator)),
            new PromptVariable("{recipient_xenotype_or_human}", "TtpXenotypeOfAIPawn".Translate(), (aiRecipient, initiator) => Helpers.GetXenoTypeNameOrHuman(aiRecipient)),
            new PromptVariable("{initiator_xenotype_or_human}", "TtpXenotypeOfPlayerPawn".Translate(), (aiRecipient, initiator) => Helpers.GetXenoTypeNameOrHuman(initiator)),
            new PromptVariable("{recipient_memories_with_initiator}", "TtpMemoriesWithInitiatorOfAIPawn".Translate(), (aiRecipient, initiator) => PawnRelationshipTrackerLLM.GetPromptFriendlyOpinionStringFor(aiRecipient.ThingID, initiator.ThingID)),
            new PromptVariable("{initiator_memories_with_recipient}", "TtpMemoriesWithRecipientOfPlayerPawn".Translate(), (aiRecipient, initiator) => PawnRelationshipTrackerLLM.GetPromptFriendlyOpinionStringFor(initiator.ThingID, aiRecipient.ThingID)),
            new PromptVariable("{recipient_ideo_description}", "TtpIdeoDescriptionOfAIPawn".Translate(), (aiRecipient, initiator) => Helpers.GetIdeoDescription(aiRecipient)),
            new PromptVariable("{initiator_ideo_description}", "TtpIdeoDescriptionOfPlayerPawn".Translate(), (aiRecipient, initiator) => Helpers.GetIdeoDescription(initiator)),
            new PromptVariable("{recipient_ideo_memes}", "TtpIdeoMemesOfAIPawn".Translate(), (aiRecipient, initiator) => Helpers.GetIdeoMemesDescriptions(aiRecipient)),
            new PromptVariable("{initiator_ideo_memes}", "TtpIdeoMemesOfPlayerPawn".Translate(), (aiRecipient, initiator) => Helpers.GetIdeoMemesDescriptions(initiator)),
            new PromptVariable("{recipient_ideo_certainty}", "TtpIdeoCertaintyOfAIPawn".Translate(), (aiRecipient, initiator) => (Math.Round(Helpers.GetIdeoCertainty(aiRecipient) * 100f)).ToString()),
            new PromptVariable("{initiator_ideo_certainty}", "TtpIdeoCertaintyOfPlayerPawn".Translate(), (aiRecipient, initiator) => (Math.Round(Helpers.GetIdeoCertainty(initiator) * 100f)).ToString()),
            // Add any additional PromptVariables as needed
        };



        public static string PrintExplanations()
        {
            string explanation = "";

            foreach(PromptVariable pVar in promptVariables)
            {
                explanation += pVar.placeholder + " : " + pVar.explanation + "\n\n";
            }

            return explanation;
        }

        public static string PreparePromptFor(Pawn initiator, Pawn aiRecipient, string basePrompt)
        {
            string editedPrompt = basePrompt;

            foreach(PromptVariable promptVar  in promptVariables)
            {
                editedPrompt = editedPrompt.Replace(promptVar.placeholder, promptVar.DoReplace(aiRecipient, initiator));
            }

            editedPrompt = editedPrompt.Replace("\n", "").Replace("\r", "");
            string pattern = @"<color=#(?:[0-9a-fA-F]{6}|[0-9a-fA-F]{8})>(.*?)</color>";

            // Replace the color tags with just the name
            editedPrompt = Regex.Replace(editedPrompt, pattern, "$1");

            return editedPrompt;
        }
    }
}
