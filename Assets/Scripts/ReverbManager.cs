using UnityEngine;
using UnityEngine.Rendering;

public class ReverbManager : MonoBehaviour
{
    [SerializeField]
    private RenderTexture impulseResponseTexture;

    [SerializeField]
    private Material impulseResponseMaterial;
    
    private ComputeBuffer argsBuffer;

    private void Start()
    {
        argsBuffer = new ComputeBuffer(1,4 * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint[] argsData = { 2, 100, 0, 0 };
        argsBuffer.SetData(argsData);
    }

    void Update()
    {
        CommandBuffer cmd = CommandBufferPool.Get("ImpulseResponseGeneration");
        cmd.SetRenderTarget(impulseResponseTexture);
        cmd.ClearRenderTarget(false,true,Color.black);
        cmd.DrawProceduralIndirect(Matrix4x4.identity,impulseResponseMaterial,0,MeshTopology.Lines,argsBuffer);
        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Dispose();
    }
}
