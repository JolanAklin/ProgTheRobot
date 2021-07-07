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
        forloop,
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
            foreach (Error error in specificErrors.Values)
            {
                returnString += " - " + error.ToString() + Environment.NewLine;
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
                return new ValidationReturn(ValidationStatus.KO);
            case ValidationType.forloop:
                return new ValidationReturn(ValidationStatus.KO);
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
    /// <param name="toValidate">this string should be only in english</param>
    /// <returns>An object containing the validation status and errors</returns>
    private static ValidationReturn ValidateTest(string toValidate)
    {
        string toValidateNonAltered = toValidate;
        ValidationReturn vr = new ValidationReturn(ValidationStatus.OK);

        string regexPattern = @"[^a-z^A-Z^0-9^+^\-^*^/^(^)^>^<^=^\s]+";
        Regex regex1 = new Regex(regexPattern);
        if(regex1.IsMatch(toValidate))
        {
            vr.ChangeValidationStatus(ValidationStatus.KO);
            Match match = regex1.Match(toValidate);
            vr.AddSpecificError((uint)match.Index, new ValidationReturn.Error((uint)match.Index, (uint)match.Index + (uint)match.Length, $"\"{match.Value}\" means nothing"));
        }

        toValidate = FullNameToAbrev(toValidate);


        string[] stringsToFind = new string[] { "Or", "And" };
        string[] exprSplit = toValidate.Split(stringsToFind, StringSplitOptions.None);

        string findIndexAndOr = @"Or|And";
        Regex findIndexAndOrRegex = new Regex(findIndexAndOr);
        MatchCollection mc = findIndexAndOrRegex.Matches(toValidateNonAltered);

        // used to add to the posInStartString to keep the error at the right position since "And" and "Or" keyword are removed from the main string
        Dictionary<int, uint> andOrPlaceLength = new Dictionary<int, uint>();
        foreach (Match matchItem in mc)
        {
            andOrPlaceLength.Add(matchItem.Index, (uint)matchItem.Length);
        }

        uint posInStartString = 0;

        foreach (string item in exprSplit)
        {
            if(andOrPlaceLength.ContainsKey((int)posInStartString))
            {
                posInStartString += andOrPlaceLength[(int)posInStartString] + 1;
            }

            string[] separators = new string[] { " " };
            string[] smallExprSplit = item.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            uint codeBlockLength = 0;
            switch (smallExprSplit.Length)
            {
                // get the corresponding function, if it exist, and adds it to the result list
                case 1:
                    if (abrevToFullName.ContainsKey(smallExprSplit[0]))
                        codeBlockLength = (uint)abrevToFullName[smallExprSplit[0]].Length + 1;
                    else
                        codeBlockLength = (uint)smallExprSplit[0].Length + 1;

                    if (!(smallExprSplit[0][0] == 'b' && abrevToFullName.ContainsKey(smallExprSplit[0])))
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, "Is not a valid boolean function."));
                    }
                    posInStartString += codeBlockLength;
                    break;
                // get the corresponding function, if it exist, invert the result if there is a No or Non at the begining and adds it to the result list
                case 2:
                    codeBlockLength = (uint)smallExprSplit[0].Length + 2; // 2 = space between the 2 texts + a space a the end
                    if (abrevToFullName.ContainsKey(smallExprSplit[1]))
                        codeBlockLength += (uint)abrevToFullName[smallExprSplit[1]].Length;
                    else
                        codeBlockLength += (uint)smallExprSplit[1].Length;

                    if (smallExprSplit[0] != "No")
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, $"The keyword \"{smallExprSplit[0]}\" is unknown."));
                    }
                    else
                    {
                        if(!(smallExprSplit[1][0] == 'b' && abrevToFullName.ContainsKey(smallExprSplit[1])))
                        {
                            vr.ChangeValidationStatus(ValidationStatus.KO);
                            vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, "Is not a valid boolean function."));
                        }
                    }
                    posInStartString += codeBlockLength;
                    break;
                // evaluate an expression like this one : test + 2 = myVar + 4
                default:
                    string[] delimiters = new string[] { "=", "<", ">", ">=", "<=", "<>" };

                    // every string while be replaced by the number 1 to test if the expression is correct with a datatable
                    List<string> exprPart1 = new List<string>();
                    List<string> exprPart2 = new List<string>();
                    string pattern = @"^[a-zA-Z]+$";
                    Regex regex = new Regex(pattern);

                    uint posInStringAtStartOfTest = posInStartString;

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
                                    break;
                                }
                            }
                            if (!findDelimiter)
                            {
                                if(regex.IsMatch(exprBits))
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
                        if (exprBits[0] == 'b' && abrevToFullName.ContainsKey(exprBits))
                        {
                            codeBlockLength = (uint)abrevToFullName[exprBits].Length + 1; // space after this word

                            vr.ChangeValidationStatus(ValidationStatus.KO);
                            vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, "Only integer function and variable can be used in this context."));
                        }
                        else if (exprBits[0] == 'i' && abrevToFullName.ContainsKey(exprBits))
                        {
                            codeBlockLength = (uint)abrevToFullName[exprBits].Length + 1; // space after this word
                        }
                        else
                        {
                            codeBlockLength = (uint)exprBits.Length + 1; // +1 = space after this word
                        }
                        posInStartString += codeBlockLength;
                    }
                    if (exprPart1.Count <= 0 || exprPart2.Count <= 0)
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStringAtStartOfTest, new ValidationReturn.Error(posInStringAtStartOfTest, posInStartString, $"You need two things to compare."));
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
                            vr.AddSpecificError(posInStringAtStartOfTest, new ValidationReturn.Error(posInStringAtStartOfTest, posInStartString, $"{e.Message}")); // need to display custom errors in order to be localized
                        }

                    }
                    break;
            }
        }
        return vr;
    }

    private static ValidationReturn ValidateAction(string toValidate)
    {
        return new ValidationReturn(ValidationStatus.KO);
    }

    private static ValidationReturn ValidateForLoop(string[] toValidate)
    {
        return new ValidationReturn(ValidationStatus.KO);
    }

    // all of this should be moved else where

    /* one string in clear language and one string with function name abreviated
     * displayed string :   Wall distance = 10 And Wall right
     * abreviated string :  iwd = 10 And bwr
     * i = integer function and b = boolean function
     */
    // should be loaded dynamicly with a language file in the future
    public static Dictionary<string, string> abrevToFullName = new Dictionary<string, string>();
    public static Dictionary<string, string> fullNameToAbrev = new Dictionary<string, string>()
    {
        // boolean func
        {"Wall in front","bwf"},
        {"Wall right","bwr"},
        {"Wall left","bwl"},
        {"Out","bo"},
        {"Robot on an outlet","boao"},
        {"Tile marked","btm"},
        {"Ball on the ground","bbg"},
        // int func
        {"Wall distance","iwd"},
        {"Power","ip"},
        {"x robot","ixr"},
        {"y robot","iyr"},
        {"dx robot","idxr"},
        {"dy robot","idyr"},
        {"x ball","ixb"},
        {"y ball","iyb"},
    };

    public static void InverseKV()
    {
        foreach (KeyValuePair<string,string> item in fullNameToAbrev)
        {
            abrevToFullName.Add(item.Value, item.Key);
        }
    }

    private static string FullNameToAbrev(string toConvert)
    {
        foreach (KeyValuePair<string, string> item in fullNameToAbrev)
        {
            toConvert = toConvert.Replace(item.Key, item.Value);
        }
        return toConvert;
    }
}
