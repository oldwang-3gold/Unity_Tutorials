Shader "Custom/DistortionFlow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _FlowMap ("Flow (RG, A noise)", 2D) = "black" {}
		[NoScaleOffset] _DerivHeightMap ("Deriv (AG) Height (B)", 2D) = "black" {}
		_UJump ("U jump per phase", Range(-0.25, 0.25)) = 0.25
		_VJump ("V jump per phase", Range(-0.25, 0.25)) = 0.25
		_Tiling ("Tiling", Float) = 1
		_Speed ("Speed", Float) = 1
		_FlowStrength ("Flow Strength", Float) = 1
		_FlowOffset ("Flow Offset", Float) = 0
		_HeightScale ("Height Scale, Constant", Float) = 0.25
		_HeightScaleModulated ("Height Scale, Modulated", Float) = 0.75
		_WaterFogColor ("Water Fog Color", Color) = (0, 0, 0, 0)
		_WaterFogDensity ("Water Fog Density", Range(0, 2)) = 0.1
		_RefractionStrength ("Refraction Strength", Range(0, 1)) = 0.25
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
		
		GrabPass {"_WaterBackground"}

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha finalcolor:ResetAlpha // 已经获取过背景texture进行混合过了，由于Alpha blend会再去和背景混合，所以我们需要将最终输出的颜色权重值改为1即alpha=1

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		#include "Flow.cginc"
		#include "LookingThroughWater.cginc"

        sampler2D _MainTex, _FlowMap, _DerivHeightMap;
		float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;
		float _HeightScale, _HeightScaleModulated;

        struct Input
        {
            float2 uv_MainTex;
			float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		float3 UnpackDerivativeHeight (float4 textureData) {
			float3 dh = textureData.agb;
			dh.xy = dh.xy * 2 - 1; // 存储的法线贴图都是正值（在生成法线贴图的时候 都做了 (dh.xy + 1) / 2的取整操作）
			return dh;
		}
		
		void ResetAlpha (Input IN, SurfaceOutputStandard o, inout fixed4 color) {
			color.a = 1;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			//float2 flowVector = tex2D(_FlowMap, IN.uv_MainTex).rg * 2 - 1;
			float3 flow = tex2D(_FlowMap, IN.uv_MainTex).rgb;
			flow.xy = flow.xy * 2 - 1;
			flow *= _FlowStrength;
			float noise = tex2D(_FlowMap, IN.uv_MainTex).a;
			float time = _Time.y * _Speed + noise;
			float2 jump = float2(_UJump, _VJump);
			float3 uvwA = FlowUVW(IN.uv_MainTex, flow.xy, jump, _FlowOffset, _Tiling, time, false);
			float3 uvwB = FlowUVW(IN.uv_MainTex, flow.xy, jump, _FlowOffset, _Tiling, time, true);
			
			float finalHeightScale = flow.z * _HeightScaleModulated + _HeightScale;
			
			float3 dhA = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwA.xy)) * 
				(uvwA.z * finalHeightScale);
			float3 dhB = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwB.xy)) * 
				(uvwB.z * finalHeightScale);
			o.Normal = normalize(float3(-(dhA.xy + dhB.xy), 1));
			
			fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
            fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;
			fixed4 c = (texA + texB) * _Color;
			o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
			
			o.Emission = ColorBelowWater(IN.screenPos, o.Normal) * (1 - c.a);
        }
        ENDCG
    }
}
