using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeInfoBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InfoBar.infoObject.InfoType infoType;
    public void OnPointerEnter(PointerEventData eventData)
    {
        InfoBar.instance.ChangeInfos(infoType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(infoType == InfoBar.instance.currentInfo)
            InfoBar.instance.ChangeInfos(InfoBar.infoObject.InfoType.none);
    }
}
