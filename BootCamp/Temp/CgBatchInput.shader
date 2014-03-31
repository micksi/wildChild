Shader "Custom/Bumped specular" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
		_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_AmbientWhiteLight ("Ambient white light", Float) = 3
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	uniform float4 _LightColor0; 

	float4 _Color;
	float4 _SpecColor;
	float4 _ReflectColor;
	float _Shininess;
	float _AmbientWhiteLight;
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

	struct GouradFragment
	{
		float4 pos : SV_POSITION;
		float4 texcoord : TEXCOORD1;
		float4 color : TEXCOORD2;
	};

	struct FullonFragment {
	    float4 vertex : SV_POSITION;
	    float4 tangent : TEXCOORD1;
	    float3 normal : TEXCOORD2;
	    float4 texcoord : TEXCOORD0;
	    float3 viewDir : TEXCOORD3;
	};

	// Simple passthrough vert
	FragmentInput passthroughVert(VertexInput input)
	{
		FragmentInput output;
		output.sv_position = mul(UNITY_MATRIX_MVP, input.position);
		output.uvcoords = input.uvcoords;
		return output;
	}

	// Vertex diffuse lighting 
	GouradFragment gouradVert(appdata_base input)
	{
		GouradFragment output;
		output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
		output.texcoord = input.texcoord;

		// ShadeVertexLights computes illumination from four 
		// per-vertex lights and ambient, given object space
		// position & normal. 
		output.color = _AmbientWhiteLight * _Color
					   * float4(ShadeVertexLights (input.vertex, input.normal), 1);

		return output;
	}

	// Full shizzle
	FullonFragment fullonVert(appdata_tan input)
	{
		FullonFragment output;

		output.vertex = mul(UNITY_MATRIX_MVP, input.vertex);
		output.texcoord = input.texcoord;
		output.normal = mul(float4(input.normal, 0), _World2Object);
		output.tangent = mul(_Object2World, input.tangent);
		output.viewDir = float3(mul(_Object2World, input.vertex) 
               - float4(_WorldSpaceCameraPos, 1.0));

		return output;
	}

	// Full shizzle
	float4 fullonFrag(FullonFragment input) : COLOR
	{
		float4 output = float4(0,0,0,0);

		// Light dir - assume 0 is directional
		float3 lightDir = normalize(float3(_WorldSpaceLightPos0));
		float normalDotLightClipped = max(0, dot(lightDir, input.normal));

		// Diffuse
        float lightAttenuation = normalDotLightClipped;

		float4 diffuseTex = tex2D(_MainTex, input.texcoord.xy);
		float4 diffuseCombined = diffuseTex * _Color;
		float4 diffuseAttenuated = diffuseCombined * lightAttenuation;
		
		//output = diffuseAttenuated;
        
		// Specularity
		
		float3 specular = float3(0);
		if(normalDotLightClipped > 0)
		{
			float3 reflectedLightDir = reflect(-lightDir, input.normal);
			float reflectedLightDotViewClipped = 
				max(0.0, dot(reflectedLightDir, -input.viewDir));

			specular = float3(_LightColor0)
                  	 * _SpecColor
                  	 * pow(reflectedLightDotViewClipped, _Shininess);
		}

		// TODO get specular to work
		output += float4(specular,0);
 
		// Reflection
		float3 reflectedDir = 
               reflect(input.viewDir, normalize(input.normal));
		float4 reflection = texCUBE(_Cube, reflectedDir);

		//output += reflection;

		// Bump

		// 

		return output;
	}


	float4 gouradFrag(GouradFragment input) : COLOR
	{
		float4 output = input.color * tex2D(_MainTex, input.texcoord.xy);
		return output;
	}

	float4 basicFrag(FragmentInput input) : COLOR
	{
		float4 output = tex2D(_MainTex, input.uvcoords.xy);//float4(1,0,0,0);
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

	/*SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400
		
		Pass {
			//Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex passthroughVert
			#pragma fragment basicFrag
			ENDCG
		}
	}*/

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass {
			//Tags { "LightMode" = "Pixel" }
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex fullonVert
			#pragma fragment fullonFrag
			ENDCG
		}
	}
/*
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex gouradVert
			#pragma fragment gouradFrag
			ENDCG
		}
	}*/
	
	//FallBack "VertexLit"
}
