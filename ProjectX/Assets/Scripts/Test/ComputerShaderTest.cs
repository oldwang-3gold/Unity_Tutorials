using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerShaderTest : MonoBehaviour
{
    public ComputeShader computerShader;
    public Material material;

    public struct ParticleData
    {
        public Vector3 pos;
        public Color color;
    }
    public int particleCount = 20000;
    private ComputeBuffer m_ParticleBuffer;
    private int m_ParticleKernelIndex;
    private int m_ParticleDataBufferId = Shader.PropertyToID("_particleDataBuffer");

    // Start is called before the first frame update
    void Start()
    {
        // Example 1
        //RenderTexture renderTexture = new RenderTexture(256, 256, 16);
        //renderTexture.enableRandomWrite = true;
        //renderTexture.Create();
        //material.mainTexture = renderTexture;
        //// 找到核函数索引
        //int kernelIndex = computerShader.FindKernel("CSMain");
        //// 根据索引与目标CS中存储结果的result绑定，这样对于material的shader最后输出就是result
        //computerShader.SetTexture(kernelIndex, "Result", renderTexture);
        //computerShader.Dispatch(kernelIndex, 256 / 8, 256 / 8, 1);

        // Example 2
        // stride指每个子buffer占用的空间，这里用到一个float3和float4 每个float是4字节
        m_ParticleBuffer = new ComputeBuffer(particleCount, (3 + 4) * 4);
        ParticleData[] particleDatas = new ParticleData[particleCount];
        m_ParticleBuffer.SetData(particleDatas);
        m_ParticleKernelIndex = computerShader.FindKernel("UpdateParticle");
    }

    // Update is called once per frame
    void Update()
    {
        computerShader.SetBuffer(m_ParticleKernelIndex, "ParticleBuffer", m_ParticleBuffer);
        computerShader.SetFloat("Time", Time.time);
        computerShader.Dispatch(m_ParticleKernelIndex, particleCount / 1000, 1, 1);
        material.SetBuffer(m_ParticleDataBufferId, m_ParticleBuffer);
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, particleCount);
    }

    private void OnDestroy()
    {
        m_ParticleBuffer.Release();
        m_ParticleBuffer = null;
    }
}
