using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-1000)]
public class VFXForceFieldManager : MonoBehaviour
{
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private VFXForceField[] targets;
    [SerializeField] private ComputeShader shader;

    private int _createVectorFieldIndex;
    private int _clearIndex;
    private RenderTexture _texture;

    private bool _setUp = false;

    private void Awake()
    {
        _clearIndex = shader.FindKernel("Clear");
        _createVectorFieldIndex = shader.FindKernel("CreateVectorField");
    }

    private void Update()
    {
        if (!visualEffect.isActiveAndEnabled) return;

        if (!_setUp)
        {
            _texture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBHalf)
            {
                enableRandomWrite = true,
                dimension = TextureDimension.Tex3D,
                volumeDepth = 256
            };
            _texture.Create();
            shader.SetTexture(_createVectorFieldIndex, "Destination", _texture);
            shader.SetTexture(_clearIndex, "Destination", _texture);
            
            visualEffect.SetTexture("Vector Field", _texture);
            
            _setUp = true;
        }

        if (targets.Length == 0)
        {
            shader.Dispatch(_clearIndex, Mathf.CeilToInt(_texture.width / 8f), Mathf.CeilToInt(_texture.height / 8f),
                Mathf.CeilToInt(_texture.volumeDepth / 8f));
            return;
        }
        
        var buffer = new ComputeBuffer(targets.Length, Marshal.SizeOf(typeof(ForceFieldData)));
        buffer.SetData(targets.Select(item => item.GetData()).ToArray());
        
        shader.SetBuffer(_createVectorFieldIndex, "Data", buffer);
        
        var position = this.transform.position;
        shader.SetFloats("Center", position.x, position.y, position.z);
        visualEffect.SetVector3("Center Position", position);
        var lossyScale = this.transform.lossyScale;
        shader.SetFloats("Size", lossyScale.x, lossyScale.y, lossyScale.z);
        visualEffect.SetVector3("Size", lossyScale);

        shader.Dispatch(_createVectorFieldIndex, Mathf.CeilToInt(_texture.width / 8f), Mathf.CeilToInt(_texture.height / 8f),
            Mathf.CeilToInt(_texture.volumeDepth / 8f));
        
        buffer.Release();
    }

    private void OnDrawGizmos()
    {
        var transform1 = this.transform;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform1.position, transform1.lossyScale);
    }
}
