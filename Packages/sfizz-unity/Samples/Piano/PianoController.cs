using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace F1yingBanana.SfizzUnity.Piano {
  /// <summary>
  /// A sample piano that translates keystrokes to sound.
  /// </summary>
  public class PianoController : MonoBehaviour {
    [field:SerializeField]
    public SfizzPlayer Player { get; set; }

    [field:Range(0, 127)]
    [field:SerializeField]
    public int Velocity {
      get; set;
    } = 64;

    [field:SerializeField]
    public Object InitialSfzFile {
      get; set;
    }

    private void Start() {
      if (!Application.isEditor) {
        Debug.LogError(
            "PianoController does not work in standalone builds, see the class for more info.");
        return;
      }

#if UNITY_EDITOR
      // Note that this will NOT work outside the Unity editor, due to how Unity removes directory
      // structure of assets in builds, even if we are saving the path instead of getting it
      // dynamically with AssetDatabase. In your own code, put your sfz files, e.g. piano.sfz, in
      // Assets/StreamingAssets/piano.sfz and use Application.streamingAssetsPath/piano.sfz instead.
      // For downloaded assets, extract the files into Application.persistentDataPath first.
      string path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.dataPath),
                                                  AssetDatabase.GetAssetPath(InitialSfzFile)));

      if (!Player.Sfizz.LoadFile(path)) {
        Debug.LogWarning($"Sfz not found at the given path: {path}, player will remain silent.");
      }
#endif
    }

    private void Update() {
      int octaveModifier = 0;

      if (Keyboard.current.leftCtrlKey.isPressed) {
        octaveModifier -= 12;
      }

      if (Keyboard.current.rightCtrlKey.isPressed) {
        octaveModifier += 12;
      }

      if (Keyboard.current.leftShiftKey.isPressed) {
        octaveModifier -= 1;
      }

      if (Keyboard.current.rightShiftKey.isPressed) {
        octaveModifier += 1;
      }

      // Since render is called every frame on the player, which is in sync with Unity's event
      // system, each key press has essentially 0 delay in the buffer.
      if (Keyboard.current.aKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 60 + octaveModifier, Velocity);
      } else if (Keyboard.current.aKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 60 + octaveModifier, Velocity);
      }

      if (Keyboard.current.sKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 62 + octaveModifier, Velocity);
      } else if (Keyboard.current.sKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 62 + octaveModifier, Velocity);
      }

      if (Keyboard.current.dKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 64 + octaveModifier, Velocity);
      } else if (Keyboard.current.dKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 64 + octaveModifier, Velocity);
      }

      if (Keyboard.current.fKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 65 + octaveModifier, Velocity);
      } else if (Keyboard.current.fKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 65 + octaveModifier, Velocity);
      }

      if (Keyboard.current.gKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 67 + octaveModifier, Velocity);
      } else if (Keyboard.current.gKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 67 + octaveModifier, Velocity);
      }

      if (Keyboard.current.hKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 69 + octaveModifier, Velocity);
      } else if (Keyboard.current.hKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 69 + octaveModifier, Velocity);
      }

      if (Keyboard.current.jKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 71 + octaveModifier, Velocity);
      } else if (Keyboard.current.jKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 71 + octaveModifier, Velocity);
      }

      if (Keyboard.current.kKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 72 + octaveModifier, Velocity);
      } else if (Keyboard.current.kKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 72 + octaveModifier, Velocity);
      }

      if (Keyboard.current.lKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 74 + octaveModifier, Velocity);
      } else if (Keyboard.current.lKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 74 + octaveModifier, Velocity);
      }

      if (Keyboard.current.semicolonKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 76 + octaveModifier, Velocity);
      } else if (Keyboard.current.semicolonKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 76 + octaveModifier, Velocity);
      }

      if (Keyboard.current.quoteKey.wasPressedThisFrame) {
        Player.Sfizz.SendNoteOn(0, 77 + octaveModifier, Velocity);
      } else if (Keyboard.current.quoteKey.wasReleasedThisFrame) {
        Player.Sfizz.SendNoteOff(0, 77 + octaveModifier, Velocity);
      }

      // Pedal.
      if (Keyboard.current.spaceKey.wasPressedThisFrame) {
        Player.Sfizz.SendCC(0, 64, 127);
      } else if (Keyboard.current.spaceKey.wasReleasedThisFrame) {
        Player.Sfizz.SendCC(0, 64, 0);
      }
    }
  }
}
