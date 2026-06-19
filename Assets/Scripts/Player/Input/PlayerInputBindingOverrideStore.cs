using System;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputBindingOverrideStore
{
    private const string BindingOverridesKey = "settings.input.bindingOverrides";

    public static event Action Changed;

    public static void Apply(PlayerInputActions inputs)
    {
        if (inputs == null)
        {
            throw new ArgumentNullException(nameof(inputs));
        }

        inputs.asset.RemoveAllBindingOverrides();

        string bindingOverrides = PlayerPrefs.GetString(BindingOverridesKey, string.Empty);

        if (string.IsNullOrWhiteSpace(bindingOverrides))
        {
            return;
        }

        inputs.asset.LoadBindingOverridesFromJson(bindingOverrides);
    }

    public static void Save(PlayerInputActions inputs)
    {
        if (inputs == null)
        {
            throw new ArgumentNullException(nameof(inputs));
        }

        string bindingOverrides = inputs.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(BindingOverridesKey, bindingOverrides);
        PlayerPrefs.Save();
        InvokeChanged();
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(BindingOverridesKey);
        PlayerPrefs.Save();
        InvokeChanged();
    }

    private static void InvokeChanged()
    {
        if (Changed != null)
        {
            Changed.Invoke();
        }
    }
}
