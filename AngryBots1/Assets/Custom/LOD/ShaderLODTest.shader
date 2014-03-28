Shader "Custom/ShaderLODTest" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Colour ("Colour", Color) = (1,1,1,1)
	}
	SubShader {
		LOD 200
	    Tags { "RenderType" = "Opaque" }
	    CGPROGRAM
	    #pragma surface surf BlinnPhong

	    sampler2D _MainTex;
	    float4 _Colour; 

	    struct Input {
	        float4 color : COLOR;
	        float2 uv_MainTex;
	    };

	    void surf (Input IN, inout SurfaceOutput o) {
	        o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
	        o.Emission = _Colour;
	    }
	    ENDCG
    }

    SubShader {
		LOD 50
	    Tags { "RenderType" = "Opaque" }
	    CGPROGRAM
	    #pragma surface surf Lambert

	    struct Input {
	        float4 color : COLOR;
	    };
	    
	    void surf (Input IN, inout SurfaceOutput o) {
	        o.Albedo = 1;
	    }
	    ENDCG
    }

	FallBack "Diffuse"
}
