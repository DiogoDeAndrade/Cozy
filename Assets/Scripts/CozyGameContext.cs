using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CopyGameContext : DefaultExpressionContextEvaluator
{
    protected bool Teleport(string subjectTagName, string targetTagName)
    {
        var subjectTag = GetTagByName(subjectTagName);
        if (subjectTag == null)
        {
            return false;
        }
        var subject = Hypertag.FindFirstObjectWithHypertag<GridObject>(subjectTag);
        if (subject == null)
        {
            Debug.LogError($"Can't find subject tagged with {subjectTagName}");
            return false;
        }
        var targetTag = GetTagByName(targetTagName);
        if (targetTag == null)
        {
            return false;
        }
        var target = Hypertag.FindFirstObjectWithHypertag<Transform>(targetTag);
        if (target == null)
        {
            Debug.LogError($"Can't find target tagged with {targetTagName}");
            return false;
        }

        subject.TeleportTo(target.position);

        return true;
    }
}
