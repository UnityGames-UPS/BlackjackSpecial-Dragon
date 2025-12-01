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
  [SerializeField] private Transform FirstSplit_Transform;
  [SerializeField] private Transform SecondSplit_Transform;
  [SerializeField] private Transform ChipsLost_Transform;

  [Header("Lists and Arrays")]
  [SerializeField] private Transform[] CoinContainers_Transform;
  [SerializeField] private GameObject[] Coins_Prefab;
  [SerializeField] private List<GameObject> instantiated_Coins;
  [SerializeField] private List<int> instantiated_Value;
  [SerializeField] private List<GameObject> multiplyinstantiated_Coins;
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
  [SerializeField] private TMP_Text TotalBet_Text;
  [SerializeField] private TMP_Text Balance_Text;
  [SerializeField] private TMP_Text Winnings_Text;
  [SerializeField] private TMP_Text PlayerTotal_Text;
  [SerializeField] private TMP_Text DealerTotal_Text;
  [SerializeField] private TMP_Text FirstSplitTotal_Text;
  [SerializeField] private TMP_Text SecondSplitTotal_Text;

  [Header("Test Data")]
  [SerializeField] internal bool useTestData = false;
  [SerializeField] internal List<int> playerData;
  [SerializeField] internal List<int> firstplayerData;
  [SerializeField] internal List<int> secondplayerData;
  [SerializeField] internal List<int> dealerData;

  internal List<Card> playerCards = new();
  internal List<Card> dealerCards = new();

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
    if (FirstSplit_Transform) FirstSplit_Transform.gameObject.SetActive(false);
    if (SecondSplit_Transform) SecondSplit_Transform.gameObject.SetActive(false);
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
    foreach (var chip in CoinContainers_Transform)
    {
      chip.GetChild(0).GetComponent<TMP_Text>().text = socket.bets[index].ToString();
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
      uiManager.ShowMaxBetPopup("Max Main Bet Reached!");
      return false;
    }

    if (newSideBet > newMainBet)
    {
      uiManager.ShowMaxBetPopup("Side Bet Cannot Exceed Main Bet!");
      return false;
    }

    int amountToAddMain = mainBet; // need to add the SAME amount again
    AddDoubleChips(amountToAddMain, false);

    mainBet = newMainBet;

    if (MainBetText_Object && MainBetText_Text)
    {
      MainBetText_Object.SetActive(true);
      MainBetText_Text.text = mainBet.ToString();
    }

    if (multiplierBet > 0)
    {
      int amountToAddSide = multiplierBet;
      AddDoubleChips(amountToAddSide, true);

      multiplierBet = newSideBet;

      if (MultiplierBetText_Object && MultiplierBetText_Text)
      {
        MultiplierBetText_Object.SetActive(true);
        MultiplierBetText_Text.text = multiplierBet.ToString();
      }
    }

    // Update Total Bet UI
    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");

    return true;
  }

  internal void BetOnButton()
  {
    if (mainBet + amount_array[CoinCounter] > maxBetAmount)
    {
      uiManager.ShowMaxBetPopup("Max Bet Reached!");
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
    coin.transform.DOLocalMove(new Vector2(0, instantiated_Coins.Count * 2), 0.2f).OnComplete(delegate { OptimizeCoinStack(); });
    instantiated_Coins.Add(coin);
    instantiated_Value.Add(amount_array[CoinCounter]);
    mainBet += amount_array[CoinCounter];
    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
    if (MainBetText_Object && MainBetText_Text)
    {
      MainBetText_Object.SetActive(true);
      MainBetText_Text.text = mainBet.ToString();
    }
    betHistory.Add("main_" + amount_array[CoinCounter]);
  }

  internal void MultiplierBetOnButton()
  {
    if (mainBet == 0)
    {
      return;
    }
    if (multiplierBet + amount_array[CoinCounter] > mainBet)
    {
      uiManager.ShowMaxBetPopup("Max Multiplier Bet Reached!");
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
    coin.transform.DOLocalMove(new Vector2(0, multiplyinstantiated_Coins.Count * 2), 0.2f).OnComplete(delegate { MultiplyOptimizeCoinStack(); });
    multiplyinstantiated_Coins.Add(coin);
    multiplyinstantiated_Value.Add(amount_array[CoinCounter]);
    multiplierBet += amount_array[CoinCounter];
    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
    if (MultiplierBetText_Object && MultiplierBetText_Text)
    {
      MultiplierBetText_Object.SetActive(true);
      MultiplierBetText_Text.text = multiplierBet.ToString();
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
      foreach (int value in instantiated_Value)
      {
        if (value == currentValue)
        {
          currentCount++;
        }
      }

      int ratio = (currentValue > 0) ? (nextValue / currentValue) : 0;
      if (ratio > 0 && currentCount >= ratio)
      {
        int numNewCoins = currentCount / ratio;
        int numToRemove = numNewCoins * ratio;

        for (int k = 0; k < numToRemove; k++)
        {
          int indexToRemove = instantiated_Value.LastIndexOf(currentValue);
          if (indexToRemove != -1)
          {
            Destroy(instantiated_Coins[indexToRemove]);
            instantiated_Coins.RemoveAt(indexToRemove);
            instantiated_Value.RemoveAt(indexToRemove);
            betHistory.Remove("main_" + currentValue);
          }
        }

        for (int k = 0; k < numNewCoins; k++)
        {
          GameObject coin = Instantiate(Coins_Prefab[i + 1], CoinContainers_Transform[i + 1]);
          coin.transform.SetParent(ChipsParent_Transform);
          instantiated_Coins.Add(coin);
          instantiated_Value.Add(nextValue);
          betHistory.Add("main_" + nextValue);
        }

        OptimizeCoinStack();
        return;
      }
    }

    var coinValuePairs = new List<KeyValuePair<int, GameObject>>();
    for (int i = 0; i < instantiated_Value.Count; i++)
    {
      coinValuePairs.Add(new KeyValuePair<int, GameObject>(instantiated_Value[i], instantiated_Coins[i]));
    }
    coinValuePairs.Sort((pair1, pair2) => pair2.Key.CompareTo(pair1.Key));

    instantiated_Coins.Clear();
    instantiated_Value.Clear();

    foreach (var pair in coinValuePairs)
    {
      instantiated_Value.Add(pair.Key);
      instantiated_Coins.Add(pair.Value);
    }

    for (int coinIndex = 0; coinIndex < instantiated_Coins.Count; coinIndex++)
    {
      instantiated_Coins[coinIndex].transform.localPosition = new Vector2(0, coinIndex * 2.5f);
      instantiated_Coins[coinIndex].transform.SetSiblingIndex(coinIndex);
    }

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
      foreach (int value in multiplyinstantiated_Value)
      {
        if (value == currentValue)
        {
          currentCount++;
        }
      }

      int ratio = (currentValue > 0) ? (nextValue / currentValue) : 0;
      if (ratio > 0 && currentCount >= ratio)
      {
        int numNewCoins = currentCount / ratio;
        int numToRemove = numNewCoins * ratio;

        for (int k = 0; k < numToRemove; k++)
        {
          int indexToRemove = multiplyinstantiated_Value.LastIndexOf(currentValue);
          if (indexToRemove != -1)
          {
            Destroy(multiplyinstantiated_Coins[indexToRemove]);
            multiplyinstantiated_Coins.RemoveAt(indexToRemove);
            multiplyinstantiated_Value.RemoveAt(indexToRemove);
            betHistory.Remove("multiplier_" + currentValue);
          }
        }

        for (int k = 0; k < numNewCoins; k++)
        {
          GameObject coin = Instantiate(Coins_Prefab[i + 1], CoinContainers_Transform[i + 1]);
          coin.transform.SetParent(MultiplyChipsParent_Transform);
          multiplyinstantiated_Coins.Add(coin);
          multiplyinstantiated_Value.Add(nextValue);
          betHistory.Add("multiplier_" + nextValue);
        }

        MultiplyOptimizeCoinStack();
        return;
      }
    }

    var coinValuePairs = new List<KeyValuePair<int, GameObject>>();
    for (int i = 0; i < multiplyinstantiated_Value.Count; i++)
    {
      coinValuePairs.Add(new KeyValuePair<int, GameObject>(multiplyinstantiated_Value[i], multiplyinstantiated_Coins[i]));
    }
    coinValuePairs.Sort((pair1, pair2) => pair2.Key.CompareTo(pair1.Key));

    multiplyinstantiated_Coins.Clear();
    multiplyinstantiated_Value.Clear();

    foreach (var pair in coinValuePairs)
    {
      multiplyinstantiated_Value.Add(pair.Key);
      multiplyinstantiated_Coins.Add(pair.Value);
    }

    for (int coinIndex = 0; coinIndex < multiplyinstantiated_Coins.Count; coinIndex++)
    {
      multiplyinstantiated_Coins[coinIndex].transform.localPosition = new Vector2(0, coinIndex * 2);
      multiplyinstantiated_Coins[coinIndex].transform.SetSiblingIndex(coinIndex);
    }

    if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");
  }


  internal void ClearBet()
  {
    foreach (GameObject coin in instantiated_Coins)
    {
      Destroy(coin);
    }
    foreach (GameObject coin in multiplyinstantiated_Coins)
    {
      Destroy(coin);
    }
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
    {
      cardTransforms.Add(PlayerContainer_Transform.GetChild(i));
    }

    for (int i = 0; i < DealerContainer_Transform.childCount; i++)
    {
      cardTransforms.Add(DealerContainer_Transform.GetChild(i));
      if (DealerContainer_Transform.GetChild(i).GetComponent<Image>().sprite == card_Back)
      {
        dealerFlippedIndex = cardTransforms.LastIndexOf(DealerContainer_Transform.GetChild(i));
      }
    }

    for (int i = 0; i < FirstSplit_Transform.childCount; i++)
    {
      cardTransforms.Add(FirstSplit_Transform.GetChild(i));
    }

    for (int i = 0; i < SecondSplit_Transform.childCount; i++)
    {
      cardTransforms.Add(SecondSplit_Transform.GetChild(i));
    }

    List<Tween> tweens = new();
    int index = 0;
    foreach (var cards in cardTransforms)
    {
      CardScript script = cards.GetComponent<CardScript>();
      if (index != dealerFlippedIndex)
        script.OnFlipMethod(card_Back, 0, null);

      cards.DOScale(0.7f, 0.3f).SetDelay(0.5f);
      cards.GetComponent<Image>().DOFade(0, 0.3f).SetDelay(0.5f);
      cards.DORotate(EndDeck_Transform.eulerAngles, 0.3f).SetDelay(0.5f);
      Tween t = cards.DOMove(EndDeck_Transform.position, 0.3f).SetDelay(0.5f).OnStart(() =>
      {
        script.layoutElement.ignoreLayout = true;
      })
      .OnComplete(() =>
      {
        Destroy(cards.gameObject);
      });
      tweens.Add(t);
      index++;
    }

    if (tweens.Count > 0)
    {
      yield return tweens[^1].WaitForCompletion();
    }
    yield return new WaitForSeconds(0.5f);

    playerCards.Clear();
    dealerCards.Clear();
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
    // if (FirstSplit_Transform) FirstSplit_Transform.gameObject.SetActive(false);
    // FirstSplit_Transform.GetChild(0).gameObject.SetActive(true);
    // FirstSplit_Transform.GetChild(1).gameObject.SetActive(true);
    // if (SecondSplit_Transform) SecondSplit_Transform.gameObject.SetActive(false);
    // SecondSplit_Transform.GetChild(0).gameObject.SetActive(true);
    // SecondSplit_Transform.GetChild(1).gameObject.SetActive(true);
    // if (PlayerContainer_Transform) PlayerContainer_Transform.gameObject.SetActive(false);
    // PlayerContainer_Transform.GetChild(0).gameObject.SetActive(false);
    // PlayerContainer_Transform.GetChild(1).gameObject.SetActive(false);

    isSplit = false;
    isFirstSplit = false;
  }

  internal void SplitButton()
  {
    if (FirstSplit_Transform) FirstSplit_Transform.gameObject.SetActive(true);
    if (SecondSplit_Transform) SecondSplit_Transform.gameObject.SetActive(true);
    GameObject tempCard = PlayerContainer_Transform.GetChild(2).gameObject;
    tempCard.transform.SetParent(FirstSplit_Transform);
    tempCard.transform.DOLocalMove(new Vector2(0, 0), 0.3f);
    tempCard.transform.DOScale(Vector3.one, 0.3f);
    tempCard = PlayerContainer_Transform.GetChild(2).gameObject;
    tempCard.transform.SetParent(SecondSplit_Transform);
    tempCard.transform.DOLocalMove(new Vector2(0, 0), 0.3f);
    tempCard.transform.DOScale(Vector3.one, 0.3f);
    if (FirstSplitTotal_Text) FirstSplitTotal_Text.text = SplitPlayerNumberValue(firstplayerData[FirstSplitplayerCounter], true);
    string temp1 = SplitPlayerNumberValue(FirstSplitplayerCounter, true);
    Debug.Log("my valus is " + temp1);
    if (SecondSplitTotal_Text) SecondSplitTotal_Text.text = SplitPlayerNumberValue(secondplayerData[SecondSplitplayerCounter], false);
    FirstSplitplayerCounter++;
    SecondSplitplayerCounter++;
    // if (PlayerContainer_Transform) PlayerContainer_Transform.gameObject.SetActive(false);
    isSplit = true;
    isFirstSplit = true;
  }

  internal void UndoBetButton()
  {
    if (betHistory.Count > 0)
    {
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
            OptimizeCoinStack(); // Re-stack
          }
        }
        if (mainBet > 0)
        {
          if (MainBetText_Object && MainBetText_Text)
          {
            MainBetText_Text.text = mainBet.ToString();
          }
        }
        else if (mainBet == 0)
        {
          if (MainBetText_Object && MainBetText_Text)
          {
            MainBetText_Object.SetActive(false);
            MainBetText_Text.text = "0";
          }
        }
        else
        {
          Debug.LogError("Main bet went negative!");
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
            MultiplyOptimizeCoinStack(); // Re-stack
          }
        }
        if (multiplierBet > 0)
        {
          if (MultiplierBetText_Object && MultiplierBetText_Text)
          {
            MultiplierBetText_Text.text = multiplierBet.ToString();
          }
        }
        else if (multiplierBet == 0)
        {
          if (MultiplierBetText_Object && MultiplierBetText_Text)
          {
            MultiplierBetText_Object.SetActive(false);
            MultiplierBetText_Text.text = "0";
          }
        }
        else
        {
          Debug.LogError("Multiplier bet went negative!");
        }
      }
      if (TotalBet_Text) TotalBet_Text.text = (mainBet + multiplierBet).ToString("N2");

    }
  }


  internal void OnDealerButton(Card cardData = null)
  {
    GameObject card = Instantiate(Cards_Prefab, Deck_Transform);
    card.transform.localPosition = Vector3.zero;
    card.transform.localScale -= card.transform.localScale * 0.2f;
    card.transform.SetParent(DealerContainer_Transform);

    Sprite S = null;
    if (cardData == null)
      S = SelectRandomArray(playerData[playerCounter]);
    else
      S = SelectSprite(cardData);

    card.transform.DOScale(Vector3.one, 0.3f);
    card.transform.DOLocalRotate(Vector3.zero, 0.3f);
    card.transform.DOLocalMove(Vector3.zero, 0.3f).OnComplete(delegate
    {
      card.GetComponent<CardScript>().OnFlipMethod(S, 2, cardData);
    });
  }

  internal void OnDealerButtonClosedCard()
  {
    GameObject card = Instantiate(Cards_Prefab, Deck_Transform);
    card.transform.localPosition = Vector3.zero;
    card.transform.localScale -= card.transform.localScale * 0.2f;
    card.transform.SetParent(DealerContainer_Transform);
    card.transform.DOScale(Vector3.one, 0.3f);
    card.transform.DOLocalRotate(Vector3.zero, 0.3f);
    card.transform.DOLocalMove(Vector3.zero, 0.3f).OnComplete(delegate
    {
      CardScript cardScript = card.GetComponent<CardScript>();
      tempdealer = cardScript;
      cardScript.layoutElement.ignoreLayout = false;
      isFlippin = false;
    });
  }

  internal bool CheckMultiplier()
  {
    if (multiplyinstantiated_Value.Count > 0)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  internal void OnDealerOpenFlipped(Card CardData)
  {
    Sprite S = null;
    if (useTestData)
    {
      S = SelectRandomArray(dealerData[dealerCounter]);
    }
    else
    {
      S = SelectSprite(CardData);
    }
    tempdealer.OnFlipMethod(S, 2, CardData);
  }

  internal void OnPlayerDealButton(Card CardData)
  {
    GameObject card = Instantiate(Cards_Prefab, Deck_Transform);
    card.transform.localPosition = Vector3.zero;
    card.transform.localScale -= card.transform.localScale * 0.2f;

    card.transform.SetParent(PlayerContainer_Transform);

    Sprite S = null;
    if (useTestData)
      S = SelectRandomArray(playerData[playerCounter]);
    else
      S = SelectSprite(CardData);

    card.transform.DOScale(Vector3.one, 0.3f);
    card.transform.DOLocalRotate(Vector3.zero, 0.3f);
    card.transform.DOLocalMove(Vector3.zero, 0.3f).OnComplete(delegate
    {
      card.GetComponent<CardScript>().OnFlipMethod(S, 1, CardData);
    });
  }

  internal void SplitStandButton()
  {
    isFirstSplit = false;
  }

  internal void OnSplitDealButton()
  {
    if (isFirstSplit)
    {
      GameObject card = Instantiate(Cards_Prefab, Deck_Transform);
      card.transform.localPosition = Vector2.zero;
      card.transform.SetParent(FirstSplit_Transform);
      Sprite tempArr = SelectRandomArray(firstplayerData[FirstSplitplayerCounter]);
      card.transform.DOLocalMove(new Vector2(0, 0), 0.3f).OnComplete(delegate
      {
        card.GetComponent<CardScript>().OnFlipMethod(tempArr, 3);
      });
      card.transform.DOScale(Vector3.one, 0.3f);
    }
    else
    {
      GameObject card = Instantiate(Cards_Prefab, Deck_Transform);
      card.transform.localPosition = Vector2.zero;
      card.transform.SetParent(SecondSplit_Transform);
      Sprite tempArr = SelectRandomArray(secondplayerData[SecondSplitplayerCounter]);
      card.transform.DOLocalMove(new Vector2(0, 0), 0.3f).OnComplete(delegate
      {
        card.GetComponent<CardScript>().OnFlipMethod(tempArr, 4);
      });
      card.transform.DOScale(Vector3.one, 0.3f);
    }
  }

  internal void AfterCardFlip(int value, Card card)
  {
    switch (value)
    {
      case 1:
        if (useTestData)
        {
          if (PlayerTotal_Text) PlayerTotal_Text.text = PlayerNumberTestValue(playerData[playerCounter]);
        }
        else
        {
          if (card != null) playerCards.Add(card);
          if (PlayerTotal_Text) PlayerTotal_Text.text = CalculateHandValue(playerCards);
        }
        playerCounter++;
        break;
      case 2:
        if (useTestData)
        {
          if (DealerTotal_Text) DealerTotal_Text.text = DealerNumberTestValue(dealerData[dealerCounter]);
        }
        else
        {
          if (card != null) dealerCards.Add(card);
          if (DealerTotal_Text) DealerTotal_Text.text = CalculateHandValue(dealerCards);
        }
        dealerCounter++;
        break;
      case 3:
        if (FirstSplitTotal_Text) FirstSplitTotal_Text.text = SplitPlayerNumberValue(firstplayerData[FirstSplitplayerCounter], true);
        FirstSplitplayerCounter++;
        break;
      case 4:
        if (SecondSplitTotal_Text) SecondSplitTotal_Text.text = SplitPlayerNumberValue(secondplayerData[SecondSplitplayerCounter], false);
        SecondSplitplayerCounter++;
        break;
    }
    isFlippin = false;
  }

  private string CalculateHandValue(List<Card> hand)
  {
    int totalValue = 0;
    int aceCount = 0;

    foreach (var card in hand)
    {
      string cardRank = card.rank.ToUpper();
      int value = 0;
      switch (cardRank)
      {
        case "J":
        case "Q":
        case "K":
          value = 10;
          break;
        case "A":
          value = 11;
          aceCount++;
          break;
        default:
          value = int.Parse(cardRank);
          break;
      }
      totalValue += value;
    }

    while (totalValue > 21 && aceCount > 0)
    {
      totalValue -= 10;
      aceCount--;
    }

    if (hand.Count == 2 && totalValue == 21)
    {
      return "21";
    }

    bool hasAce = false;
    foreach (var card in hand)
    {
      if (card.rank.ToUpper() == "A")
      {
        hasAce = true;
        break;
      }
    }

    if (hasAce)
    {
      int valueWithAceAsOne = 0;
      foreach (var card in hand)
      {
        string cardRank = card.rank.ToUpper();
        int value = 0;
        if (cardRank == "A")
        {
          value = 1;
        }
        else if (cardRank == "J" || cardRank == "Q" || cardRank == "K")
        {
          value = 10;
        }
        else
        {
          value = int.Parse(cardRank);
        }
        valueWithAceAsOne += value;
      }

      if (totalValue != valueWithAceAsOne && totalValue <= 21)
      {
        return $"{valueWithAceAsOne} / {totalValue}";
      }
    }

    return totalValue.ToString();
  }

  internal bool LowBalCheck(int value = 0)
  {
    if (socket.PlayerData.balance < mainBet + multiplierBet + value)
    {
      uiManager.ShowMaxBetPopup("Insufficient Balance!");
      return true;
    }
    return false;
  }

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
          string specialval = totalValue.ToString() + "/" + (totalValue + 9).ToString();
          return specialval.ToString();
        }
      default:
        totalValue += value;
        return totalValue.ToString();
    }
  }

  private string SplitPlayerNumberValue(int value, bool isFirst)
  {
    if (isFirst)
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
            string specialval = FirsttotalValue.ToString() + "/" + (FirsttotalValue + 9).ToString();
            return specialval.ToString();
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
            string specialval = SecondtotalValue.ToString() + "/" + (SecondtotalValue + 9).ToString();
            return specialval.ToString();
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
          string specialval = totaldealerValue.ToString() + "/" + (totaldealerValue + 9).ToString();
          return specialval.ToString();
        }
      default:
        totaldealerValue += value;
        return totaldealerValue.ToString();
    }
  }

  private Sprite SelectRandomArray(int value)
  {
    int randomIndex = Random.Range(0, 4); // Generates a random number between 0 and 3
    Sprite[] temparr = null;
    switch (randomIndex)
    {
      case 0:
        temparr = clubs_Sprite;
        break;
      case 1:
        temparr = spades_Sprite;
        break;
      case 2:
        temparr = hearts_Sprite;
        break;
      case 3:
        temparr = diamonds_Sprite;
        break;
    }

    int randomNumber = Random.Range(9, 12);
    switch (value)
    {
      case 10:
        return temparr[randomNumber];
      default:
        return temparr[value - 1];
    }
  }

  private Sprite SelectSprite(Card cardData)
  {
    Sprite[] SuitsArr = null;
    string suit = cardData.suit.ToLower();

    switch (suit)
    {
      case "clubs":
        SuitsArr = clubs_Sprite;
        break;
      case "spades":
        SuitsArr = spades_Sprite;
        break;
      case "hearts":
        SuitsArr = hearts_Sprite;
        break;
      case "diamonds":
        SuitsArr = diamonds_Sprite;
        break;
    }

    if (SuitsArr == null)
    {
      Debug.LogError("Suit array is null for suit: " + suit);
      return card_Back; // Return a default sprite to avoid crashes
    }

    string cardValue = cardData.rank.ToUpper();

    switch (cardValue)
    {
      case "J":
        return SuitsArr[10];
      case "Q":
        { }
        return SuitsArr[11];
      case "K":
        return SuitsArr[12];
      case "A":
        return SuitsArr[0];
      default:
        int val = int.Parse(cardValue);
        return SuitsArr[val - 1];
    }
  }

  internal void LostChipsAnimation()
  {
    foreach (GameObject coin in instantiated_Coins)
    {
      Vector3 initPos = coin.transform.position;
      coin.transform.DOMove(ChipsLost_Transform.position, 0.3f).OnComplete(() =>
      {
        coin.SetActive(false);
        coin.transform.position = initPos;
      });
    }

    foreach (GameObject coin in multiplyinstantiated_Coins)
    {
      Vector3 initPos = coin.transform.position;
      coin.transform.DOMove(ChipsLost_Transform.position, 0.3f).OnComplete(() =>
      {
        coin.SetActive(false);
        coin.transform.position = initPos;
      });
    }

    if (MainBetText_Object.activeSelf)
    {
      MainBetText_Object.SetActive(false);
    }
    if(MultiplierBetText_Object.activeSelf)
    {
      MultiplierBetText_Object.SetActive(false);
    }
  }

  internal void PlayerBust()
  {
    LostChipsAnimation();
    string total = PlayerTotal_Text.text;
    PlayerTotal_Text.text = "BUST " + total;
  }

  internal void DealerBust()
  {
    string total = DealerTotal_Text.text;
    DealerTotal_Text.text = "BUST " + total;
  }

  internal void SetDealerValue(int value)
  {
    DealerTotal_Text.text = value.ToString();
  }

  internal void PlayerPush()
  {
    string total = PlayerTotal_Text.text;
    PlayerTotal_Text.text = "PUSH " + total;
  }

  private void AddDoubleChips(int amountToAdd, bool isSide)
  {
    int[] chipValues = socket.bets.ToArray();

    // Largest → smallest chip
    for (int i = chipValues.Length - 1; i >= 0; i--)
    {
      while (amountToAdd >= chipValues[i])
      {
        AddChipInstance(chipValues[i], i, isSide);
        amountToAdd -= chipValues[i];
      }
    }
  }

  private void AddChipInstance(int value, int prefabIndex, bool isSide)
  {
    Transform targetParent = isSide ? MultiplyChipsParent_Transform : ChipsParent_Transform;

    GameObject coin = Instantiate(Coins_Prefab[prefabIndex], CoinContainers_Transform[prefabIndex]);
    coin.transform.localPosition = Vector2.zero;
    coin.transform.SetParent(targetParent);
    coin.transform.localScale = Vector3.one;

    List<GameObject> targetList = isSide ? multiplyinstantiated_Coins : instantiated_Coins;
    List<int> valueList = isSide ? multiplyinstantiated_Value : instantiated_Value;

    coin.transform.DOLocalMove(new Vector2(0, targetList.Count * 2f), 0.2f)
        .OnComplete(() =>
        {
          if (isSide)
            MultiplyOptimizeCoinStack();
          else
            OptimizeCoinStack();
        });

    targetList.Add(coin);
    valueList.Add(value);

    betHistory.Add((isSide ? "multiplier_" : "main_") + value);
  }
}
