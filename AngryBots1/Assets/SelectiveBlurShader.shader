// SelectiveBlurShader.shader
Shader "Custom/SelectiveBlurShader" {
	Properties
	{
		_MainTex ("", any) = "" {} 
		_FocusX ("Focus X", Float) = 0
		_FocusY ("Focus Y", Float) = 0

		_MaxSampleRadius("Maximum sample radius at full effect", Float) = 1.25
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
			float4 _MainTex_TexelSize;
			float _FocusX;
			float _FocusY;

			float _MaxSampleRadius;
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

				// Set taps in the corners of a square surrounding the active point
				output.taps[0] = - _MainTex_TexelSize * float2(1,  1);
				output.taps[1] = + _MainTex_TexelSize * float2(1,  1);
				output.taps[2] = - _MainTex_TexelSize * float2(1, -1);
				output.taps[3] = + _MainTex_TexelSize * float2(1, -1);

				return output;
			}
			
			float4 frag(FragmentInput input) : COLOR
			{
				float4 color = float4(0,0,0,0);
				float4 baseColor = tex2D(_MainTex, input.uvcoords.xy);

				float distanceFromMouse = distance(float2(_FocusX, _FocusY), input.uvcoords.xy);
				// Calculate the magnitude of our effect
				float effectMagnitude = (distanceFromMouse  - _EffectStartDistance) 
								 	  / (_EffectMaxDistance - _EffectStartDistance);

				// Are we too close to the mouse to apply effect?
				if(effectMagnitude < 0)
				{
					color = baseColor;
				}
				else
				{
					// Clip effectMagnitude to 1
					if(effectMagnitude > 1)
						effectMagnitude = 1;

					// sampleRadius increases as we move away from focus point --> as effectMagnitude increases
					float sampleRadius = _MaxSampleRadius * effectMagnitude;

					// Collect 4 samples around our current point
					color += tex2D(_MainTex, input.uvcoords + input.taps[0] * sampleRadius);//modifier);
					color += tex2D(_MainTex, input.uvcoords + input.taps[1] * sampleRadius);//modifier);
					color += tex2D(_MainTex, input.uvcoords + input.taps[2] * sampleRadius);//modifier);
					color += tex2D(_MainTex, input.uvcoords + input.taps[3] * sampleRadius);//modifier);
					
					// Normalize the sample sum
					color *= 0.25;

					// Blend blur with current point
					color = lerp(baseColor, color, effectMagnitude);
				}

				return color;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}