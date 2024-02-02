using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.LLMs.Networking.SerializableTypes
{
    [Serializable]
    public class MessageRequest
    {
        public string model;
        public Message[] messages;
    }
}
