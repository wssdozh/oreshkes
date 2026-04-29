using UnityEngine;

public static class AudioOneShotGate
{
    private const float HitCooldownSeconds = 0.035f;
    private const float FootstepCooldownSeconds = 0.08f;
    private const float CoinCooldownSeconds = 0.12f;
    private const float HitSuppressCoinSeconds = 0.2f;
    private const float FootstepSuppressCoinSeconds = 0.08f;

    private static float s_nextHitTime;
    private static float s_nextFootstepTime;
    private static float s_nextCoinTime;
    private static float s_coinBlockedUntil;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        s_nextHitTime = 0f;
        s_nextFootstepTime = 0f;
        s_nextCoinTime = 0f;
        s_coinBlockedUntil = 0f;
    }

    public static bool TryPlay(
        AudioSource audioSource,
        AudioClip clip,
        float volumeScale,
        AudioOneShotCategory category)
    {
        if (CanPlay(category) == false)
            return false;

        Register(category);
        audioSource.PlayOneShot(clip, volumeScale);

        return true;
    }

    private static bool CanPlay(AudioOneShotCategory category)
    {
        float time = Time.time;

        if (category == AudioOneShotCategory.Hit)
            return time >= s_nextHitTime;

        if (category == AudioOneShotCategory.Footstep)
            return time >= s_nextFootstepTime;

        if (category == AudioOneShotCategory.Coin)
            return time >= s_nextCoinTime && time >= s_coinBlockedUntil;

        return true;
    }

    private static void Register(AudioOneShotCategory category)
    {
        float time = Time.time;

        if (category == AudioOneShotCategory.Hit)
        {
            s_nextHitTime = time + HitCooldownSeconds;
            s_coinBlockedUntil = Mathf.Max(s_coinBlockedUntil, time + HitSuppressCoinSeconds);

            return;
        }

        if (category == AudioOneShotCategory.Footstep)
        {
            s_nextFootstepTime = time + FootstepCooldownSeconds;
            s_coinBlockedUntil = Mathf.Max(s_coinBlockedUntil, time + FootstepSuppressCoinSeconds);

            return;
        }

        if (category == AudioOneShotCategory.Coin)
            s_nextCoinTime = time + CoinCooldownSeconds;
    }
}
