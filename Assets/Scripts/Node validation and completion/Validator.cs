using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

public static class Validator
{
    /// <summary>
    /// Define different validation types
    /// </summary>
    public enum ValidationType
    {
        test,
        action,
        forloopvar,
        readWrite,
        affectation,
        subProrgam,
        forloopexpression,
    }

    public enum ValidationStatus
    {
        OK,
        KO,
    }

    /// <summary>
    /// Returned after a validation with useful infos such as errors
    /// </summary>
    public struct ValidationReturn
    {
        public struct Error
        {
            public uint startPos { private set; get; }
            public uint endPos { private set; get; }
            public string message { private set; get; }

            public Error(uint start, uint end, string message)
            {
                startPos = start;
                endPos = end;
                this.message = message;
            }

            public override string ToString()
            {
                return $"From : {startPos}, To : {endPos}, Message : {message}";
            }
        }

        public ValidationStatus validationStatus { private set; get; }
        /// <summary>
        /// Errors in a specific place on the string to validate.
        /// </summary>
        public Dictionary<uint, Error> specificErrors { private set; get; }
        /// <summary>
        /// General errors
        /// </summary>
        public List<string> generalErrors { private set; get; }

        public ValidationReturn(ValidationStatus validationStatus)
        {
            this.validationStatus = validationStatus;
            specificErrors = new Dictionary<uint, Error>();
            generalErrors = new List<string>();
        }

        public void AddSpecificError(uint pos, Error error)
        {
            if (!specificErrors.ContainsKey(pos))
                specificErrors.Add(pos, error);
        }

        public void AddGeneralErrors(string error)
        {
            generalErrors.Add(error);
        }

        public void ChangeValidationStatus(ValidationStatus validationStatus)
        {
            this.validationStatus = validationStatus;
        }

        public override string ToString()
        {
            string returnString = $"{validationStatus}{Environment.NewLine}Errors :{Environment.NewLine}";
            foreach (KeyValuePair<uint, Error> error in specificErrors)
            {
                returnString += " - " + error.Value.ToString() + Environment.NewLine;
            }
            foreach (string error in generalErrors)
            {
                returnString += " - " + error.ToString() + Environment.NewLine;
            }
            return returnString;
        }
    }

    // using one string for the content validation and one for the display is recommended
    /// <summary>
    /// validate node content. 
    /// </summary>
    /// <param name="type">Validation type to use</param>
    /// <param name="toValidate">The node content string to validate</param>
    /// <returns></returns>
    public static ValidationReturn Validate(ValidationType type, string toValidate)
    {
        switch (type)
        {
            case ValidationType.test:
                return ValidateTest(toValidate);

            case ValidationType.action:
                return ValidateAction(toValidate);

            case ValidationType.forloopvar:
                return ValidateForLoopVar(toValidate);

            case ValidationType.forloopexpression:
                return ValidateForLoopUntil(toValidate);

            case ValidationType.readWrite:
                return ValidateReadWrite(toValidate);

            case ValidationType.affectation:
                return ValidateAffectation(toValidate);

            default:
                return new ValidationReturn(ValidationStatus.KO);
        }
    }
    public static ValidationReturn Validate(ValidationType type, string[] toValidate)
    {
        return new ValidationReturn(ValidationStatus.KO);
    }

    // input string should be english only
    // this validator works in a new way. Remaking a new translation system would greatly improve the overall experience

    // function names are now converted to something simpler to use for the machine. Wall in front becomes bwf. Explanation below.

    /// <summary>
    /// Validate if and while node content
    /// </summary>
    /// <param name="toValidate">This string should use the internal format for nodes</param>
    /// <returns>An object containing the validation status and errors</returns>
    private static ValidationReturn ValidateTest(string toValidate)
    {
        if (toValidate == "")
            return new ValidationReturn(ValidationStatus.OK);

        string toValidateNonAltered = toValidate;
        ValidationReturn vr = new ValidationReturn(ValidationStatus.OK);

        string regexPattern = @"[^a-zA-Z0-9+\-*/()><=\s]+";
        Regex regex1 = new Regex(regexPattern);
        if (regex1.IsMatch(toValidate))
        {
            vr.ChangeValidationStatus(ValidationStatus.KO);
            Match match = regex1.Match(toValidate);
            vr.AddSpecificError((uint)match.Index, new ValidationReturn.Error((uint)match.Index, (uint)match.Index + (uint)match.Length, $"\"{match.Value}\" means nothing"));
            return vr;
        }

        toValidate = LanguageManager.instance.FullNameToAbrev(toValidate);


        string[] stringsToFind = new string[] { "bopor#", "bopand#" };
        string[] exprSplit = toValidate.Split(stringsToFind, StringSplitOptions.None);

        string findIndexAndOr = @$"{LanguageManager.instance.AbrevToFullName("bopor#")}|{LanguageManager.instance.AbrevToFullName("bopand#")}";
        Regex findIndexAndOrRegex = new Regex(findIndexAndOr);
        MatchCollection mc = findIndexAndOrRegex.Matches(toValidateNonAltered);

        // used to add to the posInStartString to keep the error at the right position since "And" and "Or" keyword are removed from the main string
        Dictionary<int, uint> andOrPlaceLength = new Dictionary<int, uint>();
        foreach (Match matchItem in mc)
        {
            andOrPlaceLength.Add(matchItem.Index, (uint)matchItem.Length);
        }

        uint posInStartString = 0;
        uint andOrAddedLength = 0;

        foreach (string item in exprSplit)
        {
            andOrAddedLength = 0;
            if (andOrPlaceLength.ContainsKey((int)posInStartString))
            {
                andOrAddedLength = andOrPlaceLength[(int)posInStartString] + 1;
                posInStartString += andOrAddedLength;
            }

            string[] separators = new string[] { " " };
            string[] smallExprSplit = item.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            uint codeBlockLength = 0;
            string fullname = "";
            string[] testOperators = new string[] { "=", "<", ">", ">=", "<=", "<>" };
            switch (smallExprSplit.Length)
            {
                case 0:
                    if (andOrAddedLength > 0)
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStartString - andOrAddedLength, new ValidationReturn.Error(posInStartString - andOrAddedLength, posInStartString - 1, $"\"And\" and \"Or\" keywords must have a statement before and after them"));
                    }
                    break;
                case 1:
                    // get the corresponding function, if it exist, and adds it to the result list
                    codeBlockLength = (uint)LanguageManager.instance.getFullnameFromAbrev(smallExprSplit[0]).Length;

                    if (GetFunctionType(smallExprSplit[0]) != FunctionType.@bool)
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, $"\"{fullname}\" is not a valid boolean function."));
                    }
                    posInStartString += codeBlockLength + 1; // +1 = space after this word
                    break;

                case 2:
                    // get the corresponding function, if it exist, invert the result if there is a No or Non at the begining and adds it to the result list
                    codeBlockLength = 0;
                    FunctionType firstWordType = GetFunctionType(smallExprSplit[0]);
                    FunctionType secondWordType = GetFunctionType(smallExprSplit[1]);

                    string firstWordFullname = LanguageManager.instance.getFullnameFromAbrev(smallExprSplit[0]);
                    string secondWordFullname = LanguageManager.instance.getFullnameFromAbrev(smallExprSplit[1]);

                    uint firstWordLength = (uint)firstWordFullname.Length;
                    uint secondWordLength = (uint)secondWordFullname.Length;

                    codeBlockLength = firstWordLength + 1 + secondWordLength;

                    if (firstWordType != FunctionType.boolOp && secondWordType != FunctionType.@bool)
                    {
                        // is not an boolean statement
                        goto default;
                    }

                    if (firstWordType == FunctionType.boolOp && secondWordType != FunctionType.@bool)
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStartString + firstWordLength + 1, new ValidationReturn.Error(posInStartString + firstWordLength + 1, posInStartString + 1 + firstWordLength + secondWordLength, $"\"{secondWordFullname}\" is not a valid boolean function."));
                    }
                    else if (firstWordType != FunctionType.boolOp && secondWordType == FunctionType.@bool)
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + firstWordLength, $"The keyword \"{firstWordFullname}\" is unknown in the current context."));
                    }
                    posInStartString += codeBlockLength + 1; // + 1 = space after this word
                    break;

                default:
                    // evaluate an expression like this one : test + 2 = myVar + 4
                    codeBlockLength = 0;
                    // every string while be replaced by the number 1 to test if the expression is correct with a datatable
                    List<string> exprPart1 = new List<string>();
                    List<string> exprPart2 = new List<string>();
                    string pattern = @"^[a-zA-Z#]+$";
                    Regex regex = new Regex(pattern);

                    uint posInStringAtStartOfTest = posInStartString;

                    bool findDelimiter = false;
                    foreach (string exprBits in smallExprSplit)
                    {
                        if (!findDelimiter)
                        {
                            foreach (string del in testOperators)
                            {
                                if (exprBits == del)
                                {
                                    findDelimiter = true;
                                    break;
                                }
                            }
                            if (!findDelimiter)
                            {
                                if (regex.IsMatch(exprBits))
                                    exprPart1.Add("1");
                                else
                                    exprPart1.Add(exprBits);
                            }
                        }
                        else
                        {
                            if (regex.IsMatch(exprBits))
                                exprPart2.Add("1");
                            else
                                exprPart2.Add(exprBits);
                        }

                        FunctionType exprType = GetFunctionType(exprBits);

                        if (exprType != FunctionType.@int && exprType != FunctionType.word && exprType != FunctionType.number && exprType != FunctionType.@operator)
                        {
                            string funcName = LanguageManager.instance.getFullnameFromAbrev(exprBits);
                            codeBlockLength = (uint)funcName.Length;

                            vr.ChangeValidationStatus(ValidationStatus.KO);
                            vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, "Only integer function and variable can be used in this context."));
                        }
                        else if (exprType == FunctionType.@int)
                        {
                            codeBlockLength = (uint)LanguageManager.instance.getFullnameFromAbrev(exprBits).Length;
                        }
                        else
                        {
                            codeBlockLength = (uint)exprBits.Length;
                            if (LanguageManager.instance.ReservedKeywords[ValidationType.test].Contains(exprBits))
                            {
                                vr.ChangeValidationStatus(ValidationStatus.KO);
                                vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, $"\"{exprBits}\" is a reserved keyword and therefore can't be used as a variable"));
                            }
                        }

                        posInStartString += codeBlockLength + 1; // +1 = space after this word
                    }
                    if (exprPart1.Count <= 0 || exprPart2.Count <= 0)
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStringAtStartOfTest, new ValidationReturn.Error(posInStringAtStartOfTest, posInStartString - 1, $"You need two things to compare."));
                    }
                    else
                    {
                        try
                        {
                            new DataTable().Compute(string.Join(" ", exprPart1.ToArray()), null); // use datatable to test if the expression is correct, if the expression is not it will say it even if there is a by 0 division
                            new DataTable().Compute(string.Join(" ", exprPart2.ToArray()), null); // use datatable to test if the expression is correct, if the expression is not it will say it even if there is a by 0 division
                        }
                        catch (Exception e)
                        {
                            vr.ChangeValidationStatus(ValidationStatus.KO);
                            vr.AddSpecificError(posInStringAtStartOfTest, new ValidationReturn.Error(posInStringAtStartOfTest, posInStartString - 1, $"{e.Message}")); // need to display custom errors in order to be localized
                        }

                    }
                    break;
            }
        }
        return vr;
    }

    /// <summary>
    /// validate robot actions, such as go forward or turn left
    /// </summary>
    /// <param name="toValidate">This string should use the internal format for nodes</param>
    /// <returns>An object containing the validation status and errors</returns>
    private static ValidationReturn ValidateAction(string toValidate)
    {
        ValidationReturn vr = new ValidationReturn(ValidationStatus.OK);

        string toValidateUnAltered = toValidate;
        toValidate = LanguageManager.instance.FullNameToAbrev(toValidate);
        string[] splited = toValidate.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        FunctionType type;
        switch (splited.Length)
        {
            case 0:
                break;
            case 1:
                type = GetFunctionType(splited[0]);
                if (type != FunctionType.action)
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(0, new ValidationReturn.Error(0, (uint)toValidateUnAltered.Length, $"\"{toValidate}\" is not an action."));
                }
                break;
            default:
                type = GetFunctionType(splited[0]);
                uint posInDisplayedString = (uint)LanguageManager.instance.getFullnameFromAbrev(splited[0]).Length;

                if (type != FunctionType.action)
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(0, new ValidationReturn.Error(0, posInDisplayedString, $"\"{splited[0]}\" is not an action."));
                }
                posInDisplayedString++;
                vr.ChangeValidationStatus(ValidationStatus.KO);
                vr.AddSpecificError(posInDisplayedString, new ValidationReturn.Error(posInDisplayedString, (uint)toValidateUnAltered.Length, $"This node takes only one action as parameter."));
                break;
        }

        return vr;
    }

    /// <summary>
    /// validate the variable that will be incremented in the for loop
    /// </summary>
    /// <param name="toValidate">This string should use the internal format for nodes</param>
    /// <returns>An object containing the validation status and errors</returns>
    private static ValidationReturn ValidateForLoopVar(string toValidate)
    {
        ValidationReturn vr = new ValidationReturn(ValidationStatus.OK);

        string toValidateUnAltered = toValidate;
        toValidate = LanguageManager.instance.FullNameToAbrev(toValidate);
        string[] splited = toValidate.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        FunctionType type;
        switch (splited.Length)
        {
            case 0:
                break;
            case 1:
                type = GetFunctionType(splited[0]);
                uint endpos = (uint)LanguageManager.instance.getFullnameFromAbrev(splited[0]).Length;
                if(type != FunctionType.word)
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(0, new ValidationReturn.Error(0, endpos, "Should be a variable"));
                }
                break;
            default:
                uint pos = (uint)LanguageManager.instance.getFullnameFromAbrev(splited[0]).Length + 1;
                uint startpos = pos;
                for (int i = 1; i < splited.Length; i++)
                {
                    pos += (uint)LanguageManager.instance.getFullnameFromAbrev(splited[i]).Length;
                }
                vr.ChangeValidationStatus(ValidationStatus.KO);
                vr.AddSpecificError(startpos, new ValidationReturn.Error(startpos, pos, "Expecting only a variable"));
                goto case 1;
        }
        return vr;
    }

    /// <summary>
    /// validate the until and by increments clauses of the for loop
    /// </summary>
    /// <param name="toValidate">This string should use the internal format for nodes</param>
    /// <returns>An object containing the validation status and errors</returns>
    private static ValidationReturn ValidateForLoopUntil(string toValidate)
    {
        ValidationReturn vr = new ValidationReturn(ValidationStatus.OK);

        string toValidateUnAltered = toValidate;
        toValidate = LanguageManager.instance.FullNameToAbrev(toValidate);
        string[] splited = toValidate.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        FunctionType type;
        string expr = "";
        uint posInDisplayString = 0;
        uint startPos = posInDisplayString;
        for (int i = 0; i < splited.Length; i++)
        {
            expr += Regex.Replace(splited[i], @"^[a-zA-Z#]+$", "1");
            type = GetFunctionType(splited[i]);
            string name = LanguageManager.instance.getFullnameFromAbrev(splited[i]);
            if (type != FunctionType.@int && type != FunctionType.word && type != FunctionType.number && type != FunctionType.@operator)
            {
                vr.ChangeValidationStatus(ValidationStatus.KO);
                vr.AddSpecificError(posInDisplayString, new ValidationReturn.Error(posInDisplayString, posInDisplayString + (uint)name.Length, "Only integer function and variable can be used in this context."));
            }
            else if (type != FunctionType.@int && LanguageManager.instance.ReservedKeywords[ValidationType.forloopexpression].Contains(name))
            {
                vr.ChangeValidationStatus(ValidationStatus.KO);
                vr.AddSpecificError(posInDisplayString, new ValidationReturn.Error(posInDisplayString, posInDisplayString + (uint)name.Length, $"\"{name}\" is a resserved keyword."));
            }
            posInDisplayString += (uint)name.Length + 1;
        }
        try
        {
            new DataTable().Compute(expr, null);
        }
        catch (Exception e)
        {
            vr.ChangeValidationStatus(ValidationStatus.KO);
            vr.AddSpecificError(startPos, new ValidationReturn.Error(startPos, posInDisplayString - 1, $"{e.Message}"));
        }
        return vr;
    }

    /// <summary>
    /// validate read and write statements
    /// </summary>
    /// <param name="toValidate">This string should use the internal format for nodes</param>
    /// <returns>An object containing the validation status and errors</returns>
    private static ValidationReturn ValidateReadWrite(string toValidate)
    {
        ValidationReturn vr = new ValidationReturn(ValidationStatus.OK);

        string toValidateUnAltered = toValidate;
        toValidate = LanguageManager.instance.FullNameToAbrev(toValidate);
        string[] splited = toValidate.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        uint pos = 0;
        FunctionType type;
        switch (splited.Length)
        {
            case 0:
                break;
            case 1: // test if the first word is Read or Write
                pos = (uint)LanguageManager.instance.getFullnameFromAbrev(splited[0]).Length;

                type = GetFunctionType(splited[0]);
                if(type != FunctionType.keywordReadWrite || (splited[0] != "kwread#" && splited[0] != "kwwrite#"))
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(0, new ValidationReturn.Error(0, pos, "Expecting \"Read\" or \"Write\" keyword."));
                }
                break;
            case 2: // test if the second word is a variable
                type = GetFunctionType(splited[1]);
                pos = (uint)LanguageManager.instance.getFullnameFromAbrev(splited[0]).Length + 1;

                uint endPos = pos;
                string secondWord = LanguageManager.instance.getFullnameFromAbrev(splited[1]);
                endPos += (uint)secondWord.Length;

                if (type != FunctionType.word)
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(pos, new ValidationReturn.Error(pos, endPos, "Expecting a variable"));
                }else if(LanguageManager.instance.ReservedKeywords[ValidationType.readWrite].Contains(secondWord))
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(pos, new ValidationReturn.Error(pos, endPos, $"\"{secondWord}\" is a reserved keyword."));
                }
                goto case 1;
            default: // say that everything more is wrong
                for (int i = 0; i < 2; i++)
                {
                    pos += (uint)LanguageManager.instance.getFullnameFromAbrev(splited[i]).Length + 1;
                }
                vr.ChangeValidationStatus(ValidationStatus.KO);
                vr.AddSpecificError(pos, new ValidationReturn.Error(pos, (uint)toValidateUnAltered.Length, "Expecting only 2 arguments. \"Read\" or \"Write\" keyword and a variable."));
                goto case 2;
        }
        return vr;
    }

    /// <summary>
    /// validate affectation statements
    /// </summary>
    /// <param name="toValidate">This string should use the internal format for nodes</param>
    /// <returns>An object containing the validation status and errors</returns>
    private static ValidationReturn ValidateAffectation(string toValidate)
    {
        ValidationReturn vr = new ValidationReturn(ValidationStatus.OK);

        string toValidateUnAltered = toValidate;
        toValidate = LanguageManager.instance.FullNameToAbrev(toValidate);
        string[] splited = toValidate.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        string fullname;
        FunctionType type;
        switch (splited.Length)
        {
            case 0:
                break;
            case 1:
                type = GetFunctionType(splited[0]);
                fullname = LanguageManager.instance.getFullnameFromAbrev(splited[0]);
                if (type != FunctionType.word && LanguageManager.instance.ReservedKeywords[ValidationType.affectation].Contains(fullname))
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(0, new ValidationReturn.Error(0, (uint)fullname.Length, $"\"{fullname}\" is a resserved keyword."));
                }
                else if (type != FunctionType.word)
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(0, new ValidationReturn.Error(0, (uint)fullname.Length, "Should be a variable."));
                }
                break;
            case 2:
                fullname = LanguageManager.instance.getFullnameFromAbrev(splited[0]);
                string funcName = LanguageManager.instance.getFullnameFromAbrev(splited[1]);
                if (splited[1] != "=")
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError((uint)fullname.Length + 1, new ValidationReturn.Error((uint)fullname.Length + 1, (uint)fullname.Length + 1 + (uint)funcName.Length, $"Expecting \"=\" sign"));
                }
                goto case 1;
            default:
                string expr = "";
                uint posInDisplayString = (uint)LanguageManager.instance.getFullnameFromAbrev(splited[0]).Length + 1 + (uint)LanguageManager.instance.getFullnameFromAbrev(splited[1]).Length + 1;
                uint startPos = posInDisplayString;
                for (int i = 2; i < splited.Length; i++)
                {
                    expr += Regex.Replace(splited[i], @"^[a-zA-Z#]+$", "1");
                    type = GetFunctionType(splited[i]);
                    string name = LanguageManager.instance.getFullnameFromAbrev(splited[i]);
                    if (type != FunctionType.@int && type != FunctionType.word && type != FunctionType.number && type != FunctionType.@operator)
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInDisplayString, new ValidationReturn.Error(posInDisplayString, posInDisplayString + (uint)name.Length, "Only integer function and variable can be used in this context."));
                    }
                    else if(type != FunctionType.@int && LanguageManager.instance.ReservedKeywords[ValidationType.affectation].Contains(name))
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInDisplayString, new ValidationReturn.Error(posInDisplayString, posInDisplayString + (uint)name.Length, $"\"{name}\" is a resserved keyword."));
                    }
                    posInDisplayString += (uint)name.Length + 1;
                }
                try
                {
                    new DataTable().Compute(expr, null);
                }
                catch (Exception e)
                {
                    vr.ChangeValidationStatus(ValidationStatus.KO);
                    vr.AddSpecificError(startPos, new ValidationReturn.Error(startPos, posInDisplayString - 1, $"{e.Message}"));
                }
                goto case 2;
        }
        return vr;
    }

    public enum FunctionType
    {
        /// <summary>
        /// A function returning an int
        /// </summary>
        @int,
        /// <summary>
        /// A function returning a boolean
        /// </summary>
        @bool,
        /// <summary>
        /// A boolean operator such as Not, Or,...
        /// </summary>
        boolOp,
        /// <summary>
        /// Contain only char between a-z
        /// </summary>
        word,
        /// <summary>
        /// Not a function and not a boolean operator and does not contain only char between a-z
        /// </summary>
        other,
        /// <summary>
        /// A robot action like go forward
        /// </summary>
        action,
        unknown,
        number,
        @operator,
        keywordReadWrite,
    }

    /// <summary>
    /// Return the type of a function abreviation
    /// </summary>
    /// <param name="function"></param>
    /// <returns></returns>
    public static FunctionType GetFunctionType(string function)
    {
        if(!LanguageManager.instance.AbrevToFullNameContainsKey(function))
        {
            if (Regex.IsMatch(function, @"^[a-z]+$", RegexOptions.IgnoreCase))
            {
                return FunctionType.word;
            }else if (Regex.IsMatch(function, @"^-?[0-9]+$"))
            {
                return FunctionType.number;
            }
            else if (Regex.IsMatch(function, @"^[+\-*/()><=]+$"))
            {
                return FunctionType.@operator;
            }
            else
            {
                return FunctionType.other;
            }
        }
        else if(function.StartsWith("bop"))
        {
            return FunctionType.boolOp;
        }
        else if(function.StartsWith("ac"))
        {
            return FunctionType.action;
        }else if(function.StartsWith("kw"))
        {
            return FunctionType.keywordReadWrite;
        }
        else if(function[0] == 'i')
        {
            return FunctionType.@int;
        }
        else if (function[0] == 'b')
        {
            return FunctionType.@bool;
        }
        else
        {
            return FunctionType.unknown;
        }
    }
}
