using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;
using Best.SocketIO;
using Best.SocketIO.Events;

public class SocketIOManager : MonoBehaviour
{
  [SerializeField] private BJController bJController;
  [SerializeField] private UIManager UiManager;
  [SerializeField] internal JSFunctCalls JSManager;
  [SerializeField] internal string TestSocketURI = "http://localhost:5000/";
  [SerializeField] private string TestToken;
  [SerializeField] private GameObject RaycastBlocker;
  internal Root Data = new();
  internal Player PlayerData = new();
  internal List<int> bets = new();
  internal Root ResultData = new();
  internal bool IsResultDone = false;

  internal string SocketURI = null;
  protected string NameSpace = "playground";
  protected string myAuth;
  protected string nameSpace;

  private SocketManager Manager;
  private Socket GameSocket;
  private const int maxReconnectionAttempts = 6;
  private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(10);

  private bool hasEverConnected = false;
  private const int MaxReconnectAttempts = 5;
  private const float ReconnectDelaySeconds = 2f;

  private float lastPongTime = 0f;
  private float pingInterval = 2f;
  private bool waitingForPong = false;
  private int missedPongs = 0;
  private const int MaxMissedPongs = 5;
  private Coroutine PingRoutine; //Back2 end

  void Awake()
  {
    Application.runInBackground = true;
  }

  private void Start()
  {
    OpenSocket();
  }

  private IEnumerator WaitForAuthToken(SocketOptions options)
  {
    // Wait until myAuth is not null
    while (myAuth == null)
    {
      Debug.Log("My Auth is null");
      yield return null;
    }
    while (SocketURI == null)
    {
      Debug.Log("My Socket is null");
      yield return null;
    }

    Debug.Log("My Auth is not null");
    // Once myAuth is set, configure the authFunction
    Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
    {
      return new
      {
        token = myAuth,
      };
    };
    options.Auth = authFunction;

    Debug.Log("Auth function configured with token: " + myAuth);

    // Proceed with connecting to the server
    SetupSocketManager(options);
  }
  void ReceiveAuthToken(string jsonData)
  {
    Debug.Log("Received data: " + jsonData);

    // Parse the JSON data
    var data = JsonUtility.FromJson<AuthTokenData>(jsonData);
    SocketURI = data.socketURL;
    myAuth = data.cookie;
    nameSpace = data.nameSpace;
  }

  private void OpenSocket()
  {
    SocketOptions options = new SocketOptions();
    options.AutoConnect = false;
    options.Reconnection = false;
    options.Timeout = TimeSpan.FromSeconds(3);
    options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket;

#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("authToken");
    StartCoroutine(WaitForAuthToken(options));
#else
    Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
    {
      return new
      {
        token = TestToken
      };
    };
    options.Auth = authFunction;
    SetupSocketManager(options);
#endif
  }

  private void SetupSocketManager(SocketOptions options)
  {
    // Create and setup SocketManager
#if UNITY_EDITOR
    this.Manager = new SocketManager(new Uri(TestSocketURI), options);
#else
    this.Manager = new SocketManager(new Uri(SocketURI), options);
#endif

    if (string.IsNullOrEmpty(NameSpace) | string.IsNullOrWhiteSpace(NameSpace))
    {
      GameSocket = this.Manager.Socket;
    }
    else
    {
      Debug.Log("Namespace used :" + NameSpace);
      GameSocket = this.Manager.GetSocket("/" + NameSpace);
    }
    // Set subscriptions
    GameSocket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
    GameSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected);
    GameSocket.On<Error>(SocketIOEventTypes.Error, OnError);
    GameSocket.On<string>("game:init", OnListenEvent);
    GameSocket.On<string>("result", OnListenEvent);
    GameSocket.On<string>("pong", OnPongReceived);

    Manager.Open();
  }

  // Connected event handler implementation
  void OnConnected(ConnectResponse resp)
  {
    Debug.Log("✅ Connected to server.");

    if (hasEverConnected)
    {
      // UiManager.CheckAndClosePopups();
    }

    hasEverConnected = true;
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    SendPing();
  } //Back2 end

  private void OnDisconnected() //Back2 Start
  {
    Debug.LogWarning("⚠️ Disconnected from server.");
    ResetPingRoutine();
    // UiManager.DisconnectionPopup();
  } //Back2 end

  private void OnPongReceived(string data) //Back2 Start
  {
    // Debug.Log("✅ Received pong from server.");
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    // Debug.Log($"⏱️ Updated last pong time: {lastPongTime}");
    // Debug.Log($"📦 Pong payload: {data}");
  }

  private void OnError(Error err)
  {
    Debug.LogError("Socket Error Message: " + err);
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("error");
#endif
  }

  private void OnListenEvent(string data)
  {
    ParseResponse(data);
  }

  private void SendPing() //Back2 Start
  {
    ResetPingRoutine();
    PingRoutine = StartCoroutine(PingCheck());
  }

  void ResetPingRoutine()
  {
    if (PingRoutine != null)
    {
      StopCoroutine(PingRoutine);
    }
    PingRoutine = null;
  }

  private IEnumerator PingCheck()
  {
    while (true)
    {
      // Debug.Log($"🟡 PingCheck | waitingForPong: {waitingForPong}, missedPongs: {missedPongs}, timeSinceLastPong: {Time.time - lastPongTime}");

      if (missedPongs == 0)
      {
        UiManager.CheckAndClosePopup();
      }

      // If waiting for pong, and timeout passed
      if (waitingForPong)
      {
        if (missedPongs == 2)
        {
          UiManager.ReconnectionPopup();
        }
        missedPongs++;
        Debug.LogWarning($"⚠️ Pong missed #{missedPongs}/{MaxMissedPongs}");

        if (missedPongs >= MaxMissedPongs)
        {
          Debug.LogError("❌ Unable to connect to server — 5 consecutive pongs missed.");
          UiManager.DisconnectionPopup();
          yield break;
        }
      }

      // Send next ping
      waitingForPong = true;
      lastPongTime = Time.time;
      // Debug.Log("📤 Sending ping...");
      SendDataWithNamespace("ping");
      yield return new WaitForSeconds(pingInterval);
    }
  } //Back2 end

  private void SendDataWithNamespace(string eventName, string json = null)
  {
    // Send the message
    if (GameSocket != null && GameSocket.IsOpen)
    {
      if (json != null)
      {
        GameSocket.Emit(eventName, json);
        Debug.Log("JSON data sent: " + eventName + " :" + json);
      }
      else
      {
        GameSocket.Emit(eventName);
        // Debug.Log("Event sent: " + eventName);
      }
    }
    else
    {
      Debug.LogWarning("Socket is not connected.");
    }
  }

  internal void CloseGame()
  {
    Debug.Log("Unity: Closing Game");
    StartCoroutine(CloseSocket());
  }

  internal IEnumerator CloseSocket() //Back2 Start
  {
    RaycastBlocker.SetActive(true);
    ResetPingRoutine();

    Debug.Log("Closing Socket");

    Manager?.Close();
    Manager = null;

    Debug.Log("Waiting for socket to close");

    yield return new WaitForSeconds(0.5f);

    Debug.Log("Socket Closed");

#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnExit"); //Telling the react platform user wants to quit and go back to homepage
#endif
  } //Back2 end

  private void ParseResponse(string jsonObject)
  {
    Debug.Log(jsonObject);
    Data = JsonConvert.DeserializeObject<Root>(jsonObject);

    // if (!Data.success && Data.id.ToLower() != "initdata")
    // {
    //   ResultData = Data;
    //   IsResultDone = true;
    //   return;
    // }

    string id = Data.id;
    PlayerData = Data.player;
    switch (id.ToLower())
    {
      case "initdata":
        {
          bets = Data.gameData.bets;
          HandleInit();
          break;
        }
      default:
        {
          ResultData = Data;
          IsResultDone = true;
          break;
        }
    }
  }

  private void HandleInit()
  {
    bJController.HandleInit();
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnEnter");
#endif
    RaycastBlocker.SetActive(false);
  }

  internal void RequestEvent(string eventName)
  {
    IsResultDone = false;
    RequestClass message = new RequestClass
    {
      type = eventName,
      payload = new ReqPayload
      {
        mainBet = bJController.mainBet,
        sideBet = bJController.multiplierBet,
        accept = true
      }
    };

    string json = JsonUtility.ToJson(message);
    SendDataWithNamespace("request", json);
  }
}

[Serializable]
public class RequestClass
{
  public string type;
  public ReqPayload payload;
}

[Serializable]
public class ReqPayload
{
  public int mainBet;
  public int sideBet;
  public bool accept = true;
}

[Serializable]
public class Player
{
  public double balance;
}

[Serializable]
public class GameData
{
  public List<int> bets;
  public int deckCount;
  public Paytable paytable;
  public int historyLimit;
}

[Serializable]
public class Paytable
{
  public int winningHand;
  public int insurance;
  public double blackJack;
  public int push;
}

[Serializable]
public class Payload
{
  public List<Hand> hands;
  public List<Hand> playerHands;
  public Hand dealerHand;
  public Card dealerUpCard;
  public Card card;
  public int currentHandIndex;
  public bool isAceSplit;
  public int handIndex;
  public int handValue;
  public bool isSoft;
  public bool isBust;
  public bool canSplit;
  public bool canDouble;
  public bool canInsure;
  public int sideBetMultiplier;
  public string gamePhase;
  public List<HandResult> handResults;
  public double handBet;
  public double totalWin;
  public double sideBetWin;
  public double sideBet;
  public double insuranceWin;
  public double insuranceBet;
}

[Serializable]
public class Hand
{
  public List<Card> cards;
  public int value;
  public bool isSoft;
  public bool isBlackjack;
  public bool isBust;
  public double bet;
}

[Serializable]
public class Card
{
  public string rank;
  public string suit;
}


[Serializable]
public class Root
{
  public bool success;
  public string id = "";
  public GameData gameData;
  public Payload payload;
  public Player player;
}

[Serializable]
public class HandResult
{
  public string result;
  public double payout;
  public int handValue;
}

[Serializable]
public class AuthTokenData
{
  public string cookie;
  public string socketURL;
  public string nameSpace;
}
