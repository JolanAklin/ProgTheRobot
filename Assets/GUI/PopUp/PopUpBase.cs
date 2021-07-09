using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpBase : PopUp
{
    public Animator bgAnimator;
    public Animator popUpAnimator;

    public void ClosePopUp()
    {
        bgAnimator.Play("Base Layer.bgAnimReverse");
        popUpAnimator.Play("Base Layer.PopUpClose");
        StartCoroutine("CloseAtEndAnim");
    }

    IEnumerator CloseAtEndAnim()
    {
        yield return new WaitForSecondsRealtime(popUpAnimator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(this.gameObject);
    }
}
