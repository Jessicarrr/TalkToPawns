using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AiConversations.GUI
{
    public class ChatWindow : Window
    {
        // Define a delegate for the message sent event
        public delegate void MessageSentEventHandler(Pawn sender, string message);
        public delegate void OnWindowClosedEventHandler(List<ChatMessage> chatHistory);

        // Define an event based on the delegate
        public event MessageSentEventHandler OnMessageSent;
        public event OnWindowClosedEventHandler OnWindowClosed;

        private Pawn selfPawn;
        private Pawn talkedToPawn;

        private String inputText = "";
        private Vector2 scrollPosition;

        private float startingHeight = 300f;
        private float startingWidth = 300f;

        private Rect chatDisplayRect;

        private bool shouldUpdateScrollToBottom = false;

        public bool loadingAiResponse = false;

        internal List<ChatMessage> chatHistory = new List<ChatMessage>();

        public override void Close(bool doCloseSound = true)
        {
            List<ChatMessage> chatHistoryCopy = chatHistory.ListFullCopy();
            Find.WindowStack.TryRemove(this, doCloseSound);
            chatHistory.Clear();
            OnWindowClosed?.Invoke(chatHistoryCopy);
            
            //Log.Message("Closed window");
            
            
        }

        public void UpdatePawns(Pawn selfPawn, Pawn talkedToPawn)
        {
            this.selfPawn = selfPawn;
            this.talkedToPawn = talkedToPawn;
            this.optionalTitle = "Chatting with " + talkedToPawn.LabelShort + " as " + selfPawn.LabelShort;
            chatHistory.Clear();
        }

        public ChatWindow()
        {
            this.doCloseX = true;
            //this.doCloseButton = true;
            this.draggable = true;
            this.preventCameraMotion = false;
            this.resizeable = true;
        }

        public override Vector2 InitialSize
        {
            get { return new Vector2(startingWidth, startingHeight); }
        }

        public override void DoWindowContents(Rect inRect)
        {
            //Text.Font = GameFont.Medium;
            //Widgets.Label(new Rect(0, 0, inRect.width, 30f), "Chatting with " + talkedToPawn.LabelShort + " as " + selfPawn.LabelShort);

            DoChatArea(inRect);

            if (shouldUpdateScrollToBottom == true)
            {
                ScrollToBottom();
                shouldUpdateScrollToBottom = false;
            }
        }

        private void DoChatArea(Rect inRect)
        {
            // Chat display area (optional scroll view for chat history)
            chatDisplayRect = new Rect(0f, 0f, inRect.width, inRect.height - 55f);
            Widgets.BeginScrollView(chatDisplayRect, ref scrollPosition, new Rect(0f, 0f, chatDisplayRect.width - 16f, this.startingHeight));
            // Display chat messages here

            DrawChatMessageHistory(chatDisplayRect);

            Widgets.EndScrollView();

            // Text entry field at the bottom
            Rect textFieldRect = new Rect(0f, inRect.height - 50f, inRect.width * 0.95f, 32f);
            inputText = Widgets.TextField(textFieldRect, inputText);

            if (loadingAiResponse == true)
            {
                Rect loadingWidgetRect = new Rect(0f, inRect.height - 17f, inRect.width, 20f);
                Text.Font = GameFont.Tiny;
                Widgets.Label(loadingWidgetRect, "Awaiting response...");
            }
        }

        private void DrawChatMessageHistory(Rect chatDisplayRect)
        {
            float y = 0f;
            float totalContentHeight = 0f;

            foreach (ChatMessage message in chatHistory)
            {
                float messageHeight = message.DrawAndCalculateHeight(0f, y, chatDisplayRect.width - 16f, 5f);
                y += messageHeight;
                totalContentHeight += messageHeight;
            }

            this.startingHeight = Math.Max(totalContentHeight, chatDisplayRect.height);
        }

        private float GetChatContentHeight()
        {
            float contentHeight = 0f;
            foreach (ChatMessage message in chatHistory)
            {
                contentHeight += CalculateMessageHeight(message, chatDisplayRect.width);
            }
            return contentHeight;
        }

        private bool IsScrollBarAtBottom()
        {
            float contentHeight = GetChatContentHeight();

            if (scrollPosition.y >= contentHeight - chatDisplayRect.height - 20)
                return true;

            return false;
        }

        private void ScrollToBottom()
        {
            float contentHeight = GetChatContentHeight();
            scrollPosition.y = contentHeight - chatDisplayRect.height + 20;
        }

        private float CalculateMessageHeight(ChatMessage message, float width)
        {
            float buffer = 10f;  // A small buffer to account for rounding issues
            return Text.CalcHeight(message.messageText, width) + buffer;
        }

        public override void OnAcceptKeyPressed()
        {
            // Call a method to handle the sending of the message
            SendMessage();
            // Do not call the base method, as that would close the window
        }

        public void AiSendMessage(Pawn pawn, string message)
        {
            bool wasScrollbarAtBottom = IsScrollBarAtBottom();
            Log.Message($"Was scrollbar at bottom?: {wasScrollbarAtBottom}");
            ChatMessage newMessage = new ChatMessage(pawn, message);
            chatHistory.Add(newMessage);

            if (wasScrollbarAtBottom)
            {
                shouldUpdateScrollToBottom = true;
                Log.Message($"Scrolling to bottom");
            }
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }

            if (loadingAiResponse == true)
            {
                return;
            }
            // Code to handle the message sending
            // ...

            // Clear the input field after sending
            bool wasScrollbarAtBottom = IsScrollBarAtBottom();
            Log.Message($"Was scrollbar at bottom?: {wasScrollbarAtBottom}");
            ChatMessage newMessage = new ChatMessage(selfPawn, inputText);
            //Log.Message($"New message: {newMessage.messageText}");
            chatHistory.Add(newMessage);
            inputText = "";
            //Log.Message($"New message 2: {newMessage.messageText}");

            if (wasScrollbarAtBottom)
            {
                shouldUpdateScrollToBottom = true;
                Log.Message($"Scrolling to bottom");
            }

            OnMessageSent?.Invoke(newMessage.pawn, newMessage.messageText);



        }
    }
}
