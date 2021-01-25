using UnityEngine;
using Language;

public class TranslationTestScript : MonoBehaviour
{
    private void Start()
    {
        var hello = Translation.Get("welcome");
        Debug.Log(hello);
    }
}