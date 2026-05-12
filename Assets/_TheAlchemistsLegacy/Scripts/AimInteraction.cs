using UnityEngine;

public static class AimInteraction
{
    public const float DefaultRadius = 0.22f;

    public static bool Cast(Camera playerCamera, float range, out RaycastHit hit)
    {
        return Cast(playerCamera, range, DefaultRadius, ~0, QueryTriggerInteraction.Collide, out hit);
    }

    public static bool Cast(Camera playerCamera, float range, float radius, out RaycastHit hit)
    {
        return Cast(playerCamera, range, radius, ~0, QueryTriggerInteraction.Collide, out hit);
    }

    public static bool Cast(Camera playerCamera, float range, float radius, LayerMask layerMask, out RaycastHit hit)
    {
        return Cast(playerCamera, range, radius, layerMask, QueryTriggerInteraction.Collide, out hit);
    }

    public static bool Cast(Camera playerCamera, float range, float radius, LayerMask layerMask, QueryTriggerInteraction triggerInteraction, out RaycastHit hit)
    {
        hit = default;
        if (playerCamera == null)
        {
            return false;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        float safeRadius = Mathf.Max(0.0f, radius);

        if (safeRadius > 0.0f && Physics.SphereCast(ray, safeRadius, out hit, range, layerMask, triggerInteraction))
        {
            return true;
        }

        return Physics.Raycast(ray, out hit, range, layerMask, triggerInteraction);
    }
}
