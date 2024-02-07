using AiConversations.Relationships.CachedData;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.Relationships
{
    public class PawnRelationshipTrackerLLM
    {
        [DataMember(Name = "memories")]
        public static List<PawnRelationshipMemoryLLM> memories = new List<PawnRelationshipMemoryLLM>();

        private static OpinionStringCache? opinionCache;

        public static void AddTestMemory(Pawn memoryHolder, Pawn thoughtAbout)
        {
            PawnRelationshipMemoryLLM memory = new PawnRelationshipMemoryLLM(memoryHolder.ThingID, thoughtAbout.ThingID, 1, "A test memory. Num memories: " + (memories.Count() + 1));
            memories.Add(memory);
        }

        public static bool IsCachedEntryAppropriate(string memoryHolderID, string thoughtAboutID)
        {
            if (opinionCache.HasValue == false)
            {
                return false;
            }

            if (Find.TickManager.TicksGame >= opinionCache.Value.expiryTime)
            {
                return false;
            }

            if (opinionCache.Value.memoryHolderPawnID == memoryHolderID
                && opinionCache.Value.thoughtAboutPawnID == thoughtAboutID)
            {
                return true;
            }

            
            return false;
        }

        public static string GetFullOpinionStringFor(string memoryHolderID, string thoughtAboutID)
        {
            if(IsCachedEntryAppropriate(memoryHolderID, thoughtAboutID))
            {
                return opinionCache.Value.fullOpinionString;
            }

            var memoriesOfThisPawn = memories.AsQueryable()
                .Where(queriedMemory => queriedMemory.memoryHolderPawnID == memoryHolderID && queriedMemory.thoughtAboutPawnID == thoughtAboutID)
                .OrderByDescending(queriedMemory => queriedMemory.createdAtTick / (GenDate.TicksPerDay / 4)) // Group by 6-hour segments
                .ThenByDescending(queriedMemory => Math.Abs(queriedMemory.relationshipImpact));


            if (memoriesOfThisPawn == null || memoriesOfThisPawn.Count() <= 0)
            {
                return "";
            }

            int numMemoriesProcessed = 0;
            int maxMemoriesExplicitlyExplained = 7;
            int numExcessMemories = 0;
            int relationshipImpactFromExcessMemories = 0;

            StringBuilder builder = new StringBuilder();

            foreach(PawnRelationshipMemoryLLM memory in memoriesOfThisPawn)
            {
                numMemoriesProcessed++;

                if (numMemoriesProcessed <= maxMemoriesExplicitlyExplained)
                {
                    builder.AppendLine(memory.GetOpinionString());
                    continue;
                }
                numExcessMemories += 1;
                relationshipImpactFromExcessMemories += memory.relationshipImpact;

            }
            
            if(numExcessMemories > 0)
            {
                builder.AppendLine("- Other chat sessions (x" + numExcessMemories + "): " + relationshipImpactFromExcessMemories);
            }

            var opinionString = builder.ToString();
            opinionCache = new OpinionStringCache(memoryHolderID, thoughtAboutID, opinionString);

            return opinionString;
        }

        private static void DeleteExpiredMemories()
        {
            List<PawnRelationshipMemoryLLM> expiredMemories = new List<PawnRelationshipMemoryLLM>();

            foreach(var memory in memories)
            {
                if(memory.IsExpired())
                {
                    expiredMemories.Add(memory);
                }
            }

            Log.Message("Deleting " + expiredMemories.Count() + " expired memories");

            foreach(var memory in expiredMemories)
            {
                memories.Remove(memory);
            }
        }

        private static bool TryModifyDuplicateMemory(Pawn memoryHolderPawn, Pawn thoughtAboutPawn, int relationshipImpact, string description)
        {
            var memory = memories.AsQueryable()
                .Where(queriedMemory => queriedMemory.memoryHolderPawnID == memoryHolderPawn.ThingID)
                .Where(queriedMemory => queriedMemory.thoughtAboutPawnID == thoughtAboutPawn.ThingID)
                .Where(queriedMemory => queriedMemory.description == description)
                .FirstOrDefault();

            if (memory == null)
            {
                return false;
            }

            memory.AddRepeat(relationshipImpact);
            Log.Message("Modified memory '" + memory.description + "'. Total repeats: " + memory.timesRepeated);
            opinionCache = null;
            return true;
        }

        public static void TryCreateMemoryFromString(Pawn memoryHolderPawn, Pawn thoughtAboutPawn, string aiResponse)
        {
            // Regular expression to match the pattern "+1 Description" or "-1 Description"
            Regex regex = new Regex(@"^([+-]?\d+)\s(.+)");
            Match match = regex.Match(aiResponse);

            // Check if the string matches the expected format
            if (match.Success && match.Groups.Count == 3) // Groups[0] is the entire match, Groups[1] is the number, Groups[2] is the description
            {
                // Parsing the number as integer
                int relationshipImpact = int.Parse(match.Groups[1].Value);

                // Extracting the description
                string description = match.Groups[2].Value;

                description = description.TrimEnd(' ', ',', '.');

                if(TryModifyDuplicateMemory(memoryHolderPawn, thoughtAboutPawn, relationshipImpact, description) == true)
                {
                    DeleteExpiredMemories();
                    return;
                }

                // Creating the memory with extracted values
                PawnRelationshipMemoryLLM memory = new PawnRelationshipMemoryLLM(memoryHolderPawn.ThingID, thoughtAboutPawn.ThingID, relationshipImpact, description);
                memory.FirstTimeSetup();
                
                memories.Add(memory);
                opinionCache = null;
                Log.Message("Added memory '" + memory.description + "'. Total memories: " + memories.Count());
                DeleteExpiredMemories();
            }
            else
            {
                // Log message if the format doesn't match or if there's an error
                Log.Message("Could not parse AI response correctly when trying to add a new pawn memory. AI Response was:\n" + aiResponse);
            }
        }
    }
}
