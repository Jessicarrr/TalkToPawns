using System;
using UnityEngine;
using Verse;

namespace AiConversations.GUI
{
    public class ReadOnlyTextWindow : Window
    {
        private string displayText = "";
        private Vector2 scrollPosition;

        private float startingHeight = 500f; // Initial window height
        private float startingWidth = 500f; // Initial window width

        // Constructor to set window properties
        public ReadOnlyTextWindow(string text)
        {
            this.displayText = text;
            this.doCloseX = true;
            this.draggable = true;
            this.preventCameraMotion = false;
            this.resizeable = true;
            this.optionalTitle = "Prompt Variables";
            //this.doCloseButton = true;
        }

        public override Vector2 InitialSize
        {
            get { return new Vector2(startingWidth, startingHeight); }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect textDisplayRect = new Rect(0f, 0f, inRect.width, inRect.height);
            Widgets.BeginScrollView(textDisplayRect, ref scrollPosition, new Rect(0f, 0f, textDisplayRect.width - 16f, Text.CalcHeight(displayText, textDisplayRect.width - 16f)));

            // Display the read-only text
            Widgets.TextArea(new Rect(0f, 0f, textDisplayRect.width - 48f, Text.CalcHeight(displayText, textDisplayRect.width)), displayText);

            Widgets.EndScrollView();
        }

        // Example method to set or update the text displayed in the window
        public void UpdateDisplayText(string newText)
        {
            this.displayText = newText;
        }
    }
}
