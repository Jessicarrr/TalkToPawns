using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AiConversations.GUI.ModSettingsPartials
{
    internal class SettingsArea_General : SettingsArea
    {
        public string prompt = "This conversation takes place on a planet known as a RimWorld, populated mostly by humans. Your name is {recipient_name}, and you are a {recipient_age} year old {recipient_gender}. You are talking to {initiator_name}. Your traits are: {recipient_traits_list}. Your thoughts about {initiator_name} are as such: {opinion_on_initiator}. You are currently {recipient_current_action}. {say_if_recipient_is_trader} Your current mood is {recipient_mood}. {say_any_low_needs} {say_if_recipient_is_slave} Your most recent memories are: {recipient_recent_memories}.";
        public string promptDescription = "Prompt - tell the AI how to act";

        public string temperature = "1.0";
        public string maxTokens = "256";
        public string topP = "1.0";
        public string frequencyPenalty = "0.0";

        // ChatGPT Settings offsets and multipliers
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float titleLeftOffset = 0.00f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float titleTopOffset = 0.01f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float titleWidthMult = 0.98f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float titleHeightMult = 0.25f;

        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float promptLabelLeftOffset = 0.00f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float promptLabelTopOffset = 0.25f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float promptLabelWidthMult = 0.43f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float promptLabelHeightMult = 0.25f;

        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float promptTextAreaLeftOffset = 0.0f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float promptTextAreaTopOffset = 0.42f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float promptTextAreaWidthMult = 0.47f;
        [TweakValue("dropdown ai select offset", 0f, 1f)]
        static float promptTextAreaHeightMult = 0.80f;

        public override void Draw(Rect area)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(area.x + area.width * titleLeftOffset, area.y + area.height * titleTopOffset, area.width * titleWidthMult, area.height * titleHeightMult), "General Settings");

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(area.x + area.width * promptLabelLeftOffset, area.y + area.height * promptLabelTopOffset, area.width * promptLabelWidthMult, area.height * promptLabelHeightMult), promptDescription);

            prompt = Widgets.TextArea(new Rect(area.x + area.width * promptTextAreaLeftOffset, area.y + area.height * promptTextAreaTopOffset, area.width * promptTextAreaWidthMult, area.height * promptTextAreaHeightMult), prompt);

            Rect tempRect = new Rect(
                area.x + area.width * (promptLabelLeftOffset + promptLabelWidthMult + 0.10f),
                area.y + area.height * promptLabelTopOffset,
                area.width * 0.20f,
                area.height * 0.22f);

            DrawSmallSetting(tempRect, "Temperature (0-2)", ref temperature);

            Rect maxTokensRect = new Rect(
                area.x + area.width * (promptLabelLeftOffset + promptLabelWidthMult + 0.23f),
                area.y + area.height * promptLabelTopOffset,
                area.width * 0.20f,
                area.height * 0.22f);

            DrawSmallSetting(maxTokensRect, "Max Tokens (up to 512)", ref maxTokens);

            Rect topPRect = OffsetRect(tempRect, 0.0f, 1f, 1f, 1f);

            DrawSmallSetting(topPRect, "Top P (0-1)", ref topP);

            Rect freqPenaltyRect = OffsetRect(maxTokensRect, 0.0f, 1.0f, 1f, 1f);

            DrawSmallSetting(freqPenaltyRect, "Frequency Penalty (-2 to 2)", ref frequencyPenalty);
        }
    }
}
