using UnityEngine;

public static class ObjectDetector
{
    public static Vector3Int? RaycastGround(Ray ray)
    {
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
        {
            Debug.Log($"Hit: {hit.collider.name}");
            if (hit.transform.GetComponent<RoadGround>() != null)
            {
                Vector3Int positionInt = Vector3Int.RoundToInt(hit.point);
                return positionInt;  
            }
        }

        return null;
    }

    public static GameObject RaycastAll(Ray ray)
    {
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
        {
            return hit.transform.gameObject;
        }

        return null;
    }
}