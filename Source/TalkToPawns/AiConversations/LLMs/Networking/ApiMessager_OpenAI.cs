using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Net.Http;
using Verse;
using AiConversations.GUI;
using UnityEngine.Networking;
using System.Net;
using System.Collections; // This is important for IEnumerator
using System.Collections.Generic;
using NAudio.CoreAudioApi;
using UnityEngine;
using AiConversations.LLMs.Networking.Extensions;
using AiConversations.LLMs.Networking.SerializableTypes;
using System.Runtime.Remoting.Messaging;

namespace AiConversations.LLMs
{
    internal class ApiMessager_OpenAI : ApiMessager
    {
        public new string baseUrl = "https://api.openai.com/v1/chat/completions";
        

        public override async void Send(Pawn initiator, Pawn talkedToPawn, List<ChatMessage> chatHistory, string prompt)
        {
            List<string> serializedMessages = new List<string>();

            // Manually serialize the system message
            serializedMessages.Add(JsonUtility.ToJson(new Networking.SerializableTypes.Message
            {
                role = "system",
                content = prompt
            }));

            // Serialize each chat message
            foreach (var msg in chatHistory)
            {
                string gptFriendlyRole = "user";

                if (msg.pawn.Name.ToStringFull == initiator.Name.ToStringFull)
                {
                    gptFriendlyRole = "user";
                }
                if (msg.pawn.Name.ToStringFull == talkedToPawn.Name.ToStringFull)
                {
                    gptFriendlyRole = "assistant";
                }

                serializedMessages.Add(JsonUtility.ToJson(new Networking.SerializableTypes.Message
                {
                    role = gptFriendlyRole,
                    content = msg.messageText
                }));
            }

            // Manually construct the JSON array string
            string messagesJsonArray = "[" + string.Join(",", serializedMessages) + "]";

            // Manually assemble the final JSON string
            string jsonData = $"{{\"model\":\"{TTPModSettings.chatGPTSettings.selectedOpenAiModel}\",\"messages\":{messagesJsonArray}}}";

            Log.Message("Json Data compiled: " + jsonData);
            await SendPostRequestAsync(jsonData);
        }

        public async Task<string> SendPostRequestAsync(string jsonData)
        {
            try
            {
                var request = UnityWebRequest.Post(baseUrl, jsonData);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + TTPModSettings.chatGPTSettings.openAiApiKey);

                string responseText = await request.SendWebRequestAsync();
                Log.Message("theeee response" + responseText);
                InvokeEvent(responseText);
                
                return responseText;
            }
            catch(Exception ex)
            {
                // Attempt to parse the response body to log more detailed error information
                Log.Message($"Exception happened: {ex.Message}, stacktrace:\n{ex.StackTrace}");

                InvokeEvent(ex.Message);

                return null;
            }
        }
    }
}
