using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SurfacePathGenerator: MonoBehaviour
{
    public int maxPathCount;
    public float maxPathDistance;
    public int maxTriesPerPoint;
    public List<SurfacePath> SurfacePaths = new();
    
    private ISurfacePointProvider surfacePointProvider;

    private bool CheckSurfacePointVisibility(SurfacePoint origin, SurfacePoint destination, float maxDistance)
    {
        bool hit = Physics.Raycast(origin.positionWS,  (destination.positionWS - origin.positionWS).normalized, out RaycastHit hitInfo, maxDistance);
        if (hit)
        {
            return Math.Abs(hitInfo.distance - origin.GetDistanceTo(destination)) < Single.Epsilon * 10f;
        }

        return false;
    }
    
    private void GeneratePathsFromPoint(SurfacePoint originPoint)
    {
        float pathDistance = 0f;
        int tries = 0;
        List<SurfacePoint> surfacePathPoints = new()
        {
            originPoint
        };
        
        while (tries < maxTriesPerPoint)
        {
            SurfacePoint nextPoint = surfacePointProvider.GetRandomSurfacePoint();
            bool visible = CheckSurfacePointVisibility(originPoint, nextPoint ,maxPathDistance - pathDistance);
            if (!visible)
            {
                continue;
            }
            
            surfacePathPoints.Add(nextPoint);
            pathDistance += surfacePathPoints[^2].GetDistanceTo(surfacePathPoints[^1]);
            
            SurfacePaths.Add(new SurfacePath()
            {
                SurfacePoints = surfacePathPoints
            });
            
            tries++;
        }
        
    }

    public void GenerateSurfacePaths()
    {
        surfacePointProvider = GetComponent<SurfacePointGenerator>();
        List<SurfacePoint> points = surfacePointProvider.GetSurfacePoints();
        foreach (SurfacePoint t in points)
        {
            GeneratePathsFromPoint(t);
        }
    }
}

[CustomEditor(typeof(SurfacePathGenerator))]
public class SurfacePathGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SurfacePathGenerator generator = (SurfacePathGenerator)target;

        if (GUILayout.Button("Generate Surface Paths"))
        {
            generator.GenerateSurfacePaths();
        }
    }
}