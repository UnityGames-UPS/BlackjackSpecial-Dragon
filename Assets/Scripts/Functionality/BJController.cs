using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BJController : MonoBehaviour
{
  [SerializeField] private SocketIOManager socket;
  [SerializeField] private UIManager uiManager;

  [SerializeField] internal GameObject MainBetText_Object;
  [SerializeField] private TMP_Text MainBetText_Text;
  [SerializeField] internal GameObject MultiplierBetText_Object;
  [SerializeField] private TMP_Text MultiplierBetText_Text;

  [Header("Transforms")]
  [SerializeField] private Transform ChipsParent_Transform;
  [SerializeField] private Transform MultiplyChipsParent_Transform;
  [SerializeField] private Transform PlayerContainer_Transform;
  [SerializeField] private Transform DealerContainer_Transform;
  [SerializeField] private Transform Deck_Transform;
  [SerializeField] private Transform EndDeck_Transform;
  [SerializeField] internal Transform FirstSplit_Transform;
  [SerializeField] internal Transform SecondSplit_Transform;
  [SerializeField] private Transform ChipsLost_Transform;

  [Header("Lists and Arrays")]
  [SerializeField] private Transform[] CoinContainers_Transform;
  [SerializeField] private GameObject[] Coins_Prefab;
  [SerializeField] private List<GameObject> instantiated_Coins;
  [SerializeField] private List<int> instantiated_Value;
  [SerializeField] private List<GameObject> multiplyinstantiated_Coins;
  [SerializeField] internal List<GameObject> firstHand_Coins;
  [SerializeField] internal List<GameObject> secondHand_Coins;
  [SerializeField] private List<int> multiplyinstantiated_Value;
  [SerializeField] internal List<string> betHistory;

  [Header("Integers")]
  [SerializeField] private int CoinCounter = 0;
  [SerializeField] private int[] amount_array;
  [SerializeField] internal int mainBet = 0;
  [SerializeField] internal int multiplierBet = 0;
  [SerializeField] private int dealerTotal = 0;
  [SerializeField] private int playerTotal = 0;
  private int maxBetAmount = 500;

  [Header("Prefabs")]
  [SerializeField] private GameObject Cards_Prefab;

  [Header("Card Sprites")]
  [SerializeField] private Sprite card_Back;
  [SerializeField] private Sprite[] clubs_Sprite;
  [SerializeField] private Sprite[] spades_Sprite;
  [SerializeField] private Sprite[] hearts_Sprite;
  [SerializeField] private Sprite[] diamonds_Sprite;

  [Header("Texts")]
  [SerializeField] internal TMP_Text TotalBet_Text;
  [SerializeField] private TMP_Text Balance_Text;
  [SerializeField] private TMP_Text Winnings_Text;
  [SerializeField] private TMP_Text PlayerTotal_Text;
  [SerializeField] private TMP_Text DealerTotal_Text;
  [SerializeField] internal TMP_Text FirstSplitTotal_Text;
  [SerializeField] internal TMP_Text SecondSplitTotal_Text;

  [Header("Test Data")]
  [SerializeField] internal bool useTestData = false;
  [SerializeField] internal List<int> playerData;
  [SerializeField] internal List<int> firstplayerData;
  [SerializeField] internal List<int> secondplayerData;
  [SerializeField] internal List<int> dealerData;

  internal List<Card> playerCards = new();
  internal List<Card> dealerCards = new();
  internal List<Card> firstPlayerCards = new();
  internal List<Card> secondPlayerCards = new();

  private int totalValue = 0;
  private int FirsttotalValue = 0;
  private int SecondtotalValue = 0;
  private int totaldealerValue = 0;

  internal int playerCounter = 0;
  internal int FirstSplitplayerCounter = 0;
  internal int SecondSplitplayerCounter = 0;
  internal int dealerCounter = 0;

  internal bool isFlippin = false;
  internal bool isSplit = false;
  internal bool isFirstSplit = false;

  private CardScript tempdealer = null;
  private Tween balTween;
  private Tween winTween;

  private void Start()
  {
    if (MultiplierBetText_Object) MultiplierBetText_Object.SetActive(false);
    if (MainBetText_Object) MainBetText_Object.SetActive(false);
  }

  internal void HandleInit()
  {
    Balance_Text.text = 0.ToString("N2");
    UpdateBalance(socket.PlayerData.balance);
    Winnings_Text.text = 0.ToString("N2");
    UpdateWinnings(0);

    maxBetAmount = socket.bets[^1];
    int index = 0;
    foreach (var container in CoinContainers_Transform)
    {
      if (container && container.childCount > 0)
      {
        TMP_Text t = container.GetChild(0).GetComponent<TMP_Text>();
        if (t != null) t.text = socket.bets[index].ToString();
      }
      index++;
    }
    LowBalCheck(socket.bets[0]);
  }

  internal void UpdateBalance(double amount)
  {
    double startBalance = double.Parse(Balance_Text.text);
    balTween?.Kill();
    balTween = DOTween.To(() => startBalance, x => startBalance = x, amount, 0.25f).OnUpdate(() =>
    {
      Balance_Text.text = startBalance.ToString("N2");
    });
  }

  internal void UpdateWinnings(double amount)
  {
    double startWin = 0;
    winTween?.Kill();
    winTween = DOTween.To(() => startWin, x => startWin = x, amount, 0.25f).OnUpdate(() =>
    {
      Winnings_Text.text = startWin.ToString("N2");
    });
  }

  internal void SelectCoin(int counter)
  {
    CoinCounter = counter;
  }

  internal bool TryDoubleBet()
  {
    int newMainBet = mainBet * 2;
    int newSideBet = multiplierBet * 2;

    if (newMainBet > maxBetAmount)
    {
      uiManager.ShowPopup("Max Main Bet Reached!");
      return false;
    }

    if (newSideBet > newMainBet)
    {
      uiManager.ShowPopup("Side Bet Cannot Exceed Main Bet!");
      return false;
    }

    int amountToAddMain = mainBet;
    AddDoubleChips(amountToAddMain, false);

    mainBet = newMainBet;

    if (MainBetText_Object && MainBetText_Text)
    {
      MainBetText_Object.SetActive(true);
      MainBetText_Text.text = mainBet.ToString("N2");
    }

    if (multiplierBet > 0)
    {
      int amountToAddSide = multiplierBet;
      AddDoubleChips(amountToAddSide, true);

      multiplierBet = newSideBet;

      if (MultiplierBetText_Object && MultiplierBetText_Text)
      {
        MultiplierBetText_Object.SetActive(true);
        MultiplierBetText_Text.text = multiplierBet.ToString("N2");
      }
    }

    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");

    return true;
  }

  internal void BetOnButton()
  {
    if (mainBet + amount_array[CoinCounter] > maxBetAmount)
    {
      uiManager.ShowPopup("Max Bet Reached!");
      return;
    }
    if (LowBalCheck(amount_array[CoinCounter]))
    {
      return;
    }
    uiManager.ShowInitialButtons();

    GameObject coin = Instantiate(Coins_Prefab[CoinCounter], CoinContainers_Transform[CoinCounter]);
    coin.transform.localPosition = Vector2.zero;
    coin.transform.SetParent(ChipsParent_Transform);
    coin.transform.localScale = Vector3.one;
    coin.transform.DOLocalMove(new Vector2(0, instantiated_Coins.Count * 2), 0.2f)
        .OnComplete(() => OptimizeCoinStack());

    instantiated_Coins.Add(coin);
    instantiated_Value.Add(amount_array[CoinCounter]);
    mainBet += amount_array[CoinCounter];

    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
    if (MainBetText_Object && MainBetText_Text)
    {
      MainBetText_Object.SetActive(true);
      MainBetText_Text.text = mainBet.ToString("N2");
    }
    betHistory.Add("main_" + amount_array[CoinCounter]);
  }

  internal void MultiplierBetOnButton()
  {
    if (multiplierBet + amount_array[CoinCounter] > mainBet)
    {
      uiManager.ShowPopup("Max Multiplier Bet Reached!");
      return;
    }
    if (LowBalCheck(amount_array[CoinCounter]))
    {
      return;
    }
    uiManager.ShowInitialButtons();

    GameObject coin = Instantiate(Coins_Prefab[CoinCounter], CoinContainers_Transform[CoinCounter]);
    coin.transform.localPosition = Vector2.zero;
    coin.transform.SetParent(MultiplyChipsParent_Transform);
    coin.transform.localScale = Vector3.one;
    coin.transform.DOLocalMove(new Vector2(0, multiplyinstantiated_Coins.Count * 2), 0.2f)
        .OnComplete(() => MultiplyOptimizeCoinStack());

    multiplyinstantiated_Coins.Add(coin);
    multiplyinstantiated_Value.Add(amount_array[CoinCounter]);
    multiplierBet += amount_array[CoinCounter];

    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
    if (MultiplierBetText_Object && MultiplierBetText_Text)
    {
      MultiplierBetText_Object.SetActive(true);
      MultiplierBetText_Text.text = multiplierBet.ToString("N2");
    }
    betHistory.Add("multiplier_" + amount_array[CoinCounter]);
  }

  private void OptimizeCoinStack()
  {
    int[] coinValues = socket.bets.ToArray();

    for (int i = 0; i < coinValues.Length - 1; i++)
    {
      int currentValue = coinValues[i];
      int nextValue = coinValues[i + 1];

      int currentCount = 0;
      foreach (int v in instantiated_Value)
      {
        if (v == currentValue) currentCount++;
      }

      int ratio = (currentValue > 0) ? (nextValue / currentValue) : 0;
      if (ratio > 0 && currentCount >= ratio)
      {
        int numNewCoins = currentCount / ratio;
        int numToRemove = numNewCoins * ratio;

        for (int k = 0; k < numToRemove; k++)
        {
          int idx = instantiated_Value.LastIndexOf(currentValue);
          if (idx != -1)
          {
            Destroy(instantiated_Coins[idx]);
            instantiated_Coins.RemoveAt(idx);
            instantiated_Value.RemoveAt(idx);
            betHistory.Remove("main_" + currentValue);
          }
        }

        for (int k = 0; k < numNewCoins; k++)
        {
          GameObject c = Instantiate(Coins_Prefab[i + 1], CoinContainers_Transform[i + 1]);
          c.transform.SetParent(ChipsParent_Transform);
          instantiated_Coins.Add(c);
          instantiated_Value.Add(nextValue);
          betHistory.Add("main_" + nextValue);
        }

        OptimizeCoinStack();
        return;
      }
    }

    SortAndArrangeCoins(instantiated_Value, instantiated_Coins, 2.5f);
    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
  }

  private void MultiplyOptimizeCoinStack()
  {
    int[] coinValues = socket.bets.ToArray();

    for (int i = 0; i < coinValues.Length - 1; i++)
    {
      int currentValue = coinValues[i];
      int nextValue = coinValues[i + 1];

      int currentCount = 0;
      foreach (int v in multiplyinstantiated_Value)
      {
        if (v == currentValue) currentCount++;
      }

      int ratio = (currentValue > 0) ? (nextValue / currentValue) : 0;
      if (ratio > 0 && currentCount >= ratio)
      {
        int numNewCoins = currentCount / ratio;
        int numToRemove = numNewCoins * ratio;

        for (int k = 0; k < numToRemove; k++)
        {
          int idx = multiplyinstantiated_Value.LastIndexOf(currentValue);
          if (idx != -1)
          {
            Destroy(multiplyinstantiated_Coins[idx]);
            multiplyinstantiated_Coins.RemoveAt(idx);
            multiplyinstantiated_Value.RemoveAt(idx);
            betHistory.Remove("multiplier_" + currentValue);
          }
        }

        for (int k = 0; k < numNewCoins; k++)
        {
          GameObject c = Instantiate(Coins_Prefab[i + 1], CoinContainers_Transform[i + 1]);
          c.transform.SetParent(MultiplyChipsParent_Transform);
          multiplyinstantiated_Coins.Add(c);
          multiplyinstantiated_Value.Add(nextValue);
          betHistory.Add("multiplier_" + nextValue);
        }

        MultiplyOptimizeCoinStack();
        return;
      }
    }

    SortAndArrangeCoins(multiplyinstantiated_Value, multiplyinstantiated_Coins, 2f);
    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
  }

  private void SortAndArrangeCoins(List<int> values, List<GameObject> coins, float ySpacing)
  {
    var pairs = new List<KeyValuePair<int, GameObject>>();
    for (int i = 0; i < values.Count; i++)
      pairs.Add(new KeyValuePair<int, GameObject>(values[i], coins[i]));

    pairs.Sort((a, b) => b.Key.CompareTo(a.Key));

    values.Clear();
    coins.Clear();

    foreach (var p in pairs)
    {
      values.Add(p.Key);
      coins.Add(p.Value);
    }

    for (int i = 0; i < coins.Count; i++)
    {
      coins[i].transform.localPosition = new Vector2(0, i * ySpacing);
      coins[i].transform.SetSiblingIndex(i);
    }
  }

  internal void ClearBet()
  {
    foreach (GameObject coin in instantiated_Coins) Destroy(coin);
    foreach (GameObject coin in multiplyinstantiated_Coins) Destroy(coin);
    foreach (GameObject coin in firstHand_Coins) Destroy(coin);
    foreach (GameObject coin in secondHand_Coins) Destroy(coin);

    firstHand_Coins.Clear();
    firstHand_Coins.TrimExcess();
    firstHand_Coins.RemoveAll(c => c == null);
    secondHand_Coins.RemoveAll(c => c == null);
    secondHand_Coins.Clear();
    secondHand_Coins.TrimExcess();
    instantiated_Coins.Clear();
    instantiated_Coins.TrimExcess();
    instantiated_Value.Clear();
    instantiated_Value.TrimExcess();
    multiplyinstantiated_Coins.Clear();
    multiplyinstantiated_Coins.TrimExcess();
    multiplyinstantiated_Value.Clear();
    multiplyinstantiated_Value.TrimExcess();
    betHistory.Clear();

    mainBet = 0;
    multiplierBet = 0;

    if (MainBetText_Object && MainBetText_Text)
    {
      MainBetText_Object.SetActive(false);
      MainBetText_Text.text = "0";
    }
    if (MultiplierBetText_Object && MultiplierBetText_Text)
    {
      MultiplierBetText_Object.SetActive(false);
      MultiplierBetText_Text.text = "0";
    }
    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
  }

  internal IEnumerator ClearCards()
  {
    List<Transform> cardTransforms = new();
    int dealerFlippedIndex = -1;

    for (int i = 0; i < PlayerContainer_Transform.childCount; i++)
      cardTransforms.Add(PlayerContainer_Transform.GetChild(i));

    for (int i = 0; i < DealerContainer_Transform.childCount; i++)
    {
      cardTransforms.Add(DealerContainer_Transform.GetChild(i));
      if (DealerContainer_Transform.GetChild(i).GetComponent<Image>().sprite == card_Back)
      {
        dealerFlippedIndex = cardTransforms.LastIndexOf(DealerContainer_Transform.GetChild(i));
      }
    }

    for (int i = 0; i < FirstSplit_Transform.childCount; i++)
      cardTransforms.Add(FirstSplit_Transform.GetChild(i));

    for (int i = 0; i < SecondSplit_Transform.childCount; i++)
      cardTransforms.Add(SecondSplit_Transform.GetChild(i));

    List<Tween> tweens = new();
    int idx = 0;
    foreach (var t in cardTransforms)
    {
      CardScript script = t.GetComponent<CardScript>();
      if (idx != dealerFlippedIndex)
        script.OnFlipMethod(card_Back, 0, null);

      t.DOScale(0.7f, 0.3f).SetDelay(0.5f);
      t.GetComponent<Image>().DOFade(0, 0.3f).SetDelay(0.5f);
      t.DORotate(EndDeck_Transform.eulerAngles, 0.3f).SetDelay(0.5f);

      Tween moveTween = t.DOMove(EndDeck_Transform.position, 0.3f).SetDelay(0.5f)
          .OnStart(() => { script.layoutElement.ignoreLayout = true; })
          .OnComplete(() => { Destroy(t.gameObject); });

      tweens.Add(moveTween);
      idx++;
    }

    if (tweens.Count > 0)
      yield return tweens[^1].WaitForCompletion();

    yield return new WaitForSeconds(0.5f);

    playerCards.Clear();
    dealerCards.Clear();
    firstPlayerCards.Clear();
    secondPlayerCards.Clear();

    playerCounter = 0;
    dealerCounter = 0;
    FirstSplitplayerCounter = 0;
    SecondSplitplayerCounter = 0;

    totalValue = 0;
    totaldealerValue = 0;
    FirsttotalValue = 0;
    SecondtotalValue = 0;

    if (PlayerTotal_Text) PlayerTotal_Text.text = "0";
    if (DealerTotal_Text) DealerTotal_Text.text = "0";
    if (FirstSplitTotal_Text) FirstSplitTotal_Text.text = "0";
    if (SecondSplitTotal_Text) SecondSplitTotal_Text.text = "0";

    isSplit = false;
    isFirstSplit = false;
  }

  internal IEnumerator SplitButton()
  {
    GameObject tempCard = PlayerContainer_Transform.GetChild(0).gameObject;
    tempCard.transform.SetParent(FirstSplit_Transform);
    tempCard.transform.DOLocalMove(new Vector2(0, 0), 0.3f);
    tempCard.transform.DOScale(Vector3.one, 0.3f);

    tempCard = PlayerContainer_Transform.GetChild(0).gameObject;
    tempCard.transform.SetParent(SecondSplit_Transform);
    tempCard.transform.DOLocalMove(new Vector2(0, 0), 0.3f);
    tempCard.transform.DOScale(Vector3.one, 0.3f);

    List<Hand> playerHands = socket.ResultData.id.ToLower().Contains("gameresult") ?
        socket.ResultData.payload.playerHands :
        socket.ResultData.payload.hands;

    firstPlayerCards.Add(playerHands[0].cards[0]);
    secondPlayerCards.Add(playerHands[1].cards[0]);

    if (FirstSplitTotal_Text) FirstSplitTotal_Text.text = CalculateHandValue(firstPlayerCards);
    if (SecondSplitTotal_Text) SecondSplitTotal_Text.text = CalculateHandValue(secondPlayerCards);

    isSplit = true;
    isFirstSplit = true;

    GameObject card = Instantiate(Cards_Prefab, Deck_Transform);
    card.transform.localPosition = Vector2.zero;
    card.transform.localScale -= card.transform.localScale * 0.2f;
    card.transform.SetParent(FirstSplit_Transform);
    Sprite sprite1 = SelectSprite(playerHands[0].cards[1]);
    card.transform.DOScale(Vector3.one, 0.3f);
    card.transform.DOLocalRotate(Vector3.zero, 0.3f);
    yield return card.transform.DOLocalMove(new Vector2(0, 0), 0.3f)
        .OnComplete(() => { card.GetComponent<CardScript>().OnFlipMethod(sprite1, 3, playerHands[0].cards[1]); })
        .WaitForCompletion();

    GameObject card2 = Instantiate(Cards_Prefab, Deck_Transform);
    card2.transform.localPosition = Vector2.zero;
    card2.transform.localScale -= card2.transform.localScale * 0.2f;
    card2.transform.SetParent(SecondSplit_Transform);
    Sprite sprite2 = SelectSprite(playerHands[1].cards[1]);
    card2.transform.DORotate(Vector3.zero, 0.3f);
    card2.transform.DOScale(Vector3.one, 0.3f);
    yield return card2.transform.DOLocalMove(new Vector2(0, 0), 0.3f)
        .OnComplete(() => { card2.GetComponent<CardScript>().OnFlipMethod(sprite2, 4, playerHands[1].cards[1]); })
        .WaitForCompletion();
  }

  internal void UndoBetButton()
  {
    if (betHistory.Count == 0) return;

    string lastBet = betHistory[^1];
    betHistory.RemoveAt(betHistory.Count - 1);

    string[] parts = lastBet.Split('_');
    string betType = parts[0];
    int amount = int.Parse(parts[1]);

    if (betType == "main")
    {
      if (instantiated_Coins.Count > 0)
      {
        int indexToRemove = instantiated_Value.LastIndexOf(amount);
        if (indexToRemove != -1)
        {
          Destroy(instantiated_Coins[indexToRemove]);
          instantiated_Coins.RemoveAt(indexToRemove);
          instantiated_Value.RemoveAt(indexToRemove);
          mainBet -= amount;
          OptimizeCoinStack();
        }
      }

      if (mainBet > 0)
      {
        if (MainBetText_Object && MainBetText_Text) MainBetText_Text.text = mainBet.ToString("N2");
      }
      else
      {
        if (MainBetText_Object && MainBetText_Text)
        {
          MainBetText_Object.SetActive(false);
          MainBetText_Text.text = "0";
        }
      }
    }
    else if (betType == "multiplier")
    {
      if (multiplyinstantiated_Coins.Count > 0)
      {
        int indexToRemove = multiplyinstantiated_Value.LastIndexOf(amount);
        if (indexToRemove != -1)
        {
          Destroy(multiplyinstantiated_Coins[indexToRemove]);
          multiplyinstantiated_Coins.RemoveAt(indexToRemove);
          multiplyinstantiated_Value.RemoveAt(indexToRemove);
          multiplierBet -= amount;
          MultiplyOptimizeCoinStack();
        }
      }

      if (multiplierBet > 0)
      {
        if (MultiplierBetText_Object && MultiplierBetText_Text) MultiplierBetText_Text.text = multiplierBet.ToString("N2");
      }
      else
      {
        if (MultiplierBetText_Object && MultiplierBetText_Text)
        {
          MultiplierBetText_Object.SetActive(false);
          MultiplierBetText_Text.text = "0";
        }
      }
    }

    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
  }

  internal void OnDealerButton(Card cardData = null)
  {
    GameObject cardObj = Instantiate(Cards_Prefab, Deck_Transform);
    cardObj.transform.localPosition = Vector3.zero;
    cardObj.transform.localScale *= 0.8f;
    cardObj.transform.SetParent(DealerContainer_Transform);

    Sprite sprite = cardData == null
        ? SelectRandomArray(playerData[playerCounter])
        : SelectSprite(cardData);

    AnimateCardToTable(cardObj, Vector3.zero, () =>
    {
      cardObj.GetComponent<CardScript>().OnFlipMethod(sprite, 2, cardData);
    });
  }

  internal void OnDealerButtonClosedCard()
  {
    GameObject cardObj = Instantiate(Cards_Prefab, Deck_Transform);
    cardObj.transform.localPosition = Vector3.zero;
    cardObj.transform.localScale *= 0.8f;
    cardObj.transform.SetParent(DealerContainer_Transform);

    AnimateCardToTable(cardObj, Vector3.zero, () =>
    {
      var script = cardObj.GetComponent<CardScript>();
      tempdealer = script;
      script.layoutElement.ignoreLayout = false;
      isFlippin = false;
    });
  }

  internal bool CheckMultiplier()
  {
    return multiplyinstantiated_Value.Count > 0;
  }

  internal void OnDealerOpenFlipped(Card cardData)
  {
    Sprite sprite = useTestData
        ? SelectRandomArray(dealerData[dealerCounter])
        : SelectSprite(cardData);

    tempdealer.OnFlipMethod(sprite, 2, cardData);
  }

  internal void OnPlayerDealButton(Card cardData)
  {
    GameObject cardObj = Instantiate(Cards_Prefab, Deck_Transform);
    cardObj.transform.localPosition = Vector3.zero;
    cardObj.transform.localScale *= 0.8f;
    cardObj.transform.SetParent(PlayerContainer_Transform);

    Sprite sprite = useTestData
        ? SelectRandomArray(playerData[playerCounter])
        : SelectSprite(cardData);

    AnimateCardToTable(cardObj, Vector3.zero, () =>
    {
      cardObj.GetComponent<CardScript>().OnFlipMethod(sprite, 1, cardData);
    });
  }

  internal void OnSplitDealButton(Card cardData)
  {
    GameObject cardObj = Instantiate(Cards_Prefab, Deck_Transform);
    cardObj.transform.localPosition = Vector2.zero;
    cardObj.transform.localScale *= 0.8f;

    cardObj.transform.SetParent(isFirstSplit ? FirstSplit_Transform : SecondSplit_Transform);
    Sprite sprite = SelectSprite(cardData);

    AnimateCardToTable(cardObj, new Vector2(0, 0), () =>
    {
      cardObj.GetComponent<CardScript>().OnFlipMethod(
              sprite,
              isFirstSplit ? 3 : 4,
              cardData
          );
    });
  }

  internal void AfterCardFlip(int value, Card card)
  {
    switch (value)
    {
      case 1:
        if (card != null) playerCards.Add(card);
        if (PlayerTotal_Text) PlayerTotal_Text.text = CalculateHandValue(playerCards);
        playerCounter++;
        break;

      case 2:
        if (card != null) dealerCards.Add(card);
        if (DealerTotal_Text) DealerTotal_Text.text = CalculateHandValue(dealerCards);
        dealerCounter++;
        break;

      case 3:
        if (card != null) firstPlayerCards.Add(card);
        FirstSplitTotal_Text.text = CalculateHandValue(firstPlayerCards);
        FirstSplitplayerCounter++;
        break;

      case 4:
        if (card != null) secondPlayerCards.Add(card);
        SecondSplitTotal_Text.text = CalculateHandValue(secondPlayerCards);
        SecondSplitplayerCounter++;
        break;
    }

    isFlippin = false;
  }

  private string CalculateHandValue(List<Card> hand)
  {
    int total = 0;
    int aces = 0;

    foreach (var card in hand)
    {
      string rank = card.rank.ToUpper();
      int value;

      switch (rank)
      {
        case "J":
        case "Q":
        case "K":
          value = 10;
          break;
        case "A":
          value = 11;
          aces++;
          break;
        default:
          value = int.Parse(rank);
          break;
      }

      total += value;
    }

    while (total > 21 && aces > 0)
    {
      total -= 10;
      aces--;
    }

    if (hand.Count == 2 && total == 21)
      return "21";

    bool containsAce = hand.Exists(c => c.rank.ToUpper() == "A");

    if (containsAce)
    {
      int softValue = 0;
      foreach (var card in hand)
      {
        string rank = card.rank.ToUpper();
        if (rank == "A") softValue += 1;
        else if (rank is "J" or "Q" or "K") softValue += 10;
        else softValue += int.Parse(rank);
      }

      if (softValue != total && total <= 21)
        return $"{softValue} / {total}";
    }

    if(total>21)
      return $"BUST {total}";

    return total.ToString();
  }

  private Sprite SelectRandomArray(int value)
  {
    int randomSuit = Random.Range(0, 4);
    Sprite[] arr = randomSuit switch
    {
      0 => clubs_Sprite,
      1 => spades_Sprite,
      2 => hearts_Sprite,
      _ => diamonds_Sprite,
    };

    int randomFace = Random.Range(9, 12);
    return value == 10 ? arr[randomFace] : arr[value - 1];
  }

  private Sprite SelectSprite(Card card)
  {
    Sprite[] suitArray = card.suit.ToLower() switch
    {
      "clubs" => clubs_Sprite,
      "spades" => spades_Sprite,
      "hearts" => hearts_Sprite,
      "diamonds" => diamonds_Sprite,
      _ => null
    };

    if (suitArray == null)
    {
      Debug.LogError("Invalid suit: " + card.suit);
      return card_Back;
    }

    return card.rank.ToUpper() switch
    {
      "J" => suitArray[10],
      "Q" => suitArray[11],
      "K" => suitArray[12],
      "A" => suitArray[0],
      _ => suitArray[int.Parse(card.rank) - 1]
    };
  }

  // -----------------------
  // Helper animations (B2 extraction)
  // -----------------------

  private void AnimateCardToTable(GameObject cardObj, Vector3 targetPos, System.Action onComplete)
  {
    cardObj.transform.DOScale(Vector3.one, 0.3f);
    cardObj.transform.DOLocalRotate(Vector3.zero, 0.3f);
    cardObj.transform.DOLocalMove(targetPos, 0.3f)
        .OnComplete(() => onComplete?.Invoke());
  }

  private IEnumerator RevealDealerAllCards(Payload payload)
  {
    isFlippin = true;
    OnDealerOpenFlipped(payload.dealerHand.cards[1]);
    yield return new WaitUntil(() => !isFlippin);

    if (payload.dealerHand.cards.Count > 2)
    {
      for (int i = 0; i < payload.dealerHand.cards.Count - 2; i++)
      {
        isFlippin = true;
        OnDealerButton(payload.dealerHand.cards[i + 2]);
        yield return new WaitUntil(() => !isFlippin);
      }
    }
  }

  internal void LostChipsAnimation()
  {
    foreach (GameObject coin in instantiated_Coins)
    {
      if (!coin.activeSelf) continue;
      Vector3 originalPos = coin.transform.position;

      coin.transform.DOMove(ChipsLost_Transform.position, 0.3f)
          .OnComplete(() =>
          {
            coin.SetActive(false);
            coin.transform.position = originalPos;
          });
    }

    foreach (GameObject coin in multiplyinstantiated_Coins)
    {
      Vector3 originalPos = coin.transform.position;

      coin.transform.DOMove(ChipsLost_Transform.position, 0.3f)
          .OnComplete(() =>
          {
            coin.SetActive(false);
            coin.transform.position = originalPos;
          });
    }

    if (MainBetText_Object.activeSelf) MainBetText_Object.SetActive(false);
    if (MultiplierBetText_Object.activeSelf) MultiplierBetText_Object.SetActive(false);
  }

  internal void LostFirstHandChips()
  {
    firstHand_Coins.RemoveAll(c => c == null);
    foreach (GameObject coin in firstHand_Coins)
    {
      if (coin == null) continue;
      coin.transform.DOMove(ChipsLost_Transform.position, 0.3f)
          .OnComplete(() => coin.SetActive(false));
    }
  }

  internal void LostSecondHandChips()
  {
    secondHand_Coins.RemoveAll(c => c == null);
    foreach (GameObject coin in secondHand_Coins)
    {
      if (coin == null) continue;
      coin.transform.DOMove(ChipsLost_Transform.position, 0.3f)
          .OnComplete(() => coin.SetActive(false));
    }
  }

  internal void UpdateBetText(double mainBet, double sideBet)
  {
    MainBetText_Text.text = mainBet.ToString("N2");
    MultiplierBetText_Text.text = sideBet.ToString("N2");
  }

  internal void PlayerPush()
  {
    string total = PlayerTotal_Text.text;
    PlayerTotal_Text.text = "PUSH " + total;
  }

  internal void SetDealerValue(int value)
  {
    if (value > 21)
    {
      DealerTotal_Text.text = "BUST " + value.ToString();
    }
    else
    {
      DealerTotal_Text.text = value.ToString();
    }
  }

  // --------------------------
  // LOW BALANCE CHECK
  // --------------------------

  internal bool LowBalCheck(int value = 0)
  {
    if (socket.PlayerData.balance < mainBet + multiplierBet + value)
    {
      uiManager.ShowPopup("Insufficient Balance!");
      return true;
    }
    return false;
  }

  // --------------------------
  // DOUBLE CHIP HELPER
  // --------------------------

  private void AddDoubleChips(int amountToAdd, bool isSideBet)
  {
    int[] chipValues = socket.bets.ToArray();

    for (int i = chipValues.Length - 1; i >= 0; i--)
    {
      int chipValue = chipValues[i];

      while (amountToAdd >= chipValue)
      {
        AddChipInstance(chipValue, i, isSideBet);
        amountToAdd -= chipValue;
      }
    }
  }

  private void AddChipInstance(int value, int prefabIndex, bool isSideBet)
  {
    Transform parent = isSideBet ? MultiplyChipsParent_Transform : ChipsParent_Transform;

    GameObject obj = Instantiate(Coins_Prefab[prefabIndex], CoinContainers_Transform[prefabIndex]);
    obj.transform.localPosition = Vector2.zero;
    obj.transform.SetParent(parent);
    obj.transform.localScale = Vector3.one;

    List<GameObject> list = isSideBet ? multiplyinstantiated_Coins : instantiated_Coins;
    List<int> valList = isSideBet ? multiplyinstantiated_Value : instantiated_Value;

    obj.transform.DOLocalMove(new Vector2(0, list.Count * 2f), 0.2f)
        .OnComplete(() =>
        {
          if (isSideBet) MultiplyOptimizeCoinStack();
          else OptimizeCoinStack();
        });

    list.Add(obj);
    valList.Add(value);

    betHistory.Add((isSideBet ? "multiplier_" : "main_") + value);
  }

  // --------------------------
  // SPLIT / MULTI-HAND VALUE HELPERS
  // --------------------------

  private string PlayerNumberTestValue(int value)
  {
    switch (value)
    {
      case 1:
        if (totalValue > 10)
        {
          totalValue += value;
          return totalValue.ToString();
        }
        else
        {
          totalValue += value;
          return $"{totalValue}/{totalValue + 9}";
        }

      default:
        totalValue += value;
        return totalValue.ToString();
    }
  }

  private string SplitPlayerNumberValue(int value, bool firstHand)
  {
    if (firstHand)
    {
      switch (value)
      {
        case 1:
          if (FirsttotalValue > 10)
          {
            FirsttotalValue += value;
            return FirsttotalValue.ToString();
          }
          else
          {
            FirsttotalValue += value;
            return $"{FirsttotalValue}/{FirsttotalValue + 9}";
          }

        default:
          FirsttotalValue += value;
          return FirsttotalValue.ToString();
      }
    }
    else
    {
      switch (value)
      {
        case 1:
          if (SecondtotalValue > 10)
          {
            SecondtotalValue += value;
            return SecondtotalValue.ToString();
          }
          else
          {
            SecondtotalValue += value;
            return $"{SecondtotalValue}/{SecondtotalValue + 9}";
          }

        default:
          SecondtotalValue += value;
          return SecondtotalValue.ToString();
      }
    }
  }

  private string DealerNumberTestValue(int value)
  {
    switch (value)
    {
      case 1:
        if (totaldealerValue > 10)
        {
          totaldealerValue += value;
          return totaldealerValue.ToString();
        }
        else
        {
          totaldealerValue += value;
          return $"{totaldealerValue}/{totaldealerValue + 9}";
        }

      default:
        totaldealerValue += value;
        return totaldealerValue.ToString();
    }
  }
}
