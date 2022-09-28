using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LobbyUIManager : MonoBehaviour
{
    public GameObject playersContainer;
    public GameObject playerObjectPrefab;


    public void AddPlayer(string playerName)
    {
       GameObject pop =  Instantiate(playerObjectPrefab, playerObjectPrefab.transform.position, playerObjectPrefab.transform.rotation);
        pop.transform.GetChild(0).GetComponent<Text>().text = playerName;
        pop.transform.parent = playersContainer.transform;
        pop.transform.localScale = Vector3.one;
        pop.name = playerName;
    }

    public void RemovePlayer(string playerName)
    {
        int popCount = playersContainer.transform.childCount;
        for (int i = 0; i < popCount; i++)
        {
            if (playersContainer.transform.GetChild(i).name == playerName)
            {
                Destroy(playersContainer.transform.GetChild(i).gameObject);
                return;
            }
        }
    }
}
