using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SurfacePoint
{
    public Vector3 positionWS;
    public Vector3 normalWS;
}

public static class SurfacePointExtensions
{
    public static float GetDistanceTo(this SurfacePoint origin, SurfacePoint destination)
    {
        return Vector3.Distance(origin.positionWS, destination.positionWS);
    }

    public static float PathLength(this List<SurfacePoint> path)
    {
        float totalDistance = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalDistance += path[i].GetDistanceTo(path[i + 1]);
        }

        return totalDistance;
    }
}