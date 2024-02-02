using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AiConversations.HelperClasses
{
    internal class PromptVariable
    {
        public string placeholder { get; }
        public string explanation { get; }
        public Func<Pawn, Pawn, string> ReplacementFunction { get; }
        public string directValue { get; }

        public PromptVariable(string placeholder, string explanation, Func<Pawn, Pawn, string> replacementFunction)
        {
            this.placeholder = placeholder;
            this.explanation = explanation;
            ReplacementFunction = replacementFunction;
            this.directValue = null;
        }

        public PromptVariable(string placeholder, string explanation, string directValue)
        {
            this.placeholder = placeholder;
            this.explanation = explanation;
            this.directValue = directValue;
            ReplacementFunction = null;
        }

        public string DoReplace(Pawn initiator, Pawn aiRecipient)
        {
            if (ReplacementFunction != null)
            {
                return ReplacementFunction(initiator, aiRecipient);
            }

            if(directValue != null)
            {
                return directValue;
            }

            return "Error parsing PromptVariable for " + placeholder + ". Both ReplacementFunction and DirectValue were null?";
        }
    }
}
