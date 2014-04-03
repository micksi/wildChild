Shader "Custom/ScreenFadeShader" {
	Properties
	{
		_MainTex ("", any) = "" {} 
		_Colour ("Blink colour", Color) = (0,0,0,1)
		_FadeAmount ("Fade to colour magnitude", Range(0,1)) = 1
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _Colour;
	float _FadeAmount;
	
	struct VertexInput
	{
		float4 position : POSITION;
		float4 uvcoords : TEXCOORD0;
	};
	
	struct FragmentInput
	{
		float4 sv_position : SV_POSITION;
		float4 uvcoords : TEXCOORD1;
	};

	// Simple passthrough vert
	FragmentInput vert(VertexInput input)
	{
		FragmentInput output;
		output.sv_position = mul(UNITY_MATRIX_MVP, input.position);
		output.uvcoords = input.uvcoords;
		return output;
	}
	
	float4 frag(FragmentInput input) : COLOR
	{
		float4 frame = tex2D(_MainTex, input.uvcoords.xy);
		
		float4 output = lerp(frame, _Colour, _FadeAmount);

		return output;
	}
	ENDCG

	SubShader {
		 Pass {
			  ZTest Always Cull Off ZWrite Off
			  Fog { Mode off }      

			  CGPROGRAM
			  #pragma fragmentoption ARB_precision_hint_fastest
			  #pragma vertex vert
			  #pragma fragment frag
			  ENDCG
		  }
	}
	FallBack "Diffuse"
}