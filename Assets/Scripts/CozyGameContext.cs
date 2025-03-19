using System.Collections.Generic;
using UnityEngine;

public class CopyGameContext : DefaultExpressionContextEvaluator
{
    protected void Teleport(string subjectTagName, string targetTagName)
    {
        Debug.Log($"Teleport {subjectTagName} to {targetTagName}");
    }
}
