Shader "Custom/SelectivePixelationShader" {
	Properties
	{
		_MainTex ("", any) = "" {} 
		_LoResTex("", any) = "" {} 
		_FocusX ("Focus X", Float) = 0
		_FocusY ("Focus Y", Float) = 0

		_DownSampleFactor("Downsampling factor", Float) = 2
		_EffectStartDistance("Effect start distance", Float) = 0.2
		_EffectMaxDistance("Full effect distance", Float) = 0.5
	}
	SubShader {
		Pass {
			Tags { "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			sampler2D _LoResTex;
			float4 _MainTex_TexelSize;
			float _FocusX;
			float _FocusY;

			float _DownSampleFactor;
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
				float2 taps[4] : TEXCOORD2;
			};
		
			FragmentInput vert(VertexInput input)
			{
				FragmentInput output;
				output.sv_position = mul(UNITY_MATRIX_MVP, input.position);
				output.uvcoords = input.uvcoords;

				return output;
			}
			
			float4 frag(FragmentInput input) : COLOR
			{
				float4 color = float4(0,0,0,0);

				float distanceFromMouse = distance(float2(_FocusX, _FocusY), input.uvcoords.xy);
				// Calculate the magnitude of our effect
				float effectMagnitude = (distanceFromMouse  - _EffectStartDistance) 
								 	  / (_EffectMaxDistance - _EffectStartDistance);

				// Are we too close to the mouse to apply effect?
				if(effectMagnitude < 0)
				{
					float4 baseColor = tex2D(_MainTex, input.uvcoords.xy);
					color = baseColor;
				}
				else if(effectMagnitude <= 1)
				{
					float4 baseColor = tex2D(_MainTex, input.uvcoords.xy);
					float4 pixelatedColor = tex2D(_LoResTex, input.uvcoords.xy);
					color = lerp(baseColor, pixelatedColor, effectMagnitude);
				} 
				else
				{
					float4 pixelatedColor = tex2D(_LoResTex, input.uvcoords.xy);
					color = pixelatedColor;
				}

				return color;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}