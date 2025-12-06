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
  internal void ToggleBGAudio()
  {
    if (BGAS == null) return;

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
