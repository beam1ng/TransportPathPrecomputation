using System;
using UnityEngine;

public static class BRDF
{
        public static float SimpleSpecular(SurfacePoint originPoint, SurfacePoint impactPoint,
                SurfacePoint destinationPoint, float frequency)
        {
                Vector3 inDirection = (impactPoint.positionWS - originPoint.positionWS).normalized;
                Vector3 outDirection = (destinationPoint.positionWS - originPoint.positionWS).normalized;
                Vector3 perfectReflection = Vector3.Reflect(inDirection, impactPoint.normalWS);
                float intensityFactor = Math.Clamp(Vector3.Dot(perfectReflection, outDirection), 0f, 1f);
                return intensityFactor;
        }
}