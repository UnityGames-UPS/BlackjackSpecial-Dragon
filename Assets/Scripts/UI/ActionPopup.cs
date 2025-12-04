using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public enum PopupType
{
  Split,
  Double,
  Insurance,
  Generic
}

public class ActionPopup : MonoBehaviour
{
  [SerializeField] private TMP_Text messageText;
  [SerializeField] private Button yesButton;
  [SerializeField] private Button noButton;
  [SerializeField] private Toggle dontShowAgainToggle;
  [SerializeField] private CanvasGroup canvasGroup;

  private Action onYes;
  private Action onNo;
  private PopupType currentType;

  private void Awake()
  {
    yesButton.onClick.AddListener(() =>
    {
      SaveTogglePreference();
      onYes?.Invoke();
      Close();
    });

    noButton.onClick.AddListener(() =>
    {
      SaveTogglePreference();
      onNo?.Invoke();
      Close();
    });
  }

  public void Show(string msg, PopupType type, Action yesAction, Action noAction)
  {
    currentType = type;
    onYes = yesAction;
    onNo = noAction;
    messageText.text = msg;

    bool showToggle = type == PopupType.Split || type == PopupType.Double;
    dontShowAgainToggle.gameObject.SetActive(showToggle);

    dontShowAgainToggle.isOn = false;

    gameObject.SetActive(true);
    canvasGroup.blocksRaycasts = true;
    canvasGroup.interactable = true;
    canvasGroup.alpha = 0;
    canvasGroup.DOFade(1f, 0.25f);
  }

  private void SaveTogglePreference()
  {
    if (!dontShowAgainToggle.gameObject.activeSelf) return;
    if (!dontShowAgainToggle.isOn) return;

    PlayerPrefs.SetInt($"popup_skip_{currentType}", 1);
  }

  public static bool ShouldSkip(PopupType type)
  {
    return PlayerPrefs.GetInt($"popup_skip_{type}", 0) == 1;
  }

  private void Close()
  {
    canvasGroup.blocksRaycasts = false;
    canvasGroup.interactable = false;
    canvasGroup.DOFade(0f, 0.25f)
        .OnComplete(() => gameObject.SetActive(false));
  }
}
