using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI
{
    public static class UITextFormatter
    {
        public static string CutOffNumericalPart(string SourceString, bool AddSpace = true)
        {
            if (AddSpace)
            {
                return SourceString.Substring(0, SourceString.IndexOf(":") + 1) + " ";
            }
            else
            {
                return SourceString.Substring(0, SourceString.IndexOf(":") + 1);
            }
        }
        public static string ChangeNumericalPart(string sourceString, float NewNumber)
        {
            return sourceString.Substring(0, sourceString.IndexOf(":") + 1) + " " + NewNumber.ToString();
        }
    }
}
