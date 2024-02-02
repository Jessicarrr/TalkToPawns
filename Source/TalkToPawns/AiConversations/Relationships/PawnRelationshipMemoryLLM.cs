using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.Relationships
{
    public class PawnRelationshipMemoryLLM
    {
        public int relationshipImpact;
        public string description;
        public Pawn memoryHolderPawn;
        public Pawn thoughtAboutPawn;

        public PawnRelationshipMemoryLLM(Pawn memoryHolderPawn, Pawn thoughtAboutPawn, int relationshipImpact, string description)
        {
            this.memoryHolderPawn = memoryHolderPawn;
            this.thoughtAboutPawn = thoughtAboutPawn;
            this.relationshipImpact = relationshipImpact;
            this.description = description;
        }
    }
}
