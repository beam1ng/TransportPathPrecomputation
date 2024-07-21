using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SurfacePathGenerator: MonoBehaviour
{
    public int maxPathCount;
    public float maxPathDistance;
    public int maxTriesPerPoint;
    
    [SerializeField]
    public List<SurfacePath> surfacePaths = new();
    public bool drawPaths;
    public float pathSelectionInterval = 1f;
    
    private ISurfacePointProvider surfacePointProvider;
    private SurfacePath currentPath;
    private float nextSelectionTime;

    private bool CheckSurfacePointVisibility(SurfacePoint origin, SurfacePoint destination, float maxDistance)
    {
        if (Vector3.Dot(origin.normalWS, destination.normalWS) < 0)
        {
            return false;
        }

        if (Vector3.Dot(origin.positionWS - destination.positionWS, destination.normalWS) < 0)
        {
            return false;
        }
        if (Vector3.Dot(destination.positionWS - origin.positionWS, origin.normalWS) < 0)
        {
            return false;
        }
        
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
        
        while (tries++ < maxTriesPerPoint)
        {
            SurfacePoint nextPoint = surfacePointProvider.GetRandomSurfacePoint();
            bool visible = CheckSurfacePointVisibility(surfacePathPoints[^1], nextPoint ,maxPathDistance - pathDistance);
            if (!visible)
            {
                continue;
            }
            
            surfacePathPoints.Add(nextPoint);
            pathDistance += surfacePathPoints[^2].GetDistanceTo(surfacePathPoints[^1]);
            
            surfacePaths.Add(new SurfacePath(surfacePathPoints,ReverbManager.SampleFrequencies));
            
        }
        
    }

    private void ApplyPathCountLimit()
    {
        surfacePaths = surfacePaths.OrderBy(_ => Random.value).Take(maxPathCount).ToList();
    }

    public void GenerateSurfacePaths()
    {
        surfacePaths.Clear();
        surfacePointProvider = GetComponent<SurfacePointGenerator>();
        List<SurfacePoint> points = surfacePointProvider.GetSurfacePoints();
        for (int i = 0; i < points.Count; i++)
        {
            SurfacePoint t = points[i];
            Debug.Log($"{i}/{points.Count}");
            GeneratePathsFromPoint(t);
        }

        ApplyPathCountLimit();
    }

    private void OnDrawGizmos()
    {
        if (drawPaths)
        {
            if (Time.time >= nextSelectionTime)
            {
                int newPathIndex = Random.Range(0, surfacePaths.Count / 10);
                currentPath = surfacePaths.OrderByDescending(s => s.surfacePoints.Count)
                    .ElementAtOrDefault(newPathIndex);
                nextSelectionTime = Time.time + pathSelectionInterval;
            }
            
            Gizmos.color = Color.magenta;
            for (int i = 0; i < currentPath.surfacePoints.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath.surfacePoints[i].positionWS, currentPath.surfacePoints[i + 1].positionWS);
            }
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