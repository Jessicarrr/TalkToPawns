using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiConversations.LLMs.Networking.SerializableTypes
{
    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}
