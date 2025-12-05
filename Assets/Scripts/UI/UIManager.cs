using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
  #region Serialized Fields

  [Header("Managers")]
  [SerializeField] private BJController BJmanager;
  [SerializeField] private SocketIOManager socket;
  [SerializeField] private ActionPopup actionPopup;

  [SerializeField] private Transform ChipParent_Transform;

  [Header("Initial Buttons")]
  [SerializeField] private Button UndoBet_Button;
  [SerializeField] private Button Deal_Button;
  [SerializeField] private Button ClearBet_Button;
  [SerializeField] private Button StartDoubleBet_Button;
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
  [SerializeField] private Button MidDouble_Button;
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
  [SerializeField] private GameObject MultBetBttn_Object;
  [SerializeField] private GameObject PlayerCardTotal_Object;
  [SerializeField] private GameObject DealerCardTotal_Object;
  [SerializeField] private GameObject FirstSplitCardTotal_Object;
  [SerializeField] private GameObject SecondSplitCardTotal_Object;
  [SerializeField] private GameObject ChipSelectParent_Object;
  [SerializeField] private GameObject Split_object;
  [SerializeField] private GameObject MiddleDouble_object;
  [SerializeField] private GameObject MainBetButton_Object;
  [SerializeField] private GameObject ChipContainer_Object;
  [SerializeField] private GameObject MultiplierBetButton_Object;
  [SerializeField] private GameObject ArrPointer_Object;
  [SerializeField] private GameObject FirstArrPointer_Object;
  [SerializeField] private GameObject SecondArrPointer_Object;
  [SerializeField] private GameObject PlayerBlackjack_Object;
  [SerializeField] private GameObject DealerBlackjack_Object;
  [SerializeField] private GameObject YouWin_Object;
  [SerializeField] private GameObject Insurance_Object;
  [SerializeField] private GameObject QuitPopup_Object;
  [SerializeField] private GameObject InfoPopup_Object;
  [SerializeField] private GameObject SettingsPopup_Object;
  [SerializeField] private GameObject ReconnectPopup_Object;
  [SerializeField] private GameObject DisconnectPopup_Object;

  [SerializeField] private CanvasGroup Popup_CG;
  [SerializeField] private TMP_Text FirstHandBet_Text;
  [SerializeField] private TMP_Text SecondHandBet_Text;
  [SerializeField] private TMP_Text Popup_Text;
  [SerializeField] private TMP_Text YouWin_Text;
  [SerializeField] private TMP_Text XMultiplier_Text;
  [SerializeField] private TMP_Text InsuranceTotal_Text;
  [SerializeField] private Transform MultiplierHistoryParent;
  [SerializeField] private GameObject HistoryPrefab;

  [SerializeField] private Image PlayerTotalBG_Image;
  [SerializeField] private Image FirstSplitTotalBG_Image;
  [SerializeField] private Image SecondSplitTotalBG_Image;
  [SerializeField] private Sprite TotalBG_Orange;
  [SerializeField] private Sprite TotalBG_Green;


  [Header("Transforms")]
  [SerializeField] private Transform FirstHandChipContainer;
  [SerializeField] private Transform SecondHandChipContainer;
  [SerializeField] private Transform ScrollParent_Transform;
  [SerializeField] private Transform Selected_Transform;
  [SerializeField] private Transform multiplierChipsParent_Transform;

  [Header("Dragon Animation Routine")]
  [SerializeField] private GameObject DragonNormal_Object;
  [SerializeField] private GameObject DragonFire_Object;
  [SerializeField] private GameObject Fire_Object;
  [SerializeField] private ImageAnimation Box_Animation;
  [SerializeField] private Transform LeftBox_Transform;
  [SerializeField] private Sprite Empty_Sprite;

  [SerializeField] private ScrollRect ChipScroller;

  #endregion

  #region Private fields

  private Sequence PopupSeq;
  private bool isExit = false;
  private int chipCounter = 0;
  private Queue<GameObject> historyQueue = new Queue<GameObject>();

  #endregion

  #region Unity Lifecycle

  private void Start()
  {
    // Initial visual setup
    SafeSetActive(MultBetBttn_Object, true);
    SafeSetActive(ChipSelectParent_Object, true);
    SafeSetActive(PlayerCardTotal_Object, false);
    SafeSetActive(DealerCardTotal_Object, false);
    Popup_CG.alpha = 0;

    // Setup listeners (keeps logic exact but reduces duplication)
    SetupButtonListeners();

    // Initialise chips selection state
    if (Chips_Object != null && Chips_Object.Length > 0)
    {
      if (Chips_Object[0])
      {
        SetTransformParent(Chips_Object[0].transform, Selected_Transform);
        Chips_Object[0].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
      }
    }
    chipCounter = 0;

    // Initial arrow/pointers state
    if (RightArr_Button != null && LeftArr_Button != null)
    {
      LeftArr_Button.interactable = false;
    }
    SafeSetActive(ArrPointer_Object, false);
    SafeSetActive(FirstArrPointer_Object, false);
    SafeSetActive(SecondArrPointer_Object, false);
  }

  #endregion

  #region Setup Helpers

  private void SetupButtonListeners()
  {
    // Use helper AddListenerSafe to avoid repeating null checks
    AddListenerSafe(MainBet_Button, () => OnBet(true));
    AddListenerSafe(MultiplierBet_Button, () => OnBet(false));
    AddListenerSafe(Deal_Button, OnDeal);
    AddListenerSafe(Hit_Button, () => OnHit());
    AddListenerSafe(Stand_button, OnStand);
    AddListenerSafe(ClearBet_Button, OnClear);
    AddListenerSafe(UndoBet_Button, OnUndo);
    AddListenerSafe(Rebet_Button, OnRebet);
    AddListenerSafe(StartDoubleBet_Button, OnStartDouble);
    AddListenerSafe(MidDouble_Button, CheckDoublePopup);
    AddListenerSafe(RebetDeal_Button, OnRebetDeal);
    AddListenerSafe(RebetDouble_Button, OnRebetDouble);
    AddListenerSafe(Split_Button, ConfirmSplit);
    AddListenerSafe(Quit_Button, () => OpenPopup(QuitPopup_Object));
    AddListenerSafe(Info_Button, () => OpenPopup(InfoPopup_Object));
    AddListenerSafe(Settings_Button, () => OpenPopup(SettingsPopup_Object));
    AddListenerSafe(QuitYes_Button, CallOnGameQuit);
    AddListenerSafe(QuitNo_Button, () => ClosePopup(QuitPopup_Object));
    AddListenerSafe(InfoClose_Button, () => ClosePopup(InfoPopup_Object));
    AddListenerSafe(SettingsClose_Button, () => ClosePopup(SettingsPopup_Object));

    AddListenerSafe(LeftArr_Button, () => OnChipScroll(false));
    AddListenerSafe(RightArr_Button, () => OnChipScroll(true));

    // Chips array listeners
    if (Chips_Button != null)
    {
      for (int i = 0; i < Chips_Button.Length; i++)
      {
        int idx = i;
        AddListenerSafe(Chips_Button[i], () => OnChipButtonClick(idx));
      }
    }
  }

  private void AddListenerSafe(Button btn, UnityEngine.Events.UnityAction action)
  {
    if (btn == null) return;
    btn.onClick.RemoveAllListeners();
    btn.onClick.AddListener(action);
  }

  private void SafeSetActive(GameObject obj, bool active)
  {
    if (obj == null) return;
    obj.SetActive(active);
  }

  private void SetTransformParent(Transform child, Transform parent)
  {
    if (child == null || parent == null) return;
    child.SetParent(parent);
  }

  #endregion

  #region Chip Selection / Scrolling

  void OnChipButtonClick(int index)
  {
    // Return previously selected chip to scroll parent and reset its scale
    if (Chips_Object.Length > chipCounter && Chips_Object[chipCounter] != null)
    {
      SetTransformParent(Chips_Object[chipCounter].transform, ScrollParent_Transform);
      Chips_Object[chipCounter].transform.localScale = Vector3.one;
    }

    chipCounter = index;
    SetScrollToChip(chipCounter);

    if (Chips_Object.Length > chipCounter && Chips_Object[chipCounter] != null)
    {
      Chips_Object[chipCounter].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
      SetTransformParent(Chips_Object[chipCounter].transform, Selected_Transform);
    }

    // Arrow interactable updates
    if (chipCounter <= 0)
      if (LeftArr_Button) LeftArr_Button.interactable = false;

    if (chipCounter >= Chips_Object.Length - 1)
      if (RightArr_Button) RightArr_Button.interactable = false;

    if (chipCounter > 0)
      if (LeftArr_Button) LeftArr_Button.interactable = true;

    if (chipCounter < Chips_Object.Length - 1)
      if (RightArr_Button) RightArr_Button.interactable = true;

    BJmanager.SelectCoin(chipCounter);
  }

  private void OnChipScroll(bool direction)
  {
    // direction == true means scroll right (increment)
    if (Chips_Object == null || Chips_Object.Length == 0) return;

    // return prev
    if (Chips_Object.Length > chipCounter && Chips_Object[chipCounter] != null)
    {
      SetTransformParent(Chips_Object[chipCounter].transform, ScrollParent_Transform);
      Chips_Object[chipCounter].transform.localScale = Vector3.one;
    }

    chipCounter = Mathf.Clamp(direction ? chipCounter + 1 : chipCounter - 1, 0, Chips_Object.Length - 1);
    SetScrollToChip(chipCounter);

    if (Chips_Object[chipCounter] != null)
    {
      Chips_Object[chipCounter].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
      SetTransformParent(Chips_Object[chipCounter].transform, Selected_Transform);
    }

    // update arrows
    if (RightArr_Button) RightArr_Button.interactable = chipCounter < Chips_Object.Length - 1;
    if (LeftArr_Button) LeftArr_Button.interactable = chipCounter > 0;

    BJmanager.SelectCoin(chipCounter);
  }

  private void SetScrollToChip(int index)
  {
    if (ScrollParent_Transform == null || ChipScroller == null || Chips_Object == null || Chips_Object.Length == 0) return;

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

  #endregion

  #region Betting / Buttons

  void ConfirmSplit()
  {
    if (ActionPopup.ShouldSkip(PopupType.Split))
    {
      StartCoroutine(OnSplitDealCoroutine());
      return;
    }

    string msg = "SPLIT REQUIRES DOUBLING MAIN BET\nAND GUARANTEED MULTIPLIER SIDE BET";

    actionPopup.Show(
        msg,
        PopupType.Split,
        yesAction: () => StartCoroutine(OnSplitDealCoroutine()),
        noAction: () => Debug.Log("Split cancelled")
    );
  }

  private void OnBet(bool isMultiplier)
  {
    if (isMultiplier)
      BJmanager.BetOnButton();
    else
      BJmanager.MultiplierBetOnButton();
  }

  private void OnDeal()
  {
    StartCoroutine(OnDealCoroutine());
  }

  private void OnClear()
  {
    SafeSetActive(InitialButtons_object, false);
    BJmanager.ClearBet();
  }

  private void OnUndo()
  {
    BJmanager.UndoBetButton();
    if (BJmanager.betHistory.Count == 0 && InitialButtons_object != null)
    {
      InitialButtons_object.SetActive(false);
    }
  }

  private void OnStartDouble()
  {
    BJmanager.TryDoubleBet();
  }

  private void OnHit()
  {
    StartCoroutine(OnHitCoroutine());
  }

  private void OnStand()
  {
    StartCoroutine(OnStandCoroutine());
  }

  void CheckDoublePopup()
  {
    if (BJmanager.isSplit)
    {
      StartCoroutine(OnDoubleCoroutine());
      return;
    }

    if (ActionPopup.ShouldSkip(PopupType.Double))
    {
      StartCoroutine(OnDoubleCoroutine());
      return;
    }

    string msg = "DOUBLING DOWN REQUIRES DOUBLING\nMAIN AND SIDE BET";
    actionPopup.Show(
      msg,
      PopupType.Double,
      () => StartCoroutine(OnDoubleCoroutine()),
      () => Debug.Log("Refused Double")
    );
  }

  private void OnRebet()
  {
    StartCoroutine(OnRebetCoroutine());
  }

  private void OnRebetDeal()
  {
    StartCoroutine(OnRebetDealCoroutine());
  }

  private void OnRebetDouble()
  {
    StartCoroutine(OnRebetDoubleCoroutine());
  }

  #endregion

  #region Coroutines (Maintained original behavior & flow)

  private IEnumerator OnRebetCoroutine()
  {
    SafeSetActive(RebetButtons_object, false);
    ResetUI();
    yield return BJmanager.ClearCards();
    SafeSetActive(MainBetButton_Object, true);
    SafeSetActive(MultBetBttn_Object, true);
    SafeSetActive(ChipSelectParent_Object, true);
    SafeSetActive(InitialButtons_object, true);
  }

  private IEnumerator OnRebetDealCoroutine()
  {
    SafeSetActive(RebetButtons_object, false);
    ResetUI();
    SafeSetActive(MainBetButton_Object, false);
    yield return BJmanager.ClearCards();
    OnDeal();
  }

  private IEnumerator OnRebetDoubleCoroutine()
  {
    ResetUI();
    SafeSetActive(RebetButtons_object, false);
    SafeSetActive(MainBetButton_Object, false);
    yield return BJmanager.ClearCards();
    OnDeal();
  }

  private void ToggleSplitHandBet()
  {
    string bet = BJmanager.mainBet.ToString("N2");
    FirstHandBet_Text.text = bet;
    SecondHandBet_Text.text = bet;

    // instantiate chip containers for split hands and collect their children into BJmanager lists
    Instantiate(ChipContainer_Object, FirstHandChipContainer, false);
    foreach (Transform child in FirstHandChipContainer.GetChild(0).GetComponentsInChildren<Transform>())
    {
      BJmanager.firstHand_Coins.Add(child.gameObject);
    }

    Instantiate(ChipContainer_Object, SecondHandChipContainer, false);
    foreach (Transform child in SecondHandChipContainer.GetChild(0).GetComponentsInChildren<Transform>())
    {
      BJmanager.secondHand_Coins.Add(child.gameObject);
    }
  }

  private IEnumerator OnSplitDealCoroutine()
  {
    socket.RequestEvent("SPLIT");
    yield return new WaitUntil(() => socket.IsResultDone);

    if (!socket.ResultData.success)
    {
      ShowPopup("Insufficient balance to split!");
      SafeSetActive(Split_object, false);
      yield break;
    }

    double totalBet = BJmanager.mainBet + BJmanager.multiplierBet;
    BJmanager.TotalBet_Text.text = (totalBet * 2).ToString("N2");
    BJmanager.UpdateBetText(BJmanager.mainBet, BJmanager.multiplierBet * 2);
    BJmanager.UpdateBalance(socket.ResultData.player.balance);

    ChipParent_Transform.gameObject.SetActive(false);
    SafeSetActive(ArrPointer_Object, false);
    SafeSetActive(Split_object, false);
    SafeSetActive(MiddleButtons_object, false);
    if (MiddleDouble_object.activeSelf) SafeSetActive(MiddleDouble_object, false);
    SafeSetActive(PlayerCardTotal_Object, false);

    yield return BJmanager.SplitButton();
    ToggleSplitHandBet();
    SafeSetActive(FirstSplitCardTotal_Object, true);
    SafeSetActive(SecondSplitCardTotal_Object, true);

    // If result is not a direct game result, handle intermediate states; else handle immediate game result flow
    if (!socket.ResultData.id.ToLower().Contains("gameresult"))
    {
      if (socket.ResultData.payload.isAceSplit)
      {
        SafeSetActive(FirstArrPointer_Object, false);
        SafeSetActive(SecondArrPointer_Object, false);
        BJmanager.isFirstSplit = false;
        OnStand();
      }
      else if (socket.ResultData.payload.currentHandIndex == 0)
      {
        BJmanager.isFirstSplit = true;
        SafeSetActive(MiddleButtons_object, true);
        SafeSetActive(FirstArrPointer_Object, true);
        SafeSetActive(SecondArrPointer_Object, false);
        if (BJmanager.FirstSplitplayerCounter == 2)
        {
          SafeSetActive(MiddleDouble_object, true);
        }
      }
      else
      {
        BJmanager.isFirstSplit = false;
        SafeSetActive(MiddleButtons_object, true);
        SafeSetActive(FirstArrPointer_Object, false);
        SafeSetActive(SecondArrPointer_Object, true);
        if (BJmanager.SecondSplitplayerCounter == 2)
        {
          SafeSetActive(MiddleDouble_object, true);
        }
      }
    }
    else
    {
      SafeSetActive(FirstArrPointer_Object, false);
      SafeSetActive(SecondArrPointer_Object, false);
      // The behavior for complete game result after split (kept intact)
      BJmanager.isFlippin = true;
      BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);

      yield return new WaitUntil(() => !BJmanager.isFlippin);

      if (socket.ResultData.payload.dealerHand.cards.Count > 2)
      {
        for (int i = 0; i < socket.ResultData.payload.dealerHand.cards.Count - 2; i++)
        {
          BJmanager.isFlippin = true;
          BJmanager.OnDealerButton(socket.ResultData.payload.dealerHand.cards[i + 2]);
          yield return new WaitUntil(() => !BJmanager.isFlippin);
        }
      }

      BJmanager.UpdateBalance(socket.ResultData.player.balance);
      BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);

      HandResult FirstHandResult = socket.ResultData.payload.handResults[0];
      string FirstResult = FirstHandResult.result.ToLower();
      BJmanager.FirstSplitTotal_Text.text = FirstHandResult.handValue.ToString();
      BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);

      if (FirstResult.Contains("win"))
      {
        YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
        SafeSetActive(YouWin_Object, true);
        BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
        FirstHandBet_Text.text = FirstHandResult.payout.ToString("N2");
        SetFirstSplitBGWin();
      }
      else if (FirstResult.Contains("lose") || FirstResult.Contains("bust"))
      {
        BJmanager.LostFirstHandChips();
      }

      if (FirstResult.Contains("push"))
        BJmanager.FirstSplitTotal_Text.text = "PUSH " + BJmanager.FirstSplitTotal_Text.text;

      HandResult SecondHandResult = socket.ResultData.payload.handResults[1];
      string SecondResult = SecondHandResult.result.ToLower();
      BJmanager.SecondSplitTotal_Text.text = SecondHandResult.handValue.ToString();
      if (SecondResult.Contains("win"))
      {
        YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
        SafeSetActive(YouWin_Object, true);
        BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
        SecondHandBet_Text.text = SecondHandResult.payout.ToString("N2");
        SetSecondSplitBGWin();
      }
      else if (SecondResult.Contains("lose") || SecondResult.Contains("bust"))
      {
        BJmanager.LostSecondHandChips();
      }

      if (SecondResult.Contains("push"))
        BJmanager.SecondSplitTotal_Text.text = "PUSH " + BJmanager.SecondSplitTotal_Text.text;

      if (FirstResult.Contains("bust"))
        BJmanager.FirstSplitTotal_Text.text = "BUST " + FirstHandResult.handValue;

      if (SecondResult.Contains("bust"))
        BJmanager.SecondSplitTotal_Text.text = "BUST " + SecondHandResult.handValue;

      if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
        InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

      if (socket.ResultData.payload.dealerHand.isBlackjack)
        SafeSetActive(DealerBlackjack_Object, true);

      SafeSetActive(RebetButtons_object, true);
    }
  }

  private IEnumerator OnDealCoroutine()
  {
    SafeSetActive(InitialButtons_object, false);
    if (MainBet_Button) MainBet_Button.gameObject.SetActive(false);
    if (MultiplierBet_Button) MultiplierBet_Button.gameObject.SetActive(false);
    SafeSetActive(ChipSelectParent_Object, false);

    // Move chip parent down (same logic)
    ChipParent_Transform.localPosition = new(ChipParent_Transform.localPosition.x, ChipParent_Transform.localPosition.y - 132, ChipParent_Transform.localPosition.z);

    socket.RequestEvent("DEAL");
    yield return new WaitUntil(() => socket.IsResultDone);

    BJmanager.isFlippin = true;
    BJmanager.OnPlayerDealButton(socket.ResultData.payload.playerHands[0].cards[BJmanager.playerCounter]);
    yield return new WaitUntil(() => !BJmanager.isFlippin);

    Card dealerCard = socket.ResultData.id.ToLower().Contains("gameresult") ? socket.ResultData.payload.dealerHand.cards[0] : socket.ResultData.payload.dealerUpCard;
    BJmanager.isFlippin = true;
    BJmanager.OnDealerButton(dealerCard);
    yield return new WaitUntil(() => !BJmanager.isFlippin);

    BJmanager.isFlippin = true;
    BJmanager.OnPlayerDealButton(socket.ResultData.payload.playerHands[0].cards[BJmanager.playerCounter]);
    yield return new WaitUntil(() => !BJmanager.isFlippin);

    BJmanager.isFlippin = true;
    BJmanager.OnDealerButtonClosedCard();
    yield return new WaitUntil(() => !BJmanager.isFlippin);

    SafeSetActive(PlayerCardTotal_Object, true);
    SafeSetActive(DealerCardTotal_Object, true);
    BJmanager.UpdateBalance(socket.ResultData.player.balance);

    if (BJmanager.CheckMultiplier())
      yield return DragonRoutine();

    string gameState = socket.ResultData.payload.gamePhase.ToLower();

    if (gameState.Contains("player-turn"))
    {
      if (socket.ResultData.payload.playerHands[0].isBlackjack)
      {
        SafeSetActive(PlayerBlackjack_Object, true);
        if (socket.ResultData.payload.dealerUpCard.rank == "A")
        {
          OnStand();
        }
      }
      else
      {
        SafeSetActive(ArrPointer_Object, true);
        SafeSetActive(Split_object, socket.ResultData.payload.canSplit);
        SafeSetActive(MidDouble_Button.gameObject, socket.ResultData.payload.canDouble);
        CheckInsurance(socket.ResultData.payload.canInsure);
        SafeSetActive(MiddleButtons_object, true);
      }
    }
    else if (gameState.Contains("completed"))
    {
      BJmanager.SetPlayerValue(socket.ResultData.payload.handResults[0].handValue);
      BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);
      double win = socket.ResultData.payload.totalWin;
      if (win > 0)
      {
        if (socket.ResultData.payload.handResults[0].result.ToLower().Contains("blackjack"))
          SafeSetActive(PlayerBlackjack_Object, true);

        YouWin_Text.text = win.ToString("N2");
        SafeSetActive(YouWin_Object, true);
        BJmanager.UpdateWinnings(win);
        BJmanager.UpdateBetText(socket.ResultData.payload.totalWin - socket.ResultData.payload.sideBetWin, socket.ResultData.payload.sideBetWin);
        SetPlayerTotalBGWin();
      }
      else
      {
        BJmanager.LostChipsAnimation();
      }
      if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
        InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

      if (socket.ResultData.payload.dealerHand.isBlackjack)
        SafeSetActive(DealerBlackjack_Object, true);

      SafeSetActive(RebetButtons_object, true);
    }
  }

  private IEnumerator OnHitCoroutine()
  {
    SafeSetActive(MiddleButtons_object, false);
    if (Split_object && Split_object.activeSelf) SafeSetActive(Split_object, false);
    if (MiddleDouble_object && MiddleDouble_object.activeSelf) SafeSetActive(MiddleDouble_object, false);
    SafeSetActive(ArrPointer_Object, false);

    socket.RequestEvent("HIT");
    yield return new WaitUntil(() => socket.IsResultDone);

    if (!BJmanager.isSplit)
    {
      string result = socket.ResultData.id.ToLower();

      BJmanager.isFlippin = true;
      BJmanager.OnPlayerDealButton(!result.Contains("gameresult") ? socket.ResultData.payload.card : socket.ResultData.payload.playerHands[0].cards[BJmanager.playerCounter]);
      yield return new WaitUntil(() => !BJmanager.isFlippin);

      if (result.Contains("gameresult") && socket.ResultData.payload.handResults[0].result.ToLower().Contains("bust"))
      {
        // Dealer flip flow
        BJmanager.isFlippin = true;
        BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);
        yield return new WaitUntil(() => !BJmanager.isFlippin);

        if (socket.ResultData.payload.dealerHand.cards.Count > 2)
        {
          for (int i = 0; i < socket.ResultData.payload.dealerHand.cards.Count - 2; i++)
          {
            BJmanager.isFlippin = true;
            BJmanager.OnDealerButton(socket.ResultData.payload.dealerHand.cards[i + 2]);
            yield return new WaitUntil(() => !BJmanager.isFlippin);
          }
        }

        BJmanager.UpdateBalance(socket.ResultData.player.balance);
        BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);

        BJmanager.SetPlayerValue(socket.ResultData.payload.handResults[0].handValue);
        BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);

        if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
          InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

        if (socket.ResultData.payload.dealerHand.isBlackjack)
          SafeSetActive(DealerBlackjack_Object, true);

        BJmanager.LostChipsAnimation();

        SafeSetActive(RebetButtons_object, true);
        yield break;
      }

      if (socket.ResultData.payload.handValue == 21)
      {
        OnStand();
        yield break;
      }
      SafeSetActive(ArrPointer_Object, true);
      SafeSetActive(MiddleButtons_object, true);
    }
    else
    {
      // split-hand branch: preserved as-is
      Card PlayerCard = new();
      bool isGameCompleted = false;
      if (socket.ResultData.payload.gamePhase.ToLower().Contains("completed"))
      {
        isGameCompleted = true;
        PlayerCard = socket.ResultData.payload.playerHands[1].cards[BJmanager.SecondSplitplayerCounter];
      }
      else
      {
        PlayerCard = socket.ResultData.payload.card;
        // if (socket.ResultData.payload.isBust && !BJmanager.isFirstSplit)
        // {
        //   isGameCompleted = true;
        // }
      }

      BJmanager.isFlippin = true;
      BJmanager.OnSplitDealButton(PlayerCard);
      yield return new WaitUntil(() => !BJmanager.isFlippin);

      if (isGameCompleted)
      {
        if (socket.ResultData.id.ToLower().Contains("gameresult"))
        {
          SafeSetActive(FirstArrPointer_Object, false);
          SafeSetActive(SecondArrPointer_Object, false);
          BJmanager.isFlippin = true;
          BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);
          yield return new WaitUntil(() => !BJmanager.isFlippin);

          if (socket.ResultData.payload.dealerHand.cards.Count > 2)
          {
            for (int i = 0; i < socket.ResultData.payload.dealerHand.cards.Count - 2; i++)
            {
              BJmanager.isFlippin = true;
              BJmanager.OnDealerButton(socket.ResultData.payload.dealerHand.cards[i + 2]);
              yield return new WaitUntil(() => !BJmanager.isFlippin);
            }
          }

          BJmanager.UpdateBalance(socket.ResultData.player.balance);
          BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);

          // First hand result handling (kept exact)
          HandResult FirstHandResult = socket.ResultData.payload.handResults[0];
          string FirstResult = FirstHandResult.result.ToLower();
          BJmanager.FirstSplitTotal_Text.text = FirstHandResult.handValue.ToString();
          BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);
          if (FirstResult.Contains("win"))
          {
            YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
            SafeSetActive(YouWin_Object, true);
            BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
            FirstHandBet_Text.text = FirstHandResult.payout.ToString("N2");
            SetFirstSplitBGWin();
          }
          else if (FirstResult.Contains("lose") && FirstResult.Contains("bust"))
            BJmanager.LostFirstHandChips();

          if (FirstResult.Contains("push"))
            BJmanager.FirstSplitTotal_Text.text = "PUSH " + BJmanager.FirstSplitTotal_Text.text;

          // Second hand result handling
          HandResult SecondHandResult = socket.ResultData.payload.handResults[1];
          string SecondResult = SecondHandResult.result.ToLower();
          BJmanager.SecondSplitTotal_Text.text = SecondHandResult.handValue.ToString();
          if (SecondResult.Contains("win"))
          {
            YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
            SafeSetActive(YouWin_Object, true);
            BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
            SecondHandBet_Text.text = SecondHandResult.payout.ToString("N2");
            SetSecondSplitBGWin();
          }
          else if (SecondResult.Contains("lose") || SecondResult.Contains("bust"))
            BJmanager.LostSecondHandChips();

          if (SecondResult.Contains("push"))
            BJmanager.SecondSplitTotal_Text.text = "PUSH " + BJmanager.SecondSplitTotal_Text.text;

          if (FirstResult.Contains("bust"))
            BJmanager.FirstSplitTotal_Text.text = "BUST " + FirstHandResult.handValue;

          if (SecondResult.Contains("bust"))
            BJmanager.SecondSplitTotal_Text.text = "BUST " + SecondHandResult.handValue;
          if (socket.ResultData.payload.dealerHand.isBlackjack) SafeSetActive(DealerBlackjack_Object, true);

          if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
            InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

          SafeSetActive(RebetButtons_object, true);
        }
      }
      else
      {
        if (BJmanager.isFirstSplit)
        {
          if (socket.ResultData.payload.isBust)
          {
            if (BJmanager.SecondSplitplayerCounter == 2)
            {
              SafeSetActive(MiddleDouble_object, true);
            }
            BJmanager.isFirstSplit = false;
            SafeSetActive(FirstArrPointer_Object, false);
            SafeSetActive(SecondArrPointer_Object, true);
          }
          if (socket.ResultData.payload.handValue == 21)
          {
            OnStand();
          }
          else
          {
            SafeSetActive(MiddleButtons_object, true);
          }
        }
        else
        {
          if (socket.ResultData.payload.handValue == 21)
          {
            SafeSetActive(FirstArrPointer_Object, false);
            SafeSetActive(SecondArrPointer_Object, false);
            OnStand();
          }
          else
          {
            SafeSetActive(MiddleButtons_object, true);
          }
        }
      }
    }
  }

  private IEnumerator OnStandCoroutine()
  {
    if (ArrPointer_Object && ArrPointer_Object.activeInHierarchy) SafeSetActive(ArrPointer_Object, false);
    SafeSetActive(MiddleButtons_object, false);

    socket.RequestEvent("STAND");
    yield return new WaitUntil(() => socket.IsResultDone);

    if (!BJmanager.isSplit)
    {
      // Dealer flips and plays out (kept exact)
      BJmanager.isFlippin = true;
      BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);
      yield return new WaitUntil(() => !BJmanager.isFlippin);

      if (socket.ResultData.payload.dealerHand.cards.Count > 2)
      {
        for (int i = 0; i < socket.ResultData.payload.dealerHand.cards.Count - 2; i++)
        {
          BJmanager.isFlippin = true;
          BJmanager.OnDealerButton(socket.ResultData.payload.dealerHand.cards[i + 2]);
          yield return new WaitUntil(() => !BJmanager.isFlippin);
        }
      }

      BJmanager.SetPlayerValue(socket.ResultData.payload.handResults[0].handValue);
      BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);
      BJmanager.UpdateBalance(socket.ResultData.player.balance);
      string result = socket.ResultData.payload.handResults[0].result.ToLower();
      BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);

      if (result.Contains("push"))
      {
        BJmanager.PlayerPush();
      }
      else if (result.Contains("win"))
      {
        YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
        SafeSetActive(YouWin_Object, true);
        BJmanager.UpdateBetText(socket.ResultData.payload.totalWin - socket.ResultData.payload.sideBetWin, socket.ResultData.payload.sideBetWin);
        SetPlayerTotalBGWin();
      }
      else if (result.Contains("lose") || result.Contains("bust"))
      {
        BJmanager.LostChipsAnimation();
      }

      if (socket.ResultData.payload.dealerHand.isBlackjack)
        SafeSetActive(DealerBlackjack_Object, true);
      if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
        InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

      SafeSetActive(RebetButtons_object, true);
    }
    else if (BJmanager.isFirstSplit)
    {
      BJmanager.isFirstSplit = false;
      SafeSetActive(FirstArrPointer_Object, false);
      if (!socket.ResultData.id.ToLower().Contains("gameresult"))
      {
        SafeSetActive(MiddleButtons_object, true);
        SafeSetActive(SecondArrPointer_Object, true);
      }
      else
      {
        BJmanager.isSplit = false;
        SafeSetActive(FirstArrPointer_Object, false);
        SafeSetActive(SecondArrPointer_Object, false);

        BJmanager.isFlippin = true;
        BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);
        yield return new WaitUntil(() => !BJmanager.isFlippin);

        if (socket.ResultData.payload.dealerHand.cards.Count > 2)
        {
          for (int i = 0; i < socket.ResultData.payload.dealerHand.cards.Count - 2; i++)
          {
            BJmanager.isFlippin = true;
            BJmanager.OnDealerButton(socket.ResultData.payload.dealerHand.cards[i + 2]);
            yield return new WaitUntil(() => !BJmanager.isFlippin);
          }
        }

        BJmanager.UpdateBalance(socket.ResultData.player.balance);
        BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);

        HandResult FirstHandResult = socket.ResultData.payload.handResults[0];
        string FirstResult = FirstHandResult.result.ToLower();
        BJmanager.FirstSplitTotal_Text.text = FirstHandResult.handValue.ToString();
        BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);

        if (FirstResult.Contains("win"))
        {
          YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
          SafeSetActive(YouWin_Object, true);
          BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
          FirstHandBet_Text.text = FirstHandResult.payout.ToString("N2");
          SetFirstSplitBGWin();
        }
        else if (FirstResult.Contains("lose") || FirstResult.Contains("bust"))
          BJmanager.LostFirstHandChips();

        if (FirstResult.Contains("push"))
          BJmanager.FirstSplitTotal_Text.text = "PUSH " + BJmanager.FirstSplitTotal_Text.text;

        HandResult SecondHandResult = socket.ResultData.payload.handResults[1];
        string SecondResult = SecondHandResult.result.ToLower();
        BJmanager.SecondSplitTotal_Text.text = SecondHandResult.handValue.ToString();

        if (SecondResult.Contains("win"))
        {
          YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
          SafeSetActive(YouWin_Object, true);
          BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
          SecondHandBet_Text.text = SecondHandResult.payout.ToString("N2");
          SetSecondSplitBGWin();
        }
        else if (SecondResult.Contains("lose") || SecondResult.Contains("bust"))
          BJmanager.LostSecondHandChips();

        if (SecondResult.Contains("push"))
          BJmanager.SecondSplitTotal_Text.text = "PUSH " + BJmanager.SecondSplitTotal_Text.text;
        if (FirstResult.Contains("bust"))
          BJmanager.FirstSplitTotal_Text.text = "BUST " + FirstHandResult.handValue;

        if (SecondResult.Contains("bust"))
          BJmanager.SecondSplitTotal_Text.text = "BUST " + SecondHandResult.handValue;
        if (socket.ResultData.payload.dealerHand.isBlackjack)
          SafeSetActive(DealerBlackjack_Object, true);

        if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
          InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

        SafeSetActive(RebetButtons_object, true);
      }
    }
    else
    {
      SafeSetActive(SecondArrPointer_Object, false);
      BJmanager.isSplit = false;
      BJmanager.isFlippin = true;
      BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);
      yield return new WaitUntil(() => !BJmanager.isFlippin);

      if (socket.ResultData.payload.dealerHand.cards.Count > 2)
      {
        for (int i = 0; i < socket.ResultData.payload.dealerHand.cards.Count - 2; i++)
        {
          BJmanager.isFlippin = true;
          BJmanager.OnDealerButton(socket.ResultData.payload.dealerHand.cards[i + 2]);
          yield return new WaitUntil(() => !BJmanager.isFlippin);
        }
      }

      BJmanager.UpdateBalance(socket.ResultData.player.balance);
      BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);

      HandResult FirstHandResult = socket.ResultData.payload.handResults[0];
      string FirstResult = FirstHandResult.result.ToLower();
      BJmanager.FirstSplitTotal_Text.text = FirstHandResult.handValue.ToString();
      BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);

      if (FirstResult.Contains("win"))
      {
        YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
        SafeSetActive(YouWin_Object, true);
        BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
        FirstHandBet_Text.text = FirstHandResult.payout.ToString("N2");
        SetFirstSplitBGWin();
      }
      else if (FirstResult.Contains("lose") || FirstResult.Contains("bust"))
        BJmanager.LostFirstHandChips();

      if (FirstResult.Contains("push"))
        BJmanager.FirstSplitTotal_Text.text = "PUSH " + BJmanager.FirstSplitTotal_Text.text;

      HandResult SecondHandResult = socket.ResultData.payload.handResults[1];
      string SecondResult = SecondHandResult.result.ToLower();
      BJmanager.SecondSplitTotal_Text.text = SecondHandResult.handValue.ToString();

      if (SecondResult.Contains("win"))
      {
        YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
        SafeSetActive(YouWin_Object, true);
        BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
        SecondHandBet_Text.text = SecondHandResult.payout.ToString("N2");
        SetSecondSplitBGWin();
      }
      else if (SecondResult.Contains("lose") || FirstResult.Contains("bust"))
        BJmanager.LostSecondHandChips();

      if (SecondResult.Contains("push"))
        BJmanager.SecondSplitTotal_Text.text = "PUSH " + BJmanager.SecondSplitTotal_Text.text;

      if (FirstResult.Contains("bust"))
        BJmanager.FirstSplitTotal_Text.text = "BUST " + FirstHandResult.handValue;

      if (SecondResult.Contains("bust"))
        BJmanager.SecondSplitTotal_Text.text = "BUST " + SecondHandResult.handValue;

      if (socket.ResultData.payload.dealerHand.isBlackjack)
        SafeSetActive(DealerBlackjack_Object, true);

      if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
        InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

      SafeSetActive(RebetButtons_object, true);
    }
  }

  IEnumerator OnInsuranceCoroutine()
  {
    socket.RequestEvent("INSURANCE");
    yield return new WaitUntil(() => socket.IsResultDone);

    if (!socket.ResultData.success)
    {
      ShowPopup("Insufficient balance for insurance!");
      yield break;
    }

    if (socket.ResultData.payload.insuranceBet > 0)
    {
      InsuranceTotal_Text.text = socket.ResultData.payload.insuranceBet.ToString("N2");
      Insurance_Object.SetActive(true);
      BJmanager.UpdateBalance(socket.PlayerData.balance);
      BJmanager.TotalBet_Text.text = (BJmanager.mainBet + BJmanager.multiplierBet + socket.ResultData.payload.insuranceBet).ToString("N2");
    }
  }

  IEnumerator OnDoubleCoroutine()
  {
    socket.RequestEvent("DOUBLE");
    yield return new WaitUntil(() => socket.IsResultDone);

    if (!socket.ResultData.success)
    {
      ShowPopup("Insufficient balance for double!");
      yield break;
    }

    string resultId = socket.ResultData.id.ToLower();
    if (resultId.Contains("gameresult"))
    {
      SafeSetActive(MiddleButtons_object, false);
      if (!BJmanager.isSplit)
      {
        SafeSetActive(ArrPointer_Object, false);
        double totalBet = socket.ResultData.payload.playerHands[0].bet + socket.ResultData.payload.sideBet;
        BJmanager.TotalBet_Text.text = totalBet.ToString("N2");
        BJmanager.UpdateBetText(socket.ResultData.payload.playerHands[0].bet, socket.ResultData.payload.sideBet);

        BJmanager.isFlippin = true;
        BJmanager.OnPlayerDealButton(!resultId.Contains("gameresult") ? socket.ResultData.payload.card : socket.ResultData.payload.playerHands[0].cards[BJmanager.playerCounter]);
        yield return new WaitUntil(() => !BJmanager.isFlippin);

        yield return new WaitForSecondsRealtime(0.5f);

        BJmanager.isFlippin = true;
        BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);
        yield return new WaitUntil(() => !BJmanager.isFlippin);

        if (socket.ResultData.payload.dealerHand.cards.Count > 2)
        {
          for (int i = 0; i < socket.ResultData.payload.dealerHand.cards.Count - 2; i++)
          {
            BJmanager.isFlippin = true;
            BJmanager.OnDealerButton(socket.ResultData.payload.dealerHand.cards[i + 2]);
            yield return new WaitUntil(() => !BJmanager.isFlippin);
          }
        }

        BJmanager.UpdateBalance(socket.ResultData.player.balance);
        BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);
        BJmanager.SetPlayerValue(socket.ResultData.payload.handResults[0].handValue);
        BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);

        if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
          InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

        string gameResult = socket.ResultData.payload.handResults[0].result.ToLower();
        if (gameResult.Contains("lose") || gameResult.Contains("bust"))
        {
          BJmanager.LostChipsAnimation();
        }

        if (gameResult.Contains("push"))
        {
          BJmanager.PlayerPush();
        }

        if (gameResult.Contains("win"))
        {
          SetPlayerTotalBGWin();
          BJmanager.UpdateBetText(socket.ResultData.payload.handResults[0].payout, socket.ResultData.payload.sideBetWin);
        }

        if (socket.ResultData.payload.dealerHand.isBlackjack)
          SafeSetActive(DealerBlackjack_Object, true);

        SafeSetActive(RebetButtons_object, true);
      }
      else
      {
        double totalBet = socket.ResultData.payload.playerHands[0].bet + socket.ResultData.payload.playerHands[1].bet + socket.ResultData.payload.sideBet;
        BJmanager.TotalBet_Text.text = totalBet.ToString("N2");
        SecondHandBet_Text.text = socket.ResultData.payload.playerHands[1].bet.ToString("N2");
        SafeSetActive(FirstArrPointer_Object, false);
        SafeSetActive(SecondArrPointer_Object, false);

        Card PlayerCard = socket.ResultData.payload.playerHands[1].cards[^1];
        BJmanager.isFlippin = true;
        BJmanager.OnSplitDealButton(PlayerCard);
        yield return new WaitUntil(() => !BJmanager.isFlippin);
        yield return new WaitForSecondsRealtime(0.5f);

        BJmanager.isSplit = false;
        BJmanager.isFirstSplit = false;

        BJmanager.isFlippin = true;
        BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);
        yield return new WaitUntil(() => !BJmanager.isFlippin);

        if (socket.ResultData.payload.dealerHand.cards.Count > 2)
        {
          for (int i = 0; i < socket.ResultData.payload.dealerHand.cards.Count - 2; i++)
          {
            BJmanager.isFlippin = true;
            BJmanager.OnDealerButton(socket.ResultData.payload.dealerHand.cards[i + 2]);
            yield return new WaitUntil(() => !BJmanager.isFlippin);
          }
        }

        BJmanager.UpdateBalance(socket.ResultData.player.balance);
        BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);

        // First hand result handling (kept exact)
        HandResult FirstHandResult = socket.ResultData.payload.handResults[0];
        string FirstResult = FirstHandResult.result.ToLower();
        BJmanager.FirstSplitTotal_Text.text = FirstHandResult.handValue.ToString();
        BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);
        if (FirstResult.Contains("win"))
        {
          YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
          SafeSetActive(YouWin_Object, true);
          BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
          FirstHandBet_Text.text = FirstHandResult.payout.ToString("N2");
          SetFirstSplitBGWin();
        }
        else if (FirstResult.Contains("lose") || FirstResult.Contains("bust"))
          BJmanager.LostFirstHandChips();

        if (FirstResult.Contains("push"))
          BJmanager.FirstSplitTotal_Text.text = "PUSH " + BJmanager.FirstSplitTotal_Text.text;

        // Second hand result handling
        HandResult SecondHandResult = socket.ResultData.payload.handResults[1];
        string SecondResult = SecondHandResult.result.ToLower();
        BJmanager.SecondSplitTotal_Text.text = SecondHandResult.handValue.ToString();
        if (SecondResult.Contains("win"))
        {
          YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
          SafeSetActive(YouWin_Object, true);
          BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
          SecondHandBet_Text.text = SecondHandResult.payout.ToString("N2");
          SetSecondSplitBGWin();
        }
        else if (SecondResult.Contains("lose") || SecondResult.Contains("bust"))
          BJmanager.LostSecondHandChips();

        if (SecondResult.Contains("push"))
          BJmanager.SecondSplitTotal_Text.text = "PUSH " + BJmanager.SecondSplitTotal_Text.text;

        if (FirstResult.Contains("bust"))
          BJmanager.FirstSplitTotal_Text.text = "BUST " + FirstHandResult.handValue;

        if (SecondResult.Contains("bust"))
          BJmanager.SecondSplitTotal_Text.text = "BUST " + SecondHandResult.handValue;
        if (socket.ResultData.payload.dealerHand.isBlackjack) SafeSetActive(DealerBlackjack_Object, true);

        if (Insurance_Object.activeSelf && socket.ResultData.payload.insuranceWin > 0)
          InsuranceTotal_Text.text = socket.ResultData.payload.insuranceWin.ToString("N2");

        SafeSetActive(RebetButtons_object, true);
      }
    }
    else
    {
      if (BJmanager.isSplit)
      {
        double totalBet = socket.ResultData.payload.handBet + BJmanager.mainBet + BJmanager.multiplierBet;
        BJmanager.TotalBet_Text.text = totalBet.ToString("N2");
        FirstHandBet_Text.text = socket.ResultData.payload.handBet.ToString("N2");

        Card PlayerCard = socket.ResultData.payload.card;
        BJmanager.isFlippin = true;
        BJmanager.OnSplitDealButton(PlayerCard);
        yield return new WaitUntil(() => !BJmanager.isFlippin);

        if (BJmanager.isFirstSplit)
        {
          BJmanager.isFirstSplit = false;
        }
        SafeSetActive(FirstArrPointer_Object, false);
        SafeSetActive(SecondArrPointer_Object, true);
        SafeSetActive(MiddleButtons_object, true);
      }
    }
  }

  #endregion

  #region Dragon Routine & Multiplier History (kept logic)

  private IEnumerator DragonRoutine()
  {
    XMultiplier_Text.gameObject.SetActive(false);
    SafeSetActive(DragonNormal_Object, false);
    SafeSetActive(DragonFire_Object, true);

    ImageAnimation dragonAnimation = DragonFire_Object.GetComponent<ImageAnimation>();
    yield return FireAnimationRoutine(dragonAnimation);

    int m = socket.ResultData.payload.sideBetMultiplier;
    XMultiplier_Text.text = "x" + m;

    XMultiplier_Text.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    XMultiplier_Text.gameObject.SetActive(true);

    XMultiplier_Text.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)
    .OnComplete(() =>
    {
      AddMultiplierHistory(m);
    });

    if (Box_Animation) Box_Animation.StartAnimation();

    yield return new WaitUntil(() => dragonAnimation.rendererDelegate.sprite == dragonAnimation.textureArray[^1]);

    SafeSetActive(DragonFire_Object, false);
    SafeSetActive(DragonNormal_Object, true);
    yield return new WaitForSeconds(1f);
  }

  internal void AddMultiplierHistory(int multiplier)
  {
    GameObject newEntry = Instantiate(HistoryPrefab, MultiplierHistoryParent);
    newEntry.transform.SetAsFirstSibling();
    newEntry.GetComponentInChildren<TMP_Text>().text = "x" + multiplier;

    historyQueue.Enqueue(newEntry);
    while (historyQueue.Count > 3)
    {
      GameObject old = historyQueue.Dequeue();
      Destroy(old);
    }
  }

  IEnumerator FireAnimationRoutine(ImageAnimation DragonAnimation)
  {
    yield return new WaitUntil(() => DragonAnimation.rendererDelegate.sprite == DragonAnimation.textureArray[17]);
    ImageAnimation fireAnimation = Fire_Object.GetComponent<ImageAnimation>();
    Fire_Object.SetActive(true);
    yield return new WaitUntil(() => fireAnimation.rendererDelegate.sprite == fireAnimation.textureArray[^1]);
    Fire_Object.SetActive(false);
  }

  #endregion

  #region Popups / UI Reset / Helpers

  private void CheckInsurance(bool triggered)
  {
    if (!triggered)
      return;

    string msg = "INSURANCE?";
    actionPopup.Show(
      msg,
      PopupType.Insurance,
      () => StartCoroutine(OnInsuranceCoroutine()),
      () => { Debug.Log("Refused insaurance"); }
    );
  }

  internal void ShowInitialButtons()
  {
    SafeSetActive(InitialButtons_object, true);
  }

  internal void ShowPopup(string message)
  {
    Popup_Text.text = message;
    if (PopupSeq != null && PopupSeq.IsActive())
    {
      PopupSeq.Kill();
    }
    PopupSeq = DOTween.Sequence();
    PopupSeq.Append(Popup_CG.DOFade(1, 0.3f))
            .AppendInterval(1.5f)
            .Append(Popup_CG.DOFade(0, 0.3f));
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
    if (ReconnectPopup_Object != null && ReconnectPopup_Object.activeInHierarchy)
      ClosePopup(ReconnectPopup_Object);

    if (DisconnectPopup_Object != null && DisconnectPopup_Object.activeInHierarchy)
    {
      if (BlackBackground_Button) BlackBackground_Button.gameObject.SetActive(false);

      List<GameObject> popups = new() { QuitPopup_Object, InfoPopup_Object, SettingsPopup_Object, ReconnectPopup_Object, DisconnectPopup_Object };
      foreach (GameObject obj in popups)
      {
        if (obj) obj.SetActive(false);
      }
    }
  }

  private void ResetUI()
  {
    SafeSetActive(DealerCardTotal_Object, false);
    SafeSetActive(Insurance_Object, false);
    // Destroy chip containers and reset pointers & UI
    if (FirstHandChipContainer.childCount > 0)
      Destroy(FirstHandChipContainer.GetChild(0).gameObject);

    if (SecondHandChipContainer.childCount > 0)
      Destroy(SecondHandChipContainer.GetChild(0).gameObject);

    SafeSetActive(ArrPointer_Object, false);
    SafeSetActive(FirstArrPointer_Object, false);
    SafeSetActive(SecondArrPointer_Object, false);
    SafeSetActive(FirstSplitCardTotal_Object, false);
    SafeSetActive(SecondSplitCardTotal_Object, false);
    SafeSetActive(PlayerCardTotal_Object, false);
    SafeSetActive(DealerBlackjack_Object, false);
    SafeSetActive(PlayerBlackjack_Object, false);
    if (YouWin_Object != null && YouWin_Object.activeInHierarchy) YouWin_Object.SetActive(false);

    BJmanager.TotalBet_Text.text = (BJmanager.mainBet + BJmanager.multiplierBet).ToString("N2");
    BJmanager.UpdateWinnings(0);
    ResetPlayerTotalBG();
    ResetSplitTotalBGs();

    if (!ChipParent_Transform.gameObject.activeSelf)
    {
      ChipParent_Transform.gameObject.SetActive(true);
    }
    // Reset chip parent position
    ChipParent_Transform.localPosition = new(ChipParent_Transform.localPosition.x, ChipParent_Transform.localPosition.y + 132, ChipParent_Transform.localPosition.z);

    if (ChipContainer_Object)
    {
      for (int i = 0; i < ChipContainer_Object.transform.childCount; i++)
        ChipContainer_Object.transform.GetChild(i).gameObject.SetActive(true);
    }

    for (int i = 0; i < multiplierChipsParent_Transform.childCount; i++)
    {
      multiplierChipsParent_Transform.GetChild(i).gameObject.SetActive(true);
    }

    if (BJmanager.MainBetText_Object != null) BJmanager.MainBetText_Object.SetActive(true);
    if (BJmanager.multiplierBet > 0)
      if (BJmanager.MultiplierBetText_Object != null) BJmanager.MultiplierBetText_Object.SetActive(true);

    BJmanager.UpdateBetText(BJmanager.mainBet, BJmanager.multiplierBet);
  }

  private void OpenPopup(GameObject popup)
  {
    if (DisconnectPopup_Object != null && DisconnectPopup_Object.activeInHierarchy) return;
    if (popup == DisconnectPopup_Object && isExit) return;

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
    if (DisconnectPopup_Object != null && DisconnectPopup_Object.activeInHierarchy) return;

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
  internal void SetPlayerTotalBGWin()
  {
    if (PlayerTotalBG_Image && TotalBG_Green)
      PlayerTotalBG_Image.sprite = TotalBG_Green;
  }

  internal void ResetPlayerTotalBG()
  {
    if (PlayerTotalBG_Image && TotalBG_Orange)
      PlayerTotalBG_Image.sprite = TotalBG_Orange;
  }

  internal void ResetSplitTotalBGs()
  {
    if (FirstSplitTotalBG_Image) FirstSplitTotalBG_Image.sprite = TotalBG_Orange;
    if (SecondSplitTotalBG_Image) SecondSplitTotalBG_Image.sprite = TotalBG_Orange;
  }

  internal void SetFirstSplitBGWin()
  {
    if (FirstSplitTotalBG_Image) FirstSplitTotalBG_Image.sprite = TotalBG_Green;
  }

  internal void SetSecondSplitBGWin()
  {
    if (SecondSplitTotalBG_Image) SecondSplitTotalBG_Image.sprite = TotalBG_Green;
  }


  void CallOnGameQuit()
  {
    isExit = true;
    socket.CloseGame();
  }

  #endregion
}
