using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.Relationships
{
    public class PawnRelationshipTrackerLLM
    {
        public static List<PawnRelationshipMemoryLLM> memories = new List<PawnRelationshipMemoryLLM>();

        public static void AddTestMemory(Pawn memoryHolder, Pawn thoughtAbout)
        {
            PawnRelationshipMemoryLLM memory = new PawnRelationshipMemoryLLM(memoryHolder, thoughtAbout, 1, "A test memory. Num memories: " + (memories.Count() + 1));
            memories.Add(memory);
        }
    }
}
