// #define DEBUG_VERBOSE

using UnityEngine;

namespace F1yingBanana.SfizzUnity {
  /// <summary>
  /// A sampler that interfaces with a <see cref="Sfizz"/> wrapper object to generate audio based on
  /// note inputs and a sound bank. Render is called every <see cref="Update"/>.
  /// </summary>
  [RequireComponent(typeof(AudioSource))]
  public sealed class SfizzPlayer : MonoBehaviour {
    /// <summary>
    /// The target sample rate of the audio being generated. This is passed into <see cref="Sfizz"/>
    /// and also affects buffer sizes. This should not be changed every frame as it requires
    /// reallocating all the buffers.
    /// </summary>
    [field:Min(1)]
    [field:SerializeField]
    public int SampleRate { get; set; } = 44100;

    /// <summary>
    /// Occasionally, the audio source playback might lag behind the audio generation too much, and
    /// we perceive a latency. If the latency is higher than this value (in ms), then we speed up
    /// playback to re-sync with generation. This value should be larger than 1/<see
    /// cref="Time.unscaledDeltaTime"/> to avoid syncing every update frame. Otherwise an
    /// audio-version of the screendoor effect will occur, resulting in discontinuous audio.
    /// </summary>
    [field:Min(1)]
    [field:SerializeField]
    public int LatencyThreshold {
      get; set;
    } = 100;

    [field:SerializeField]
    public bool UseUnscaledTime {
      get; set;
    } = true;

    /// <summary>
    /// The underlying <see cref="Sfizz"/> object. Use this to trigger audio events.
    /// </summary>
    public Sfizz Sfizz { get; } = new Sfizz();
#region Implementation
    /// <summary>
    /// Only support single stereo channel per player for now.
    /// </summary>
    private const int Channels = 2;

    private AudioClip audioClip;

    private AudioSource audioSource;

    /// <summary>
    /// The size of the buffer used to store the audio data. Note that this may only be partially
    /// used.
    /// </summary>
    private int bufferSize;

    /// <summary>
    /// The buffer that holds data from <see cref="Sfizz.RenderBlock(float[][], int, int)"/>.
    /// </summary>
    private float[][] buffer;

    /// <summary>
    /// Used to determine whether sample rate has been modified (and invalidating buffers).
    /// </summary>
    private int lastSampleRate;

    /// <summary>
    /// The current offset of rendered audio data. That is, future <see
    /// cref="Sfizz.RenderBlock(float[][], int, int)"/> should populate from this frame onwards.
    /// </summary>
    private int audioOffset = 0;

    /// <summary>
    /// The amount of time we over-rendered. Each time we render a bit more than delta time, which
    /// adds up over time and increases the perceived latency. This is used to offset the rounding
    /// error.
    /// </summary>
    private float dtError;

    private void Awake() {
      audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
      UpdateAudioStream(UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
    }

    private void OnDestroy() {
      Sfizz.Dispose();
    }

    private void UpdateAudioStream(float dt) {
      // Check whether sample rate has updated, and initialize.
      UpdateSampleRate();

      // Calculate how many frames we should render. To minimize latency, this should be as short as
      // possible, but long enough to last until the next Update call.
      float targetDeltaTime = Mathf.Clamp(dt, 0, Time.maximumDeltaTime) - dtError;
      int samples = Mathf.CeilToInt(SampleRate * targetDeltaTime);
      dtError = (float)samples / SampleRate - targetDeltaTime;

      // Render the audio and update the audio stream.
      Sfizz.RenderBlock(buffer, Channels, samples);

      // Note this cannot be cached. See doc on AudioClipSizeMultiplier.
      float[] interleavedBuffer = new float[samples * Channels];
      Interleave(buffer, interleavedBuffer, samples);
      audioClip.SetData(interleavedBuffer, audioOffset);

      // Occasionally the playback may lag behind or get in front of what we rendered. We can't
      // adjust this every frame as the frames don't line up with Update and we get an audio version
      // of the screendoor effect. Instead, we interpolate if it gets too out of sync.
      float latency = audioOffset - audioSource.timeSamples;

      if (latency < 0) {
        // We might be on two ends of the loop. Perform extra checks.
        float wrapAroundLatency = bufferSize - audioSource.timeSamples + audioOffset;

        if (-latency > wrapAroundLatency) {
          // The wrapped around value is the true latency since the audio clip size >> latency.
          latency = wrapAroundLatency;
        }

        // Otherwise the latency is actually negative, which may happen if audio continues to play
        // when update is paused.
      }

      float latencyMs = 1000f * latency / SampleRate;

      if (latencyMs > LatencyThreshold || latencyMs < 0) {
        audioSource.timeSamples = audioOffset;
      }
#if DEBUG_VERBOSE
      Debug.Log($"{name} updated {samples} frames. dt={dt} dt_err={dtError} latency={latencyMs}ms");
#endif
      audioOffset = (audioOffset + samples) % bufferSize;
    }

    /// <summary>
    /// Recalculates the required buffer sizes and updates them to match.
    /// </summary>
    /// <param name="sampleRate"></param>
    private void UpdateSampleRate() {
      SampleRate = Mathf.Max(SampleRate, 1);

      if (SampleRate == lastSampleRate) {
        return;
      }

      lastSampleRate = SampleRate;

      // Buffer size is directly related to the sample rate, as we are streaming it. We ensure that
      // it never overflows.
      bufferSize = Mathf.CeilToInt(Time.maximumDeltaTime * SampleRate);
      buffer = new float [Channels][];

      for (int i = 0; i < Channels; i++) {
        buffer[i] = new float[bufferSize];
      }

      // Update Sfizz object to match.
      Sfizz.SetSamplesPerBlock(bufferSize);
      Sfizz.SetSampleRate(SampleRate);
#if DEBUG_VERBOSE
      Debug.Log($"Buffer size set to {bufferSize}, sample rate set to {SampleRate}");
#endif

      // We cannot change the sample rate without also changing the AudioClip.
      if (audioClip != null) {
        Destroy(audioClip);
      }

      audioClip = AudioClip.Create("stream", bufferSize, Channels, SampleRate, false);
      audioSource.clip = audioClip;
      audioSource.loop = true;
      audioSource.Play();
      audioOffset = 0;
    }

    private void Interleave(float[][] input, float[] output, int samples) {
      int k = 0;

      for (int i = 0; i < samples; i++) {
        for (int j = 0; j < Channels; j++) {
          output[k++] = input[j][i];
        }
      }
    }
#endregion
  }
}
