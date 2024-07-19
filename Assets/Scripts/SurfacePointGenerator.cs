using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class SurfacePointGenerator : MonoBehaviour, ISurfacePointProvider
{
    public Bounds generationBounds;
    public int scansPerTracerScanArea;
    public float tracerScanSize;
    public List<Vector3> tracerScanPositions;
    public bool shouldDrawTracerScanAreas;
    public bool shouldDrawBounds;
    public bool shouldDrawSurfacePoints;
    [SerializeField]
    public List<SurfacePoint> surfacePoints;

    public List<SurfacePoint> GetSurfacePoints()
    {
        return surfacePoints;
    }

    public SurfacePoint GetRandomSurfacePoint()
    {
        return surfacePoints[Random.Range(0, surfacePoints.Count)];
    }

    public void FixBoundsSize()
    {
        Vector3 originalSize = generationBounds.size;
        Vector3 adjustedSize = new Vector3();

        for (int i = 0; i < 3; i++)
        {
            float component = originalSize[i];

            if (Mathf.Abs(component % tracerScanSize) < float.Epsilon * 10f)
            {
                adjustedSize[i] = component;
            }
            else
            {
                adjustedSize[i] = Mathf.Ceil(component / tracerScanSize) * tracerScanSize;
            }
        }

        generationBounds.size = adjustedSize;
    }

    public void GenerateSurfacePoints()
    {
        FixBoundsSize();
        GenerateTracerScanPositions();
        surfacePoints.Clear();
        TraceAndScan();
    }

    private void ScanPosition(Vector3 tracerScanPosition, int scansCount)
    {
        //todo: Distribute sample directions in more uniform way
        for (int i = 0; i < scansCount; i++)
        {
            Vector3 scanDirection = Random.insideUnitSphere.normalized;
            var hit = Physics.Raycast(tracerScanPosition, scanDirection, out RaycastHit hitInfo, 2f * tracerScanSize);

            if (hit)
            {
                surfacePoints.Add(new SurfacePoint()
                {
                    positionWS = hitInfo.point,
                    normalWS = hitInfo.normal
                });
            }
        }
    }

    private void TraceAndScan()
    {
        foreach (Vector3 tracerScanPosition in tracerScanPositions)
        {
            ScanPosition(tracerScanPosition, scansPerTracerScanArea);
        }
    }

    private void GenerateTracerScanPositions()
    {
        tracerScanPositions.Clear();

        Vector3 tracerScanPositionsCountPerAlignedGrid = new Vector3(
            MathF.Round(generationBounds.size.x / tracerScanSize),
            MathF.Round(generationBounds.size.y / tracerScanSize),
            MathF.Round(generationBounds.size.z / tracerScanSize)
        );

        for (int x = 0; x < tracerScanPositionsCountPerAlignedGrid.x; x++)
        {
            for (int y = 0; y < tracerScanPositionsCountPerAlignedGrid.y; y++)
            {
                for (int z = 0; z < tracerScanPositionsCountPerAlignedGrid.z; z++)
                {
                    tracerScanPositions.Add(new Vector3(
                        generationBounds.min.x + (x + 0.5f) * tracerScanSize,
                        generationBounds.min.y + (y + 0.5f) * tracerScanSize,
                        generationBounds.min.z + (z + 0.5f) * tracerScanSize));
                }
            }
        }
    }

    private void DrawBounds()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
        Gizmos.DrawCube(generationBounds.center, generationBounds.size);
    }

    private void DrawTracerScanAreas()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 tracerScanPosition in tracerScanPositions)
        {
            Gizmos.DrawWireCube(tracerScanPosition, tracerScanSize * Vector3.one);
        }
    }

    private void DrawSurfacePoints()
    {
        Gizmos.color = Color.cyan;
        foreach (SurfacePoint surfacePoint in surfacePoints)
        {
            Gizmos.DrawRay(surfacePoint.positionWS, 0.15f * surfacePoint.normalWS);
        }
    }

    private void OnDrawGizmos()
    {
        if (shouldDrawBounds)
        {
            DrawBounds();
        }

        if (shouldDrawTracerScanAreas)
        {
            DrawTracerScanAreas();
        }

        if (shouldDrawSurfacePoints)
        {
            DrawSurfacePoints();
        }
    }
}

[CustomEditor(typeof(SurfacePointGenerator))]
public class SurfacePointGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SurfacePointGenerator generator = (SurfacePointGenerator)target;

        if (GUILayout.Button("Generate surface points"))
        {
            Undo.RecordObject(generator, "Generated surface points");
            generator.GenerateSurfacePoints();
        }

        if (GUILayout.Button("Fix generator bounds"))
        {
            Undo.RecordObject(generator, "Fixed bounds size");
            generator.FixBoundsSize();
        }
    }
}