using AiConversations.GUI;
using AiConversations.LLMs.Networking.SerializableTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AiConversations.LLMs
{
    public abstract class ApiMessager
    {
        public ApiMessager() { }

        //protected static readonly HttpClient client = new HttpClient();
        // Define a delegate for the message sent event
        public delegate void MessageReceivedEventHandler(string response, bool isSummary = false);

        // Define an event based on the delegate
        public event MessageReceivedEventHandler OnMessageReceived;

        public string baseUrl = "";

        public bool isCallingApi = false;

        public abstract void RequestChatMemory(Pawn initiator, Pawn talkedToPawn, List<ChatMessage> chatHistory, string chatPrompt, string memoryPrompt);

        public abstract void Send(Pawn initiator, Pawn talkedToPawn, List<ChatMessage> chatHistory, string prompt);

        internal abstract string GetMessageFromResponse(string response);

        protected void InvokeEvent(string response, bool isSummary = false)
        {
            OnMessageReceived?.Invoke(response, isSummary);
        }
    }
}
