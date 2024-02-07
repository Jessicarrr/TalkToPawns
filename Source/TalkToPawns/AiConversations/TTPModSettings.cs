using AiConversations;
using AiConversations.HelperClasses;
using HugsLib;
using HugsLib.Settings;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
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
    internal SettingHandle<float> topPHandle;
    internal SettingHandle<float> frequencyPenaltyHandle;

    internal string summaryPrompt = "Your task is to evaluate how {recipient_name} was treated in the conversation, and then provide a single summary with a relationship score modifier reflecting the overall interaction. Your response must conform to the following format: \"[score] [brief summary]\". Examples include \"+1 Enjoyable chat\" or \"-2 Insulted me\". The summary should be concise, limited to one sentence with no more than ten words, and must encapsulate the general tone of the interaction without enumerating specific events or responses. Provide only one summary and one score that reflects the aggregate sentiment of the conversation. Do not list multiple interactions or provide a detailed breakdown. The perspective should be that of {recipient_name}, offering a direct and summarized reflection of the treatment they received.";

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

        apiKeyHandle = Settings.GetHandle("apiKey",
            "API Key",
            "API key for OpenAI service. This can be gotten from the OpenAI Playground website. Requires an account.",
            "");

        promptHandle = Settings.GetHandle("prompt",
            "Default Prompt",
            "Default prompt used for AI interactions. Here are the variables you may use:\n" + GetPromptVariableExplanations(),
            "This conversation takes place on a planet known as a RimWorld, populated mostly by humans." +
            " Your name is {recipient_name}, and you are a {recipient_age} year old {recipient_gender}." +
            " You are talking to {initiator_name}, who is a {initiator_age} year old {initiator_gender}." +
            " Your traits are: {recipient_traits_list}. Try your best to match your tone to your traits." +
            " Your thoughts about {initiator_name} are as such: {opinion_on_initiator}." +
            " You are currently {recipient_current_action}. {say_if_recipient_is_trader}" +
            " Your current mood is {recipient_mood}. Your current needs are: {say_recipient_low_needs}." +
            " {say_if_recipient_is_slave}. You have the following health conditions:" +
            " {recipient_health_conditions}. When talking about these facts, try to use different" +
            " wording than the wording used here. Your most recent memories are:" +
            " {recipient_recent_memories}");

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

        temperatureHandle = Settings.GetHandle("temperature",
            "Temperature",
            "Set the temperature. This controls how much 'randomness' or unpredictability to use. Higher values mean more creative. Lower values mean more deterministic.",
            0.2f, Validators.FloatRangeValidator(0f, 2f));

        maxTokensHandle = Settings.GetHandle("maxTokens",
            "Max Tokens",
            "Maximum number of tokens. More tokens means this will allow longer responses.",
            512, Validators.IntRangeValidator(1, 512));

        topPHandle = Settings.GetHandle("topP",
            "Top P",
            "Top P setting",
            1f, Validators.FloatRangeValidator(0f, 1f));

        frequencyPenaltyHandle = Settings.GetHandle("frequencyPenalty",
            "Frequency Penalty",
            "Frequency penalty for repetition. Higher the penalty, the less tolerant of repetition the model will be.",
            0f, Validators.FloatRangeValidator(-2f, 2f));

        // Visibility control for ChatGPT Settings based on LLM Model selection
        chatGptModelHandle.VisibilityPredicate = () => llmModelHandle.Value == Enums.API.ChatGPT;
        apiKeyHandle.VisibilityPredicate = () => llmModelHandle.Value == Enums.API.ChatGPT;

        // Additional logic might be required to dynamically update the description for chatGptModelHandle based on selection
    }
}
