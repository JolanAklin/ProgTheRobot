// Copyright 2021 Jolan Aklin

//This file is part of Prog The Robot.

//Prog The Robot is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 3 of the License.

//Prog The Robot is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Prog the robot.  If not, see<https://www.gnu.org/licenses/>.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;

public class VarsManager
{
    private Dictionary<string, int> vars = new Dictionary<string, int>();
    private RobotManager rm;
    private Robot robot;

    public VarsManager(RobotManager robotManager, Robot robot)
    {
        this.robot = robot;
        rm = robotManager;
    }

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

    public class BoolFunctionReturn
    {
        public bool error = false;
        public bool result;

        public override string ToString()
        {
            return result.ToString();
        }
    }

    public BoolFunctionReturn GetBoolFunction(string funcName)
    {
        bool result = false;
        switch (funcName)
        {
            case "bwf#":
                result = rm.WallInFront();
                break;
            case "bwr#":
                result = rm.WallRight();
                break;
            case "bwl#":
                result = rm.WallLeft();
                break;
            case "bo#":
                result = rm.IsOut();
                break;
            case "boao#":
                result = rm.IsOnAnOutlet();
                break;
            case "btm#":
                result = rm.IsCaseMarked();
                break;
            //case "CaseDevantOccupée":
            //case "TileInFrontOccupied":
            //    return false;
            case "bbg#":
                foreach (Ball ball in robot.robotManager.balls)
                {
                    if (!ball.ballTaken)
                    {
                        result = true;
                        break;
                    }
                }
                break;

            default:
                return new BoolFunctionReturn() { error = true, };
        }
        return new BoolFunctionReturn() { result = result };
    }

    public class FunctionReturn
    {
        public bool error = false;
        public int result;

        public override string ToString()
        {
            return result.ToString();
        }
    }

    public FunctionReturn GetFunction(string funcName)
    {
        int result = 0;
        switch (funcName)
        {
            case "iwd#":
                result = rm.WallDistance();
                break;
            case "ip#":
                result = (int)robot.Power;
                break;
            case "irpx#":
                result = Mathf.RoundToInt(rm.transform.position.x);
                break;
            case "irpy#":
                result = Mathf.RoundToInt(rm.transform.position.z);
                break;
            case "irdx#":
                switch(Mathf.RoundToInt(rm.transform.rotation.eulerAngles.y))
                {
                    case 0:
                    case 180:
                        result = 0;
                        break;
                    case 90:
                        result = 1;
                        break;
                    case 270:
                        result = -1;
                        break;
                }
                break;
            case "irdy#":
                switch (Mathf.RoundToInt(rm.transform.rotation.eulerAngles.y))
                {
                    case 90:
                    case -90:
                        result = 0;
                        break;
                    case 0:
                        result = 1;
                        break;
                    case 180:
                        result = -1;
                        break;
                }
                break;
            case "ibpx#":
                foreach (Ball ball in robot.robotManager.balls)
                {
                    if (!ball.ballTaken)
                    {
                        result = Mathf.RoundToInt(ball.parent.transform.position.x);
                        break;
                    }
                }
                break;
            case "ibpy#":
                foreach (Ball ball in robot.robotManager.balls)
                {
                    if(!ball.ballTaken)
                    {
                        result = Mathf.RoundToInt(ball.parent.transform.position.z);
                        break;
                    }
                }
                break;

            default:
                return new FunctionReturn() { error = true };
        }
        return new FunctionReturn() { result = result };
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
            if(Regex.IsMatch(expression[i], @"^[a-zA-Z]+$"))
            {
                FunctionReturn fReturn = GetFunction(expression[i]);
                if(!fReturn.error)
                {
                    expression[i] = fReturn.result.ToString();
                }
                else if(vars.ContainsKey(expression[i]))
                {
                    expression[i] = vars[expression[i]].ToString();
                }
                else
                {
                    return null;
                }
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
        if (Regex.IsMatch(expression, @"^[a-zA-Z]+$"))
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

    public class Evaluation
    {
        public bool error = false;
        public bool result;
    }

    public string ReplaceFunctionByValue(string expression)
    {
        foreach (string function in LanguageManager.instance.FullNameToAbrevDict.Values)
        {
            if(function[0] == 'b')
            {
                expression = expression.Replace(function, GetBoolFunction(function).result ? "True" : "False");
            }else if (function[0] == 'i')
            {
                expression = expression.Replace(function, GetFunction(function).result.ToString());
            }
        }

        return expression;
    }

    public Evaluation Evaluate(string expression)
    {
        expression = ReplaceFunctionByValue(expression);

        string[] stringsToFind = new string[] { "bopor#", "bopand#" };

        List<string> findOrder = new List<string>();

        // https://stackoverflow.com/questions/17892237/occurrences-of-a-liststring-in-a-string-c-sharp
        int count = 0;
        foreach (var stringToFind in stringsToFind)
        {
            int currentIndex = 0;

            while ((currentIndex = expression.IndexOf(stringToFind, currentIndex, StringComparison.Ordinal)) != -1)
            {
                findOrder.Add(stringToFind);
                currentIndex++;
                count++;
            }
        }

        string[] exprSplit = expression.Split(stringsToFind, StringSplitOptions.None);

        List<bool> results = new List<bool>();

        foreach (string item in exprSplit)
        {
            string[] separators = new string[] { " " };
            string[] smallExprSplit = item.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            BoolFunctionReturn fBoolReturn = null;
            switch (smallExprSplit.Length)
            {
                // get the corresponding function, if it exist, and adds it to the result list
                case 1:
                    // start tpi
                    switch (smallExprSplit[0])
                    {
                        case "True":
                            fBoolReturn = new BoolFunctionReturn() { result = true };
                            break;
                        case "False":
                            fBoolReturn = new BoolFunctionReturn() { result = false };
                            break;
                        default:
                            fBoolReturn = new BoolFunctionReturn() { error = true };
                            break;
                    }
                    // end tpi
                    if (!fBoolReturn.error)
                        results.Add(fBoolReturn.result);
                    else
                        return new Evaluation() { error = true };
                    break;
                // get the corresponding function, if it exist, invert the result if there is a No or Non at the begining and adds it to the result list
                case 2:
                    // start tpi
                    switch (smallExprSplit[1])
                    {
                        case "True":
                            fBoolReturn = new BoolFunctionReturn() { result = true };
                            break;
                        case "False":
                            fBoolReturn = new BoolFunctionReturn() { result = false };
                            break;
                        default:
                            fBoolReturn = new BoolFunctionReturn() { error = true };
                            break;
                    }
                    // end tpi
                    if (!fBoolReturn.error)
                    {
                        if (smallExprSplit[0] == "bopnot#")
                        {
                            results.Add(!fBoolReturn.result);
                        }
                        else
                        {
                            return new Evaluation() { error = true };
                        }
                    }
                    else
                    {
                        return new Evaluation() { error = true };
                    }
                    break;
                // evaluate an expression like this : test + 2 = myVar + 4
                default:
                    string[] delimiters = new string[] { "=", "<", ">", ">=", "<=", "<>" };
                    List<string> exprPart1 = new List<string>();
                    List<string> exprPart2 = new List<string>();
                    string foundDel = "";
                    bool findDelimiter = false;
                    foreach (string exprBits in smallExprSplit)
                    {
                        if (!findDelimiter)
                        {
                            foreach (string del in delimiters)
                            {
                                if (exprBits == del)
                                {
                                    findDelimiter = true;
                                    foundDel = del;
                                    break;
                                }
                            }
                            if (!findDelimiter)
                                exprPart1.Add(exprBits);
                        }
                        else
                        {
                            exprPart2.Add(exprBits);
                        }
                    }
                    // calculate the result of each part of the expression
                    int value1 = 0;
                    int value2 = 0;
                    try
                    {
                        value1 = Convert.ToInt32(new DataTable().Compute(string.Join(" ", exprPart1.ToArray()), null));
                        value2 = Convert.ToInt32(new DataTable().Compute(string.Join(" ", exprPart2.ToArray()), null));
                    }
                    catch (Exception)
                    {
                        return new Evaluation() { error = true };
                    }
                    // find the result of the expression
                    switch (foundDel)
                    {
                        case "=":
                            results.Add(value1 == value2);
                            break;
                        case "<":
                            results.Add(value1 < value2);
                            break;
                        case ">":
                            results.Add(value1 > value2);
                            break;
                        case "<=":
                            results.Add(value1 <= value2);
                            break;
                        case ">=":
                            results.Add(value1 >= value2);
                            break;
                        case "<>":
                            results.Add(value1 != value2);
                            break;
                        default:
                            return new Evaluation() { error = true };
                    }
                    break;
            }
        }

        // make the final result
        bool finalResult = false;
        if (findOrder.Count > 0)
        {
            if (findOrder[0] == "bopand#")
            {
                if (results[0] && results[1])
                    finalResult = true;
                else
                    finalResult = false;
            }
            else if (findOrder[0] == "bopor#")
            {
                if (results[0] || results[1])
                    finalResult = true;
                else
                    finalResult = false;
            }
            for (int i = 2; i < results.Count; i++)
            {
                if (findOrder[i - 1] == "bopand#")
                {
                    if (finalResult && results[i])
                        finalResult = true;
                    else
                        finalResult = false;
                }
                else if (findOrder[i - 1] == "bopor#")
                {
                    if (finalResult || results[i])
                        finalResult = true;
                    else
                        finalResult = false;
                }
            }
        }
        else
        {
            finalResult = results[0];
        }
        return new Evaluation() { result = finalResult };
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
