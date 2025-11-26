using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
  [SerializeField] private SocketIOManager socket;

  [SerializeField] private Transform ChipParent_Transform;
  [Header("Initial Buttons")]
  [SerializeField] private Button UndoBet_Button;
  [SerializeField] private Button Deal_Button;
  [SerializeField] private Button ClearBet_Button;
  [SerializeField] private Button DoubleBet_Button;
  [SerializeField] private Button MainBet_Button;
  [SerializeField] private Button MultiplierBet_Button;
  [SerializeField] private Button Quit_Button;
  [SerializeField] private Button QuitYes_Button;
  [SerializeField] private Button QuitNo_Button;
  [SerializeField] private Button Info_Button;
  [SerializeField] private Button InfoClose_Button;
  [SerializeField] private Button Settings_Button;
  [SerializeField] private Button SettingsClose_Button;
  [SerializeField] private Button BlackBackground_Button;

  [Header("Middle Buttons")]
  [SerializeField] private Button Hit_Button;
  [SerializeField] private Button Stand_button;
  [SerializeField] private Button DoubleShow_Button;
  [SerializeField] private Button Split_Button;

  [Header("Rebet Buttons")]
  [SerializeField] private Button Rebet_Button;
  [SerializeField] private Button RebetDeal_Button;
  [SerializeField] private Button RebetDouble_Button;

  [Header("List & Arrays")]
  [SerializeField] private Button[] Chips_Button;
  [SerializeField] private GameObject[] Chips_Object;

  [Header("Betting Buttons")]
  [SerializeField] private Button LeftArr_Button;
  [SerializeField] private Button RightArr_Button;

  [Header("GameObjets")]
  [SerializeField] private GameObject InitialButtons_object;
  [SerializeField] private GameObject MiddleButtons_object;
  [SerializeField] private GameObject RebetButtons_object;
  [SerializeField] private GameObject MainBet_object;
  [SerializeField] private GameObject PlayerCardTotal_Object;
  [SerializeField] private GameObject DealerCardTotal_Object;
  [SerializeField] private GameObject ChipBets_Object;
  [SerializeField] private GameObject Split_object;
  [SerializeField] private GameObject MiddleDouble_object;
  [SerializeField] private GameObject BetButton_Object;
  [SerializeField] private GameObject ChipContainer_Object;
  [SerializeField] private GameObject MultiplierBetButton_Object;
  [SerializeField] private GameObject ArrPointer_Object;
  [SerializeField] private GameObject FirstArrPointer_Object;
  [SerializeField] private GameObject SecondArrPointer_Object;

  [SerializeField] private GameObject QuitPopup_Object;
  [SerializeField] private GameObject InfoPopup_Object;
  [SerializeField] private GameObject SettingsPopup_Object;
  [SerializeField] private GameObject ReconnectPopup_Object;
  [SerializeField] private GameObject DisconnectPopup_Object;

  [SerializeField] private CanvasGroup MaxBetPopup_CG;
  [SerializeField] private TMP_Text MaxBetPopup_Text;

  [Header("Managers")]
  [SerializeField] private BJController BJmanager;

  [Header("Transforms")]
  [SerializeField] private Transform ScrollParent_Transform;
  [SerializeField] private Transform Selected_Transform;
  [SerializeField] private Transform[] PDummyPos_Transform;
  [SerializeField] private Transform[] DDummyPos_Transform;
  [SerializeField] private Transform[] FSDummyPos_Transform;
  [SerializeField] private Transform[] SSDummyPos_Transform;

  [Header("Dragon Animation Routine")]
  [SerializeField] private GameObject DragonNormal_Object;
  [SerializeField] private GameObject DragonFire_Object;
  [SerializeField] private GameObject Fire_Object;
  [SerializeField] private Transform X2_Transform;
  [SerializeField] private GameObject X2_Object;
  [SerializeField] private ImageAnimation Box_Animation;
  [SerializeField] private Transform LeftBox_Transform;
  [SerializeField] private Sprite X2_Sprite;
  [SerializeField] private Sprite Empty_Sprite;


  [SerializeField] private ScrollRect ChipScroller;
  Sequence maxBetSequence;
  bool isExit = false;
  int chipCounter = 0;

  private void Start()
  {
    if (MainBet_object) MainBet_object.SetActive(true);
    if (ChipBets_Object) ChipBets_Object.SetActive(true);
    if (PlayerCardTotal_Object) PlayerCardTotal_Object.SetActive(false);
    if (DealerCardTotal_Object) DealerCardTotal_Object.SetActive(false);

    if (MainBet_Button) MainBet_Button.onClick.RemoveAllListeners();
    if (MainBet_Button) MainBet_Button.onClick.AddListener(delegate { OnBet(true); });

    if (MultiplierBet_Button) MultiplierBet_Button.onClick.RemoveAllListeners();
    if (MultiplierBet_Button) MultiplierBet_Button.onClick.AddListener(delegate { OnBet(false); });

    if (Deal_Button) Deal_Button.onClick.RemoveAllListeners();
    if (Deal_Button) Deal_Button.onClick.AddListener(OnDeal);

    if (Hit_Button) Hit_Button.onClick.RemoveAllListeners();
    if (Hit_Button) Hit_Button.onClick.AddListener(delegate { OnHit(); });

    if (Stand_button) Stand_button.onClick.RemoveAllListeners();
    if (Stand_button) Stand_button.onClick.AddListener(OnStand);

    if (ClearBet_Button) ClearBet_Button.onClick.RemoveAllListeners();
    if (ClearBet_Button) ClearBet_Button.onClick.AddListener(OnClear);

    if (UndoBet_Button) UndoBet_Button.onClick.RemoveAllListeners();
    if (UndoBet_Button) UndoBet_Button.onClick.AddListener(OnUndo);

    if (Rebet_Button) Rebet_Button.onClick.RemoveAllListeners();
    if (Rebet_Button) Rebet_Button.onClick.AddListener(OnRebet);

    if (DoubleBet_Button) DoubleBet_Button.onClick.RemoveAllListeners();
    if (DoubleBet_Button) DoubleBet_Button.onClick.AddListener(OnInitialDouble);

    if (DoubleShow_Button) DoubleShow_Button.onClick.RemoveAllListeners();
    if (DoubleShow_Button) DoubleShow_Button.onClick.AddListener(OnDoubleAndStand);

    if (RebetDeal_Button) RebetDeal_Button.onClick.RemoveAllListeners();
    if (RebetDeal_Button) RebetDeal_Button.onClick.AddListener(OnRebetDeal);

    if (RebetDouble_Button) RebetDouble_Button.onClick.RemoveAllListeners();
    if (RebetDouble_Button) RebetDouble_Button.onClick.AddListener(OnRebetDouble);

    if (Split_Button) Split_Button.onClick.RemoveAllListeners();
    if (Split_Button) Split_Button.onClick.AddListener(OnSplit);

    if (Quit_Button) Quit_Button.onClick.RemoveAllListeners();
    if (Quit_Button) Quit_Button.onClick.AddListener(() => OpenPopup(QuitPopup_Object));

    if (Info_Button) Info_Button.onClick.RemoveAllListeners();
    if (Info_Button) Info_Button.onClick.AddListener(() => OpenPopup(InfoPopup_Object));

    if (Settings_Button) Settings_Button.onClick.RemoveAllListeners();
    if (Settings_Button) Settings_Button.onClick.AddListener(() => OpenPopup(SettingsPopup_Object));

    if (QuitYes_Button) QuitYes_Button.onClick.RemoveAllListeners();
    if (QuitYes_Button) QuitYes_Button.onClick.AddListener(CallOnGameQuit);

    if (QuitNo_Button) QuitNo_Button.onClick.RemoveAllListeners();
    if (QuitNo_Button) QuitNo_Button.onClick.AddListener(() => ClosePopup(QuitPopup_Object));

    if (InfoClose_Button) InfoClose_Button.onClick.RemoveAllListeners();
    if (InfoClose_Button) InfoClose_Button.onClick.AddListener(() => ClosePopup(InfoPopup_Object));

    if (SettingsClose_Button) SettingsClose_Button.onClick.RemoveAllListeners();
    if (SettingsClose_Button) SettingsClose_Button.onClick.AddListener(() => ClosePopup(SettingsPopup_Object));

    if (Split_object) Split_object.SetActive(false);
    if (MiddleDouble_object) MiddleDouble_object.SetActive(true);

    if (LeftArr_Button) LeftArr_Button.onClick.RemoveAllListeners();
    if (LeftArr_Button) LeftArr_Button.onClick.AddListener(delegate { OnChipScroll(false); });

    if (RightArr_Button) RightArr_Button.onClick.RemoveAllListeners();
    if (RightArr_Button) RightArr_Button.onClick.AddListener(delegate { OnChipScroll(true); });

    for (int i = 0; i < Chips_Button.Length; i++)
    {
      int index = i;
      Chips_Button[i].onClick.AddListener(() =>
      {
        OnChipButtonClick(index);
      });
    }

    if (Chips_Object[0]) Chips_Object[0].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    if (Chips_Object[0]) Chips_Object[0].transform.SetParent(Selected_Transform);

    chipCounter = 0;

    if (RightArr_Button) LeftArr_Button.interactable = false;
    if (ArrPointer_Object) ArrPointer_Object.SetActive(false);
    if (FirstArrPointer_Object) FirstArrPointer_Object.SetActive(false);
    if (SecondArrPointer_Object) SecondArrPointer_Object.SetActive(false);
    MaxBetPopup_CG.alpha = 0;
  }

  void OnChipButtonClick(int index)
  {
    if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.SetParent(ScrollParent_Transform);
    if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.localScale = Vector3.one;

    chipCounter = index;

    SetScrollToChip(chipCounter);
    if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.SetParent(Selected_Transform);

    if (chipCounter == Chips_Object.Length - 1)
    {
      if (RightArr_Button) RightArr_Button.interactable = false;
    }
    if (chipCounter == 0)
    {
      if (LeftArr_Button) LeftArr_Button.interactable = false;
    }
    if (chipCounter > 0)
    {
      if (LeftArr_Button) LeftArr_Button.interactable = true;
    }
    if (chipCounter < Chips_Object.Length - 1)
    {
      if (RightArr_Button) RightArr_Button.interactable = true;
    }
    BJmanager.SelectCoin(chipCounter);
  }

  private void OnChipScroll(bool direction)
  {
    if (direction)
    {
      if (LeftArr_Button) LeftArr_Button.interactable = true;
      if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.SetParent(ScrollParent_Transform);
      if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.localScale = Vector3.one;
      chipCounter++;
      SetScrollToChip(chipCounter);
      if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
      if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.SetParent(Selected_Transform);
      if (chipCounter == Chips_Object.Length - 1)
      {
        if (RightArr_Button) RightArr_Button.interactable = false;
      }
    }
    else
    {
      if (RightArr_Button) RightArr_Button.interactable = true;
      if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.SetParent(ScrollParent_Transform);
      if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.localScale = Vector3.one;
      chipCounter--;
      SetScrollToChip(chipCounter);
      if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
      if (Chips_Object[chipCounter]) Chips_Object[chipCounter].transform.SetParent(Selected_Transform);
      if (chipCounter == 0)
      {
        if (LeftArr_Button) LeftArr_Button.interactable = false;
      }
    }
    BJmanager.SelectCoin(chipCounter);
  }

  private void SetScrollToChip(int index)
  {
    float contentWidth = ScrollParent_Transform.parent.GetComponent<RectTransform>().rect.width;
    float viewportWidth = ChipScroller.viewport.rect.width;

    float scrollableWidth = contentWidth - viewportWidth;
    if (scrollableWidth <= 0) return;

    RectTransform chip = Chips_Object[index].GetComponent<RectTransform>();
    float itemWidth = chip.rect.width;

    float itemX = index * itemWidth;

    float normalized = Mathf.Clamp01(itemX / scrollableWidth);

    ChipScroller.horizontalNormalizedPosition = normalized;
  }

  private void OnBet(bool isMultiplier)
  {
    if (isMultiplier)
    {
      BJmanager.BetOnButton();
    }
    else
    {
      BJmanager.MultiplierBetOnButton();
    }
  }

  private void OnDeal()
  {
    StartCoroutine(OnStartDeal());
  }

  private void OnClear()
  {
    if (InitialButtons_object) InitialButtons_object.SetActive(false);
    BJmanager.ClearBet();
  }

  private void OnUndo()
  {
    BJmanager.UndoBetButton();
    if (BJmanager.betHistory.Count == 0)
    {
      InitialButtons_object.SetActive(false);
    }
  }

  private void OnInitialDouble()
  {
    BJmanager.DoubleBetButton();
  }

  private void OnHit(bool isDouble = false)
  {
    if (Split_object) Split_object.SetActive(false);
    if (ArrPointer_Object) ArrPointer_Object.SetActive(false);
    StartCoroutine(HitDealButton(isDouble));
  }

  private void OnStand()
  {
    if (ArrPointer_Object) ArrPointer_Object.SetActive(false);
    StartCoroutine(DealerFinalButton());
  }

  private void OnDoubleAndStand()
  {
    if (ArrPointer_Object) ArrPointer_Object.SetActive(false);
    BJmanager.DoubleBetButton();
    OnHit(true);
  }

  private void OnRebet()
  {
    StartCoroutine(OnRebetCoroutine());
  }

  private IEnumerator OnRebetCoroutine()
  {
    if (RebetButtons_object) RebetButtons_object.SetActive(false);
    yield return StartCoroutine(BJmanager.ClearCards());
    if (BetButton_Object) BetButton_Object.SetActive(true);
    // if (ChipContainer_Object) ChipContainer_Object.SetActive(true);
    // if (MultiplierBetButton_Object) MultiplierBetButton_Object.SetActive(true);
    if (MiddleDouble_object) MiddleDouble_object.SetActive(true);
    if (MainBet_object) MainBet_object.SetActive(true);
    if (ChipBets_Object) ChipBets_Object.SetActive(true);
    if (PlayerCardTotal_Object) PlayerCardTotal_Object.SetActive(false);
    if (DealerCardTotal_Object) DealerCardTotal_Object.SetActive(false);
    if (InitialButtons_object) InitialButtons_object.SetActive(true);
  }

  private void OnRebetDeal()
  {
    StartCoroutine(OnRebetDealCoroutine());
  }

  private IEnumerator OnRebetDealCoroutine()
  {
    if (BetButton_Object) BetButton_Object.SetActive(false);
    // if (ChipContainer_Object) ChipContainer_Object.SetActive(false);
    // if (MultiplierBetButton_Object) MultiplierBetButton_Object.SetActive(false);
    if (MiddleDouble_object) MiddleDouble_object.SetActive(true);
    if (RebetButtons_object) RebetButtons_object.SetActive(false);
    yield return StartCoroutine(BJmanager.ClearCards());
    OnDeal();
  }

  private void OnRebetDouble()
  {
    StartCoroutine(OnRebetDoubleCoroutine());
  }

  private IEnumerator OnRebetDoubleCoroutine()
  {
    if (BetButton_Object) BetButton_Object.SetActive(false);
    // if (ChipContainer_Object) ChipContainer_Object.SetActive(false);
    // if (MultiplierBetButton_Object) MultiplierBetButton_Object.SetActive(false);
    if (MiddleDouble_object) MiddleDouble_object.SetActive(true);
    BJmanager.DoubleBetButton();
    if (RebetButtons_object) RebetButtons_object.SetActive(false);
    yield return StartCoroutine(BJmanager.ClearCards());
    OnDeal();
  }

  private void OnSplit()
  {
    if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
    StartCoroutine(OnSplitDeal());
  }

  private IEnumerator OnSplitDeal()
  {
    if (MiddleDouble_object) MiddleDouble_object.SetActive(false);
    if (Split_object) Split_object.SetActive(false);
    BJmanager.SplitButton();
    yield return new WaitUntil(() => BJmanager.isSplit);
    if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
  }

  private IEnumerator OnStartDeal()
  {
    if (InitialButtons_object) InitialButtons_object.SetActive(false);
    if (BetButton_Object) BetButton_Object.SetActive(false);
    ChipParent_Transform.localPosition = new(ChipParent_Transform.localPosition.x, ChipParent_Transform.localPosition.y - 130, ChipParent_Transform.localPosition.z);
    // if (ChipContainer_Object) ChipContainer_Object.SetActive(false);
    // if (MultiplierBetButton_Object) MultiplierBetButton_Object.SetActive(false);
    if (MainBet_object) MainBet_object.SetActive(false);
    if (ChipBets_Object) ChipBets_Object.SetActive(false);
    BJmanager.isFlippin = true;
    BJmanager.OnPlayerDealButton(PDummyPos_Transform[BJmanager.playerCounter].localPosition);
    yield return new WaitUntil(() => !BJmanager.isFlippin);
    BJmanager.isFlippin = true;
    BJmanager.OnDealerButton(DDummyPos_Transform[BJmanager.dealerCounter].localPosition);
    yield return new WaitUntil(() => !BJmanager.isFlippin);
    BJmanager.isFlippin = true;
    BJmanager.OnPlayerDealButton(PDummyPos_Transform[BJmanager.playerCounter].localPosition);
    yield return new WaitUntil(() => !BJmanager.isFlippin);
    BJmanager.OnDealerButtonClosedCard(DDummyPos_Transform[BJmanager.dealerCounter].localPosition);
    if (PlayerCardTotal_Object) PlayerCardTotal_Object.SetActive(true);
    if (DealerCardTotal_Object) DealerCardTotal_Object.SetActive(true);
    if (BJmanager.CheckMultiplier())
    {
      yield return DragonRoutine();
      // if (LeftBox_Transform) LeftBox_Transform.GetChild(LeftBox_Transform.childCount - 1).SetAsFirstSibling();
      // if (LeftBox_Transform) LeftBox_Transform.GetChild(0).GetComponent<Image>().sprite = X2_Sprite;
    }
    else
    {
      // if (LeftBox_Transform) LeftBox_Transform.GetChild(LeftBox_Transform.childCount - 1).SetAsFirstSibling();
      // if (LeftBox_Transform) LeftBox_Transform.GetChild(0).GetComponent<Image>().sprite = Empty_Sprite;
    }
    if (ArrPointer_Object) ArrPointer_Object.SetActive(true);
    if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
    if (BJmanager.playerData[0] == BJmanager.playerData[1])
    {
      if (Split_object) Split_object.SetActive(true);
    }
  }

  private IEnumerator DragonRoutine()
  {
    if (DragonFire_Object) DragonFire_Object.SetActive(true);
    if (Fire_Object) Fire_Object.SetActive(true);
    if (DragonNormal_Object) DragonNormal_Object.SetActive(false);
    yield return new WaitForSeconds(1.5f);
    if (X2_Transform) X2_Transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    if (X2_Transform) X2_Transform.localPosition = new Vector2(200, -90);
    if (X2_Object) X2_Object.SetActive(true);
    if (X2_Transform) X2_Transform.DOLocalMove(new Vector2(400, 225), 1f);
    if (X2_Transform) X2_Transform.DOScale(Vector3.one, 1f);
    if (Box_Animation) Box_Animation.StartAnimation();
    yield return new WaitForSeconds(0.5f);
    if (DragonFire_Object) DragonFire_Object.SetActive(false);
    if (Fire_Object) Fire_Object.SetActive(false);
    if (DragonNormal_Object) DragonNormal_Object.SetActive(true);
    yield return new WaitForSeconds(1f);
  }

  private IEnumerator HitDealButton(bool isDouble)
  {
    if (!BJmanager.isSplit)
    {
      if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
      BJmanager.isFlippin = true;
      BJmanager.OnPlayerDealButton(PDummyPos_Transform[BJmanager.playerCounter].localPosition);
      yield return new WaitUntil(() => !BJmanager.isFlippin);
      if (isDouble)
      {
        OnStand();
      }
      else
      {
        if (ArrPointer_Object) ArrPointer_Object.SetActive(true);
        if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
      }
    }
    else
    {
      if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
      BJmanager.isFlippin = true;
      BJmanager.OnSplitDealButton();
      yield return new WaitUntil(() => !BJmanager.isFlippin);
      if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
    }
  }

  private IEnumerator DealerFinalButton()
  {
    if (!BJmanager.isSplit)
    {
      if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
      BJmanager.isFlippin = true;
      BJmanager.OnDealerOpenFlipped();
      yield return new WaitUntil(() => !BJmanager.isFlippin);
      for (int i = 0; i < BJmanager.dealerData.Count - 2; i++)
      {
        BJmanager.isFlippin = true;
        BJmanager.OnDealerButton(DDummyPos_Transform[BJmanager.dealerCounter].localPosition);
        yield return new WaitUntil(() => !BJmanager.isFlippin);
      }
      if (RebetButtons_object) RebetButtons_object.SetActive(true);
    }
    else if (BJmanager.isFirstSplit)
    {
      if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
      BJmanager.SplitStandButton();
      yield return new WaitUntil(() => !BJmanager.isFirstSplit);
      if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
    }
    else
    {
      if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
      BJmanager.isFlippin = true;
      BJmanager.OnDealerOpenFlipped();
      yield return new WaitUntil(() => !BJmanager.isFlippin);
      for (int i = 0; i < BJmanager.dealerData.Count - 2; i++)
      {
        BJmanager.isFlippin = true;
        BJmanager.OnDealerButton(DDummyPos_Transform[BJmanager.dealerCounter].localPosition);
        yield return new WaitUntil(() => !BJmanager.isFlippin);
      }
      if (RebetButtons_object) RebetButtons_object.SetActive(true);
    }
  }

  internal void ShowInitialButtons()
  {
    if (InitialButtons_object) InitialButtons_object.SetActive(true);
  }

  internal void ShowMaxBetPopup(string message)
  {
    MaxBetPopup_Text.text = message;
    if (maxBetSequence != null && maxBetSequence.IsActive())
    {
      maxBetSequence.Kill();
    }
    maxBetSequence = DOTween.Sequence();
    maxBetSequence.Append(MaxBetPopup_CG.DOFade(1, 0.3f))
    .AppendInterval(1.5f)
    .Append(MaxBetPopup_CG.DOFade(0, 0.3f));
  }

  internal void ReconnectionPopup()
  {
    OpenPopup(ReconnectPopup_Object);
  }

  internal void DisconnectionPopup()
  {
    OpenPopup(DisconnectPopup_Object);
  }

  internal void CheckAndClosePopup()
  {
    if (ReconnectPopup_Object.activeInHierarchy)
    {
      ClosePopup(ReconnectPopup_Object);
    }
    if (DisconnectPopup_Object.activeInHierarchy)
    {
      if (BlackBackground_Button) BlackBackground_Button.gameObject.SetActive(false);
      List<GameObject> popups = new() { QuitPopup_Object, InfoPopup_Object, SettingsPopup_Object, ReconnectPopup_Object, DisconnectPopup_Object };
      foreach (GameObject obj in popups)
      {
        if (obj) obj.SetActive(false);
      }
    }
  }

  private void OpenPopup(GameObject popup)
  {
    if (DisconnectPopup_Object.activeInHierarchy)
    {
      return;
    }
    if (popup == DisconnectPopup_Object && isExit)
    {
      return;
    }
    if (BlackBackground_Button) BlackBackground_Button.gameObject.SetActive(true);
    List<GameObject> popups = new() { QuitPopup_Object, InfoPopup_Object, SettingsPopup_Object, ReconnectPopup_Object, DisconnectPopup_Object };
    foreach (GameObject obj in popups)
    {
      if (obj != popup)
      {
        if (obj) obj.SetActive(false);
      }
    }
    if (popup) popup.SetActive(true);
  }

  private void ClosePopup(GameObject popup)
  {
    if (DisconnectPopup_Object.activeInHierarchy)
    {
      return;
    }

    if (BlackBackground_Button) BlackBackground_Button.gameObject.SetActive(false);
    List<GameObject> popups = new() { QuitPopup_Object, InfoPopup_Object, SettingsPopup_Object, ReconnectPopup_Object, DisconnectPopup_Object };
    foreach (GameObject obj in popups)
    {
      if (obj != popup)
      {
        if (obj) obj.SetActive(false);
      }
    }
  }

  void CallOnGameQuit()
  {
    isExit = true;
    socket.CloseGame();
  }
}
