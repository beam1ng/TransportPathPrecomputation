using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class SurfacePathGenerator: MonoBehaviour
{
    public ISurfacePointProvider SurfacePointProvider;
    public int maxPathCount;
    public float maxPathDistance;
    public int maxTriesPerPoint;

    [SerializeField]
    public List<SurfacePath> SurfacePaths;


    private void GeneratePathsFromPoint(SurfacePoint point)
    {
        float pathDistance = 0f;
        int tries = 0;
        while (tries < maxTriesPerPoint)
        {
            //get another point
            //check visibility (continue with invisible)
            //check pathdistance+
            //if valid distance:
            //  add pathdistance+
            //  calculate and add term
            //  calculatetime delay
            
            
            tries++;
        }
    }

    public void GenerateSurfacePaths()
    {
        SurfacePointProvider = GetComponent<SurfacePointGenerator>();
        List<SurfacePoint> points = SurfacePointProvider.GetSurfacePoints();
        for (int i = 0; i < points.Count; i++)
        {
            GeneratePathsFromPoint(points[i]);
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