using AiConversations.GUI;
using AiConversations.GUI.ModSettingsPartials;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AiConversations
{
    public class TTPModSettings : ModSettings
    {
        // AI Selection Area Settings
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectionAreaTopOffset = 0.0f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectionAreaHeightMult = 0.22f;

        

        // AI Selection offsets and multipliers
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectLabelLeftOffset = 0.01f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectLabelTopOffset = 0.00f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectLabelWidthMult = 0.98f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectLabelHeightMult = 0.43f;

        // Dropdown for AI selection
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectLeftOffset = 0.01f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectTopOffset = 0.45f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectWidthMult = 0.98f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float aiSelectHeightMult = 0.35f;

        static string aiSelectDescription = "Select an AI that will speak on behalf of your pawns. OpenAI / ChatGPT is an online service that's more straightforward but costs some amount of money per message, depending on which model you use. Koboldcpp is a program which allows you to run an AI on your computer locally. This requires a strong computer but is free and better for privacy. 'None' means this mod is effectively disabled.";

 
        internal static Enums.API selectedAiType = Enums.API.None;

        EnumDropdownMenu<Enums.API> aiSelectDropdownMenu;

        internal static SettingsArea_ChatGPT chatGPTSettings = new SettingsArea_ChatGPT();
        internal static SettingsArea_General generalSettings = new SettingsArea_General();

        public void setup()
        {
            aiSelectDropdownMenu = new EnumDropdownMenu<Enums.API>(selectedAiType);
            aiSelectDropdownMenu.OnDropdownItemSelected += selectedItem =>
            {
                selectedAiType = selectedItem;
                Log.Message("Selected AI type: " + selectedAiType);
            };
        }

        // Save settings
        public override void ExposeData()
        {
            Scribe_Values.Look(ref selectedAiType, "selectedAiType", Enums.API.None, true);
            Scribe_Values.Look(ref chatGPTSettings.openAiApiKey, "openAiApiKey");
            Scribe_Values.Look(ref chatGPTSettings.selectedOpenAiModel, "selectedOpenAiModel", "gpt-3.5-turbo", true);
            Scribe_Values.Look(ref generalSettings.prompt, "prompt", "", true);
            Scribe_Values.Look(ref generalSettings.temperature, "temperature", "", true);
            Scribe_Values.Look(ref generalSettings.maxTokens, "maxTokens", "", true);
            Scribe_Values.Look(ref generalSettings.topP, "topP", "", true);
            Scribe_Values.Look(ref generalSettings.frequencyPenalty, "frequencyPenalty", "", true);
            base.ExposeData();
        }

        // Display settings menu
        public void DisplaySettingsMenu(Rect inRect)
        {
            // AI Selection Area
            Rect aiSelectionArea = new Rect(inRect.x, inRect.y + inRect.height * aiSelectionAreaTopOffset, inRect.width, inRect.height * aiSelectionAreaHeightMult);
            DrawAISelectionArea(aiSelectionArea);

            // ChatGPT Settings Area
            if (selectedAiType == Enums.API.ChatGPT)
            {
                DrawChatGPTSettings(inRect);
            }
        }

        public void DrawChatGPTSettings(Rect inRect)
        {
            float chatGPTSettingsAreaTopOffset = 0.228f;
            float chatGPTSettingsAreaHeightMult = 0.30f;
            Rect chatGPTSettingsArea = new Rect(inRect.x, inRect.y + inRect.height * chatGPTSettingsAreaTopOffset, inRect.width, inRect.height * chatGPTSettingsAreaHeightMult);
            chatGPTSettings.Draw(chatGPTSettingsArea);

            float generalSettingsAreaTopOffset = 0.55f;
            float generalSettingsAreaHeightMult = 0.30f;
            Rect generalSettingsArea = new Rect(inRect.x, inRect.y + inRect.height * generalSettingsAreaTopOffset, inRect.width, inRect.height * generalSettingsAreaHeightMult);
            generalSettings.Draw(generalSettingsArea);
        }

        // Draw AI Selection Area
        private void DrawAISelectionArea(Rect area)
        {
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(area.x + area.width * aiSelectLabelLeftOffset, area.y + area.height * aiSelectLabelTopOffset, area.width * aiSelectLabelWidthMult, area.height * aiSelectLabelHeightMult), aiSelectDescription);

            aiSelectDropdownMenu.DrawDropdown(new Rect(area.x + area.width * aiSelectLeftOffset, area.y + area.height * aiSelectTopOffset, area.width * aiSelectWidthMult, area.height * aiSelectHeightMult));
        }
    }
}