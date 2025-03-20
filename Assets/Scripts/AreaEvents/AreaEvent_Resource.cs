using UnityEngine;
using NaughtyAttributes;
using System;

[Serializable]
public class AreaEventCondition
{
    public enum ConditionType { ResourceValue, Expression };
    public enum Comparison { Less, LessEqual, Greater, GreaterEqual, Equal, NotEqual };

    [SerializeField]
    private ConditionType   conditionType;
    [SerializeField, ShowIf(nameof(needTargetTag))]
    private Hypertag        targetTag;
    [SerializeField, ShowIf(nameof(needResource))]
    private ResourceType    resourceType;
    [SerializeField, ShowIf(nameof(needComparison))]
    private Comparison      comparison;
    [SerializeField, ShowIf(nameof(needComparison))]
    private float           refValue;
    [SerializeField, ShowIf(nameof(needExpression))]
    private string          expression; 

    bool needTargetTag => conditionType == ConditionType.ResourceValue;
    bool needResource => conditionType == ConditionType.ResourceValue;
    bool needComparison => conditionType == ConditionType.ResourceValue;
    bool needExpression => conditionType == ConditionType.Expression;

    public bool CheckCondition(UCExpression.IContext context)
    {
        switch (conditionType)
        {
            case ConditionType.ResourceValue:
                {
                    var obj = Hypertag.FindFirstObjectWithHypertag<Transform>(targetTag);
                    if (obj == null) return false;

                    var resHandler = obj.FindResourceHandler(resourceType);
                    if (resHandler == null) return false;

                    float value = resHandler.resource;

                    switch (comparison)
                    {
                        case Comparison.Less:
                            return value < refValue;
                        case Comparison.LessEqual:
                            return value <= refValue;
                        case Comparison.Greater:
                            return value > refValue;
                        case Comparison.GreaterEqual:
                            return value >= refValue;
                        case Comparison.Equal:
                            return value == refValue;
                        case Comparison.NotEqual:
                            return value != refValue;
                    }
                }
                break;
            case ConditionType.Expression:
                {
                    if (UCExpression.TryParse(expression, out var parsedExpression))
                    {
                        return parsedExpression.EvaluateBool(context);
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to parse expression {expression}!");
                    }
                }
                break;
            default:
                break;
        }

        return false;
    }
}
