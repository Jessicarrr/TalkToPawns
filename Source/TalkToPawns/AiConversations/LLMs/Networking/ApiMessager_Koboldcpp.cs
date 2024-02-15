using AiConversations.GUI;
using AiConversations.LLMs;
using AiConversations.LLMs.Networking.Extensions;
using AiConversations.LLMs.Networking.SerializableTypes;
using AiConversations.LLMs.Networking.SerializableTypes.Kobold;
using JsonFx.Json;
using JsonFx.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Verse; // Ensure this is included for UnityWebRequest

public class ApiMessager_KoboldCpp : ApiMessager
{
    public new string baseUrl = "http://localhost:5001/api/v1/generate/";

    private string RemoveSurrogatePairs(string text)
    {
        string pattern = @"\\u[a-fA-F0-9]{4}\\u[a-fA-F0-9]{4}";
        string sanitized = Regex.Replace(text, pattern, "");
        return sanitized;
    }

    private string RemoveAllPossiblyInvalidChars(string text)
    {
        string pattern = @"\\u[a-fA-F0-9]{4}";
        string sanitized = Regex.Replace(text, pattern, "");
        return sanitized;
    }

    internal override string GetMessageFromResponse(string response)
    {
        try
        {
            var reader = new JsonReader();
            var responseButRemoveIllegalChars = RemoveSurrogatePairs(response);
            var result = reader.Read<KoboldResponse>(responseButRemoveIllegalChars);
            var aiResponseText = result.results[0].text;
            int cutOffIndex = aiResponseText.IndexOf("##");
            string final = "";
            if (cutOffIndex != -1) // If "##" is found
            {
                final = aiResponseText.Substring(0, cutOffIndex);
                // Use the result as needed
                return final.TrimStart('\n', ' ').TrimEnd('\n', ' ');
            }
            else
            {
                // "##" was not found in the string, handle accordingly
                return final = aiResponseText.TrimStart('\n', ' ', '#').TrimEnd('\n', ' ', '#'); ; // Outputs the original string, as "##" was not found
            }
        }
        catch (SerializationException e)
        {
            try
            {
                var reader = new JsonReader();
                var responseButRemoveIllegalChars = RemoveAllPossiblyInvalidChars(response);
                var result = reader.Read<KoboldResponse>(responseButRemoveIllegalChars);
                var aiResponseText = result.results[0].text;
                int cutOffIndex = aiResponseText.IndexOf("##");
                string final = "";
                if (cutOffIndex != -1) // If "##" is found
                {
                    final = aiResponseText.Substring(0, cutOffIndex);
                    // Use the result as needed
                    return final.TrimStart('\n', ' ').TrimEnd('\n', ' ');
                }
                else
                {
                    // "##" was not found in the string, handle accordingly
                    return final = aiResponseText.TrimStart('\n', ' ', '#').TrimEnd('\n', ' ', '#'); ; // Outputs the original string, as "##" was not found
                }
            }
            catch (SerializationException e2)
            {
                Log.Message("SerializationException in ApiMessager_Koboldcpp:\n" + response + "\n" + e2.Message + "\n" + e2.StackTrace);
                return "There was an error parsing the AI's response: " + e2.Message;
            }
        }
    }

    public override async void RequestChatMemory(Pawn initiator, Pawn talkedToPawn, List<ChatMessage> chatHistory, string chatPrompt, string memoryPrompt)
    {
        var modSettings = TTPModSettings.GetInstance();
        // For this example, we're simplifying the process. You'll need to adjust this to fit your actual requirements.
        string messages = "" ;

        if(modSettings.includeChatPromptInMemoryPrompt.Value == true)
        {
            messages += chatPrompt;
        }
        //string messages = chatPrompt + "\n" ;

        // Combine chat history into the prompt if needed
        foreach (var msg in chatHistory)
        {
            messages += "### "+ msg.pawn.Name.ToStringFull + ": " + msg.messageText + "\n";
        }

        messages += memoryPrompt;
        //messages += "\n### Response: ";
        

        var payload = new
        {
            prompt = messages,
            temperature = modSettings.temperatureHandle.Value,
            top_p = modSettings.topPHandle.Value,
            max_length = modSettings.maxTokensForMemoriesHandle.Value,
            rep_pen = modSettings.frequencyPenaltyHandle.Value,
            stop_sequence = new string[] { "#", initiator.Name.ToStringFull + ":" },
        };

        JsonWriter writer = new JsonWriter();
        string jsonData = writer.Write(payload);

        await SendPostRequestAsync(talkedToPawn, initiator, jsonData, true);
    }

    public override async void Send(Pawn initiator, Pawn talkedToPawn, List<ChatMessage> chatHistory, string prompt)
    {
        string messageToSend = "### " + prompt;
        string messages = "";

        foreach(ChatMessage message in chatHistory)
        {
            messages += "### " + message.pawn.Name.ToStringFull + ": " + message.messageText;
        }

        messages += "### " + talkedToPawn.Name.ToStringFull + ": ";

        messageToSend += messages;
        var modSettings = TTPModSettings.GetInstance();

        var payload = new
        {
            prompt = messageToSend,
            temperature = modSettings.temperatureHandle.Value,
            top_p = modSettings.topPHandle.Value,
            max_length = modSettings.maxTokensHandle.Value,
            rep_pen = modSettings.frequencyPenaltyHandle.Value,
            stop_sequence = new string[] { "##", initiator.Name.ToStringFull + ":" },
        };

        JsonWriter writer = new JsonWriter();
        string jsonData = writer.Write(payload);
        Log.Message("Sending to Koboldcpp: " + jsonData);

        await SendPostRequestAsync(talkedToPawn, initiator, jsonData);
    }

    private async Task<string> SendPostRequestAsync(Pawn ai, Pawn player, string jsonData, bool isSummary = false)
    {
        try
        {
            var request = new UnityWebRequest(baseUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            //request.SetRequestHeader("Content-Type", "application/json");

            string responseText = await request.SendWebRequestAsync();
            Log.Message("Koboldcpp Response: " + responseText);
            string message = GetMessageFromResponse(responseText);
            InvokeEvent(ai, player, message, isSummary);

            return responseText;
        }
        catch (Exception ex)
        {
            Log.Message($"KoboldCpp Exception: {ex.Message}, stacktrace:\n{ex.StackTrace}");
            InvokeEvent(ai, player, ex.Message);
            return null;
        }

        
    }
    
}
