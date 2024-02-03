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
            new PromptVariable("{recipient_age}", "The biological age of the AI pawn", (aiRecipient, initiator) => aiRecipient.ageTracker.AgeBiologicalYears.ToString()),
            new PromptVariable("{initiator_age}", "The biological age of the player pawn", (aiRecipient, initiator) => initiator.ageTracker.AgeBiologicalYears.ToString()),
            new PromptVariable("{recipient_gender}", "The gender of the AI pawn (male/female)", (aiRecipient, initiator) => aiRecipient.gender.ToString()),
            new PromptVariable("{initiator_gender}", "The gender of the player pawn (male/female)", (aiRecipient, initiator) => initiator.gender.ToString()),
            new PromptVariable("{initiator_name}", "The name of the player controlled pawn", (aiRecipient, initiator) => initiator.Name.ToStringFull),
            new PromptVariable("{recipient_name}", "The name of the player controlled pawn", (aiRecipient, initiator) => aiRecipient.Name.ToStringFull),
            new PromptVariable("{recipient_traits_list}", "The traits of the AI pawn", (aiRecipient, initiator) => Helpers.GetListOfTraits(aiRecipient)),
            new PromptVariable("{initiator_traits_list}", "The traits of the player pawn", (initiaiRecipient, initiator) => Helpers.GetListOfTraits(initiator)),
            new PromptVariable("{opinion_on_initiator}", "The AI pawn's opinion on the player pawn", (aiRecipient, initiator) => aiRecipient.relations.OpinionExplanation(initiator)),
            new PromptVariable("{opinion_on_recipient}", "The player pawn's opinion on the AI pawn", (aiRecipient, initiator) => initiator.relations.OpinionExplanation(aiRecipient)),
            new PromptVariable("{recipient_current_action}", "Activity the AI pawn is currently doing.", (aiRecipient, initiator) => Helpers.GetCurrentActivityString(aiRecipient)),
            new PromptVariable("{initiator_current_action}", "Activity the player pawn is currently doing", (aiRecipient, initiator) => Helpers.GetCurrentActivityString(initiator)),
            new PromptVariable("{say_if_recipient_is_trader}", "Says directly if the pawn is a trader. If not a trader, then this will be blank.", (aiRecipient, initiator) => Helpers.PrintIfPawnIsTrader(aiRecipient)),
            new PromptVariable("{say_if_initiator_is_trader}", "Says if the player pawn is a trader. If not a trader, then this will be blank.", (aiRecipient, initiator) => Helpers.PrintIfPawnIsTrader(initiator)),
            new PromptVariable("{recipient_mood}", "Says a word that describes the ai recipient's mood", (aiRecipient, initiator) => aiRecipient.needs.mood?.MoodString),
            new PromptVariable("{initiator_mood}", "Says a word that describes the player pawn's mood", (aiRecipient, initiator) => initiator.needs.mood?.MoodString),
            new PromptVariable("{say_recipient_low_needs}", "Lists the ai recipient's needs that need attending. Says 'none' if no needs need attending", (aiRecipient, initiator) => Helpers.GetNeedsThatNeedAttending(aiRecipient)),
            new PromptVariable("{say_initiator_low_needs}", "Lists the player pawn's needs that need attending. Says 'none' if no needs need attending", (aiRecipient, initiator) => Helpers.GetNeedsThatNeedAttending(initiator)),
            new PromptVariable("{say_if_recipient_is_slave}", "Says if the ai recipient is a slave. If they are not a slave, this will be blank.", (aiRecipient, initiator) => Helpers.PrintIfPawnIsSlave(aiRecipient)),
            new PromptVariable("{say_if_initiator_is_slave}", "Says if the player pawn is a slave. If they are not a slave, this will be blank.", (aiRecipient, initiator) => Helpers.PrintIfPawnIsSlave(initiator)),
            new PromptVariable("{recipient_recent_memories}", "Gives a list of all the memories of the ai pawn. If there are no memories, will say 'None'", (aiRecipient, initiator) => Helpers.GetRecentMemories(aiRecipient)),
            new PromptVariable("{initiator_recent_memories}", "Gives a list of all the memories of the player pawn. If there are no memories, will say 'None'", (aiRecipient, initiator) => Helpers.GetRecentMemories(initiator)),
            new PromptVariable("{recipient_health_conditions}", "Lists the health conditions affecting the ai pawn. If there are none, this will say 'none'.", (aiRecipient, initiator) => Helpers.GetDescriptionsOfHediffs(aiRecipient)),
            new PromptVariable("{initiator_health_conditions}", "Lists the health conditions affecting the player pawn. If there are none, this will say 'none'.", (aiRecipient, initiator) => Helpers.GetDescriptionsOfHediffs(initiator)),
            //new PromptVariable("{}", "", (aiRecipient, initiator) => tix)
        };

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
