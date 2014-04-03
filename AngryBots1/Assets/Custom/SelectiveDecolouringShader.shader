Shader "Custom/SelectiveDecolouringShader" {
	Properties
	{
		_MainTex ("", any) = "" {} 
		_FocusX ("Focus X", Float) = 0
		_FocusY ("Focus Y", Float) = 0

		_EffectStartDistance("Effect start distance", Float) = 0.2
		_EffectMaxDistance("Full effect distance", Float) = 0.5
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float _FocusX;
	float _FocusY;

	float _EffectStartDistance;
	float _EffectMaxDistance;
	
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
		float4 output = float4(0,0,0,0);
		float4 colour = tex2D(_MainTex, input.uvcoords.xy);
		
		float luminance = Luminance(colour.rgb);		
		float4 grey = float4(luminance, luminance, luminance, colour.a);

		float distanceFromMouse = distance(float2(_FocusX, _FocusY), input.uvcoords.xy);
		
		// Calculate the magnitude of our effect as a function of distance to mouse
		float effectMagnitude = (distanceFromMouse  - _EffectStartDistance) 
						 	  / (_EffectMaxDistance - _EffectStartDistance);

		// Blend colour and grey based on effect magnitude
		if(effectMagnitude <= 0)
		{
			output = colour;
		}
		else if(effectMagnitude < 1)
		{
			output = lerp(colour, grey, effectMagnitude);
		}
		else
		{
			output = grey;
		}

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