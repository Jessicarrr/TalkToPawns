using HarmonyLib;
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
        public int timesRepeated = 1;

        public PawnRelationshipMemoryLLM(string memoryHolderPawnID, string thoughtAboutPawnID, int relationshipImpact, string description)
        {
            this.memoryHolderPawnID = memoryHolderPawnID;
            this.thoughtAboutPawnID = thoughtAboutPawnID;
            this.relationshipImpact = relationshipImpact;
            this.description = description;
        }

        public void AddRepeat(int relationshipImpact)
        {
            this.relationshipImpact += relationshipImpact;
            timesRepeated++;
        }

        public string GetOpinionString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" - " + description);

            if(timesRepeated > 1)
            {
                stringBuilder.Append(" (x" + timesRepeated + ")");
            }
            
            stringBuilder.Append(": " + GetRelationshipImpactString());

            return stringBuilder.ToString();
        }

        public string GetRelationshipImpactString()
        {
            if (relationshipImpact < 0)
            {
                return relationshipImpact.ToString();
            }
            if (relationshipImpact >= 0)
            {
                return "+" + relationshipImpact;
            }
            return "+" + relationshipImpact;
        }
    }
}
