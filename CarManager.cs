using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class CarManager : MonoBehaviourPun//, IPunOwnershipCallbacks
{
    

    public GameObject CarInCanvas;
    [Space]
    public GameObject playerCanvas;
    public GameObject CarCanvas;
    [Space]
    public GameObject carCamera;

    GameObject player;
    public bool iscarFree = true;

    private GameObject crosshair;
    //private CarAudio _carAudio;
    void Awake()
    {
        crosshair = GameObject.Find("CrosshairCanvas(Clone)");
    }
    private void Start()
    {
      //  _carAudio = GetComponent<CarAudio>();
       
    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnClick_CarButton();
        }
    }
   
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            if (IsCarFree())
                collisionWithPlayer(collision.gameObject);
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player" && collider.GetComponent<PhotonView>().IsMine)
        {
            CarInCanvas.SetActive(false);
        }
    }
    void collisionWithPlayer(GameObject carPlayer)
    {
        if (carPlayer.GetComponent<PhotonView>().IsMine)
            CarInCanvas.SetActive(true);
        //  else
        //    CarInCanvas.SetActive(false);

        if (carPlayer.GetComponent<PhotonView>().IsMine)
            player = carPlayer;
    }

    //when click on CarUI button...
    public void OnClick_CarButton()
    {
     
        if (IsCarFree())
        {
            //_carAudio.PlaySound();
            GetIn();
        }
        else
        {
           // _carAudio.StopSound();
            GetOut();
        }
    }
    private bool IsCarFree()
    {
        
        return iscarFree;
    }
   
   [PunRPC]
    void SetCarState(bool carFree)
    {
        iscarFree = carFree;  
    }
    //player gets in the car...
    private void GetIn()
    {
        
        base.photonView.RequestOwnership();
        player.GetPhotonView().RPC("HideShowPlayer", RpcTarget.OthersBuffered, false);
      
        if (crosshair == null)
            crosshair = GameObject.Find("CrosshairCanvas(Clone)");
        crosshair.SetActive(false);

        playerCanvas.SetActive(false);
        CarCanvas.SetActive(true);
        // changed ...

        CarInCanvas.transform.SetParent(CarCanvas.transform);
        player.transform.SetParent(this.transform);
        player.SetActive(false);
 
        photonView.RPC("SetCarState", RpcTarget.AllBufferedViaServer, false);
       
        carCamera.SetActive(true);
    }

    private void GetOut()
    {
        player.SetActive(true);
        player.GetPhotonView().RPC("HideShowPlayer", RpcTarget.OthersBuffered, true);

        crosshair.SetActive(true);
        playerCanvas.SetActive(true);
        CarCanvas.SetActive(false);
        // changed ...
        CarInCanvas.transform.parent = null;

        player.transform.parent = null;
        photonView.RPC("SetCarState", RpcTarget.AllBufferedViaServer, true);
        carCamera.SetActive(false);

    }

    // Un-Used Function
    GameObject FindPlayerWithViewId(int viewId)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
            return null;

        foreach (GameObject player in players)
        {
            if (player.name != "Car")
            { 
                if (!player.GetComponent<PhotonView>().IsMine && player.GetComponent<PhotonView>().ViewID == viewId)
                {
                    return player;
                }
            }
        }
        return null;
    }
}
