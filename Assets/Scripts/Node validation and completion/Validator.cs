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
            if(!specificErrors.ContainsKey(pos))
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
        if(toValidate == "")
            return new ValidationReturn(ValidationStatus.OK);

        string toValidateNonAltered = toValidate;
        ValidationReturn vr = new ValidationReturn(ValidationStatus.OK);

        // need to test with this [^a-zA-Z...] should work
        string regexPattern = @"[^a-z^A-Z^0-9^+^\-^*^/^(^)^>^<^=^\s]+";
        Regex regex1 = new Regex(regexPattern);
        if(regex1.IsMatch(toValidate))
        {
            vr.ChangeValidationStatus(ValidationStatus.KO);
            Match match = regex1.Match(toValidate);
            vr.AddSpecificError((uint)match.Index, new ValidationReturn.Error((uint)match.Index, (uint)match.Index + (uint)match.Length, $"\"{match.Value}\" means nothing"));
            return vr;
        }

        toValidate = LanguageManager.instance.FullNameToAbrev(toValidate);


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
            string fullname = "";
            switch (smallExprSplit.Length)
            {
                // get the corresponding function, if it exist, and adds it to the result list
                case 1:
                    fullname = LanguageManager.instance.getFullnameFromAbrev(smallExprSplit[0]);
                    if (fullname != null)
                        codeBlockLength = (uint)fullname.Length;
                    else
                        codeBlockLength = (uint)smallExprSplit[0].Length;

                    if (!(smallExprSplit[0][0] == 'b' && LanguageManager.instance.AbrevToFullNameContainsKey(smallExprSplit[0])))
                    {
                        if(!(smallExprSplit[0] == "False" || smallExprSplit[0] == "True"))
                        {
                            vr.ChangeValidationStatus(ValidationStatus.KO);
                            vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, "Is not a valid boolean function."));
                        }
                    }
                    posInStartString += codeBlockLength + 1; // +1 = space after this word
                    break;
                // get the corresponding function, if it exist, invert the result if there is a No or Non at the begining and adds it to the result list
                case 2:
                    codeBlockLength = (uint)smallExprSplit[0].Length + 1; // +1 = space after this word

                    fullname = LanguageManager.instance.getFullnameFromAbrev(smallExprSplit[1]);
                    if (fullname != null)
                        codeBlockLength += (uint)fullname.Length;
                    else
                        codeBlockLength += (uint)smallExprSplit[1].Length;
                    if (smallExprSplit[0] != "Not")
                    {
                        vr.ChangeValidationStatus(ValidationStatus.KO);
                        vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, $"The keyword \"{smallExprSplit[0]}\" is unknown."));
                    }
                    else
                    {
                        if(!(smallExprSplit[1][0] == 'b' && LanguageManager.instance.AbrevToFullNameContainsKey(smallExprSplit[1])))
                        {
                            if (!(smallExprSplit[1] == "False" || smallExprSplit[1] == "True"))
                            {
                                vr.ChangeValidationStatus(ValidationStatus.KO);
                                vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, "Is not a valid boolean function."));
                            }
                        }
                    }
                    posInStartString += codeBlockLength + 1; // +1 = space after this word
                    break;
                // evaluate an expression like this one : test + 2 = myVar + 4
                default:
                    string[] delimiters = new string[] { "=", "<", ">", ">=", "<=", "<>" };

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

                        if (exprBits[0] == 'b' && LanguageManager.instance.AbrevToFullNameContainsKey(exprBits))
                        {
                            codeBlockLength = (uint)LanguageManager.instance.getFullnameFromAbrev(exprBits).Length; // space after this word

                            vr.ChangeValidationStatus(ValidationStatus.KO);
                            vr.AddSpecificError(posInStartString, new ValidationReturn.Error(posInStartString, posInStartString + codeBlockLength, "Only integer function and variable can be used in this context."));
                        }
                        else if (exprBits[0] == 'i' && LanguageManager.instance.AbrevToFullNameContainsKey(exprBits))
                        {
                            codeBlockLength = (uint)LanguageManager.instance.getFullnameFromAbrev(exprBits).Length; // space after this word
                        }
                        else
                        {
                            codeBlockLength = (uint)exprBits.Length; 
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

    private static ValidationReturn ValidateAction(string toValidate)
    {
        return new ValidationReturn(ValidationStatus.KO);
    }

    private static ValidationReturn ValidateForLoop(string[] toValidate)
    {
        return new ValidationReturn(ValidationStatus.KO);
    }
}
