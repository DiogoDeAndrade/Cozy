using UnityEngine;

public class ExpressionContextEvaluator : MonoBehaviour, UCExpression.IContext
{
    public bool EvaluateBool(string varName)
    {        
        return DialogueManager.HasDialogueEvent(varName, int.MaxValue);
    }

    public float EvaluateNumber(string varName)
    {
        return 0.0f;
    }

    public UCExpression.DataType GetDataType(string varName)
    {
        return UCExpression.DataType.Bool;
    }
}
