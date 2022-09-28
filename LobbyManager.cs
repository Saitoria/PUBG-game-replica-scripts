using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Realtime;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("---UI Screens---")]
    public GameObject roomUI;
    public GameObject connectUI;
    public GameObject lobbyUI;

    [Header("---UI Text---")]
    public Text statusText;
    public Text connectingText;
    public Text startBtnText;
    public Text lobbyText;
   

    [Header("---UI InputFields---")]
    public InputField createRoom;
    public InputField joinRoom;
    public InputField userName;
    public Button startButton;
   

    #region UNITY_METHODS
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
       PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    private void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;

    }
    #endregion
    public override void OnConnectedToMaster()
    {
        connectingText.text = "Joining Lobby...";
        PhotonNetwork.JoinLobby(TypedLobby.Default);  
    }
    public override void OnJoinedLobby()
    {
        connectUI.SetActive(false);
        roomUI.SetActive(true);
        userName.text = "Player" + Random.Range(100, 999);
        statusText.text = "Joined To Lobby";
    }

    public override void OnJoinedRoom()
    {

        int sizeOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        AssignTeam(sizeOfPlayers);
        roomUI.SetActive(false);
        lobbyUI.SetActive(true);

        
        foreach (Player p in PhotonNetwork.CurrentRoom.Players.Values)
        {

            GetComponent<LobbyUIManager>().AddPlayer(p.NickName);
        }
       // GetComponent<LobbyUIManager>().AddPlayer(PhotonNetwork.LocalPlayer.NickName);



        if (PhotonNetwork.IsMasterClient)
        {
            startBtnText.text = "waiting for players";
        }
        else {
            startBtnText.text = "Ready!";
        }
       // PhotonNetwork.LoadLevel(1);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        connectUI.SetActive(true);
        connectingText.text = "Disconnected... "+cause.ToString();
        roomUI.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        int roomName = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(roomName.ToString(), roomOptions, TypedLobby.Default, null);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        GetComponent<LobbyUIManager>().AddPlayer(newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        GetComponent<LobbyUIManager>().RemovePlayer(otherPlayer.NickName);
    }
    #region ButtonClicks

    public void Onclick_CreateBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(createRoom.text, roomOptions, TypedLobby.Default,null);
    }

    public void Onclick_JoinBtn()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(joinRoom.text, roomOptions, TypedLobby.Default);
    }
    public void OnClick_PlayNow()
    {
        if (string.IsNullOrEmpty(userName.text))
        {
            userName.text = "User" + Random.Range(100, 999);
        }
        PhotonNetwork.LocalPlayer.NickName = userName.text;

        PhotonNetwork.JoinRandomRoom();
        statusText.text = "Creating Room... Please Wait...";

    }
    #endregion
    
    #region My_Functions
    void AssignTeam(int sizeOfPlayer)
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        if (sizeOfPlayer % 2 == 0)
        {
            hash.Add("Team",0);
        }
        else {
            hash.Add("Team", 1);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
      
    }
    
    public void OnClickStartButton()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            SendMsg();
            startButton.interactable = false;
            startBtnText.text = "Wait...";
        }
        else {
            if (count > 0)
            {
                PhotonNetwork.LoadLevel(1);
               // lobbyText.text = "All Set : Play the Game Scene";
            }
        }
        
        
    }
    #region Raise_Events
    enum EventCodes
    {
        ready = 1
    }

    int count = 1;
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        object content = photonEvent.CustomData;
        EventCodes code = (EventCodes)eventCode;

        if (code == EventCodes.ready)
        {
            object[] datas = content as object[];

            if (PhotonNetwork.IsMasterClient)
            {
                count++;
                if (count == 4)
                    startBtnText.text = "START !";
                else
                startBtnText.text = "Only " + count + "/ 4 players are Ready";
            }
            
        }
    }
    public void SendMsg()
    {
        string message = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        object[] datas = new object[] { message };
        RaiseEventOptions options = new RaiseEventOptions
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.MasterClient,


        };
        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent((byte)EventCodes.ready, datas, options, sendOptions);
    }
    #endregion

    #endregion
}