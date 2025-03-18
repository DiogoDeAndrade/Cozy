using System.Collections.Generic;
using UnityEngine;

public class DefaultExpressionContextEvaluator : MonoBehaviour, UCExpression.IContext
{
    Dictionary<string, object> variables = new();

    public bool GetVarBool(string varName)
    {        
        if (variables.TryGetValue(varName, out object value))
        {
            if (value is bool boolValue) return boolValue;
        }
        return false;
    }

    public float GetVarNumber(string varName)
    {
        if (variables.TryGetValue(varName, out object value))
        {
            if (value is float floatValue) return floatValue;
        }
        return 0.0f;
    }

    public UCExpression.DataType GetDataType(string varName)
    {
        if (variables.TryGetValue(varName, out object value))
        {
            if (value is float) return UCExpression.DataType.Number;
            if (value is bool) return UCExpression.DataType.Bool;
        }
        return UCExpression.DataType.Bool;
    }

    public void SetVariable(string varName, float value)
    {
        variables[varName] = value;
    }

    public void SetVariable(string varName, bool value)
    {
        variables[varName] = value;
    }

    public void TestFunction(bool b, float abc)
    {
        Debug.Log($"TestFunction called with parameters = ({b}, {abc})");
    }

    public void Close()
    {
        DialogueManager.Instance.EndDialogue();
    }
}
