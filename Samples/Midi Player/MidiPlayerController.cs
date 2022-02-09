using Melanchall.DryWetMidi.Core;
using System.Diagnostics;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace F1yingBanana.SfizzUnity.MidiPlayer {
  public sealed class MidiPlayerController : MonoBehaviour {
    [field:SerializeField]
    public Object Midi { get; set; }

    [field:SerializeField]
    public Object[] SfzFiles {
      get; set;
    }

    [field:Range(1, Sfizz.MaxSampleRate)]
    [field:SerializeField]
    public int SampleRate {
      get; set;
    } = 44100;

    [field:SerializeField]
    public SfizzMidiRenderer SfizzMidiRenderer {
      get; set;
    }

    // Start is called before the first frame update
    public void RenderMidi() {
      Stopwatch stopwatch = Stopwatch.StartNew();
      MidiFile midiFile = MidiFile.Read(GetObjectPath(Midi));
      AudioClip[] clips;

      if (SfzFiles.Length == 1) {
        clips = SfizzMidiRenderer.Render(midiFile, GetObjectPath(SfzFiles[0]), SampleRate);
      } else {
        string[] paths = new string[SfzFiles.Length];

        for (int i = 0; i < SfzFiles.Length; i++) {
          paths[i] = GetObjectPath(SfzFiles[i]);
        }

        clips = SfizzMidiRenderer.Render(midiFile, paths, SampleRate);
      }

      stopwatch.Stop();
      UnityEngine.Debug.Log($"Rendering took {stopwatch.ElapsedMilliseconds} ms.");

      for (int i = 0; i < clips.Length; i++) {
        AudioClip clip = clips[i];

        if (clip != null) {
          GameObject source = new GameObject();
          source.name = $"Track {i}";
          source.transform.SetParent(transform);
          AudioSource audioSource = source.AddComponent<AudioSource>();
          audioSource.clip = clip;
          audioSource.Play();
        }
      }
    }

    /// <summary>
    /// Returns the absolute path of the given resource object.
    ///
    /// Note that this will NOT work outside the Unity editor, due to how Unity removes directory
    /// structure of assets in builds, even if we are saving the path instead of getting it
    /// dynamically with AssetDatabase. In your own code, put your sfz files, e.g. piano.sfz, in
    /// Assets/StreamingAssets/piano.sfz and use Application.streamingAssetsPath/piano.sfz instead.
    /// For downloaded assets, extract the files into Application.persistentDataPath first.
    /// </summary>
    private string GetObjectPath(Object @object) {
#if UNITY_EDITOR
      return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.dataPath),
                                           AssetDatabase.GetAssetPath(@object)));
#else
      Debug.LogError("This example will only work in the editor! See file comments.");
      return "";
#endif
    }
  }
}
