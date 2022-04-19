using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GetCollider
{
    private static Collider nearestCollider;
    public static Collider GetColliderAround(Transform origin, float radius, Collider[] result, LayerMask layerCheck)
    {
        int targetIndex = Physics.OverlapSphereNonAlloc(origin.position, radius, result, layerCheck, QueryTriggerInteraction.UseGlobal);

        if (targetIndex > 0)
        {
            nearestCollider = result[0];
            for (int i = 0; i < targetIndex; i++)
            {
                if (Vector3.Distance(nearestCollider.transform.position, origin.position) > Vector3.Distance(result[i].transform.position, origin.position))
                    nearestCollider = result[i];
            }
            return nearestCollider;
        }
        return null;
    }


    public static Collider GetMouseCollider(Vector2 mousePosition, LayerMask layerCheck)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerCheck))
        {
            return hit.collider;
        }
        return null;
    }
}
