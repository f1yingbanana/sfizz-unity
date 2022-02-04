using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace F1yingBanana.SfizzUnity {
  /// <summary>
  /// Wrapper around the Sfizz C headers.
  /// TODO: make this class thread-safe.
  /// </summary>
  public sealed class Sfizz : IDisposable {
    /// <summary>
    /// Oversampling factor.
    /// </summary>
    public enum OversamplingFactor { X1 = 1, X2 = 2, X4 = 4, X8 = 8 }

    /// <summary>
    /// Processing mode.
    /// </summary>
    public enum ProcessMode {
      LIVE,
      FREEWHEELING,
    }

    /// <summary>
    /// Index out of bound error for the requested CC/key label.
    /// </summary>
    public const int OUT_OF_BOUNDS_LABEL_INDEX = -1;

    /// <summary>
    /// Creates a new instance of Sfizz for audio processing.
    /// </summary>
    public Sfizz() {
      if (!initialized) {
        initialized = true;
        nativePtr = sfizz_create_synth();
      }
    }

    public void Dispose() {
      if (initialized) {
        initialized = false;
        sfizz_free_memory(nativePtr);
      }
    }

    /// <summary>
    /// While this theoretically may take on a relative path, this cannot be achieved due to how the
    /// path works differently on different build targets in Unity. This method should take an
    /// absolute path for safety. Unity also removes directory structure on built players, so it is
    /// necessary to take advantage of the StreamingAssets folder for files that should be included
    /// in the build, and persistentDataPath for Android, WebGL downloaded assets.
    /// </summary>
    public bool LoadFile(string path) {
      Debug.Assert(initialized);
      return sfizz_load_file(nativePtr, path);
    }

    public bool LoadString(string path, string text) {
      Debug.Assert(initialized);
      return sfizz_load_string(nativePtr, path, text);
    }

    public bool LoadScalaFile(string path) {
      Debug.Assert(initialized);
      return sfizz_load_scala_file(nativePtr, path);
    }

    public bool LoadScalaString(string text) {
      Debug.Assert(initialized);
      return sfizz_load_scala_string(nativePtr, text);
    }

    public void SetScalaRootKey(int rootKey) {
      Debug.Assert(initialized);
      sfizz_set_scala_root_key(nativePtr, rootKey);
    }

    public int GetScalaRootKey() {
      Debug.Assert(initialized);
      return sfizz_get_scala_root_key(nativePtr);
    }

    public void SetTuningFrequency(float frequency) {
      Debug.Assert(initialized);
      sfizz_set_tuning_frequency(nativePtr, frequency);
    }

    public float GetTuningFrequency() {
      Debug.Assert(initialized);
      return sfizz_get_tuning_frequency(nativePtr);
    }

    public void LoadStretchTuningByRatio(float ratio) {
      Debug.Assert(initialized);
      sfizz_load_stretch_tuning_by_ratio(nativePtr, ratio);
    }

    public int GetNumRegions() {
      Debug.Assert(initialized);
      return sfizz_get_num_regions(nativePtr);
    }

    public int GetNumGroups() {
      Debug.Assert(initialized);
      return sfizz_get_num_groups(nativePtr);
    }

    public int GetNumMasters() {
      Debug.Assert(initialized);
      return sfizz_get_num_masters(nativePtr);
    }

    public int GetNumCurves() {
      Debug.Assert(initialized);
      return sfizz_get_num_curves(nativePtr);
    }

    public string ExportMidnam(string model) {
      Debug.Assert(initialized);
      return sfizz_export_midnam(nativePtr, model);
    }

    public int GetNumPreloadedSamples() {
      Debug.Assert(initialized);
      return sfizz_get_num_preloaded_samples(nativePtr);
    }

    public int GetNumActiveVoices() {
      Debug.Assert(initialized);
      return sfizz_get_num_active_voices(nativePtr);
    }

    public void SetSamplesPerBlock(int samplesPerBlock) {
      Debug.Assert(initialized);
      sfizz_set_samples_per_block(nativePtr, samplesPerBlock);
    }

    public void SetSampleRate(float sampleRate) {
      Debug.Assert(initialized);
      sfizz_set_sample_rate(nativePtr, sampleRate);
    }

    public void SendNoteOn(int delay, int noteNumber, int velocity) {
      Debug.Assert(initialized);
      sfizz_send_note_on(nativePtr, delay, noteNumber, velocity);
    }

    public void SendHDNoteOn(int delay, int noteNumber, float velocity) {
      Debug.Assert(initialized);
      sfizz_send_hd_note_on(nativePtr, delay, noteNumber, velocity);
    }

    public void SendNoteOff(int delay, int noteNumber, int velocity) {
      Debug.Assert(initialized);
      sfizz_send_note_off(nativePtr, delay, noteNumber, velocity);
    }

    public void SendHDNoteOff(int delay, int noteNumber, float velocity) {
      Debug.Assert(initialized);
      sfizz_send_hd_note_off(nativePtr, delay, noteNumber, velocity);
    }

    public void SendCC(int delay, int ccNumber, int ccValue) {
      Debug.Assert(initialized);
      sfizz_send_cc(nativePtr, delay, ccNumber, ccValue);
    }

    public void SendHDCC(int delay, int ccNumber, float normValue) {
      Debug.Assert(initialized);
      sfizz_send_hdcc(nativePtr, delay, ccNumber, normValue);
    }

    public void SendProgramChange(int delay, int program) {
      Debug.Assert(initialized);
      sfizz_send_program_change(nativePtr, delay, program);
    }

    public void AutomateHDCC(int delay, int ccNumber, float normValue) {
      Debug.Assert(initialized);
      sfizz_automate_hdcc(nativePtr, delay, ccNumber, normValue);
    }

    public void SendPitchWheel(int delay, int pitch) {
      Debug.Assert(initialized);
      sfizz_send_pitch_wheel(nativePtr, delay, pitch);
    }

    public void SendHDPitchWheel(int delay, float pitch) {
      Debug.Assert(initialized);
      sfizz_send_hd_pitch_wheel(nativePtr, delay, pitch);
    }

    public void SendChannelAftertouch(int delay, int aftertouch) {
      Debug.Assert(initialized);
      sfizz_send_channel_aftertouch(nativePtr, delay, aftertouch);
    }

    public void SendHDChannelAftertouch(int delay, float aftertouch) {
      Debug.Assert(initialized);
      sfizz_send_hd_channel_aftertouch(nativePtr, delay, aftertouch);
    }

    public void SendPolyAftertouch(int delay, int noteNumber, int aftertouch) {
      Debug.Assert(initialized);
      sfizz_send_poly_aftertouch(nativePtr, delay, noteNumber, aftertouch);
    }

    public void SendHDPolyAftertouch(int delay, int noteNumber, float aftertouch) {
      Debug.Assert(initialized);
      sfizz_send_hd_poly_aftertouch(nativePtr, delay, noteNumber, aftertouch);
    }

    public void SendBpmTempo(int delay, float beatsPerMinute) {
      Debug.Assert(initialized);
      sfizz_send_bpm_tempo(nativePtr, delay, beatsPerMinute);
    }

    public void SendTimeSignature(int delay, int beatsPerBar, int beatUnit) {
      Debug.Assert(initialized);
      sfizz_send_time_signature(nativePtr, delay, beatsPerBar, beatUnit);
    }

    public void SendTimePosition(int delay, int bar, double barBeat) {
      Debug.Assert(initialized);
      sfizz_send_time_position(nativePtr, delay, bar, barBeat);
    }

    public void SendPlaybackState(int delay, int playbackState) {
      Debug.Assert(initialized);
      sfizz_send_playback_state(nativePtr, delay, playbackState);
    }

    /// <summary>
    /// Samples a specific duration of audio and writes it in the given buffer.
    /// </summary>
    /// <param name="buffer">
    /// An array of numChannels float arrays, each holding numFrames floats.
    /// </param>
    /// <param name="numChannels">
    /// The number of channels in the buffer, must be multiples of 2 (stereo).
    /// </param>
    /// <param name="numFrames">
    /// The number of frames in the buffer. Behavior is undefined if this exceeds the number set in
    /// <see cref="SetSamplesPerBlock(int)"/>.
    /// </param>
    public void RenderBlock(float[][] buffer, int numChannels, int numFrames) {
      Debug.Assert(initialized);
      Debug.Assert(numChannels % 2 == 0);
      Debug.Assert(numChannels > 0);
      Debug.Assert(numFrames > 0);
      Debug.Assert(buffer.Length == numChannels);

      // The renderBlock method takes in a list of pointers to float arrays, one for each channel.
      // We therefore need to pin each channel separately and creates an overall address array.
      int channels = buffer.Length;
      IntPtr[] ptrs = new IntPtr[channels];
      GCHandle[] handles = new GCHandle[channels];

      for (int i = 0; i < channels; i++) {
        handles[i] = GCHandle.Alloc(buffer[i], GCHandleType.Pinned);
        ptrs[i] = handles[i].AddrOfPinnedObject();
      }

      GCHandle bufferHandle = GCHandle.Alloc(ptrs, GCHandleType.Pinned);
      sfizz_render_block(nativePtr, bufferHandle.AddrOfPinnedObject(), numChannels, numFrames);

      for (int i = 0; i < channels; i++) {
        handles[i].Free();
      }

      bufferHandle.Free();
    }

    public int GetPreloadSize() {
      Debug.Assert(initialized);
      return sfizz_get_preload_size(nativePtr);
    }

    public void SetPreloadSize(int preloadSize) {
      Debug.Assert(initialized);
      sfizz_set_preload_size(nativePtr, preloadSize);
    }

    public OversamplingFactor GetOversamplingFactor() {
      Debug.Assert(initialized);
      return sfizz_get_oversampling_factor(nativePtr);
    }

    public bool SetOversamplingFactor(OversamplingFactor oversampling) {
      Debug.Assert(initialized);
      return sfizz_set_oversampling_factor(nativePtr, oversampling);
    }

    public int GetSampleQuality(ProcessMode mode) {
      Debug.Assert(initialized);
      return sfizz_get_sample_quality(nativePtr, mode);
    }

    public void SetSampleQuality(ProcessMode mode, int quality) {
      Debug.Assert(initialized);
      sfizz_set_sample_quality(nativePtr, mode, quality);
    }

    public int GetOscillatorQuality(ProcessMode mode) {
      Debug.Assert(initialized);
      return sfizz_get_oscillator_quality(nativePtr, mode);
    }

    public void SetOscillatorQuality(ProcessMode mode, int quality) {
      Debug.Assert(initialized);
      sfizz_set_oscillator_quality(nativePtr, mode, quality);
    }

    public void SetSustainCancelsRelease(bool value) {
      Debug.Assert(initialized);
      sfizz_set_sustain_cancels_release(nativePtr, value);
    }

    public void SetVolume(float volume) {
      Debug.Assert(initialized);
      sfizz_set_volume(nativePtr, volume);
    }

    public float GetVolume() {
      Debug.Assert(initialized);
      return sfizz_get_volume(nativePtr);
    }

    public void SetNumVoices(int numVoices) {
      Debug.Assert(initialized);
      sfizz_set_num_voices(nativePtr, numVoices);
    }

    public int GetNumVoices() {
      Debug.Assert(initialized);
      return sfizz_get_num_voices(nativePtr);
    }

    public int GetNumBuffers() {
      Debug.Assert(initialized);
      return sfizz_get_num_buffers(nativePtr);
    }

    public int GetNumBytes() {
      Debug.Assert(initialized);
      return sfizz_get_num_bytes(nativePtr);
    }

    public void EnableFreewheeling() {
      Debug.Assert(initialized);
      sfizz_enable_freewheeling(nativePtr);
    }

    public void DisableFreewheeling() {
      Debug.Assert(initialized);
      sfizz_disable_freewheeling(nativePtr);
    }

    public string GetUnknownOpcodes() {
      Debug.Assert(initialized);
      return sfizz_get_unknown_opcodes(nativePtr);
    }

    public bool ShouldReloadFile() {
      Debug.Assert(initialized);
      return sfizz_should_reload_file(nativePtr);
    }

    public bool ShouldReloadScala() {
      Debug.Assert(initialized);
      return sfizz_should_reload_scala(nativePtr);
    }

    public void AllSoundOff() {
      Debug.Assert(initialized);
      sfizz_all_sound_off(nativePtr);
    }

    public void AddExternalDefinitions(string id, string value) {
      Debug.Assert(initialized);
      sfizz_add_external_definitions(nativePtr, id, value);
    }

    public void ClearExternalDefinitions() {
      Debug.Assert(initialized);
      sfizz_clear_external_definitions(nativePtr);
    }

    public int GetNumKeyLabels() {
      Debug.Assert(initialized);
      return sfizz_get_num_key_labels(nativePtr);
    }

    public int GetKeyLabelNumber(int labelIndex) {
      Debug.Assert(initialized);
      return sfizz_get_key_label_number(nativePtr, labelIndex);
    }

    public string GetKeyLabelText(int labelIndex) {
      Debug.Assert(initialized);
      return sfizz_get_key_label_text(nativePtr, labelIndex);
    }

    public int GetNumCCLabels() {
      Debug.Assert(initialized);
      return sfizz_get_num_cc_labels(nativePtr);
    }

    public int GetCCLabelNumber(int labelIndex) {
      Debug.Assert(initialized);
      return sfizz_get_cc_label_number(nativePtr, labelIndex);
    }

    public string GetCCLabelText(int labelIndex) {
      Debug.Assert(initialized);
      return sfizz_get_cc_label_text(nativePtr, labelIndex);
    }
#region Implementation
#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern IntPtr sfizz_create_synth();

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_free(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_add_ref(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern bool sfizz_load_file(IntPtr synth, string path);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern bool sfizz_load_string(IntPtr synth, string path, string text);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern bool sfizz_load_scala_file(IntPtr synth, string path);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern bool sfizz_load_scala_string(IntPtr synth, string text);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_scala_root_key(IntPtr synth, int root_key);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_scala_root_key(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_tuning_frequency(IntPtr synth, float frequency);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern float sfizz_get_tuning_frequency(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_load_stretch_tuning_by_ratio(IntPtr synth, float ratio);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_regions(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_groups(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_masters(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_curves(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern string sfizz_export_midnam(IntPtr synth, string model);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_preloaded_samples(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_active_voices(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_samples_per_block(IntPtr synth, int samples_per_block);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_sample_rate(IntPtr synth, float sample_rate);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_note_on(IntPtr synth, int delay, int note_number,
                                                  int velocity);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_hd_note_on(IntPtr synth, int delay, int note_number,
                                                     float velocity);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_note_off(IntPtr synth, int delay, int note_number,
                                                   int velocity);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_hd_note_off(IntPtr synth, int delay, int note_number,
                                                      float velocity);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_cc(IntPtr synth, int delay, int cc_number, int cc_value);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_hdcc(IntPtr synth, int delay, int cc_number,
                                               float norm_value);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_program_change(IntPtr synth, int delay, int program);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_automate_hdcc(IntPtr synth, int delay, int cc_number,
                                                   float norm_value);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_pitch_wheel(IntPtr synth, int delay, int pitch);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_hd_pitch_wheel(IntPtr synth, int delay, float pitch);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_channel_aftertouch(IntPtr synth, int delay,
                                                             int aftertouch);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_hd_channel_aftertouch(IntPtr synth, int delay,
                                                                float aftertouch);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_poly_aftertouch(IntPtr synth, int delay, int note_number,
                                                          int aftertouch);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_hd_poly_aftertouch(IntPtr synth, int delay,
                                                             int note_number, float aftertouch);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_bpm_tempo(IntPtr synth, int delay,
                                                    float beats_per_minute);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_time_signature(IntPtr synth, int delay, int beats_per_bar,
                                                         int beat_unit);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_time_position(IntPtr synth, int delay, int bar,
                                                        double bar_beat);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_send_playback_state(IntPtr synth, int delay,
                                                         int playback_state);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_render_block(IntPtr synth, IntPtr channels, int num_channels,
                                                  int num_frames);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_preload_size(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_preload_size(IntPtr synth, int preload_size);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern OversamplingFactor sfizz_get_oversampling_factor(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern bool sfizz_set_oversampling_factor(IntPtr synth,
                                                             OversamplingFactor oversampling);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_sample_quality(IntPtr synth, ProcessMode mode);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_sample_quality(IntPtr synth, ProcessMode mode,
                                                        int quality);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_oscillator_quality(IntPtr synth, ProcessMode mode);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_oscillator_quality(IntPtr synth, ProcessMode mode,
                                                            int quality);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_sustain_cancels_release(IntPtr synth, bool value);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_volume(IntPtr synth, float volume);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern float sfizz_get_volume(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_set_num_voices(IntPtr synth, int num_voices);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_voices(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_buffers(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_bytes(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_enable_freewheeling(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_disable_freewheeling(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern string sfizz_get_unknown_opcodes(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern bool sfizz_should_reload_file(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern bool sfizz_should_reload_scala(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_all_sound_off(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_add_external_definitions(IntPtr synth, string id,
                                                              string value);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_clear_external_definitions(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_key_labels(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_key_label_number(IntPtr synth, int label_index);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern string sfizz_get_key_label_text(IntPtr synth, int label_index);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_num_cc_labels(IntPtr synth);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern int sfizz_get_cc_label_number(IntPtr synth, int label_index);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern string sfizz_get_cc_label_text(IntPtr synth, int label_index);

#if UNITY_IPHONE
    [DllImport("__Internal")]
#else
    [DllImport("sfizz")]
#endif
    private static extern void sfizz_free_memory(IntPtr ptr);

    /// <summary>
    /// Stores the pointer to the Sfizz object on the C++ side.
    /// </summary>
    private IntPtr nativePtr;

    /// <summary>
    /// Whether the object has been initialized. Prevents multiple <see cref="Dispose"/> calls.
    /// </summary>
    private bool initialized;
#endregion
  }
}
