using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace F1yingBanana.SfizzUnity {
  /// <summary>
  /// A renderer that takes in MIDI files, some SFZ files as instruments, and generates <see
  /// cref="AudioClip"/>s at runtime.
  /// </summary>
  public sealed class SfizzMidiRenderer : MonoBehaviour {
    /// <summary>
    /// Renders the given MIDI file with the given instrument. If the MIDI file features multiple
    /// channels, they will all be rendered with the same instrument. If the MIDI file has format 1,
    /// this will add all tracks' sound waves together. If the MIDI file has format 2, then this
    /// will render each track into a separate <see cref=" AudioClip"/>.
    /// </summary>
    /// <param name="midiFile">The MIDI file to render.</param>
    /// <param name="sfzPath">The absolute path to the instrument file.</param>
    /// <param name="sampleRate">The sample rate of the output clips.</param>
    /// <returns>
    /// An array of generated audio clips. When you are done with each clip, destroy it via <see
    /// cref="Object.Destroy(Object)"/>.
    /// </returns>
    public AudioClip[] Render(MidiFile midiFile, string sfzPath, int sampleRate) {
      return Render(midiFile, new SfizzArray(sfzPath, sampleRate), sampleRate);
    }

    /// <summary>
    /// Renders the given MIDI file with the given instruments, with the array index corresponding
    /// to the channel index. If the MIDI file has format 1, this will add all tracks' sound waves
    /// together. If the MIDI file has format 2, then this will render each track into a separate
    /// <see cref=" AudioClip"/>.
    /// </summary>
    /// <param name="midiFile">The MIDI file to render.</param>
    /// <param name="sfzPaths">The absolute paths to the instrument files.</param>
    /// <param name="sampleRate">The sample rate of the output clips.</param>
    /// <returns>
    /// An array of generated audio clips. When you are done with each clip, destroy it via <see
    /// cref="Object.Destroy(Object)"/>.
    /// </returns>
    public AudioClip[] Render(MidiFile midiFile, string[] sfzPaths, int sampleRate) {
      return Render(midiFile, new SfizzArray(sfzPaths, sampleRate), sampleRate);
    }
#region Implementation
    private sealed class SfizzArray : IDisposable {
      private Sfizz[] sfizzes;
      private bool[] sfizzLoadAttempted;
      private string[] sfzPaths;
      private bool singular;
      private int sampleRate;

      public int Length => sfizzes.Length;

      /// <summary>
      /// Creates a new Sfizz array with an array of SFZ paths.
      /// </summary>
      public SfizzArray(string[] sfzPaths, int sampleRate) {
        this.sfzPaths = sfzPaths;
        sfizzes = new Sfizz[sfzPaths.Length];
        sfizzLoadAttempted = new bool[sfzPaths.Length];
        this.sampleRate = sampleRate;
      }

      /// <summary>
      /// Creates a new Sfizz array with a single SFZ path. All channels will point to this Sfizz.
      /// </summary>
      public SfizzArray(string sfzPath, int sampleRate) {
        sfzPaths = new string[] { sfzPath };
        sfizzes = new Sfizz[1];
        sfizzLoadAttempted = new bool[1];
        singular = true;
        this.sampleRate = sampleRate;
      }

      public Sfizz this[int channel] {
        get => GetSfizz(singular ? 0 : channel);
      }

      public void Dispose() {
        foreach (Sfizz sfizz in sfizzes) {
          sfizz?.Dispose();
        }
      }

      private Sfizz GetSfizz(int channel) {
        Sfizz sfizz = null;

        if (0 <= channel && channel < sfizzes.Length) {
          sfizz = sfizzes[channel];

          if (sfizz == null && !sfizzLoadAttempted[channel]) {
            // Attempt to initialize it.
            sfizzLoadAttempted[channel] = true;
            Sfizz newSfizz = new Sfizz();

            if (newSfizz.LoadFile(sfzPaths[channel])) {
              sfizzes[channel] = newSfizz;
              sfizz = newSfizz;
              sfizz.SetSampleRate(sampleRate);
              sfizz.SetSamplesPerBlock(Sfizz.MaxBlockSize);
            } else {
              newSfizz.Dispose();
            }
          }
        }

        if (sfizz == null) {
          Debug.LogWarning(
              $"Midi contains a note on channel {channel} but this is not specified by any SFZ fi" +
              $"le. This note will be ignored.");
        }

        return sfizz;
      }
    }

    private const int BufferSize = Sfizz.MaxBlockSize;

    private AudioClip[] Render(MidiFile midiFile, SfizzArray sfizzArray, int sampleRate) {
      AudioClip[] clips;
      TempoMap tempoMap = midiFile.GetTempoMap();

      if (midiFile.OriginalFormat == MidiFileFormat.SingleTrack ||
          midiFile.OriginalFormat == MidiFileFormat.MultiTrack) {
        // All channels should be rendered on the same buffer by add.
        clips = new AudioClip[1];
        clips[0] = RenderEvents(midiFile.GetTimedEvents(), tempoMap, sampleRate, sfizzArray);
        clips[0].name = "Track";
      } else {
        List<TrackChunk> tracks = new List<TrackChunk>(midiFile.GetTrackChunks());
        clips = new AudioClip[tracks.Count];

        for (int i = 0; i < tracks.Count; i++) {
          // Each track is rendered separately. Each track may still use multiple channels though.
          clips[i] = RenderEvents(tracks[i].GetTimedEvents(), tempoMap, sampleRate, sfizzArray);
          clips[i].name = $"Track {i}";
        }
      }

      sfizzArray.Dispose();
      return clips;
    }

    private AudioClip RenderEvents(ICollection<TimedEvent> timedEventsCollection, TempoMap tempoMap,
                                   int sampleRate, SfizzArray sfizzArray) {
      if (timedEventsCollection.Count == 0) {
        return AudioClip.Create("", 0, 2, sampleRate, false);
      }

      Queue<TimedEvent> timedEvents = new Queue<TimedEvent>(timedEventsCollection);
      int totalDuration = Mathf.CeilToInt(
          (float)(timedEventsCollection.Last().TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds *
                  sampleRate / 1e6));
      AudioClip clip = AudioClip.Create("", totalDuration, 2, sampleRate, false);
      float[][] buffer = new float[][] { new float[BufferSize], new float[BufferSize] };
      float[] interleavedBuffer = new float[Sfizz.MaxBlockSize * 2];
      int offset = 0;

      while (timedEvents.Count > 0) {
        // We send a note to a particular Sfizz of the corresponding channel. An Sfizz is
        // instantiated on first use. Whenever we started using an Sfizz, we need to render its
        // buffer every time we go to a new buffer because there may be sound even if there is
        // no event (e.g. a single long note).
        TimedEvent timedEvent = timedEvents.Dequeue();

        if (timedEvent.Event is not ChannelEvent channelEvent) {
          continue;
        }

        int time =
            Mathf.CeilToInt((float)(timedEvent.TimeAs<MetricTimeSpan>(tempoMap).TotalMicroseconds *
                                    sampleRate / 1e6));

        if (time < offset) {
          // This should never happen.
          continue;
        }

        while (time > offset + Sfizz.MaxBlockSize) {
          // We need to render the inbetween buffers. Every non-null sfizz renders to the same
          // buffer, and this is pushed to the AudioClip.
          Array.Clear(interleavedBuffer, 0, interleavedBuffer.Length);
          RenderBuffer(sfizzArray, buffer, interleavedBuffer, Sfizz.MaxBlockSize);
          clip.SetData(interleavedBuffer, offset);
          offset += Sfizz.MaxBlockSize;
        }

        // Render the note within.
        Sfizz sfizz = sfizzArray[channelEvent.Channel];

        if (sfizz == null) {
          continue;
        }

        int offsetTime = time % Sfizz.MaxBlockSize;

        switch (channelEvent.EventType) {
          case MidiEventType.ChannelAftertouch:
            ChannelAftertouchEvent channelAftertouchEvent = channelEvent as ChannelAftertouchEvent;
            sfizz.SendChannelAftertouch(offsetTime, channelAftertouchEvent.AftertouchValue);
            break;
          case MidiEventType.ControlChange:
            ControlChangeEvent controlChangeEvent = channelEvent as ControlChangeEvent;
            sfizz.SendCC(offsetTime, controlChangeEvent.ControlNumber,
                         controlChangeEvent.ControlValue);
            break;
          case MidiEventType.NoteAftertouch:
            NoteAftertouchEvent noteAftertouchEvent = channelEvent as NoteAftertouchEvent;
            sfizz.SendPolyAftertouch(offsetTime, noteAftertouchEvent.NoteNumber,
                                     noteAftertouchEvent.AftertouchValue);
            break;
          case MidiEventType.NoteOff:
            NoteOffEvent noteOffEvent = channelEvent as NoteOffEvent;
            sfizz.SendNoteOff(offsetTime, noteOffEvent.NoteNumber, noteOffEvent.Velocity);
            break;
          case MidiEventType.NoteOn:
            NoteOnEvent noteOnEvent = channelEvent as NoteOnEvent;
            sfizz.SendNoteOn(offsetTime, noteOnEvent.NoteNumber, noteOnEvent.Velocity);
            break;
          case MidiEventType.PitchBend:
            PitchBendEvent pitchBendEvent = channelEvent as PitchBendEvent;
            sfizz.SendPitchWheel(offsetTime, pitchBendEvent.PitchValue);
            break;
          case MidiEventType.ProgramChange:
            ProgramChangeEvent programChangeEvent = channelEvent as ProgramChangeEvent;
            sfizz.SendProgramChange(offsetTime, programChangeEvent.ProgramNumber);
            break;
        }
      }

      // Render the last buffer and finalize.
      int lastSize = totalDuration - offset;
      float[] lastBuffer = new float[lastSize * 2];
      RenderBuffer(sfizzArray, buffer, lastBuffer, lastSize);
      clip.SetData(lastBuffer, offset);

      return clip;
    }

    private void RenderBuffer(SfizzArray sfizzArray, float[][] buffer, float[] interleavedBuffer,
                              int sampleCount) {
      for (int i = 0; i < sfizzArray.Length; i++) {
        Sfizz renderSfizz = sfizzArray[i];

        if (renderSfizz == null) {
          continue;
        }

        renderSfizz.RenderBlock(buffer, 2, sampleCount);
        InterleaveAdd(buffer, interleavedBuffer, sampleCount);
      }
    }

    private void InterleaveAdd(float[][] input, float[] output, int samples) {
      for (int i = 0; i < samples; i++) {
        output[2 * i] += input[0][i];
        output[2 * i + 1] += input[1][i];
      }
    }
#endregion
  }
}
