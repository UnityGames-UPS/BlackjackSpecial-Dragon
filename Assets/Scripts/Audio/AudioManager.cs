using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
  [SerializeField] private AudioSource BGAS;
  [SerializeField] private AudioSource ButtonAS;
  [SerializeField] private AudioSource CardFlipAS;
  [SerializeField] private AudioSource ChipSelectAS;
  [SerializeField] private AudioSource WinAS;
  [SerializeField] private Image MusicButton_Image;
  [SerializeField] private Sprite[] MusicButton_Sprites; // 0: On 1: Off
  [SerializeField] private Image SoundButton_Image;
  [SerializeField] private Sprite[] SoundButton_Sprites; // 0: On 1: Off

  private bool isBGMusicON = true;
  private bool isSoundsON = true;
  private const float fadeDuration = 0.2f;

  private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();
  private bool isForceMuted = false;

  private IEnumerable<AudioSource> AllSources
  {
    get
    {
      yield return BGAS;
      yield return ButtonAS;
      yield return CardFlipAS;
      yield return ChipSelectAS;
      yield return WinAS;
    }
  }

  // Focus-driven mute — called from BOTH UIManager.OnFocusChanged (JS path) and OnApplicationFocus.
  // Never touches the user's own music/sound choice; it captures and restores each source's .mute.
  internal void SetMuteAll(bool forceMute)
  {
    if (forceMute == isForceMuted) return; // already in that state — don't re-capture/re-restore
    isForceMuted = forceMute;

    foreach (AudioSource source in AllSources)
    {
      if (source == null) continue;

      if (forceMute)
      {
        preFocusMuteState[source] = source.mute;
        source.mute = true;
      }
      else
      {
        source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
      }
    }
  }

  // Native/editor focus path — calls the SAME method the WebGL OnFocusChanged path calls.
  private void OnApplicationFocus(bool focus)
  {
    SetMuteAll(!focus);
  }

  internal void ToggleBGAudio()
  {
    if (BGAS == null) return;

    // An explicit user interaction proves the game really has focus — a stale forced-mute must not win.
    SetMuteAll(false);

    BGAS.DOKill(); // stop previous tweens
    PlayButtonAudio();
    isBGMusicON = !isBGMusicON;

    MusicButton_Image.sprite = MusicButton_Sprites[isBGMusicON ? 0 : 1];
    if (isBGMusicON)
    {
      // Fade in
      if (!BGAS.isPlaying)
        BGAS.Play();

      BGAS.DOFade(1, fadeDuration).OnComplete(() =>
      {
        BGAS.mute = false;
      });
    }
    else
    {
      // Fade out
      BGAS.DOFade(0f, fadeDuration).OnComplete(() =>
      {
        BGAS.Pause();
      });
    }
  }

  internal void ToggleSoundsAudio()
  {
    SetMuteAll(false);
    PlayButtonAudio();
    isSoundsON = !isSoundsON;
    SoundButton_Image.sprite = SoundButton_Sprites[isSoundsON ? 0 : 1];
    FadeAudio(ButtonAS, isSoundsON);
    FadeAudio(CardFlipAS, isSoundsON);
    FadeAudio(ChipSelectAS, isSoundsON);
    FadeAudio(WinAS, isSoundsON);
  }

  internal void PlayButtonAudio()
  {
    if (!ButtonAS.mute)
      ButtonAS.Play();
  }

  internal void PlayCardFlipAudio()
  {
    if (!CardFlipAS.mute)
      CardFlipAS.Play();
  }

  internal void PlayChipSelectAudio()
  {
    if (!ChipSelectAS.mute)
      ChipSelectAS.Play();
  }

  internal void PlayWinAudio()
  {
    if (!WinAS.mute)
      WinAS.Play();
  }

  private void FadeAudio(AudioSource source, bool enable)
  {
    if (source == null) return;

    source.DOKill();

    if (enable)
    {
      source.mute = false;
      source.DOFade(1, fadeDuration);
    }
    else
    {
      source.DOFade(0f, fadeDuration).OnComplete(() =>
      {
        source.mute = true;
      });
    }
  }
}
