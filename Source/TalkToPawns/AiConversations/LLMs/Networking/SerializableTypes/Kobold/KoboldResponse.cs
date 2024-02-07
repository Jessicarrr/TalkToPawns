using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiConversations.LLMs.Networking.SerializableTypes.Kobold
{
    public class KoboldResponse
    {
        public Results[] results { get; set; }
    }

    public class Results
    {
        public string text { get; set; }
    }
}
