using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
  [Header("Managers")]
  [SerializeField] private BJController BJmanager;
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
  [SerializeField] private Button InitDouble_Button;
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
  [SerializeField] private Transform MultiplierHistoryParent;
  [SerializeField] private GameObject HistoryPrefab;
  private Queue<GameObject> historyQueue = new Queue<GameObject>();


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
  Sequence PopupSeq;
  bool isExit = false;
  int chipCounter = 0;

  private void Start()
  {
    if (MultBetBttn_Object) MultBetBttn_Object.SetActive(true);
    if (ChipSelectParent_Object) ChipSelectParent_Object.SetActive(true);
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

    if (InitDouble_Button) InitDouble_Button.onClick.RemoveAllListeners();
    if (InitDouble_Button) InitDouble_Button.onClick.AddListener(OnInitDoubleButton);

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
    Popup_CG.alpha = 0;
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

  private void OnInitDoubleButton()
  {
    if (ArrPointer_Object) ArrPointer_Object.SetActive(false);
  }

  private void OnRebet()
  {
    StartCoroutine(OnRebetCoroutine());
  }

  private IEnumerator OnRebetCoroutine()
  {
    if (RebetButtons_object) RebetButtons_object.SetActive(false);
    ResetUI();
    yield return BJmanager.ClearCards();
    if (MainBetButton_Object) MainBetButton_Object.SetActive(true);
    if (MultBetBttn_Object) MultBetBttn_Object.SetActive(true);
    if (ChipSelectParent_Object) ChipSelectParent_Object.SetActive(true);
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
    if (RebetButtons_object) RebetButtons_object.SetActive(false);
    ResetUI();
    if (MainBetButton_Object) MainBetButton_Object.SetActive(false);
    yield return BJmanager.ClearCards();
    OnDeal();
  }

  private void OnRebetDouble()
  {
    StartCoroutine(OnRebetDoubleCoroutine());
  }

  private IEnumerator OnRebetDoubleCoroutine()
  {
    ResetUI();
    if (RebetButtons_object) RebetButtons_object.SetActive(false);
    if (MainBetButton_Object) MainBetButton_Object.SetActive(false);
    yield return BJmanager.ClearCards();
    OnDeal();
  }

  private void ToggleSplitHandBet()
  {
    string bet = BJmanager.mainBet.ToString("N2");
    FirstHandBet_Text.text = bet;
    SecondHandBet_Text.text = bet;
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

  private void OnSplit()
  {
    StartCoroutine(OnSplitDealCoroutine());
  }

  private IEnumerator OnSplitDealCoroutine()
  {
    socket.RequestEvent("SPLIT");
    yield return new WaitUntil(() => socket.IsResultDone);

    if (!socket.ResultData.success)
    {
      ShowPopup("Insufficient balance to split!");
      Split_object.SetActive(false);
      yield break;
    }

    ChipParent_Transform.gameObject.SetActive(false);
    ArrPointer_Object.SetActive(false);
    if (Split_object) Split_object.SetActive(false);
    if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
    if (MiddleDouble_object.activeSelf) MiddleDouble_object.SetActive(false);
    PlayerCardTotal_Object.SetActive(false);
    yield return BJmanager.SplitButton();
    ToggleSplitHandBet();
    FirstSplitCardTotal_Object.SetActive(true);
    SecondSplitCardTotal_Object.SetActive(true);

    if (!socket.ResultData.id.ToLower().Contains("gameresult"))
    {
      if (socket.ResultData.payload.isAceSplit)
      {
        FirstArrPointer_Object.SetActive(false);
        SecondArrPointer_Object.SetActive(false);
        BJmanager.isFirstSplit = false;
        OnStand();
      }
      else if (socket.ResultData.payload.currentHandIndex == 0)
      {
        BJmanager.isFirstSplit = true;
        if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
        FirstArrPointer_Object.SetActive(true);
        SecondArrPointer_Object.SetActive(false);
      }
      else
      {
        BJmanager.isFirstSplit = false;
        if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
        FirstArrPointer_Object.SetActive(false);
        SecondArrPointer_Object.SetActive(true);
      }
    }
    else
    {
      BJmanager.isFlippin = true;
      BJmanager.OnDealerOpenFlipped(socket.ResultData.payload.dealerHand.cards[1]);
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
        if (YouWin_Object) YouWin_Object.SetActive(true);
        BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
        FirstHandBet_Text.text = FirstHandResult.payout.ToString("N2");
      }
      else if (FirstResult.Contains("lose"))
      {
        BJmanager.LostFirstHandChips();
      }

      if (FirstResult.Contains("push"))
      {
        BJmanager.FirstSplitTotal_Text.text = "PUSH " + BJmanager.FirstSplitTotal_Text.text;
      }
      else if (FirstResult.Contains("bust"))
      {
        BJmanager.FirstSplitTotal_Text.text = "BUST " + BJmanager.FirstSplitTotal_Text.text;
      }

      HandResult SecondHandResult = socket.ResultData.payload.handResults[1];
      string SecondResult = SecondHandResult.result.ToLower();
      BJmanager.SecondSplitTotal_Text.text = SecondHandResult.handValue.ToString();
      if (SecondResult.Contains("win"))
      {
        YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
        if (YouWin_Object) YouWin_Object.SetActive(true);
        BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
        SecondHandBet_Text.text = SecondHandResult.payout.ToString("N2");
      }
      else if (SecondResult.Contains("lose"))
      {
        BJmanager.LostSecondHandChips();
      }

      if (SecondResult.Contains("push"))
      {
        BJmanager.SecondSplitTotal_Text.text = "PUSH " + BJmanager.SecondSplitTotal_Text.text;
      }
      else if (SecondResult.Contains("bust"))
      {
        BJmanager.SecondSplitTotal_Text.text = "BUST " + BJmanager.SecondSplitTotal_Text.text;
      }


      if (socket.ResultData.payload.dealerHand.isBust)
      {
        BJmanager.DealerBust();
      }
      if (socket.ResultData.payload.dealerHand.isBlackjack)
      {
        DealerBlackjack_Object.SetActive(true);
      }

      if (RebetButtons_object) RebetButtons_object.SetActive(true);
    }
  }

  private IEnumerator OnStartDeal()
  {
    if (InitialButtons_object) InitialButtons_object.SetActive(false);
    MainBet_Button.gameObject.SetActive(false);
    MultiplierBet_Button.gameObject.SetActive(false);
    if (ChipSelectParent_Object) ChipSelectParent_Object.SetActive(false);

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

    if (PlayerCardTotal_Object) PlayerCardTotal_Object.SetActive(true);
    if (DealerCardTotal_Object) DealerCardTotal_Object.SetActive(true);
    BJmanager.UpdateBalance(socket.ResultData.player.balance);

    if (BJmanager.CheckMultiplier())
    {
      yield return DragonRoutine();
    }

    string gameState = socket.ResultData.payload.gamePhase.ToLower();

    if (gameState.Contains("player-turn"))
    {
      if (socket.ResultData.payload.playerHands[0].isBlackjack)
      {
        if (PlayerBlackjack_Object) PlayerBlackjack_Object.SetActive(true);
        if (socket.ResultData.payload.dealerUpCard.rank == "A")
        {
          OnStand();
        }
      }
      else
      {
        if (ArrPointer_Object) ArrPointer_Object.SetActive(true);
        if (Split_object) Split_object.SetActive(socket.ResultData.payload.canSplit);
        if (InitDouble_Button) InitDouble_Button.gameObject.SetActive(socket.ResultData.payload.canDouble);
        if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
      }
    }
    else if (gameState.Contains("completed"))
    {
      double win = socket.ResultData.payload.totalWin;
      if (win > 0)
      {
        if (socket.ResultData.payload.handResults[0].result.ToLower().Contains("blackjack"))
        {
          if (PlayerBlackjack_Object) PlayerBlackjack_Object.SetActive(true);
        }
        YouWin_Text.text = win.ToString("N2");
        if (YouWin_Object) YouWin_Object.SetActive(true);
        BJmanager.UpdateWinnings(win);
        BJmanager.UpdateBetText(socket.ResultData.payload.totalWin - socket.ResultData.payload.sideBetWin, socket.ResultData.payload.sideBetWin);
      }
      else
      {
        BJmanager.LostChipsAnimation();
      }
      if (RebetButtons_object) RebetButtons_object.SetActive(true);
    }
  }

  private IEnumerator OnHitCoroutine()
  {
    if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
    if (Split_object && Split_object.activeSelf) Split_object.SetActive(false);
    if (MiddleDouble_object.activeSelf) MiddleDouble_object.SetActive(false);
    if (ArrPointer_Object) ArrPointer_Object.SetActive(false);
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

        BJmanager.PlayerBust();
        RebetButtons_object.SetActive(true);
        BJmanager.UpdateBalance(socket.ResultData.player.balance);
        yield break;
      }

      if (ArrPointer_Object) ArrPointer_Object.SetActive(true);
      if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
    }
    else
    {
      Card PlayerCard = new();
      bool isGameCompleted = false;
      if (socket.ResultData.payload.gamePhase.ToLower().Contains("completed"))
      {
        isGameCompleted = true;
        PlayerCard = socket.ResultData.payload.playerHands[1].cards[BJmanager.FirstSplitplayerCounter];
      }
      else
      {
        PlayerCard = socket.ResultData.payload.card;
        if (socket.ResultData.payload.isBust && !BJmanager.isFirstSplit)
        {
          isGameCompleted = true;
        }
      }

      BJmanager.isFlippin = true;
      BJmanager.OnSplitDealButton(PlayerCard);
      yield return new WaitUntil(() => !BJmanager.isFlippin);

      if (isGameCompleted)
      {
        if (!socket.ResultData.id.ToLower().Contains("gameresult"))
        {
          if (socket.ResultData.payload.isBust)
          {
            BJmanager.SecondSplitTotal_Text.text = "BUST " + socket.ResultData.payload.handValue;
          }
        }
        else
        {
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
            if (YouWin_Object) YouWin_Object.SetActive(true);
            BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
            FirstHandBet_Text.text = FirstHandResult.payout.ToString("N2");
          }
          else if (FirstResult.Contains("lose"))
          {
            BJmanager.LostFirstHandChips();
          }

          if (FirstResult.Contains("push"))
          {
            BJmanager.FirstSplitTotal_Text.text = "PUSH " + BJmanager.FirstSplitTotal_Text.text;
          }
          else if (FirstResult.Contains("bust"))
          {
            BJmanager.FirstSplitTotal_Text.text = "BUST " + BJmanager.FirstSplitTotal_Text.text;
          }

          HandResult SecondHandResult = socket.ResultData.payload.handResults[1];
          string SecondResult = SecondHandResult.result.ToLower();
          BJmanager.SecondSplitTotal_Text.text = SecondHandResult.handValue.ToString();
          if (SecondResult.Contains("win"))
          {
            YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
            if (YouWin_Object) YouWin_Object.SetActive(true);
            BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
            SecondHandBet_Text.text = SecondHandResult.payout.ToString("N2");
          }
          else if (SecondResult.Contains("lose"))
          {
            BJmanager.LostSecondHandChips();
          }

          if (SecondResult.Contains("push"))
          {
            BJmanager.SecondSplitTotal_Text.text = "PUSH " + BJmanager.SecondSplitTotal_Text.text;
          }
          else if (SecondResult.Contains("bust"))
          {
            BJmanager.SecondSplitTotal_Text.text = "BUST " + BJmanager.SecondSplitTotal_Text.text;
          }


          if (socket.ResultData.payload.dealerHand.isBust)
          {
            BJmanager.DealerBust();
          }
          if (socket.ResultData.payload.dealerHand.isBlackjack)
          {
            DealerBlackjack_Object.SetActive(true);
          }

          if (RebetButtons_object) RebetButtons_object.SetActive(true);

        }
      }
      else
      {
        if (BJmanager.isFirstSplit)
        {
          if (socket.ResultData.payload.isBust)
          {
            BJmanager.isFirstSplit = false;
            BJmanager.FirstSplitTotal_Text.text = "BUST " + socket.ResultData.payload.handValue;
            FirstArrPointer_Object.SetActive(false);
            SecondArrPointer_Object.SetActive(true);
          }
          if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
        }
        else
        {
          if (socket.ResultData.payload.handValue == 21)
          {
            FirstArrPointer_Object.SetActive(false);
            SecondArrPointer_Object.SetActive(false);
            OnStand();
          }
          else
          {
            MiddleButtons_object.SetActive(true);
          }
        }
      }
    }
  }

  private IEnumerator OnStandCoroutine()
  {
    if (ArrPointer_Object && ArrPointer_Object.activeInHierarchy) ArrPointer_Object.SetActive(false);
    if (MiddleButtons_object) MiddleButtons_object.SetActive(false);
    socket.RequestEvent("STAND");
    yield return new WaitUntil(() => socket.IsResultDone);
    if (!BJmanager.isSplit)
    {
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
      string result = socket.ResultData.payload.handResults[0].result.ToLower();

      if (result.Contains("push"))
      {
        BJmanager.PlayerPush();
        BJmanager.UpdateWinnings(socket.ResultData.payload.handResults[0].payout);
      }
      else if (result.Contains("win"))
      {
        YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
        if (YouWin_Object) YouWin_Object.SetActive(true);
        BJmanager.UpdateWinnings(socket.ResultData.payload.totalWin);
        BJmanager.UpdateBetText(socket.ResultData.payload.totalWin - socket.ResultData.payload.sideBetWin, socket.ResultData.payload.sideBetWin);
      }
      else if (result.Contains("lose"))
      {
        BJmanager.SetDealerValue(socket.ResultData.payload.dealerHand.value);
        BJmanager.LostChipsAnimation();
      }

      if (socket.ResultData.payload.dealerHand.isBust)
      {
        BJmanager.DealerBust();
      }

      if (RebetButtons_object) RebetButtons_object.SetActive(true);
    }
    else if (BJmanager.isFirstSplit)
    {
      BJmanager.isFirstSplit = false;
      if (MiddleButtons_object) MiddleButtons_object.SetActive(true);
      FirstArrPointer_Object.SetActive(false);
      SecondArrPointer_Object.SetActive(true);
    }
    else
    {
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
        if (YouWin_Object) YouWin_Object.SetActive(true);
        BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
        FirstHandBet_Text.text = FirstHandResult.payout.ToString("N2");
      }
      else if (FirstResult.Contains("lose"))
      {
        BJmanager.LostFirstHandChips();
      }

      if (FirstResult.Contains("push"))
      {
        BJmanager.FirstSplitTotal_Text.text = "PUSH " + BJmanager.FirstSplitTotal_Text.text;
      }
      else if (FirstResult.Contains("bust"))
      {
        BJmanager.FirstSplitTotal_Text.text = "BUST " + BJmanager.FirstSplitTotal_Text.text;
      }

      HandResult SecondHandResult = socket.ResultData.payload.handResults[1];
      string SecondResult = SecondHandResult.result.ToLower();
      BJmanager.SecondSplitTotal_Text.text = SecondHandResult.handValue.ToString();
      if (SecondResult.Contains("win"))
      {
        YouWin_Text.text = socket.ResultData.payload.totalWin.ToString("N2");
        if (YouWin_Object) YouWin_Object.SetActive(true);
        BJmanager.UpdateBetText(BJmanager.mainBet, socket.ResultData.payload.sideBetWin);
        SecondHandBet_Text.text = SecondHandResult.payout.ToString("N2");
      }
      else if (SecondResult.Contains("lose"))
      {
        BJmanager.LostSecondHandChips();
      }

      if (SecondResult.Contains("push"))
      {
        BJmanager.SecondSplitTotal_Text.text = "PUSH " + BJmanager.SecondSplitTotal_Text.text;
      }
      else if (SecondResult.Contains("bust"))
      {
        BJmanager.SecondSplitTotal_Text.text = "BUST " + BJmanager.SecondSplitTotal_Text.text;
      }


      if (socket.ResultData.payload.dealerHand.isBust)
      {
        BJmanager.DealerBust();
      }
      if (socket.ResultData.payload.dealerHand.isBlackjack)
      {
        DealerBlackjack_Object.SetActive(true);
      }

      if (RebetButtons_object) RebetButtons_object.SetActive(true);
    }
  }

  private IEnumerator DragonRoutine()
  {
    XMultiplier_Text.gameObject.SetActive(false);
    if (DragonNormal_Object) DragonNormal_Object.SetActive(false);
    if (DragonFire_Object) DragonFire_Object.SetActive(true);
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
    if (DragonFire_Object) DragonFire_Object.SetActive(false);
    if (DragonNormal_Object) DragonNormal_Object.SetActive(true);
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

  internal void ShowInitialButtons()
  {
    if (InitialButtons_object) InitialButtons_object.SetActive(true);
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

  private void ResetUI()
  {
    Destroy(FirstHandChipContainer.GetChild(0).gameObject);
    Destroy(SecondHandChipContainer.GetChild(0).gameObject);
    ArrPointer_Object.SetActive(false);
    FirstArrPointer_Object.SetActive(false);
    SecondArrPointer_Object.SetActive(false);
    FirstSplitCardTotal_Object.SetActive(false);
    SecondSplitCardTotal_Object.SetActive(false);
    DealerBlackjack_Object.SetActive(false);
    if (PlayerBlackjack_Object.activeInHierarchy) PlayerBlackjack_Object.SetActive(false);
    if (YouWin_Object.activeInHierarchy) YouWin_Object.SetActive(false);
    BJmanager.UpdateWinnings(0);
    ChipParent_Transform.localPosition = new(ChipParent_Transform.localPosition.x, ChipParent_Transform.localPosition.y + 132, ChipParent_Transform.localPosition.z);
    for (int i = 0; i < ChipContainer_Object.transform.childCount; i++)
    {
      ChipContainer_Object.transform.GetChild(i).gameObject.SetActive(true);
    }
    for (int i = 0; i < multiplierChipsParent_Transform.childCount; i++)
    {
      multiplierChipsParent_Transform.GetChild(i).gameObject.SetActive(true);
    }
    BJmanager.MainBetText_Object.SetActive(true);
    if (BJmanager.multiplierBet > 0)
      BJmanager.MultiplierBetText_Object.SetActive(true);
    BJmanager.UpdateBetText(BJmanager.mainBet, BJmanager.multiplierBet);
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
