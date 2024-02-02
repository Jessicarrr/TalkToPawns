using AiConversations.LLMs.Networking.SerializableTypes;
using JsonFx.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiConversations.Parsing
{
    internal class JsonParser
    {
        public static ChatCompletionResponse ParseStringToDynamic(string json)
        {
            JsonReader reader = new JsonReader();
            ChatCompletionResponse response = new ChatCompletionResponse();
            ChatCompletionResponse tix = reader.Read(json, response);
            return tix;
        }
    }
}
