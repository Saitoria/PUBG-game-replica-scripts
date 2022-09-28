using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatButton : MonoBehaviour
{
    public GameObject chatMessages;

    public void OnClick()
    {
        if (chatMessages.activeSelf)
        {
            chatMessages.SetActive(false);
        }
        else {
            chatMessages.SetActive(true);
        }
    }
}
