using UnityEngine.EventSystems;
using UnityEngine;

public class FireBtnScript : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    public MyPlayer player;
    bool firing = false;
    public void SetPlayer(MyPlayer _player)
    {
        player = _player;
    }

    public void Update()
    {
        if (firing)
        {
            player.Fire();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        firing = true;
        player.Fire();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        firing = false;
        player.FireUp();
    }
}
