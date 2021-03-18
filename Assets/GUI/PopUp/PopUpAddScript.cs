using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PopUpAddScript : MonoBehaviour
{
    [HideInInspector]
    public string scriptName;

    //a voir pour les items pour la liste 

    private Action cancelAction;
    private Action okAction;

    private void Start()
    {
    }

    public void PopUpClose()
    {
        Destroy(this.gameObject);
    }

    public void OnEndEditScriptName(TMP_InputField inputField)
    {
        scriptName = inputField.text;
    }

    #region buttons action
    public void SetCancelAction(Action action)
    {
        cancelAction = action;
    }
    public void SetOkAction(Action action)
    {
        okAction = action;
    }

    public void Cancel()
    {
        cancelAction();
    }
    public void Ok()
    {
        if(scriptName.Length == 0)
        {
            gameObject.SetActive(false);
            PopUpWarning sw = WindowsManager.InstantiateWindow((int)Enum.Parse(typeof(WindowsManager.popUp), "saveWarning"), Manager.instance.canvas.transform).GetComponent<PopUpWarning>();
            sw.warningText.text = "Veuillez entrer un nom pour l'organigramme";
            sw.quitButton.gameObject.SetActive(false);
            sw.saveButton.gameObject.SetActive(false);
            sw.SetCancelAction(() =>
            {
                sw.Close();

                gameObject.SetActive(true);
            });
            return;
        }
        okAction();
    }
    #endregion
}
