using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Reflection;
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

    public string GetVarString(string varName)
    {
        if (variables.TryGetValue(varName, out object value))
        {
            if (value is string stringValue) return stringValue;
        }
        return "";
    }

    public UCExpression.DataType GetVariableDataType(string varName)
    {
        if (variables.TryGetValue(varName, out object value))
        {
            if (value is float) return UCExpression.DataType.Number;
            if (value is bool) return UCExpression.DataType.Bool;
            if (value is string) return UCExpression.DataType.String;
        }
        return UCExpression.DataType.Undefined;
    }

    public void SetVariable(string varName, float value)
    {
        variables[varName] = value;
    }

    public void SetVariable(string varName, bool value)
    {
        variables[varName] = value;
    }

    public void SetVariable(string varName, string value)
    {
        variables[varName] = value;
    }

    public void Close()
    {
        DialogueManager.Instance.EndDialogue();
    }

    public UCExpression.DataType GetFunctionType(string functionName)
    {
        var type = GetType();
        var methodInfo = type.GetMethod(functionName,
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (methodInfo == null)
        {
            throw new UCExpression.ErrorException($"Function {functionName} not found!");
        }

        if (methodInfo.ReturnType == typeof(bool)) return UCExpression.DataType.Bool;
        if (methodInfo.ReturnType == typeof(float)) return UCExpression.DataType.Number;
        if (methodInfo.ReturnType == typeof(string)) return UCExpression.DataType.String;
        if (methodInfo.ReturnType == typeof(void)) return UCExpression.DataType.None;

        throw new UCExpression.ErrorException($"Unsupported return type {methodInfo.ReturnType} for function {functionName}!");
    }
}
