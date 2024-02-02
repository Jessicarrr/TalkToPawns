using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AiConversations.GUI.ModSettingsPartials
{
    internal abstract class SettingsArea
    {
        public abstract void Draw(Rect rect);

        protected Rect OffsetRect(Rect baseRect, float xOffsetMult, float yOffsetMult, float widthMult, float heightMult)
        {
            return new Rect(
                baseRect.x + baseRect.width * xOffsetMult,
                baseRect.y + baseRect.height * yOffsetMult,
                baseRect.width * widthMult,
                baseRect.height * heightMult
            );
        }

        protected void DrawSmallSetting(Rect area, string labelText, ref string setting)
        {
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(area.x + area.width * 0.0f, area.y + area.height * 0.0f, area.width * 1.0f, area.height * 0.45f), labelText);
            setting = Widgets.TextArea(new Rect(area.x + (area.width * 0.0f), area.y + (area.height * 0.5f), area.width * 0.5f, area.height * 0.5f), setting.ToString());
        }
    }


}
