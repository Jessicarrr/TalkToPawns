using HarmonyLib;
using RimWorld;
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
        [DataMember]
        public int relationshipImpact;
        [DataMember]
        public string description;
        [DataMember]
        public string memoryHolderPawnID;
        [DataMember]
        public string thoughtAboutPawnID;
        [DataMember]
        public int timesRepeated = 1;
        [DataMember]
        public int createdAtTick { get; private set; }
        [DataMember]
        public int expiryTick { get; private set; }

        public PawnRelationshipMemoryLLM(string memoryHolderPawnID, string thoughtAboutPawnID, int relationshipImpact, string description)
        {
            this.memoryHolderPawnID = memoryHolderPawnID;
            this.thoughtAboutPawnID = thoughtAboutPawnID;
            this.relationshipImpact = relationshipImpact;
            this.description = description;
        }

        public void FirstTimeSetup()
        {
            SetCreatedAtTick();
            SetExpiryTick(createdAtTick, relationshipImpact);
        }

        private void SetCreatedAtTick()
        {
            createdAtTick = Find.TickManager.TicksGame;
        }

        private void SetExpiryTick(int startTime, int relationshipImpact)
        {
            var modSettings = TTPModSettings.GetInstance();
            int expiryTimeBase = GenDate.TicksPerDay * modSettings.memoryTimeBase.Value;

            int additionalTime = Math.Abs(relationshipImpact) * GenDate.TicksPerDay * modSettings.memoryTimePerImpact.Value;
            int totalTime = expiryTimeBase + additionalTime;
            int expiryTime = startTime + totalTime;

            this.expiryTick = expiryTime;
        }

        public bool IsExpired()
        {
            var currentTime = Find.TickManager.TicksGame;

            if (currentTime >= expiryTick)
            {
                return true;
            }

            return false;
        }

        public void AddRepeat(int relationshipImpact)
        {
            this.relationshipImpact += relationshipImpact;
            timesRepeated++;

            var currentTime = Find.TickManager.TicksGame;
            SetExpiryTick(currentTime, relationshipImpact);
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
