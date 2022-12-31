using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SRP_weakButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<SRP_ShakingEffect>().CONTROL_Shake_MoreShake();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<SRP_ShakingEffect>().CONTROL_Shake_LessShake();
    }

    public void CONTROL_RemoveButton()
    {
        Destroy(gameObject);
    }
}
