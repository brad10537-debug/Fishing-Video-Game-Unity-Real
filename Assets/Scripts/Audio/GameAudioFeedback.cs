using System.Collections.Generic;
using UnityEngine;

// Central place for prototype sound feedback.
// Audio clips are optional; if none are assigned, simple generated tones play instead.
public class GameAudioFeedback : MonoBehaviour
{
    public static GameAudioFeedback Instance { get; private set; }

    [Header("Playback")]
    [Range(0f, 1f)] public float volume = 0.35f;
    public bool useGeneratedPlaceholderTones = true;

    [Header("Optional Clips")]
    public AudioClip interactionPromptClip;
    public AudioClip fishingCastClip;
    public AudioClip fishHookClip;
    public AudioClip minigameSuccessClip;
    public AudioClip minigameFailureClip;
    public AudioClip catchRewardClip;
    public AudioClip coinGainClip;
    public AudioClip xpGainClip;
    public AudioClip levelUpClip;
    public AudioClip rodUpgradeClip;
    public AudioClip shopOpenClip;
    public AudioClip shopCloseClip;
    public AudioClip shopDeniedClip;
    public AudioClip journalOpenClip;
    public AudioClip journalCloseClip;

    private AudioSource audioSource;
    private readonly Dictionary<string, AudioClip> generatedToneCache = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void PlayInteractionPrompt() { PlayClipOrTone(interactionPromptClip, 520f, 0.04f, 0.35f); }
    public void PlayFishingCast() { PlayClipOrTone(fishingCastClip, 260f, 0.12f, 0.45f); }
    public void PlayFishHook() { PlayClipOrTone(fishHookClip, 620f, 0.08f, 0.5f); }
    public void PlayMinigameSuccess() { PlayClipOrTone(minigameSuccessClip, 760f, 0.12f, 0.65f); }
    public void PlayMinigameFailure() { PlayClipOrTone(minigameFailureClip, 170f, 0.18f, 0.5f); }
    public void PlayCatchReward(FishRarity rarity) { PlayClipOrTone(catchRewardClip, GetRarityFrequency(rarity), GetRarityDuration(rarity), 0.75f); }
    public void PlayCoinGain() { PlayClipOrTone(coinGainClip, 880f, 0.05f, 0.45f); }
    public void PlayXpGain() { PlayClipOrTone(xpGainClip, 690f, 0.05f, 0.4f); }
    public void PlayLevelUp() { PlayClipOrTone(levelUpClip, 980f, 0.22f, 0.85f); }
    public void PlayRodUpgrade() { PlayClipOrTone(rodUpgradeClip, 840f, 0.18f, 0.8f); }
    public void PlayShopOpen() { PlayClipOrTone(shopOpenClip, 440f, 0.08f, 0.45f); }
    public void PlayShopClose() { PlayClipOrTone(shopCloseClip, 330f, 0.07f, 0.35f); }
    public void PlayShopDenied() { PlayClipOrTone(shopDeniedClip, 220f, 0.14f, 0.45f); }
    public void PlayJournalOpen() { PlayClipOrTone(journalOpenClip, 540f, 0.07f, 0.4f); }
    public void PlayJournalClose() { PlayClipOrTone(journalCloseClip, 360f, 0.06f, 0.35f); }

    private void PlayClipOrTone(AudioClip clip, float frequency, float seconds, float localVolume)
    {
        if (audioSource == null)
        {
            return;
        }

        if (clip != null)
        {
            audioSource.PlayOneShot(clip, Mathf.Clamp01(volume * localVolume));
            return;
        }

        if (!useGeneratedPlaceholderTones)
        {
            return;
        }

        AudioClip tone = GetOrCreateTone(frequency, seconds);
        if (tone != null)
        {
            audioSource.PlayOneShot(tone, Mathf.Clamp01(volume * localVolume));
        }
    }

    private AudioClip GetOrCreateTone(float frequency, float seconds)
    {
        string key = frequency.ToString("0") + "_" + seconds.ToString("0.00");
        if (generatedToneCache.TryGetValue(key, out AudioClip existingTone))
        {
            return existingTone;
        }

        const int sampleRate = 44100;
        int sampleCount = Mathf.Max(1, Mathf.RoundToInt(sampleRate * Mathf.Max(0.02f, seconds)));
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Sin(Mathf.PI * i / (sampleCount - 1));
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope;
        }

        AudioClip clip = AudioClip.Create("GeneratedTone_" + key, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        generatedToneCache.Add(key, clip);
        return clip;
    }

    private float GetRarityFrequency(FishRarity rarity)
    {
        switch (rarity)
        {
            case FishRarity.Mythic:
                return 1180f;
            case FishRarity.Legendary:
                return 1040f;
            case FishRarity.Epic:
                return 930f;
            case FishRarity.Rare:
                return 820f;
            case FishRarity.Uncommon:
                return 700f;
            default:
                return 600f;
        }
    }

    private float GetRarityDuration(FishRarity rarity)
    {
        switch (rarity)
        {
            case FishRarity.Mythic:
            case FishRarity.Legendary:
                return 0.28f;
            case FishRarity.Epic:
                return 0.22f;
            case FishRarity.Rare:
                return 0.17f;
            default:
                return 0.12f;
        }
    }
}
