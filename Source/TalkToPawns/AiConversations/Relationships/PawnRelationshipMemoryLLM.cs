using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.Relationships
{
    [DataContract]
    public class PawnRelationshipMemoryLLM
    {
        public int relationshipImpact;
        public string description;
        public string memoryHolderPawnID;
        public string thoughtAboutPawnID;

        public PawnRelationshipMemoryLLM(string memoryHolderPawnID, string thoughtAboutPawnID, int relationshipImpact, string description)
        {
            this.memoryHolderPawnID = memoryHolderPawnID;
            this.thoughtAboutPawnID = thoughtAboutPawnID;
            this.relationshipImpact = relationshipImpact;
            this.description = description;
        }
    }
}
