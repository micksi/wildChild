Shader "Custom/VertexLit" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecularColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectColor ("Reflection Color", Color) = (0.05,0.05,0.05,0.5)
		_MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "" { TexGen CubeReflect }
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	float4 _Color;
	float4 _SpecularColor;
	float4 _ReflectColor;
	float _Shininess;
	float _AmbientWhiteLight;
	sampler2D _MainTex;
	sampler2D _BumpMap;
	samplerCUBE _Cube;

	ENDCG

	// Blue surface
    /*SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert
		
		struct Input {
        	float2 uv_MainTex;
      	};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = float3(0,0,1);
		}

		ENDCG
    }

    // Black fixed
    SubShader {
		Tags { "RenderType"="Opaque" }
        Pass {
            Material {
                Diffuse [_Color]
            }
        }
    }*/

	// Vertex lighting
	SubShader {
		Tags { "RenderType"="Opaque" }
        Pass {
        	Tags{ "LightMode" = "Vertex" }
            Material {
                Diffuse [_Color]
                Ambient [_Color]
                Shininess [_Shininess]
                Specular [_SpecularColor]
                Emission [_Cube]
            }
            Lighting On
           // SeparateSpecular On
            SetTexture [_MainTex] {
                Combine texture * primary DOUBLE, texture * primary
            }
            SetTexture [_Cube] {
            	constantColor[_ReflectColor]
                Combine constant * texture + previous, previous
            }
        }
    }

    FallBack "VertexLit"
}
