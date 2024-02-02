using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AiConversations.GUI.ModSettingsPartials
{
    internal class SettingsArea_ChatGPT : SettingsArea
    {
        // ChatGPT Settings offsets and multipliers
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float llmTitleLeftOffset = 0.00f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float llmTitleTopOffset = 0.01f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float llmTitleWidthMult = 0.98f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float llmTitleHeightMult = 0.25f;

        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelLabelLeftOffset = 0.00f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelLabelTopOffset = 0.25f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelLabelWidthMult = 0.43f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelLabelHeightMult = 0.25f;

        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelDropdownLeftOffset = 0.0f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelDropdownTopOffset = 0.42f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelDropdownWidthMult = 0.437f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelDropdownHeightMult = 0.22f;

        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelDescriptionLeftOffset = 0.0f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelDescriptionTopOffset = 0.64f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelDescriptionWidthMult = 0.43f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float modelDescriptionHeightMult = 0.344f;

        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyLabelLeftOffset = 0.53f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyLabelTopOffset = 0.25f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyLabelWidthMult = 0.43f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyLabelHeightMult = 0.25f;

        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyTextFieldLeftOffset = 0.53f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyTextFieldTopOffset = 0.437f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyTextFieldWidthMult = 0.43f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyTextFieldHeightMult = 0.15f;

        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyDescriptionLeftOffset = 0.53f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyDescriptionTopOffset = 0.64f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyDescriptionWidthMult = 0.43f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float apiKeyDescriptionHeightMult = 0.33f;

        internal string modelLabelText = "Model - from least sophisticated to most sophisticated";

        internal string modelDescriptionGpt35Turbo = "gpt-3.5-turbo: currently the oldest version of GPT. 3.5 makes some mistakes but overall works well.";
        internal string modelDescriptionGpt35Turbo16 = "gpt-3.5-turbo-16k: Has a 16k context size instead of the 4k context size found in gpt-3.5-turbo, which means it remembers more. Makes some mistakes but overall works well. Could cost approximately $0.70 USD after a few hours of moderate usage.";
        internal string modelDescriptionGpt35Turbo1106 = "gpt-3.5-turbo-1106: Has a 16k context size instead of the 4k context size found in gpt-3.5-turbo, which means it remembers more. Makes some mistakes but overall works well. Follows instructions better than gpt-3.5-turbo-16k.";
        internal string modelDescriptionGpt4 = "gpt-4: the model capable of advanced reasoning skills, much more sophisticated than gpt 3.5. Context size is 8k tokens. This is also the most expensive model to run. For example, I sent 4 very long messages to GPT 4 and got charged $0.17 USD.";
        internal string modelDescriptionCurrent = "";

        static string apiKeyLabelText = "API Key";
        static string apiKeyDescriptionText = "Links your ChatGPT usage to your OpenAI account. You can create an API key on the OpenAI Playground website. Once done, paste it here. Mod will not work in ChatGPT mode without this key.";

        

        StringDropdownMenu modelSelectDropdownMenu;

        public string selectedOpenAiModel = "gpt-3.5-turbo";
        public string openAiApiKey = "";

        public SettingsArea_ChatGPT()
        {
            modelDescriptionCurrent = modelDescriptionGpt35Turbo;
            setup();
        }

        internal void setup()
        {
            modelSelectDropdownMenu = new StringDropdownMenu(
                new List<string>()
                {
                    "gpt-3.5-turbo", "gpt-3.5-turbo-16k", "gpt-3.5-turbo-1106", "gpt-4"
                },
                selectedOpenAiModel
            );

            modelSelectDropdownMenu.OnDropdownItemSelected += selectedItem =>
            {
                selectedOpenAiModel = selectedItem;
                Log.Message("Selected model: " + selectedOpenAiModel);

                switch (selectedOpenAiModel)
                {
                    case "gpt-3.5-turbo":
                        modelDescriptionCurrent = modelDescriptionGpt35Turbo;
                        break;
                    case "gpt-3.5-turbo-16k":
                        modelDescriptionCurrent = modelDescriptionGpt35Turbo16;
                        break;
                    case "gpt-3.5-turbo-1106":
                        modelDescriptionCurrent = modelDescriptionGpt35Turbo1106;
                        break;
                    case "gpt-4":
                        modelDescriptionCurrent = modelDescriptionGpt4;
                        break;

                    default:
                        modelDescriptionCurrent = modelDescriptionGpt35Turbo;
                        break;
                }
            };
        }

        public override void Draw(Rect area)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(area.x + area.width * llmTitleLeftOffset, area.y + area.height * llmTitleTopOffset, area.width * llmTitleWidthMult, area.height * llmTitleHeightMult), "ChatGPT Settings");
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(area.x + area.width * modelLabelLeftOffset, area.y + area.height * modelLabelTopOffset, area.width * modelLabelWidthMult, area.height * modelLabelHeightMult), modelLabelText);
            modelSelectDropdownMenu.DrawDropdown(new Rect(area.x + area.width * modelDropdownLeftOffset, area.y + area.height * modelDropdownTopOffset, area.width * modelDropdownWidthMult, area.height * modelDropdownHeightMult));

            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(area.x + area.width * modelDescriptionLeftOffset, area.y + area.height * modelDescriptionTopOffset, area.width * modelDescriptionWidthMult, area.height * modelDescriptionHeightMult), modelDescriptionCurrent);

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(area.x + area.width * apiKeyLabelLeftOffset, area.y + area.height * apiKeyLabelTopOffset, area.width * apiKeyLabelWidthMult, area.height * apiKeyLabelHeightMult), apiKeyLabelText);
            openAiApiKey = Widgets.TextArea(new Rect(area.x + area.width * apiKeyTextFieldLeftOffset, area.y + area.height * apiKeyTextFieldTopOffset, area.width * apiKeyTextFieldWidthMult, area.height * apiKeyTextFieldHeightMult), openAiApiKey);

            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(area.x + area.width * apiKeyDescriptionLeftOffset, area.y + area.height * apiKeyDescriptionTopOffset, area.width * apiKeyDescriptionWidthMult, area.height * apiKeyDescriptionHeightMult), apiKeyDescriptionText);
        }
    }
}
