using System;
using System.Collections;
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

    public List<float> sampleFrequencies =new ()
    {
        50f, 250f,450f,700f,1000f,1370f,1850f,2500f,3400f,4800f,7000f,10500f
    };
    
    [SerializeField]
    public List<SurfacePath> SurfacePaths = new();
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

        // return CustomRaycastManager.IsDestinationReachable(origin.positionWS, destination.positionWS, maxDistance);
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
            
            SurfacePaths.Add(new SurfacePath(surfacePathPoints,sampleFrequencies));
            
        }
        
    }

    private void ApplyPathCountLimit()
    {
        SurfacePaths = SurfacePaths.OrderBy(_ => Random.value).Take(maxPathCount).ToList();
    }

    public void GenerateSurfacePaths()
    {
        SurfacePaths.Clear();
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
        if (Time.time >= nextSelectionTime)
        {
            int newPathIndex = Random.Range(0, SurfacePaths.Count / 10);
            currentPath = SurfacePaths.OrderByDescending(s => s.surfacePoints.Count)
                .ElementAtOrDefault(newPathIndex);
            nextSelectionTime = Time.time + pathSelectionInterval;
        }
        
        Gizmos.color = Color.magenta;
        for (int i = 0; i < currentPath.surfacePoints.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath.surfacePoints[i].positionWS, currentPath.surfacePoints[i + 1].positionWS);
        }
        
        // //DEBUG
        // float rayLength = 5f;
        // Vector3 rayOffset = new Vector3(0, -0.05f, 0); 
        //
        // UnityEditor.SceneView sceneView = UnityEditor.SceneView.lastActiveSceneView;
        // if (sceneView != null && sceneView.camera != null)
        // {
        //     Vector3 start = sceneView.camera.transform.position + sceneView.camera.transform.TransformDirection(rayOffset);
        //     Vector3 direction = sceneView.camera.transform.forward;
        //     Vector3 end = start + direction * rayLength;
        //
        //     bool hit = Physics.Raycast(start, direction, out RaycastHit hitInfo, rayLength);
        //
        //     Gizmos.color = hit ? Color.red : Color.green;
        //     Gizmos.DrawLine(start, end);
        //
        //     if (hit)
        //     {
        //         Gizmos.color = Color.yellow;
        //         Gizmos.DrawSphere(hitInfo.point, 0.02f);
        //     }
        // }
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