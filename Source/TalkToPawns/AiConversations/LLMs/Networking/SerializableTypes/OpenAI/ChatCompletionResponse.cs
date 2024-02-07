using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AiConversations.LLMs.Networking.SerializableTypes
{
    [DataContract]
    public class ChatCompletionResponse
    {
        public string id;
        [DataMember(Name = "object")]
        public string obj;
        public int created;
        public string model;
        public Choice[] choices;
        public Usage usage;
        public string system_fingerprint;

        public override string ToString()
        {
            var choicesStr = choices == null ? "null" : string.Join(", ", choices.Select(c => c.ToString()));
            return $"ID: {id}, Created: {created}, Model: {model}, Choices: [{choicesStr}], Usage: {usage}";
        }
    }

}
