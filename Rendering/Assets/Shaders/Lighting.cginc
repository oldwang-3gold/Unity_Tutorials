// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#if !defined(LIGHTING_INCLUDED)
#define LIGHTING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"
float4 _Tint;
sampler2D _MainTex, _DetailTex;
float4 _MainTex_ST, _DetailTex_ST;

sampler2D _NormalMap, _DetailNormalMap;
float _BumpScale, _DetailBumpScale;
float4 _HeightMap_TexelSize;
float _Metallic;
float _Smoothness;

struct VertexData {
	float4 position : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 uv : TEXCOORD0;
};

struct Interpolators {
	float4 position : SV_POSITION;
	float4 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float4 tangent : TEXCOORD2
	float3 worldPos : TEXCOORD3;
	
	#if defined(VERTEXLIGHT_ON)
		float3 vertexLightColor : TEXCOORD4;
	#endif
};

UnityLight CreateLight (Interpolators i) {
	UnityLight light;
	#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
		light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
	#else
		light.dir = _WorldSpaceLightPos0.xyz;
	#endif
	
	UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos);
	light.color = _LightColor0.rgb * attenuation;
	light.ndotl = DotClamped(i.normal, light.dir);
	return light;
}

void ComputeVertexLightColor (inout Interpolators i){
	#if defined(VERTEXLIGHT_ON)
		float3 lightPos = float3(
			unity_4LightPosX0.x, unity_4LightPosY0.x, unity_4LightPosZ0.x
		);
		float3 lightVec = lightPos - i.worldPos;
		float3 lightDir = normalize(lightVec);
		float ndotl = DotClamped(i.normal, lightDir);
		float attenuation = 1 / (1 + dot(lightVec, lightVec) * unity_4LightAtten0.x);
		i.vertexLightColor = unity_LightColor[0].rgb * ndotl * attenuation;
	#endif
}

Interpolators MyVertexProgram(VertexData v) {
	Interpolators i;
	i.position = UnityObjectToClipPos(v.position);
	i.worldPos = mul(unity_ObjectToWorld, v.position);
	// i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
	i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
	i.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex);
	//i.normal = mul(transpose((float3x3)unity_WorldToObject), v.normal);
	//i.normal = normalize(i.normal);
	i.normal = UnityObjectToWorldNormal(v.normal);
	i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
	//ComputeVertexLightColor(i);
	return i;
}

UnityIndirect CreateIndirectLight (Interpolators i){
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;
	
	#if defined(VERTEXLIGHT_ON)
		indirectLight.diffuse = i.vertexLightColor;
	#endif
	return indirectLight;
}

void InitializeFragmentNormal (inout Interpolators i) {
	//float2 du = float2(_HeightMap_TexelSize.x * 0.5, 0);
	//float u1 = tex2D(_HeightMap, i.uv - du);
	//float u2 = tex2D(_HeightMap, i.uv + du);
	//float3 tu = float3(1, u2 - u1, 0);
	
	//float2 dv = float2(0, _HeightMap_TexelSize.y * 0.5);
	//float v1 = tex2D(_HeightMap, i.uv - dv);
	//float v2 = tex2D(_HeightMap, i.uv + dv);
	//float3 tv = float3(0, v2 - v1, 1);
	//i.normal = float3(u1 - u2, 1, v1 - v2);
	
	//i.normal.xy = tex2D(_NormalMap, i.uv).wy * 2 - 1;
	//i.normal.xy *= _BumpScale;
	//i.normal.z = sqrt(1 - saturate(dot(i.normal.xy, i.normal.xy)));
	float3 mainNormal = UnpackScaleNormal(tex2D(_NormalMap, i.uv.xy), _BumpScale);
	float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, i.uv.zw), _DetailBumpScale);
	float3 tangentSpaceNormal = BlendNormals(mainNormal, detailNormal);
	
	float3 binormal = cross(i.normal, i.tangent.xyz) * 
		(i.tangent.w * unity_WorldTransformParams.w);
	i.normal = normalize(
		tangentSpaceNormal.x * i.tangent + 
		tangentSpaceNormal.y * binormal +
		tangentSpaceNormal.z * i.normal
	);
}

float4 MyFragmentProgram(Interpolators i) : SV_TARGET {
	InitializeFragmentNormal(i);
	float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
	float3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Tint.rgb;
	albedo *= tex2D(_DetailTex, i.uv.zw) * unity_ColorSpaceDouble;
	float3 specularTint;
	float oneMinusReflectivity;
	albedo = DiffuseAndSpecularFromMetallic(
		albedo, _Metallic, specularTint, oneMinusReflectivity
	);
	
	return UNITY_BRDF_PBS(
		albedo, specularTint,
		oneMinusReflectivity, _Smoothness,
		i.normal, viewDir,
		CreateLight(i), CreateIndirectLight(i)
	);
}
#endif
