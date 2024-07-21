using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct CoreSurfacePathData
{
    public float timeDelay;
    public fixed float contributions[ReverbManager.SampleFrequenciesCount];
    public Vector3 inputPosition;
    public Vector3 inputNormal;
    public Vector3 inputPathDirection;
    public Vector3 outputPosition;
    public Vector3 outputNormal;
    public Vector3 outputPathDirection;
    
    
    public CoreSurfacePathData(float[] contributions, float timeDelay, Vector3 inputPosition, Vector3 outputPosition,Vector3 inputNormal, Vector3 outputNormal, Vector3 inputPathDirection, Vector3 outputPathDirection)
    {
        this.timeDelay = timeDelay;
        this.inputPosition = inputPosition;
        this.outputPosition = outputPosition;
        this.inputNormal = inputNormal;
        this.outputNormal = outputNormal;
        this.inputPathDirection = inputPathDirection;
        this.outputPathDirection = outputPathDirection;

        for (int i = 0; i < ReverbManager.SampleFrequenciesCount; i++)
        {
            this.contributions[i] = contributions[i];
        }
    }
    
        public static int GetSize()=> sizeof(float) + ReverbManager.SampleFrequenciesCount * sizeof(float) + 18 * sizeof(float);
}

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

    public CoreSurfacePathData GetCoreData()
    {
        return new CoreSurfacePathData(this.contributions, this.timeDelay, this.surfacePoints[0].positionWS,
            this.surfacePoints[^1].positionWS, this.surfacePoints[0].normalWS,this.surfacePoints[^1].normalWS,
            Vector3.Normalize(this.surfacePoints[1].positionWS - this.surfacePoints[0].positionWS),
            Vector3.Normalize(this.surfacePoints[^2].positionWS - this.surfacePoints[^1].positionWS));
    }
    
    private static float CalculateTimeDelay(List<SurfacePoint> surfacePoints)
    {
        float speedOfSound = 343f;
        return surfacePoints.PathLength() / speedOfSound;
    }

    private static float[] CalculateContributions(List<SurfacePoint> surfacePoints, List<float> sampleFrequencies)
    {
        float[] contributions = new float[sampleFrequencies.Count];
        for (int i = 0; i < sampleFrequencies.Count; i++)
        {
            float contribution = 1f;
            for (int j = 1; j < surfacePoints.Count - 1; j++)
            {
                contribution *= BRDF.SimpleSpecular(surfacePoints[j - 1], surfacePoints[j], surfacePoints[j + 1],
                    sampleFrequencies[i]);
                
            }

            contributions[i] = contribution;
        }

        return contributions;
    }
}