using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public Camera sceneCam;
    public GameObject player;
    public Transform playerSpawnPosition;
    public Text pingrateText;
    [HideInInspector]
    public GameObject localPlayer;

    public GameObject deathScreen;
    public Text totalAlive;
    //...
    public GameObject spectateContainer;
    public GameObject spectateObject;

    void Start()
    {
        PhotonNetwork.SendRate = 25; //20
        PhotonNetwork.SerializationRate = 15;//10
        sceneCam.enabled = false;
        localPlayer =  PhotonNetwork.Instantiate(player.name, playerSpawnPosition.position, playerSpawnPosition.rotation);
        
    }

    private void Update()
    {
        //pingrateText.text = PhotonNetwork.GetPing().ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LoadLevel(0);
        }
    }
    //will be called from Myplayer.cs Death function
    public void Spectate()
    {
        sceneCam.enabled = true;//new
        deathScreen.SetActive(true);
        FindAllPlayers();
    }
    void FindAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.name.Contains("Car"))
                continue;
            if (!player.GetComponent<MyPlayer>().isDead)
            {
               GameObject so =  Instantiate(spectateObject, spectateContainer.transform);
                so.transform.Find("PlayerName").GetComponent<Text>().text = player.GetPhotonView().Owner.NickName;
                //new...
                so.transform.Find("SpectateButton").GetComponent<SpectateButtonClick>().target = player;
            }
        }
    }
   
}
