using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopUp : MonoBehaviour
{
    public void Close()
    {
        transform.root.GetComponent<PopUpBase>().ClosePopUp();
    }
}
