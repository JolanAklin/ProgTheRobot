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

    public Var GetVar(string name, int value)
    {
        if (Instance.vars.ContainsKey(name))
        {
            Var var = new Var(name);
            var.Value = value;
            var.Persist();
            return var;
        }
        else
        {
            if(CheckVarName(name))
            {
                Instance.vars.Add(name, value);
                Var var = new Var(name);
                return var;
            }
            return null;
        }
    }

    public Var GetVar(string name)
    {
        if (Instance.vars.ContainsKey(name))
        {
            Var var = new Var(name);
            return var;
        }
        return null;
    }


    /// <summary>
    /// Check if the variable name is valid
    /// </summary>
    /// <param name="name">Name of the variable</param>
    /// <returns>Return true if the variable name can be used</returns>
    public static bool CheckVarName(string name)
    {
        if (name.Any(Char.IsLetter))
        {
            switch (name)
            {
                // math symbols
                case "+":
                case "-":
                case "*":
                case "/":
                case "(":
                case ")":
                case "=":
                case "<":
                case ">":
                case ">=":
                case "<=":
                case "<>":
                // loop
                case "Pour":
                case "For":
                case "Jusque":
                case "UpTo":
                case "Pas":
                case "Step":
                case "TantQue":
                case "While":
                //command
                case "Lire":
                case "Read":
                case "Afficher":
                case "Display":
                case "Avancer":
                case "GoForward":
                case "TournerADroite":
                case "TurnRight":
                case "TournerAGauche":
                case "TurnLeft":
                case "Marquer":
                case "Mark":
                case "D�marquer":
                case "Unmark":
                case "Recharger":
                case "Reload":
                case "PoserBallon":
                case "PlaceBall":
                case "PrendreBallon":
                case "TakeBall":
                case "LancerBallon":
                case "ThrowBall":
                // fonctions
                case "MurEnFace":
                case "WallInFront":
                case "MurADroite":
                case "WallRight":
                case "MurAGauche":
                case "WallLeft":
                case "Sorti":
                case "Out":
                case "RobotSurUnePrise":
                case "RobotOnAnOutlet":
                case "CaseMarqu�":
                case "TileMarked":
                case "CaseDevantOccup�e":
                case "TileInFrontOccupied":
                case "BallonSurLeSol":
                case "BallOnTheGround":
                case "DistanceMur":
                case "WallDistance":
                case "Energie":
                case "Power":
                case "xRobot":
                case "yRobot":
                case "dxRobot":
                case "dyRobot":
                case "yBallon":
                case "xBallon":
                // logical operator
                case "Et":
                case "And":
                case "Ou":
                case "Or":
                case "Non":
                case "No":
                case "Vrai":
                case "True":
                case "Faux":
                case "False":
                    return false;
            }
            return true;
        }
        return false;
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
                else
                {
                    return null;
                }
        }
        return expression;
    }

    public void Clean()
    {
        vars = new Dictionary<string, int>();
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
}
