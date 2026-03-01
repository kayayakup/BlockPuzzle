// ── AudioManager.cs ───────────────────────────────────────────────────────────
// Place in: Assets/Scripts/Core/AudioManager.cs
//
// Generates ALL sound effects procedurally at runtime using PCM synthesis.
// No external audio files required.
//
// Sounds:
//   PlayPlace()     – soft percussive "thud" when a block is placed
//   PlayLineClear() – bright ascending arpeggio when a line is cleared
//   PlayMultiClear()– fuller chord sweep for 2+ simultaneous lines
//   PlayScore()     – quick rising "ding" for score popup
//   PlaySpawn()     – gentle whoosh when new tray blocks appear
//   PlayGameOver()  – descending "wah-wah" melody
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // One AudioSource for one-shots, a second for longer overlapping sounds
    private AudioSource _sfxSource;
    private AudioSource _bgSource;

    private AudioClip _clipPlace;
    private AudioClip _clipClear1;
    private AudioClip _clipClear2;
    private AudioClip _clipScore;
    private AudioClip _clipSpawn;
    private AudioClip _clipGameOver;
    private AudioClip _clipPickup;

    private const int SAMPLE_RATE = 44100;

    void Awake() => Instance = this;

    // ── Initialisation ────────────────────────────────────────────────────────

    public void Initialize()
    {
        _sfxSource            = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.volume      = 1f;

        _bgSource             = gameObject.AddComponent<AudioSource>();
        _bgSource.playOnAwake = false;
        _bgSource.volume      = 0.85f;

        // Build all clips (done once on startup)
        _clipPlace    = BuildPlaceSound();
        _clipClear1   = BuildClearSound(1);
        _clipClear2   = BuildClearSound(2);
        _clipScore    = BuildScoreSound();
        _clipSpawn    = BuildSpawnSound();
        _clipGameOver = BuildGameOverSound();
        _clipPickup   = BuildPickupSound();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void PlayPlace()     => _sfxSource.PlayOneShot(_clipPlace,    0.75f);
    public void PlayLineClear(int lines = 1)
    {
        var clip = lines >= 2 ? _clipClear2 : _clipClear1;
        _sfxSource.PlayOneShot(clip, 1.0f);
    }
    public void PlayScore()     => _sfxSource.PlayOneShot(_clipScore,    0.60f);
    public void PlaySpawn()     => _sfxSource.PlayOneShot(_clipSpawn,    0.35f);
    public void PlayGameOver()  => _bgSource .PlayOneShot(_clipGameOver, 0.85f);
    public void PlayPickup()    => _sfxSource.PlayOneShot(_clipPickup,   0.50f);

    // ── Sound: Block Place ────────────────────────────────────────────────────
    // Short percussive "thud" – bandpass noise + low-frequency sine punch

    private static AudioClip BuildPlaceSound()
    {
        float dur      = 0.12f;
        int   samples  = (int)(SAMPLE_RATE * dur);
        var   data     = new float[samples];

        System.Random rng = new System.Random(42);

        for (int i = 0; i < samples; i++)
        {
            float t       = (float)i / SAMPLE_RATE;
            float env     = Mathf.Exp(-t * 30f);               // fast decay
            float noise   = (float)(rng.NextDouble() * 2 - 1); // white noise
            float tone    = Mathf.Sin(2f * Mathf.PI * 180f * t); // low sine
            data[i] = (noise * 0.45f + tone * 0.55f) * env;
        }

        return MakeClip("Place", data);
    }

    // ── Sound: Line Clear ─────────────────────────────────────────────────────
    // Ascending arpeggio: 3 or 5 notes in a pentatonic scale

    private static AudioClip BuildClearSound(int lines)
    {
        // Notes (Hz) — two octaves of a pentatonic scale
        float[] notes = lines == 1
            ? new[] { 440f, 554f, 659f, 880f }
            : new[] { 440f, 523f, 659f, 784f, 1047f };

        float noteLen  = 0.075f;
        float totalDur = noteLen * notes.Length + 0.15f;
        int   total    = (int)(SAMPLE_RATE * totalDur);
        var   data     = new float[total];

        for (int n = 0; n < notes.Length; n++)
        {
            float freq  = notes[n];
            int   start = (int)(SAMPLE_RATE * n * noteLen);
            int   len   = (int)(SAMPLE_RATE * (noteLen + 0.08f));

            for (int i = 0; i < len && start + i < total; i++)
            {
                float t   = (float)i / SAMPLE_RATE;
                float env = Mathf.Exp(-t * 9f);
                // Sine + small harmonic for brightness
                float s   = Mathf.Sin(2f * Mathf.PI * freq * t)       * 0.70f
                          + Mathf.Sin(2f * Mathf.PI * freq * 2f * t)  * 0.20f
                          + Mathf.Sin(2f * Mathf.PI * freq * 3f * t)  * 0.10f;
                data[start + i] += s * env * 0.55f;
            }
        }

        Normalise(data);
        return MakeClip(lines == 1 ? "Clear1" : "Clear2", data);
    }

    // ── Sound: Score Ding ─────────────────────────────────────────────────────
    // Bright rising two-tone ding

    private static AudioClip BuildScoreSound()
    {
        float dur     = 0.18f;
        int   samples = (int)(SAMPLE_RATE * dur);
        var   data    = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t   = (float)i / SAMPLE_RATE;
            float env = Mathf.Exp(-t * 12f);
            // Two tones: root + fifth above
            data[i] = (Mathf.Sin(2f * Mathf.PI * 880f * t) * 0.6f
                     + Mathf.Sin(2f * Mathf.PI * 1320f * t) * 0.4f) * env;
        }

        return MakeClip("Score", data);
    }

    // ── Sound: Spawn whoosh ───────────────────────────────────────────────────
    // Gentle filtered rising noise

    private static AudioClip BuildSpawnSound()
    {
        float dur     = 0.16f;
        int   samples = (int)(SAMPLE_RATE * dur);
        var   data    = new float[samples];

        System.Random rng = new System.Random(7);
        float prev = 0f;

        for (int i = 0; i < samples; i++)
        {
            float t      = (float)i / SAMPLE_RATE;
            float env    = Mathf.SmoothStep(0f, 0.04f, t) * Mathf.SmoothStep(dur, dur - 0.06f, t);
            float noise  = (float)(rng.NextDouble() * 2 - 1);
            // One-pole lowpass filter for "whoosh" texture
            prev = Mathf.Lerp(prev, noise, 0.25f);
            // Rising sine sweep
            float freq   = 300f + 900f * (t / dur);
            float sweep  = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.3f;
            data[i] = (prev * 0.7f + sweep) * env * 0.45f;
        }

        return MakeClip("Spawn", data);
    }

    // ── Sound: Game Over ─────────────────────────────────────────────────────
    // Four descending notes with a bit of reverb-like decay

    private static AudioClip BuildGameOverSound()
    {
        float[] notes  = { 392f, 330f, 262f, 196f };  // G4 E4 C4 G3
        float   noteLen = 0.18f;
        float   total   = noteLen * notes.Length + 0.35f;
        int     len     = (int)(SAMPLE_RATE * total);
        var     data    = new float[len];

        for (int n = 0; n < notes.Length; n++)
        {
            float freq  = notes[n];
            int   start = (int)(SAMPLE_RATE * n * noteLen);
            int   nLen  = (int)(SAMPLE_RATE * (noteLen + 0.25f));

            for (int i = 0; i < nLen && start + i < len; i++)
            {
                float t   = (float)i / SAMPLE_RATE;
                float env = Mathf.Exp(-t * 5.5f);
                float s   = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.60f
                          + Mathf.Sin(2f * Mathf.PI * freq * 0.5f * t) * 0.28f  // sub octave
                          + Mathf.Sin(2f * Mathf.PI * freq * 2f * t)   * 0.12f; // harmonic
                data[start + i] += s * env * 0.60f;
            }
        }

        Normalise(data);
        return MakeClip("GameOver", data);
    }


    // ── Sound: Block Pickup ───────────────────────────────────────────────────
    // Quick high-pitched pop (short sine burst with attack+decay)

    private static AudioClip BuildPickupSound()
    {
        float dur     = 0.08f;
        int   samples = (int)(SAMPLE_RATE * dur);
        var   data    = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t   = (float)i / SAMPLE_RATE;
            float env = Mathf.SmoothStep(0f, 0.005f, t) * Mathf.Exp(-t * 40f);
            // Two harmonics: fundamental + octave
            data[i] = (Mathf.Sin(2f * Mathf.PI * 1200f * t) * 0.65f
                     + Mathf.Sin(2f * Mathf.PI * 2400f * t) * 0.35f) * env;
        }
        return MakeClip("Pickup", data);
    }

    // ── PCM helpers ───────────────────────────────────────────────────────────

    private static AudioClip MakeClip(string name, float[] data)
    {
        var clip = AudioClip.Create(name, data.Length, 1, SAMPLE_RATE, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static void Normalise(float[] data)
    {
        float peak = 0f;
        foreach (float s in data) if (Mathf.Abs(s) > peak) peak = Mathf.Abs(s);
        if (peak < 0.001f) return;
        float inv = 0.85f / peak;
        for (int i = 0; i < data.Length; i++) data[i] *= inv;
    }
}
