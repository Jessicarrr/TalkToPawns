using AiConversations.Relationships;
using JsonFx.Json;
using JsonFx.Serialization.Resolvers;
using JsonFx.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.Parsing
{
    internal class SaveLoadHandler
    {
        internal static string path
        {
            get
            {
                string modPath = TTPModSettings.GetInstance().ModContentPack.RootDir;

                if (string.IsNullOrEmpty(modPath))
                {
                    return null;
                }

                return Path.Combine(modPath, "Saves");
            }
        }

        internal static bool TrySaveModData(string saveFileName)
        {
            if(path == null)
            {
                Log.Warning("Could not find the path to the Talk to Pawns mod. Cannot save pawn memories or other mod specific data.");
                return false;
            }

            JsonWriter writer = new JsonWriter();

            string fileName = Path.GetFileNameWithoutExtension(saveFileName);
            
            string jsonData = writer.Write(PawnRelationshipTrackerLLM.memories);
            string destinationPath = Path.Combine(path, fileName + ".json");
            Log.Message(jsonData);
            Log.Message("Would save to: " + destinationPath);

            try
            {
                System.IO.Directory.CreateDirectory(path);
                System.IO.File.WriteAllText(destinationPath, jsonData);
                return true;
            }
            catch(Exception ex)
            {
                Log.Warning("Could not save Talk to Pawns mod data - " + ex.ToString());
                return false;
            }
        }

        internal static bool TryLoadModData(string saveFileName)
        {
            if(path == null)
            {
                Log.Warning("Could not find the path to the Talk to Pawns mod. Cannot load pawn memories or other mod specific data.");
                return false;
            }

            if(System.IO.Directory.Exists(path) == false)
            {
                Log.Message("Could not find the saves directory when loading Talk to Pawns data");
                return false;
            }

            string filePath = Path.Combine(path, saveFileName + ".json");

            if (File.Exists(filePath) == false)
            {
                Log.Message("Could not find any save file by " + filePath + " when loading Talk to Pawns data");
                return false;
            }

            string contents = File.ReadAllText(filePath);
            var reader = new JsonReader(new DataReaderSettings(new DataContractResolverStrategy()));
            List<PawnRelationshipMemoryLLM> loadedMemories = reader.Read<List<PawnRelationshipMemoryLLM>>(contents);

            //Log.Message("Loaded memories: " + loadedMemories.ToString());
            PawnRelationshipTrackerLLM.memories.Clear();
            PawnRelationshipTrackerLLM.memories.AddRange(loadedMemories);

            Log.Message("Loaded " + PawnRelationshipTrackerLLM.memories.Count() + " memories");

            return true;
        }
    }
}
