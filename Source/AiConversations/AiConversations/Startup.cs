using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            Log.Message("Talk to Pawns initializing"); //Outputs "Hello World!" to the dev console.
            var harmony = new Harmony("com.Jessicarrr.TalkToPawns");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
