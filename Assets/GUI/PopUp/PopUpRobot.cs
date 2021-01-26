using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class PopUpRobot : MonoBehaviour
{
    public string robotName;
    public float power;
    public Color robotColor;
    public Image colorButton;
    public GameObject boxVisual;

    // delegate while be triggered when a button is pressed
    private Action deleteAction;
    private Action OkAction;
    private Action cancelAction;

    public void OnEndEditName(TMP_InputField inputField)
    {
        robotName = inputField.text;
    }
    public void OnEndEditPower(TMP_InputField inputField)
    {
        float.TryParse(inputField.text, out power);
    }
    public void ChangeColor()
    {
        boxVisual.SetActive(false);
        PopUpColor cp = Instantiate(WindowsManager.instance.popUpWindowsDict[(int)Enum.Parse(typeof(WindowsManager.popUp), "colorPicker")], Manager.instance.canvas.transform).GetComponent<PopUpColor>();
        cp.SetButtonOk(() => {
            robotColor = cp.color;
            Destroy(cp.gameObject);
            colorButton.color = robotColor;
            boxVisual.SetActive(true);
        });
        cp.SetButtonCancel(() => {
            Destroy(cp.gameObject);
            boxVisual.SetActive(true);
        });
    }

    #region buttons action
    public void SetDeleteAction(Action action)
    {
        deleteAction = action;
    }
    public void SetOkAction(Action action)
    {
        OkAction = action;
    }
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }

    // method called by the button
    public void Delete()
    {
        deleteAction();
    }
    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        OkAction();
    }
    #endregion
}
