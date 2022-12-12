using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI
{
    public static class UITextFormatter
    {
        public static string CutOffNumericalPart(string sourceString, bool addSpace = true)
        {
            if (addSpace)
            {
                return sourceString.Substring(0, sourceString.IndexOf(":") + 1) + " ";
            }
            else
            {
                return sourceString.Substring(0, sourceString.IndexOf(":") + 1);
            }
        }
        public static string ChangeNumericalPart(string sourceString, float NewNumber)
        {
            return sourceString.Substring(0, sourceString.IndexOf(":") + 1) + " " + NewNumber.ToString();
        }
        public static string FormateFloat(float valueToFormate, int NumbersAfterPoint)
        {
            int pointIndex = valueToFormate.ToString().IndexOf(',');
            Debug.Log(pointIndex + " out of " + valueToFormate.ToString().Length);
            if (pointIndex == -1)
            {
                pointIndex = valueToFormate.ToString().Length;
                NumbersAfterPoint = 1;
            }
            return valueToFormate.ToString().Substring(0, pointIndex + NumbersAfterPoint);
        }
    }
}
