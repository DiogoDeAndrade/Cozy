using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public static class LifeLightUtils
{
    public static bool IsLit(Vector3 pos, ResourceType lightResType, LayerMask layerMask, Life excludeObject = null)
    {
        // Find all life
        float hitDistance = 0.0f;

        var lifeObjects = GameObject.FindObjectsByType<Life>(FindObjectsSortMode.None);

        foreach (var lifeObject in lifeObjects)
        {
            if (lifeObject == excludeObject) continue;
            if (IsLit(lifeObject, pos, lightResType, layerMask, ref hitDistance)) return true;
        }

        return false;
    }

    public static bool IsLit5(Vector3 pos, Vector2 size, ResourceType lightResType, LayerMask layerMask, Life excludeObject = null)
    {
        // Find all life
        float hitDistance = 0.0f;

        var lifeObjects = GameObject.FindObjectsByType<Life>(FindObjectsSortMode.None);

        foreach (var lifeObject in lifeObjects)
        {
            if (lifeObject == excludeObject) continue;
            if (IsLit5(lifeObject, pos, size, lightResType, layerMask, ref hitDistance)) return true;
        }

        return false;
    }

    public static bool IsLit(Life lifeObject, Vector3 pos, ResourceType lightResType, LayerMask layerMask, ref float hitDistance)
    {
        // Check if it is emitting (it can die if not enough life light itself)
        var lightLifeHandler = lifeObject.FindResourceHandler(lightResType);
        if (lightLifeHandler == null) return false;
        if (lightLifeHandler.normalizedResource <= 0.0f) return false;

        // Check distance
        Vector3 deltaPos = lifeObject.transform.position - pos;
        float distance = deltaPos.magnitude;
        if (distance > lifeObject.radius)
        {
            hitDistance = distance - lifeObject.radius;
            return false;
        }

        // Check LOS
        var hit = Physics2D.Raycast(pos, deltaPos.SafeNormalized(), distance, layerMask);
        if (hit.collider != null)
        {
            hitDistance = hit.distance;
            return false;
        }

        return true;
    }

    public static bool IsLit5(Life lifeObject, Vector3 pos, Vector2 size, ResourceType lightResType, LayerMask layerMask, ref float hitDistance)
    {
        if (IsLit(lifeObject, new Vector3(pos.x, pos.y, pos.z), lightResType, layerMask, ref hitDistance)) return true;
        if (IsLit(lifeObject, new Vector3(pos.x + size.x, pos.y, pos.z), lightResType, layerMask, ref hitDistance)) return true;
        if (IsLit(lifeObject, new Vector3(pos.x - size.x, pos.y, pos.z), lightResType, layerMask, ref hitDistance)) return true;
        if (IsLit(lifeObject, new Vector3(pos.x, pos.y + size.y, pos.z), lightResType, layerMask, ref hitDistance)) return true;
        if (IsLit(lifeObject, new Vector3(pos.x, pos.y - size.y, pos.z), lightResType, layerMask, ref hitDistance)) return true;

        return false;
    }
}
