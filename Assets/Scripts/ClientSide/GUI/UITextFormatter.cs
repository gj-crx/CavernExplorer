using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
