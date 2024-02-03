using AiConversations.Parsing;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.HarmonyPatches
{
    [HarmonyPatch(typeof(GameDataSaveLoader), nameof(GameDataSaveLoader.LoadGame), new Type[] { typeof(string) } )]
    public class GameDataSaveLoader_LoadGame
    {
        public static void Postfix(string saveFileName)
        {
            SaveLoadHandler.TryLoadModData(saveFileName);
        }
    }
}
