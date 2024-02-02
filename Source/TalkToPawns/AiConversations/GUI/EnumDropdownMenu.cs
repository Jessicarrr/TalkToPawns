using System.Collections.Generic;
using System;
using UnityEngine;
using Verse;
using System.Linq;

internal class EnumDropdownMenu<T> where T : Enum
{
    private T selectedOption;
    private T[] options;

    public delegate void DropdownItemSelectedHandler(T selectedItem);
    public event DropdownItemSelectedHandler OnDropdownItemSelected;

    public EnumDropdownMenu(T defaultValue)
    {
        options = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        selectedOption = defaultValue;
    }

    public void DrawDropdown(Rect rect)
    {
        Widgets.Dropdown<T, T>(
            rect,
            selectedOption,
            option => option, // Payload is the option itself
            GenerateMenuOptions,
            buttonLabel: selectedOption.ToString()
        );
    }

    private IEnumerable<Widgets.DropdownMenuElement<T>> GenerateMenuOptions(T option)
    {
        foreach (var opt in options)
        {
            yield return new Widgets.DropdownMenuElement<T>
            {
                option = new FloatMenuOption(opt.ToString(), () => SelectOption(opt)),
                payload = opt
            };
        }
    }

    private void SelectOption(T option)
    {
        selectedOption = option;
        OnDropdownItemSelected?.Invoke(selectedOption);
    }
}