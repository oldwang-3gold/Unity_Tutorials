// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#if !defined(LIGHTING_INCLUDED)
#define LIGHTING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"
float4 _Tint;
sampler2D _MainTex;
float4 _MainTex_ST;
float _Metallic;
float _Smoothness;

struct VertexData {
	float4 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
};

struct Interpolators {
	float4 position : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : TEXCOORD1;
	float3 worldPos : TEXCOORD2;
	
	#if defined(VERTEXLIGHT_ON)
		float3 vertexLightColor : TEXCOORD3;
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
	i.uv = TRANSFORM_TEX(v.uv, _MainTex);
	//i.normal = mul(transpose((float3x3)unity_WorldToObject), v.normal);
	//i.normal = normalize(i.normal);
	i.normal = UnityObjectToWorldNormal(v.normal);
	ComputeVertexLightColor(i);
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

float4 MyFragmentProgram(Interpolators i) : SV_TARGET {
	i.normal = normalize(i.normal);
	float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
	float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
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
