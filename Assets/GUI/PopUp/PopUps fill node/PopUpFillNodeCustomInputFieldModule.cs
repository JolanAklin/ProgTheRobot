using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpFillNodeCustomInputFieldModule : MonoBehaviour, PopUpFillNodeInputInterface
{
    public List<PopUpNodeInput> customInputs = new List<PopUpNodeInput>();
    private List<object> customInputFields = new List<object>();

    public List<object> Inputs { get => customInputFields; }

    private void Awake()
    {
        for (int i = 0; i < customInputs.Count; i++)
        {
            customInputFields.Add(customInputs[i]);
        }
    }

    private void Start()
    {
        StartCoroutine("SelectInputField");
    }

    IEnumerator SelectInputField()
    {
        yield return new WaitForEndOfFrame();
        customInputs[0].input.Select();
    }

    public bool Validate()
    {
        bool validated = true;
        foreach (PopUpNodeInput customInput in customInputFields)
        {
            customInput.Validate();
            if (customInput.validation.validationStatus == Validator.ValidationStatus.KO)
            {
                validated = false;
                break;
            }
        }
        return validated;
    }
    public void SetInputsContent(string[] content)
    {
        for (int i = 0; i < customInputFields.Count; i++)
        {
            PopUpNodeInput input = customInputFields[i] as PopUpNodeInput;
            input.blockCompletion = true;
            input.input.text = LanguageManager.instance.AbrevToFullName(content[i]);
            input.blockCompletion = false;
        }
    }
}
