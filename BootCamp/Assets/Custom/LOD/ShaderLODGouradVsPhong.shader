Shader "Custom/ShaderLODGouradVsPhong" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		half4 c = tex2D (_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}

	half4 LightingBleh (SurfaceOutput s, half3 lightDir, half atten) {
		half NdotL = dot (s.Normal, lightDir);
		half4 c;
		c.rgb = 0;//s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
		c.a = s.Alpha;
		return c;
	}

	half4 LightingMeh (SurfaceOutput s, half3 lightDir, half atten) {
		//half NdotL = dot (s.Normal, lightDir);
		half4 c;
		c.rgb = 0;// s.Albedo * _LightColor0.rgb * (NdotL * atten * 2);
		c.a = s.Alpha;
		return c;
	}

	struct PVertIn
	{
		float4 pos : POSITION;
	};

	struct PFragIn
	{
		float4 pos : SV_POSITION;
	};

	PFragIn pVert(PVertIn IN)
	{
		PFragIn OUT;
		OUT.pos = mul(UNITY_MATRIX_MVP, IN.pos);
		return OUT;
	}

	float4 pFrag(PFragIn IN) : COLOR
	{
		return float4(0, 0, 1, 1);
	}

	ENDCG

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Bleh
		ENDCG
	}
	/*SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		Pass {
			CGPROGRAM
			#pragma vertex pVert
			#pragma fragment pFrag
			ENDCG
		}
	}*/
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 50
		CGPROGRAM
		#pragma surface surf Meh
		ENDCG
	}
	//FallBack "Diffuse"
}
