using System.Collections.Generic;

public struct SurfacePath
{
    public List<SurfacePoint> SurfacePoints;
    public float TimeDelay;
    public float[] Contributions;

    public SurfacePath(List<SurfacePoint> surfacePoints, List<float> sampleFrequencies)
    {
        this.SurfacePoints = surfacePoints;
        this.TimeDelay = CalculateTimeDelay(SurfacePoints);
        this.Contributions = CalculateContributions(surfacePoints, sampleFrequencies);
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