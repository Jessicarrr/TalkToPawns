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
        private static ChatWindow singletonInstance;

        private Pawn selfPawn;
        private Pawn talkedToPawn;

        private String inputText = "";
        private Vector2 scrollPosition;

        private float startingHeight = 300f;
        private float startingWidth = 300f;

        private Rect chatDisplayRect;

        private bool shouldUpdateScrollToBottom = false;

        private bool loadingAiResponse = false;

        private List<ChatMessage> chatHistory = new List<ChatMessage>();


        public static ChatWindow GetSingletonInstance()
        {
            if (singletonInstance == null)
            {
                singletonInstance = new ChatWindow();
                
            }
            return singletonInstance;
        }

        public void UpdatePawns(Pawn selfPawn, Pawn talkedToPawn)
        {
            singletonInstance.selfPawn = selfPawn;
            singletonInstance.talkedToPawn = talkedToPawn;
            this.optionalTitle = "Chatting with " + talkedToPawn.LabelShort + " as " + selfPawn.LabelShort;
            chatHistory.Clear();
        }

        private ChatWindow()
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
            //Widgets.Label(new Rect(0, 0, inRect.width, 30f), "Chat Window");

            DoChatArea(inRect);
        }

        private void DoChatArea(Rect inRect)
        {
            // Chat display area (optional scroll view for chat history)
            chatDisplayRect = new Rect(0f, 0f, inRect.width, inRect.height - 35f);
            Widgets.BeginScrollView(chatDisplayRect, ref scrollPosition, new Rect(0f, 0f, chatDisplayRect.width - 16f, this.startingHeight));
            // Display chat messages here

            DrawChatMessageHistory(chatDisplayRect);

            Widgets.EndScrollView();

            Rect loadingWidgetRect = new Rect(0f, inRect.height - 32f, inRect.width * 0.20f, 5f);

            // Text entry field at the bottom
            Rect textFieldRect = new Rect(0f, inRect.height - 30f, inRect.width * 0.95f, 30f);
            inputText = Widgets.TextField(textFieldRect, inputText);

            if (shouldUpdateScrollToBottom == true)
            {
                ScrollToBottom();
                shouldUpdateScrollToBottom = false;
            }
        }

        private void DrawChatMessageHistory(Rect chatDisplayRect)
        {
            float y = 0f;
            foreach (ChatMessage message in chatHistory)
            {
                // Calculate the height for this message
                float messageHeight = CalculateMessageHeight(message, chatDisplayRect.width / 2); // Implement this based on the message length

                Rect messageRect = new Rect(0f, y, chatDisplayRect.width - 16f, messageHeight);
                message.Draw(messageRect);

                y += messageHeight; // Move down for the next message
            }

            // Update the scroll view height if necessary
            this.startingHeight = Math.Max(y, chatDisplayRect.height);

        }

        private float GetChatContentHeight()
        {
            float contentHeight = 0f;
            foreach (ChatMessage message in chatHistory)
            {
                contentHeight += CalculateMessageHeight(message, chatDisplayRect.width - 16f);
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
            // Implement logic to calculate height based on the message length
            // You might need to use Text.CalcHeight or similar methods
            return Text.CalcHeight(message.messageText, width) + 30;
        }

        public override void OnAcceptKeyPressed()
        {
            // Call a method to handle the sending of the message
            SendMessage();
            // Do not call the base method, as that would close the window
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
            

        }
    }
}
