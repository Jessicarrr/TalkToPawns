using AiConversations.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.Controllers
{
    public class ChatController
    {
        private ChatWindow window;
        private static ChatController instance;

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

            window.UpdatePawns(selfPawn, talkedToPawn);
            Find.WindowStack.Add(window);
        }

        private void HandleUserSentMessage(Pawn sender, string message)
        {
            // Implement your logic here
            // For example, set the loading state and call the API
            //window.loadingAiResponse = true;

            // Call your API or other code...
        }
    }
}
