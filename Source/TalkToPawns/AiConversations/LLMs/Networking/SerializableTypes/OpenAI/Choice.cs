using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiConversations.LLMs.Networking.SerializableTypes
{
    [Serializable]
    public class Choice
    {
        public int index;
        public Message message;
    }
}
