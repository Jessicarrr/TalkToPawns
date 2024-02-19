using AiConversations;
using AiConversations.GUI;
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
    internal static ReadOnlyTextWindow window;

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
            "TtpAiType".Translate(),
            "TtpAiTypeDescription".Translate(),
            Enums.API.None);

        chatGptModelHandle = Settings.GetHandle("chatGptModel",
            "TtpChatGptModel".Translate(),
            "TtpChatGptModelDescription".Translate(),
            ChatGPTModel.gpt_3_5_turbo,
            null,
            "chatGptModel_");
        // Note: You should define translation keys like chatGptModel_gpt_3_5_turbo in your language files for proper display.

        includeChatPromptInMemoryPrompt = Settings.GetHandle("includeChatPromptInMemoryPrompt",
            "TtpChatPromptInMemoryPrompt".Translate(),
            "TtpChatPromptInMemoryPromptDesc".Translate(),
            false);

        apiKeyHandle = Settings.GetHandle("apiKey",
            "TtpApiKey".Translate(),
            "TtpApiKeyDescription".Translate(),
            "");

        promptHandle = Settings.GetHandle("prompt",
            "TtpDefaultPrompt".Translate(),
            "TtpDefaultPromptDesc".Translate(),
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
            "TtpSummaryPrompt".Translate(),
            "TtpSummaryPromptDesc".Translate(),
            "Your task is to evaluate how {recipient_name} " +
                "was treated in the conversation, and then provide a single summary with a relationship score modifier reflecting the overall interaction. Your response must conform to the following format: \"[score] [brief summary]\". Examples include \"+1 Enjoyable chat\" or \"-2 Insulted me\". The summary should be concise, limited to one sentence with no more than ten words, and must encapsulate the general tone of the interaction without enumerating specific events or responses. Provide only one summary and one score that reflects the aggregate sentiment of the conversation. The perspective should be that of {recipient_name}, offering a direct and summarized reflection of the treatment they received.");

        summaryPrompt.CustomDrawerHeight = 230;

        summaryPrompt.CustomDrawer = rect =>
        {
            // Existing logic to draw and handle the text area
            Rect textFieldRect = new Rect(rect.x, rect.y, rect.width, 180); // Adjusted height for the button
            string newValue = Widgets.TextArea(textFieldRect, summaryPrompt.Value);
            bool valueChanged = false;
            if (newValue != summaryPrompt.Value)
            {
                summaryPrompt.Value = newValue;
                valueChanged = true; // Mark as changed if new value is different
            }

            // Create a button below the text area
            Rect buttonRect = new Rect(rect.x, rect.y + textFieldRect.height + 10, rect.width, 30); // Adjust y position based on text field
            if (Widgets.ButtonText(buttonRect, "TtpViewPromptVariables".Translate()))
            {
                ViewPromptVariables(); // Call the empty function when the button is pressed
            }

            return valueChanged; // Return true if value changed, otherwise false
        };

        onlyShowBadHediffs = Settings.GetHandle("onlyShowBadHediffs",
            "TtpOnlyBadHealthConditions".Translate(),
            "TtpOnlyBadHealthConditionsDesc".Translate(),
            false);

        numMemoriesInPrompt = Settings.GetHandle("numMemoriesInPrompt",
            "TtpNumMemories".Translate(),
            "TtpNumMemoriesDesc".Translate(),
            4, Validators.FloatRangeValidator(1, 20));

        memoryTimeBase = Settings.GetHandle("memoryTimeBase",
            "TtpMemoryTimeBase".Translate(),
            "TtpMemoryTimeBaseDesc".Translate(),
            10, Validators.FloatRangeValidator(1, 700));

        memoryTimePerImpact = Settings.GetHandle("memoryPerImpact",
            "TtpMemoryExtraTime".Translate(),
            "TtpMemoryExtraTimeDesc".Translate(),
            5, Validators.FloatRangeValidator(1, 700));

        maximumMemories = Settings.GetHandle("maximumMemories",
            "TtpMaxMemories".Translate(),
            "TtpMaxMemoriesDescription".Translate(),
            100, Validators.FloatRangeValidator(1, 1000));

        temperatureHandle = Settings.GetHandle("temperature",
            "TtpTemperature".Translate(),
            "TtpTemperatureDesc".Translate(),
            0.7f, Validators.FloatRangeValidator(0f, 2f));

        maxTokensHandle = Settings.GetHandle("maxTokens",
            "TtpMaxTokens".Translate(),
            "TtpMaxTokensDesc".Translate(),
            512, Validators.IntRangeValidator(1, 512));

        maxTokensForMemoriesHandle = Settings.GetHandle("maxTokensForMemories",
            "TtpMaxTokensMemories".Translate(),
            "TtpMaxTokensMemoriesDesc".Translate(),
            35, Validators.IntRangeValidator(1, 512));

        topPHandle = Settings.GetHandle("topP",
            "TtpTopP".Translate(),
            "TtpTopPDesc".Translate(),
            0.92f, Validators.FloatRangeValidator(0f, 1f));

        frequencyPenaltyHandle = Settings.GetHandle("frequencyPenalty",
            "TtpFrequencyPenalty".Translate(),
            "TtpFrequencyPenaltyDesc".Translate(),
            1.1f, Validators.FloatRangeValidator(-2f, 2f));

        // Visibility control for ChatGPT Settings based on LLM Model selection
        chatGptModelHandle.VisibilityPredicate = () => llmModelHandle.Value == Enums.API.ChatGPT;
        apiKeyHandle.VisibilityPredicate = () => llmModelHandle.Value == Enums.API.ChatGPT;
        includeChatPromptInMemoryPrompt.VisibilityPredicate = () => llmModelHandle.Value == Enums.API.Kobold;

        // Additional logic might be required to dynamically update the description for chatGptModelHandle based on selection
    }

    internal void ViewPromptVariables()
    {
        string text = PromptParser.PrintExplanations();
        if (window == null)
        {
            window = new ReadOnlyTextWindow(text);
        }

        if (Find.WindowStack.IsOpen(window) == true)
        {
            return;
        }

        //PawnRelationshipTrackerLLM.AddTestMemory(talkedToPawn, selfPawn);

        Find.WindowStack.Add(window);
    }
}
