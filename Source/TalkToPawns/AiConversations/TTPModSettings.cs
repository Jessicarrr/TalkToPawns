using AiConversations;
using AiConversations.HelperClasses;
using HugsLib;
using HugsLib.Settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Verse;

public class TTPModSettings : ModBase
{
    public override string ModIdentifier => "TalkToPawnsMod";

    internal SettingHandle<Enums.API> llmModelHandle;
    internal SettingHandle<ChatGPTModel> chatGptModelHandle;
    internal SettingHandle<string> apiKeyHandle;
    internal SettingHandle<string> promptHandle;
    internal SettingHandle<float> temperatureHandle;
    internal SettingHandle<int> maxTokensHandle;
    internal SettingHandle<int> maxTokensForMemoriesHandle;
    internal SettingHandle<float> topPHandle;
    internal SettingHandle<float> frequencyPenaltyHandle;
    internal SettingHandle<string> summaryPrompt;
    internal SettingHandle<int> numMemoriesInPrompt;
    internal SettingHandle<bool> onlyShowBadHediffs;
    internal SettingHandle<bool> includeChatPromptInMemoryPrompt;
    internal SettingHandle<int> memoryTimeBase;
    internal SettingHandle<int> memoryTimePerImpact;
    internal SettingHandle<int> maximumMemories;

    internal enum ChatGPTModel { gpt_3_5_turbo, gpt_3_5_turbo_16k, gpt_3_5_turbo_1106, gpt_4 }

    private static TTPModSettings Instance;

    private TTPModSettings() 
    {
        Instance = this;
    }

    internal static string GptModelEnumToString(ChatGPTModel model)
    {
        switch(model)
        {
            case ChatGPTModel.gpt_3_5_turbo:
                return "gpt-3.5-turbo";
            case ChatGPTModel.gpt_3_5_turbo_16k:
                return "gpt-3.5-turbo-16k";
            case ChatGPTModel.gpt_3_5_turbo_1106:
                return "gpt-3.5-turbo-1106";
            case ChatGPTModel.gpt_4:
                return "gpt-4";
            default:
                return "ModelEnumNotFoundSorry";
        }
    }

    public static TTPModSettings GetInstance()
    {
        if (Instance == null)
        {
            Instance = new TTPModSettings();
        }

        return Instance;
    }

    public static string GetPromptVariableExplanations()
    {
        StringBuilder builder = new StringBuilder();
        foreach (PromptVariable promptVar in PromptParser.promptVariables)
        {
            builder.AppendLine(promptVar.placeholder + ": " + promptVar.explanation);
        }
        return builder.ToString();
    }

    public override void DefsLoaded()
    {
        llmModelHandle = Settings.GetHandle("llmModel",
            "AI Type",
            "Select an AI that will speak on behalf of your pawns.",
            Enums.API.None);

        chatGptModelHandle = Settings.GetHandle("chatGptModel",
            "ChatGPT Model",
            "Choose the GPT model to use when ChatGPT is selected.",
            ChatGPTModel.gpt_3_5_turbo,
            null,
            "chatGptModel_");
        // Note: You should define translation keys like chatGptModel_gpt_3_5_turbo in your language files for proper display.

        includeChatPromptInMemoryPrompt = Settings.GetHandle("includeChatPromptInMemoryPrompt",
            "Include chat prompt in memory prompt?",
            "Includes the 'prompt' into the prompt for creating memories. This adds more characters to the prompt overall but might give context to the conversation.",
            false);

        apiKeyHandle = Settings.GetHandle("apiKey",
            "API Key",
            "API key for OpenAI service. This can be gotten from the OpenAI Playground website. Requires an account.",
            "");

        promptHandle = Settings.GetHandle("prompt",
            "Default Prompt",
            "Default prompt used for AI interactions. Here are the variables you may use:\n" + GetPromptVariableExplanations(),
            "{recipient_name} is a {recipient_age}-year-old {recipient_gender} from a RimWorld. Backstory: {recipient_backstory}. But this was {recipient_name}'s previous life. Now, {recipient_name} is currently talking to {initiator_name}, a {initiator_age} year old {initiator_gender}, while also {recipient_current_action}. {relation_to_initiator} - {recipient_name}'s traits: {recipient_traits_list}. {recipient_name} is in a {recipient_mood} mood. {recipient_name}'s needs: {say_recipient_low_needs}. {recipient_name}'s health conditions: {recipient_health_conditions}. Summary of {recipient_name}'s impressions of {initiator_name}: {recipient_memories_with_initiator}");

        promptHandle.CustomDrawerHeight = 185;

        // Custom drawer for a larger textbox
        promptHandle.CustomDrawer = rect =>
        {
            // Increase the height for the textbox
            Rect textFieldRect = new Rect(rect.x, rect.y, rect.width, 180);
                                                                           // Use GUI.TextField to draw the textbox, and update the handle's value with the result
            string newValue = Widgets.TextArea(textFieldRect, promptHandle.Value);
            if (newValue != promptHandle.Value)
            {
                promptHandle.Value = newValue;
                return true; // Return true to indicate that the value has changed
            }
            return false; // Return false if the value hasn't changed
        };

        summaryPrompt = Settings.GetHandle("summaryPrompt",
            "Prompt for AI to create memories",
            "Must instruct the AI to make a response that adheres to this format: '[number] [description]' - such as '+1 Good talk' or '-1 Insulted' or '2 Deep talk'. Not using this format will make the memories fail to form.",
            "Your task is to evaluate how {recipient_name} " +
                "was treated in the conversation, and then provide a single summary with a relationship score modifier reflecting the overall interaction. Your response must conform to the following format: \"[score] [brief summary]\". Examples include \"+1 Enjoyable chat\" or \"-2 Insulted me\". The summary should be concise, limited to one sentence with no more than ten words, and must encapsulate the general tone of the interaction without enumerating specific events or responses. Provide only one summary and one score that reflects the aggregate sentiment of the conversation. The perspective should be that of {recipient_name}, offering a direct and summarized reflection of the treatment they received.");

        summaryPrompt.CustomDrawerHeight = 185;

        // Custom drawer for a larger textbox
        summaryPrompt.CustomDrawer = rect =>
        {
            // Increase the height for the textbox
            Rect textFieldRect = new Rect(rect.x, rect.y, rect.width, 180);
            // Use GUI.TextField to draw the textbox, and update the handle's value with the result
            string newValue = Widgets.TextArea(textFieldRect, summaryPrompt.Value);
            if (newValue != summaryPrompt.Value)
            {
                summaryPrompt.Value = newValue;
                return true; // Return true to indicate that the value has changed
            }
            return false; // Return false if the value hasn't changed
        };

        onlyShowBadHediffs = Settings.GetHandle("onlyShowBadHediffs",
            "Only show AI bad health conditions?",
            "Only tell the AI about bad health conditions. This can keep the prompt less cluttered by random harmless conditions.",
            false);

        numMemoriesInPrompt = Settings.GetHandle("numMemoriesInPrompt",
            "Num memories in {recipient/initiator_memories_with_initiator}",
            "How many memories will be put into the prompt if you use {recipient_memories_list} or {initiator_memories_list}. Keeping this short can make the AI less confused, keeping it long can add more context to the conversation.",
            4, Validators.FloatRangeValidator(1, 20));

        memoryTimeBase = Settings.GetHandle("memoryTimeBase",
            "Base time how long memories last (days)",
            "The base time it takes for a memory to fade away (in days). Memories will always last at least this long.",
            10, Validators.FloatRangeValidator(1, 700));

        memoryTimePerImpact = Settings.GetHandle("memoryPerImpact",
            "Additional time memories last (days)",
            "The extra time for memories to fade away (in days). This is based on how impactful the memory is. For example a memory like 'Insulted me: -1' will add this amount of time. -2 will give twice that amount of days.",
            5, Validators.FloatRangeValidator(1, 700));

        maximumMemories = Settings.GetHandle("maximumMemories",
            "Maximum amount of memories to track",
            "How many memories this mod will handle before it starts to delete the memories that were going to expire the soonest",
            100, Validators.FloatRangeValidator(1, 1000));

        temperatureHandle = Settings.GetHandle("temperature",
            "Temperature",
            "Set the temperature. This controls how much 'randomness' or unpredictability to use. Higher values mean more creative. Lower values mean more deterministic.",
            0.7f, Validators.FloatRangeValidator(0f, 2f));

        maxTokensHandle = Settings.GetHandle("maxTokens",
            "Max Tokens For AI Messages",
            "Maximum number of 'tokens' the AI may use before it's cut off. More tokens means this will allow longer responses.",
            512, Validators.IntRangeValidator(1, 512));

        maxTokensForMemoriesHandle = Settings.GetHandle("maxTokensForMemories",
            "Max Tokens For Memories",
            "Maximum number of tokens when asking the AI to form a memory. More tokens means this will allow longer responses.",
            35, Validators.IntRangeValidator(1, 512));

        topPHandle = Settings.GetHandle("topP",
            "Top P",
            "Top P setting",
            0.92f, Validators.FloatRangeValidator(0f, 1f));

        frequencyPenaltyHandle = Settings.GetHandle("frequencyPenalty",
            "Frequency Penalty",
            "Frequency penalty for repetition. Higher the penalty, the less tolerant of repetition the model will be.",
            1.1f, Validators.FloatRangeValidator(-2f, 2f));

        // Visibility control for ChatGPT Settings based on LLM Model selection
        chatGptModelHandle.VisibilityPredicate = () => llmModelHandle.Value == Enums.API.ChatGPT;
        apiKeyHandle.VisibilityPredicate = () => llmModelHandle.Value == Enums.API.ChatGPT;
        includeChatPromptInMemoryPrompt.VisibilityPredicate = () => llmModelHandle.Value == Enums.API.Kobold;

        // Additional logic might be required to dynamically update the description for chatGptModelHandle based on selection
    }
}
