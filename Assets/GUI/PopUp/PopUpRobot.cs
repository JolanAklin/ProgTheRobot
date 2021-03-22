using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class PopUpRobot : MonoBehaviour
{
    [HideInInspector]
    public string robotName;
    [HideInInspector]
    public uint power;
    [HideInInspector]
    public Color robotColor;
    public Image colorButton;
    public GameObject boxVisual;
    public Image robotBodyColor;

    public TMP_InputField nameInput;
    public TMP_InputField powerInput;

    // delegate while be triggered when a button is pressed
    private Action deleteAction;
    private Action OkAction;
    private Action cancelAction;

    public void Init(Color color, string name, uint power)
    {
        robotColor = color;
        robotName = name;
        this.power = power;
        colorButton.color = color;
        nameInput.text = name;
        powerInput.text = power.ToString();
        robotBodyColor.color = color;
    }
    public void OnEndEditName(TMP_InputField inputField)
    {
        Debug.Log(inputField.text);
        robotName = inputField.text;
    }
    public void OnEndEditPower(TMP_InputField inputField)
    {
        uint.TryParse(inputField.text, out power);
    }
    public void ChangeColor()
    {
        boxVisual.SetActive(false);
        PopUpColor cp = Instantiate(WindowsManager.instance.popUpWindowsDict[(int)Enum.Parse(typeof(WindowsManager.popUp), "colorPicker")], Manager.instance.canvas.transform).GetComponent<PopUpColor>();
        cp.Init(robotColor);
        cp.SetButtonOk(() => {
            robotColor = cp.color;
            robotBodyColor.color = cp.color;
            Destroy(cp.gameObject);
            colorButton.color = robotColor;
            boxVisual.SetActive(true);
        });
        cp.SetButtonCancel(() => {
            Destroy(cp.gameObject);
            boxVisual.SetActive(true);
        });
    }

    public void PopUpClose()
    {
        Destroy(this.gameObject);
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
        if(robotName.Length == 0)
        {
            // hide this window and create another to warn the user that the robot need a name
            gameObject.SetActive(false);
            PopUpWarning sw = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "saveWarning"), Manager.instance.canvas.transform).GetComponent<PopUpWarning>();
            sw.warningText.text = "Veuillez entrer un nom pour le robot";
            sw.quitButton.gameObject.SetActive(false);
            sw.saveButton.gameObject.SetActive(false);
            sw.SetCancelAction(() =>
            {
                sw.Close();
                gameObject.SetActive(true);
            });
            nameInput.Select();
            return;
        }
        if(power <= 0)
        {
            // hide this window and create another to warn the user that the robot need a positive power
            gameObject.SetActive(false);
            PopUpWarning sw = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "saveWarning"), Manager.instance.canvas.transform).GetComponent<PopUpWarning>();
            sw.warningText.text = "Veuillez entrer une énergie plus grande que 0";
            sw.quitButton.gameObject.SetActive(false);
            sw.saveButton.gameObject.SetActive(false);
            sw.SetCancelAction(() =>
            {
                sw.Close();
                gameObject.SetActive(true);
            });
            powerInput.Select();
            return;
        }
        OkAction();
    }
    #endregion
}
