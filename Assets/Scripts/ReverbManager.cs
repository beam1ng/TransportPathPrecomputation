using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ReverbManager : MonoBehaviour
{
    public static List<float> SampleFrequencies = new()
    {
        50f, 250f, 450f, 700f, 1000f, 1370f, 1850f, 2500f, 3400f, 4800f, 7000f, 10500f
    };
    
    public const int SampleFrequenciesCount = 12;

    [SerializeField]
    private RenderTexture impulseResponseTexture;

    [SerializeField]
    private Material impulseResponseMaterial;

    [SerializeField]
    private int timeSamples;

    [SerializeField]
    private float timeRange;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioListener audioListener;

    [SerializeField]
    private List<CoreSurfacePathData> surfacePaths;

    private ComputeBuffer argsBuffer;
    private GraphicsBuffer dataBuffer;

    public void UpdateSurfacePaths(List<CoreSurfacePathData> newSurfacePaths)
    {
        this.surfacePaths = newSurfacePaths;
    }

    private void Start()
    {
        ArgsBufferSetup();
        DataBufferSetup();
        ImpulseResponseTextureSetup();
    }

    private void ArgsBufferSetup()
    {
        argsBuffer = new ComputeBuffer(1, 4 * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint[] argsData = { 2, (uint)timeSamples, 0, 0 };
        argsBuffer.SetData(argsData);
    }

    private void DataBufferSetup()
    {
        dataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,surfacePaths.Count,CoreSurfacePathData.GetSize());
        dataBuffer.SetData(surfacePaths);
    }

    private void ImpulseResponseTextureSetup()
    {
        //todo: assure the width and height of the texture match the parameters
        // impulseResponseTexture.width = timeSamples;
        // impulseResponseTexture.height = sampleFrequencies.Count;
    }

    void Update()
    {
        CommandBuffer cmd = CommandBufferPool.Get("ImpulseResponseGeneration");
        cmd.SetGlobalVector("_SourcePositionWS", audioSource.transform.position);
        cmd.SetGlobalVector("_ListenerPositionWS", audioListener.transform.position);
        cmd.SetGlobalInt("_TimeSamples", timeSamples);
        cmd.SetGlobalFloat("_TimeRange", timeRange);
        cmd.SetGlobalInt("_SampleFrequenciesCount", SampleFrequencies.Count);
        cmd.SetRenderTarget(impulseResponseTexture);
        cmd.ClearRenderTarget(false, true, Color.black);
        cmd.SetGlobalBuffer("_SurfacePaths",dataBuffer);
        cmd.DrawProceduralIndirect(Matrix4x4.identity,impulseResponseMaterial,0,MeshTopology.Lines,argsBuffer);
        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Dispose();
    }
}