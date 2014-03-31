Shader "Custom/Bumped specular" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	float4 _Color;
	float4 _SpecColor;
	float4 _ReflectColor;
	float _Shininess;
	sampler2D _MainTex;
	sampler2D _BumpMap;
	samplerCUBE _Cube;

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
	FragmentInput passthroughVert(VertexInput input)
	{
		FragmentInput output;
		output.sv_position = mul(UNITY_MATRIX_MVP, input.position);
		output.uvcoords = input.uvcoords;
		return output;
	}

	float4 frag(FragmentInput input) : COLOR
	{
		float4 output = float4(0.5,0.5,0.5,0);//tex2D(_MainTex, input.uvcoords.xy);//float4(1,0,0,0);
		/*float4 colour = tex2D(_MainTex, input.uvcoords.xy);
		
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
		}*/

		return output;
	}
	ENDCG

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400
		
		Pass {
			//Name "FORWARD"
			//Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex passthroughVert
			#pragma fragment frag
			ENDCG
		}
	}
	
	FallBack Off
}
