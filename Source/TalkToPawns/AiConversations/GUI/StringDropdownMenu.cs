using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AiConversations.GUI
{
    internal class StringDropdownMenu
    {
        private string selectedOption;
        private List<string> options;

        // Define a delegate that represents the signature of the event handler methods.
        public delegate void DropdownItemSelectedHandler(string selectedItem);

        public event DropdownItemSelectedHandler OnDropdownItemSelected;

        public StringDropdownMenu(List<string> options, string defaultValue = "")
        {
            this.options = options;

            if (defaultValue.NullOrEmpty())
            {
                selectedOption = options.FirstOrDefault();
                return;
            }

            selectedOption = defaultValue;
        }

        public void DrawDropdown(Rect rect)
        {
            Widgets.Dropdown<string, string>(
                rect,
                selectedOption,
                GetPayloadForOption,
                GenerateMenuOptions,
                buttonLabel: selectedOption
            );
        }

        private string GetPayloadForOption(string option)
        {
            // The payload could be different from the option text.
            // For simplicity, we're using the option text as the payload.
            return option;
        }

        private IEnumerable<Widgets.DropdownMenuElement<string>> GenerateMenuOptions(string option)
        {
            foreach (var opt in options)
            {
                yield return new Widgets.DropdownMenuElement<string>
                {
                    option = new FloatMenuOption(opt, () => SelectOption(opt)),
                    payload = opt
                };
            }
        }

        private void SelectOption(string option)
        {
            this.selectedOption = option;
            OnDropdownItemSelected?.Invoke(this.selectedOption); // Trigger the event.
        }
    }
}