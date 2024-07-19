using System.Collections.Generic;

public interface ISurfacePointProvider
{
    public List<SurfacePoint> GetSurfacePoints();
    public SurfacePoint GetRandomSurfacePoint();
}