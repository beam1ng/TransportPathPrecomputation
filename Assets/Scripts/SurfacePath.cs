using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct SurfacePath
{
    [SerializeField]
    public List<SurfacePoint> surfacePoints;
    public float timeDelay;
    public float[] contributions;

    public SurfacePath(List<SurfacePoint> surfacePoints, List<float> sampleFrequencies)
    {
        this.surfacePoints = surfacePoints;
        this.timeDelay = CalculateTimeDelay(this.surfacePoints);
        this.contributions = CalculateContributions(surfacePoints, sampleFrequencies);
    }

    private static float CalculateTimeDelay(List<SurfacePoint> surfacePoints)
    {
        float speedOfLight = 343f;
        return surfacePoints.PathLength() / speedOfLight;
    }

    private static float[] CalculateContributions(List<SurfacePoint> surfacePoints, List<float> sampleFrequencies)
    {
        float[] contributions = new float[sampleFrequencies.Count];
        for (int i = 0; i < sampleFrequencies.Count; i++)
        {
            float contribution = 1f;
            for (int j = 1; j < surfacePoints.Count - 1; j++)
            {
                contribution *= BRDF.SimpleLambertian(surfacePoints[j - 1], surfacePoints[j], surfacePoints[j + 1],
                    sampleFrequencies[i]);
                
            }

            contributions[i] = contribution;
        }

        return contributions;
    }
}