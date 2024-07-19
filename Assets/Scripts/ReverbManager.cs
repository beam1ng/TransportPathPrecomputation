using UnityEngine;

public class ReverbManager : MonoBehaviour
{
    [SerializeField]
    private RenderTexture impulseResponseTexture;

    [SerializeField]
    private Material impulseResponseMaterial;
    
    void Update()
    {
        Graphics.Blit(null,impulseResponseTexture,impulseResponseMaterial);
    }
}
