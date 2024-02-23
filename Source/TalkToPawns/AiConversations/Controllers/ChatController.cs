using AiConversations.GUI;
using AiConversations.HelperClasses;
using AiConversations.LLMs;
using AiConversations.LLMs.Networking.SerializableTypes;
using AiConversations.Parsing;
using AiConversations.Relationships;
using JsonFx.Json;
using JsonFx.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AiConversations.Controllers
{
    public class ChatController
    {
        private ChatWindow window;
        private static ChatController instance;

        private Dictionary<Enums.API, ApiMessager> apiTypeToApiMessager = new Dictionary<Enums.API, ApiMessager>
        {
            { Enums.API.ChatGPT, new ApiMessager_OpenAI() },
            { Enums.API.None, null },
            { Enums.API.Kobold, new ApiMessager_KoboldCpp() }

        };

        private Pawn selfPawn;
        private Pawn talkedToPawn;

        public static ChatController GetSingletonInstance()
        {
            if (instance == null)
            {
                instance = new ChatController();
            }
            return instance;
        }

        private ChatController()
        {
            foreach (KeyValuePair<Enums.API, ApiMessager> entry in apiTypeToApiMessager)
            {
                var messager = entry.Value;

                if (messager == null)
                {
                    continue;
                }

                messager.OnMessageReceived += HandleAiMessage;
                // do something with entry.Value or entry.Key
            }
        }

        public void UpdatePawns(Pawn selfPawn, Pawn talkedToPawn)
        {
            this.selfPawn = selfPawn;
            this.talkedToPawn = talkedToPawn;
        }

        public void TryOpenWindow()
        {
            if (window == null)
            {
                this.window = new ChatWindow();
                this.window.OnMessageSent += HandleUserSentMessage;
                this.window.OnWindowClosed += HandleChatWindowClosed;
            }

            if (Find.WindowStack.IsOpen(window) == true)
            {
                return;
            }

            //PawnRelationshipTrackerLLM.AddTestMemory(talkedToPawn, selfPawn);

            window.UpdatePawns(selfPawn, talkedToPawn);
            Find.WindowStack.Add(window);
        }

        private void HandleChatWindowClosed(List<ChatMessage> chatHistory)
        {
            //Log.Message("Chat window closed event caught " + chatHistory.ToString());

            if (chatHistory.Count() < 1)
            {
                TTPModSettings.Log.Trace("Chat history was empty so we don't need to ask for a summary.");
                //Log.Message("Chat history was empty so we don't need to ask for a summary.");
                return;
            }

            window.loadingAiResponse = true;

            var languageModel = TTPModSettings.GetInstance().llmModelHandle.Value;
            var apiMessager = apiTypeToApiMessager[languageModel];
            //Log.Message("preparing prompt...");
            string chatPrompt = PromptParser.PreparePromptFor(this.selfPawn, this.talkedToPawn, TTPModSettings.GetInstance().promptHandle.Value);
            string memoryPrompt = PromptParser.PreparePromptFor(this.selfPawn, this.talkedToPawn, TTPModSettings.GetInstance().summaryPrompt);

            apiMessager.RequestChatMemory(selfPawn, talkedToPawn, chatHistory, chatPrompt, memoryPrompt);
        }

        private void HandleChatSummaryResponse(Pawn ai, Pawn player, string message)
        {
            PawnRelationshipTrackerLLM.TryCreateMemoryFromString(
                ai, player, message);

            window.loadingAiResponse = false;


        }

        private void HandleAiMessage(Pawn ai, Pawn player, string response, bool isSummary = false)
        {
            if(isSummary == true)
            {
                HandleChatSummaryResponse(ai, player, response);
                window.loadingAiResponse = false;
                return;
            }
            window.AiSendMessage(ai, response);
            window.loadingAiResponse = false;
        }

        private string MakeErrorMessage(string response)
        {
            if (response.Contains("401") && TTPModSettings.GetInstance().llmModelHandle.Value == Enums.API.ChatGPT)
            {
                return "401 forbidden error - is your api key set up in the mod settings?";
            }
            return "An error occurred - " + response;
        }

        private void HandleUserSentMessage(Pawn sender, string message)
        {
            List<ChatMessage> chatHistory = window.chatHistory;
            window.loadingAiResponse = true;

            var languageModel = TTPModSettings.GetInstance().llmModelHandle.Value;
            var apiMessager = apiTypeToApiMessager[languageModel];
            //Log.Message("preparing prompt...");
            string prompt = PromptParser.PreparePromptFor(this.selfPawn, this.talkedToPawn, TTPModSettings.GetInstance().promptHandle.Value);

            //Log.Message("prepared prompt: " + prompt);

            apiMessager.Send(selfPawn, talkedToPawn, chatHistory, prompt);
            
        }
    }
}
