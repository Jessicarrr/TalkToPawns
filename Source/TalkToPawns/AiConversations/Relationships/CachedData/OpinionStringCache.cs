using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.Relationships.CachedData
{
    internal struct OpinionStringCache
    {
        internal string memoryHolderPawnID;
        internal string thoughtAboutPawnID;
        internal string fullOpinionString;
        internal int lastUpdated;
        internal int expiryTime;
        
        public OpinionStringCache(string memoryHolderPawnID, string thoughtAboutPawnID, string fullOpinionString)
        {
            this.memoryHolderPawnID = memoryHolderPawnID;
            this.thoughtAboutPawnID = thoughtAboutPawnID;
            this.fullOpinionString = fullOpinionString;
            this.lastUpdated = Find.TickManager.TicksGame;
            this.expiryTime = lastUpdated + (60 * 5); // 5 seconds + the current time?
        }
    }
}
