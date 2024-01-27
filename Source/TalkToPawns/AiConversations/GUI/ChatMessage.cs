using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AiConversations.GUI
{
    public class ChatMessage
    {
        public Pawn pawn;
        public string messageText;

        public ChatMessage(Pawn pawn, string messageText)
        {
            this.pawn = pawn;
            this.messageText = messageText;
        }

        public float DrawAndCalculateHeight(float startX, float startY, float width, float heightPadding)
        {
            // Calculate dimensions
            Text.Font = GameFont.Small;
            float nameHeight = Text.CalcHeight(pawn.Name.ToStringFull, width - 70f);
            Rect nameRect = new Rect(startX + 50f, startY, width - 70f, nameHeight);

            Text.Font = GameFont.Tiny;
            float messageWidth = width - 70f;
            float messageHeight = Text.CalcHeight(messageText, messageWidth);
            Rect messageRect = new Rect(startX + 50f, startY + nameHeight, messageWidth, messageHeight);

            // Calculate the total height of name and message
            float totalTextHeight = nameHeight + messageHeight;

            // Calculate the Y position of the portrait to align it vertically center
            float portraitY = startY + (totalTextHeight - 60f) / 2;

            // Adjust portraitY to ensure it's not negative
            portraitY = Mathf.Max(portraitY, startY - 10f);

            // Draw the portrait
            Rect portraitRect = new Rect(startX, portraitY, 60f, 60f);
            Widgets.ThingIcon(portraitRect, pawn);

            // Draw the name and message
            Text.Font = GameFont.Small;
            Widgets.Label(nameRect, pawn.Name.ToStringFull);
            Text.Font = GameFont.Tiny;
            Widgets.Label(messageRect, messageText);

            // Calculate the total height, including padding
            var totalHeight = Mathf.Max(portraitRect.yMax, messageRect.yMax) - startY;
            return totalHeight + heightPadding;
        }

    }
}
