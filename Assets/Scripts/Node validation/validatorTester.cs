using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class validatorTester : MonoBehaviour
{
    public string test = "var + 1 = test And Wall in front";
    // Start is called before the first frame update
    void Start()
    {
        Validator.InverseKV();
        Debug.Log(Validator.Validate(Validator.ValidationType.test, test));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
