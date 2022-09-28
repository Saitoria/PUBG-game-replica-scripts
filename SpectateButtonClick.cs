using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpectateButtonClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject target;
    GameObject spectateCamera;

    public void OnPointerClick(PointerEventData eventData)
    {
        spectateCamera = GameObject.Find("SpectateCamera");
        spectateCamera.GetComponent<Camera>().enabled = true;
        SmoothFollow smoothfollow = spectateCamera.GetComponent<SmoothFollow>();
        smoothfollow.target = target.transform;
        smoothfollow.enabled = true;
    }
}
