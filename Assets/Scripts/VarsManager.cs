using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class VarsManager : MonoBehaviour
{
    private static VarsManager instance;
    public static VarsManager Instance { get => instance; private set => instance = value; }

    private Dictionary<string, int> vars = new Dictionary<string, int>();

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            Debug.Log("Destroying, instance already exist");
        }
    }
    public Var getVar(string name)
    {
        if (Instance.vars.ContainsKey(name))
        {
            Var var = new Var(name);
            return var;
        }
        else
        {
            Instance.vars.Add(name, 0);
            Var var = new Var(name);
            return var;
        }
    }

    [Serializable]
    public class Var
    {
        private string name;
        public string Name { get => name; private set => name = value; }
        private int value;
        public int Value { get => value; set => this.value = value; }


        public Var(string name)
        {
            Name = name;
            Value = Instance.vars[Name];
        }

        // save the value in the dictionnary
        public void Persist()
        {
            Instance.vars[name] = Value;
        }
    }

    public string[] ReplaceStringByVar(string[] expression)
    {
        for (int i = 0; i < expression.Length; i++)
        {
            string item = expression[i];
            if(item.Any(Char.IsLetter))
                if(vars.ContainsKey(item))
                {
                    item = vars[item].ToString();
                }
            return null;
        }
        return expression;
    }

    public void Clean()
    {
        vars = new Dictionary<string, int>();
    }
}
