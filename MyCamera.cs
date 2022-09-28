using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MyCamera : MonoBehaviour
{
    public PhotonView playerPhotonView;
    public float Yaxis;
    public float Xaxis;
    public float RotationSensitivity = 8f;
    private Transform target;
    public float offset;

    float RotationMin = -40f;
    float RotationMax = 80f;
    float smoothTime = 0.12f;

    Vector3 targetRotation;
    Vector3 currentVel;
    public bool enableMobileInputs = false;
    private FixedTouchField touchField;

    private void Awake()
    {
        if (playerPhotonView.IsMine)
        {
            transform.parent = null;
            touchField = GameObject.Find("TouchPanel").GetComponent<FixedTouchField>();
            target = GetLocalPlayer().transform.GetChild(3);
        }
        else {
           // transform.GetComponentInParent<MyPlayer>().playerCam = this.gameObject;

            this.gameObject.SetActive(false);
        }
       

    }
    private void Start()
    {
        if (enableMobileInputs)
            RotationSensitivity = 0.2f;
    }
    void LateUpdate()
    {
        if (!playerPhotonView.IsMine)
            return;

        if (enableMobileInputs)
        {
            Yaxis += touchField.TouchDist.x * RotationSensitivity;
            Xaxis -= touchField.TouchDist.y * RotationSensitivity;
        }
        else { 

         Yaxis += Input.GetAxis("Mouse X")* RotationSensitivity;
         Xaxis -= Input.GetAxis("Mouse Y")* RotationSensitivity;
        }
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        transform.eulerAngles = targetRotation;

        Vector3 _offset = target.position - transform.forward * offset;
        _offset.y = 1.22f;
        transform.position = _offset;


    }

    GameObject GetLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().IsMine && player.name != "Car")
            {
                return player;
            }
        }
        return null;
    }
}
