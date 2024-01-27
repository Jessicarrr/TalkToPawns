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

        public void Draw(Rect rect)
        {
            // Draw the pawn's name
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(rect.x, rect.y, rect.width, 20f), pawn.Name.ToStringFull);

            // Draw the message
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(rect.x, rect.y + 20f, rect.width, rect.height - 20f), messageText);
        }
    }
}
