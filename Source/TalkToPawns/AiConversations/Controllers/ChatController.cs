using AiConversations.GUI;
using AiConversations.HelperClasses;
using AiConversations.LLMs;
using AiConversations.LLMs.Networking.SerializableTypes;
using AiConversations.Parsing;
using AiConversations.Relationships;
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
            { Enums.API.Kobold, null }

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
            }

            if (Find.WindowStack.IsOpen(window) == true)
            {
                return;
            }

            PawnRelationshipTrackerLLM.AddTestMemory(talkedToPawn, selfPawn);

            window.UpdatePawns(selfPawn, talkedToPawn);
            Find.WindowStack.Add(window);
        }

        private void HandleAiMessage(string response)
        {
            Log.Message("HandleAiMessage: response: " + response.ToString());

            try
            {
                ChatCompletionResponse parsed = JsonParser.ParseStringToDynamic(response);
                //Log.Message("HandleAiMessage: parsed: " + parsed.ToString());
                //Log.Message("The message: " + parsed.choices[0].message.content);
                window.AiSendMessage(talkedToPawn, parsed.choices[0].message.content);
                window.loadingAiResponse = false;
            }
            catch(SerializationException e)
            {
                Log.Message("SerializationException: " + e.ToString());
                string errorMsg = MakeErrorMessage(response);
                Log.Message(errorMsg + ", " + response);
                window.AiSendMessage(talkedToPawn, errorMsg + ", " + response);
                window.loadingAiResponse = false;
            }
        }

        private string MakeErrorMessage(string response)
        {
            if (response.Contains("401"))
            {
                return "401 forbidden error - is your api key set up in the mod settings?";
            }
            return "An unknown error occurred - " + response;
        }

        private void HandleUserSentMessage(Pawn sender, string message)
        {
            List<ChatMessage> chatHistory = window.chatHistory;
            window.loadingAiResponse = true;

            var apiMessager = apiTypeToApiMessager[TTPModSettings.selectedAiType];
            Log.Message("preparing prompt...");
            string prompt = PromptParser.PreparePromptFor(this.selfPawn, this.talkedToPawn, TTPModSettings.generalSettings.prompt);

            Log.Message("prepared prompt: " + prompt);

            apiMessager.Send(selfPawn, talkedToPawn, chatHistory, prompt);
            
        }
    }
}
