using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class VarsManager
{
    private Dictionary<string, int> vars = new Dictionary<string, int>();

    /// <summary>
    /// Get all the vars stored in this manager
    /// </summary>
    /// <returns>string array in this form : var name : var value</returns>
    public string[] GetAllVars()
    {
        string[] allVars = new string[vars.Count];
        int i = 0;
        foreach (KeyValuePair<string,int> var in vars)
        {
            allVars[i] = $"{var.Key} : {var.Value}";
        }
        return allVars;
    }

    /// <summary>
    /// Create or set a var
    /// </summary>
    /// <param name="name">The name of the var</param>
    /// <param name="value">The value to set</param>
    /// <returns></returns>
    public Var GetVar(string name, int value)
    {
        if (vars.ContainsKey(name))
        {
            Var var = new Var(name, this);
            var.Value = value;
            var.Persist();
            return var;
        }
        else
        {
            if(CheckVarName(name))
            {
                vars.Add(name, value);
                Var var = new Var(name, this);
                return var;
            }
            return null;
        }
    }

    /// <summary>
    /// Get the var with the specified name
    /// </summary>
    /// <param name="name">Var name</param>
    /// <returns>Return a var object. Return null if nothing was found</returns>
    public Var GetVar(string name)
    {
        if (vars.ContainsKey(name))
        {
            Var var = new Var(name, this);
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
                case "Demarquer":
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
                case "CaseMarqué":
                case "TileMarked":
                case "CaseDevantOccupée":
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

    /// <summary>
    /// Replace var's name with the number of the var
    /// </summary>
    /// <param name="expression">The mathematic expression that need to be transformed</param>
    /// <returns></returns>
    public string[] ReplaceStringsByVar(string[] expression)
    {
        for (int i = 0; i < expression.Length; i++)
        {
            if(expression[i].Any(Char.IsLetter))
                if(vars.ContainsKey(expression[i]))
                {
                    expression[i] = vars[expression[i]].ToString();
                }
                else
                {
                    return null;
                }
        }
        return expression;
    }

    /// <summary>
    /// Replace the string by the corresponding var value
    /// </summary>
    /// <param name="expression">The string that need to be converted to the var number</param>
    /// <returns></returns>
    public string ReplaceStringByVar(string expression)
    {
        if (expression.Any(Char.IsLetter))
            if (vars.ContainsKey(expression))
            {
                expression = vars[expression].ToString();
            }
            else
            {
                return null;
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

        private VarsManager varsManager;


        public Var(string name, VarsManager varsManager)
        {
            Name = name;
            Value = varsManager.vars[Name];
            this.varsManager = varsManager;
        }

        // save the value in the dictionnary
        public void Persist()
        {
            varsManager.vars[name] = Value;
        }
    }

    ~VarsManager()
    {
        Debug.Log("Var manager has been destroyed");
    }
}
