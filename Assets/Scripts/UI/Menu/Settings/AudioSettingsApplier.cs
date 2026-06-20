using System;
using UnityEngine;
using UnityEngine.Audio;

internal static class AudioSettingsApplier
{
    internal const string MasterParameterName = "Master";
    internal const string MusicParameterName = "Music";
    internal const string EffectsParameterName = "Effects";

    private const string MixerPath = "Audio/MainAudioMixer";
    private const float MinDb = -80.0f;
    private const float MuteThreshold = 0.0001f;
    private const float DbMultiplier = 20.0f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplySavedBeforeSceneLoad()
    {
        AudioMixer audioMixer = Resources.Load<AudioMixer>(MixerPath);

        if (audioMixer == null)
        {
            throw new InvalidOperationException(nameof(audioMixer));
        }

        SettingsSave settingsSave = new SettingsSave();
        SettingsData settingsData = settingsSave.Load(
            QualitySettings.GetQualityLevel(),
            Screen.fullScreen,
            QualitySettings.vSyncCount > 0);

        Apply(audioMixer, settingsData);
    }

    internal static void Apply(AudioMixer audioMixer, SettingsData settingsData)
    {
        if (audioMixer == null)
        {
            throw new ArgumentNullException(nameof(audioMixer));
        }

        ApplyVolume(audioMixer, MasterParameterName, settingsData.MasterVolume);
        ApplyVolume(audioMixer, MusicParameterName, settingsData.MusicVolume);
        ApplyVolume(audioMixer, EffectsParameterName, settingsData.EffectsVolume);
    }

    internal static void ApplyVolume(AudioMixer audioMixer, string parameterName, float linearValue)
    {
        if (audioMixer == null)
        {
            throw new ArgumentNullException(nameof(audioMixer));
        }

        float normalizedValue = Mathf.Clamp01(linearValue);
        float dbValue = LinearToDb(normalizedValue);
        bool isApplied = audioMixer.SetFloat(parameterName, dbValue);

        if (isApplied == false)
        {
            throw new InvalidOperationException(nameof(audioMixer));
        }
    }

    private static float LinearToDb(float linearValue)
    {
        if (linearValue <= MuteThreshold)
        {
            return MinDb;
        }

        return Mathf.Log10(linearValue) * DbMultiplier;
    }
}
