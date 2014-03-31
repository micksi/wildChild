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

	#LINE 180


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
			Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 17 to 17
//   d3d9 - ALU: 17 to 17
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "tangent" ATTR14
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 13 [_WorldSpaceCameraPos]
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
"!!ARBvp1.0
# 17 ALU
PARAM c[14] = { { 0 },
		state.matrix.mvp,
		program.local[5..13] };
TEMP R0;
MUL R0.xyz, vertex.normal.y, c[10];
MAD R0.xyz, vertex.normal.x, c[9], R0;
MAD R0.xyz, vertex.normal.z, c[11], R0;
ADD result.texcoord[2].xyz, R0, c[0].x;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
MOV result.texcoord[0], vertex.texcoord[0];
ADD result.texcoord[3].xyz, R0, -c[13];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
DP4 result.texcoord[1].w, vertex.attrib[14], c[8];
DP4 result.texcoord[1].z, vertex.attrib[14], c[7];
DP4 result.texcoord[1].y, vertex.attrib[14], c[6];
DP4 result.texcoord[1].x, vertex.attrib[14], c[5];
END
# 17 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "tangent" TexCoord2
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 12 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
"vs_2_0
; 17 ALU
def c13, 0.00000000, 0, 0, 0
dcl_position0 v0
dcl_tangent0 v1
dcl_normal0 v2
dcl_texcoord0 v3
mul r0.xyz, v2.y, c9
mad r0.xyz, v2.x, c8, r0
mad r0.xyz, v2.z, c10, r0
add oT2.xyz, r0, c13.x
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mov oT0, v3
add oT3.xyz, r0, -c12
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
dp4 oT1.w, v1, c7
dp4 oT1.z, v1, c6
dp4 oT1.y, v1, c5
dp4 oT1.x, v1, c4
"
}

SubProgram "gles " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying highp vec3 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD2;
varying highp vec4 xlv_TEXCOORD1;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec4 tmpvar_2;
  tmpvar_2.w = 0.0;
  tmpvar_2.xyz = normalize(_glesNormal);
  highp vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD1 = (_Object2World * tmpvar_1);
  xlv_TEXCOORD2 = (tmpvar_2 * _World2Object).xyz;
  xlv_TEXCOORD0 = _glesMultiTexCoord0;
  xlv_TEXCOORD3 = ((_Object2World * _glesVertex) - tmpvar_3).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
uniform highp float _Shininess;
uniform highp vec4 _SpecColor;
uniform highp vec4 _LightColor0;
uniform highp vec4 _WorldSpaceLightPos0;
void main ()
{
  highp vec3 specular_1;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_WorldSpaceLightPos0.xyz);
  highp float tmpvar_3;
  tmpvar_3 = max (0.0, dot (tmpvar_2, xlv_TEXCOORD2));
  specular_1 = vec3(0.0, 0.0, 0.0);
  if ((tmpvar_3 > 0.0)) {
    highp vec3 i_4;
    i_4 = -(tmpvar_2);
    specular_1 = ((_LightColor0.xyz * _SpecColor.xyz) * pow (max (0.0, dot ((i_4 - (2.0 * (dot (xlv_TEXCOORD2, i_4) * xlv_TEXCOORD2))), -(xlv_TEXCOORD3))), _Shininess));
  };
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = specular_1;
  gl_FragData[0] = tmpvar_5;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying highp vec3 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD2;
varying highp vec4 xlv_TEXCOORD1;
uniform highp mat4 _World2Object;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _WorldSpaceCameraPos;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_1.w = _glesTANGENT.w;
  highp vec4 tmpvar_2;
  tmpvar_2.w = 0.0;
  tmpvar_2.xyz = normalize(_glesNormal);
  highp vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = _WorldSpaceCameraPos;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD1 = (_Object2World * tmpvar_1);
  xlv_TEXCOORD2 = (tmpvar_2 * _World2Object).xyz;
  xlv_TEXCOORD0 = _glesMultiTexCoord0;
  xlv_TEXCOORD3 = ((_Object2World * _glesVertex) - tmpvar_3).xyz;
}



#endif
#ifdef FRAGMENT

varying highp vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD2;
uniform highp float _Shininess;
uniform highp vec4 _SpecColor;
uniform highp vec4 _LightColor0;
uniform highp vec4 _WorldSpaceLightPos0;
void main ()
{
  highp vec3 specular_1;
  highp vec3 tmpvar_2;
  tmpvar_2 = normalize(_WorldSpaceLightPos0.xyz);
  highp float tmpvar_3;
  tmpvar_3 = max (0.0, dot (tmpvar_2, xlv_TEXCOORD2));
  specular_1 = vec3(0.0, 0.0, 0.0);
  if ((tmpvar_3 > 0.0)) {
    highp vec3 i_4;
    i_4 = -(tmpvar_2);
    specular_1 = ((_LightColor0.xyz * _SpecColor.xyz) * pow (max (0.0, dot ((i_4 - (2.0 * (dot (xlv_TEXCOORD2, i_4) * xlv_TEXCOORD2))), -(xlv_TEXCOORD3))), _Shininess));
  };
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = specular_1;
  gl_FragData[0] = tmpvar_5;
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "tangent" TexCoord2
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 12 [_WorldSpaceCameraPos]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
"agal_vs
c13 0.0 0.0 0.0 0.0
[bc]
adaaaaaaaaaaahacabaaaaffaaaaaaaaajaaaaoeabaaaaaa mul r0.xyz, a1.y, c9
adaaaaaaabaaahacabaaaaaaaaaaaaaaaiaaaaoeabaaaaaa mul r1.xyz, a1.x, c8
abaaaaaaaaaaahacabaaaakeacaaaaaaaaaaaakeacaaaaaa add r0.xyz, r1.xyzz, r0.xyzz
adaaaaaaabaaahacabaaaakkaaaaaaaaakaaaaoeabaaaaaa mul r1.xyz, a1.z, c10
abaaaaaaaaaaahacabaaaakeacaaaaaaaaaaaakeacaaaaaa add r0.xyz, r1.xyzz, r0.xyzz
abaaaaaaacaaahaeaaaaaakeacaaaaaaanaaaaaaabaaaaaa add v2.xyz, r0.xyzz, c13.x
bdaaaaaaaaaaaeacaaaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 r0.z, a0, c6
bdaaaaaaaaaaabacaaaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 r0.x, a0, c4
bdaaaaaaaaaaacacaaaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 r0.y, a0, c5
aaaaaaaaaaaaapaeadaaaaoeaaaaaaaaaaaaaaaaaaaaaaaa mov v0, a3
acaaaaaaadaaahaeaaaaaakeacaaaaaaamaaaaoeabaaaaaa sub v3.xyz, r0.xyzz, c12
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
bdaaaaaaabaaaiaeafaaaaoeaaaaaaaaahaaaaoeabaaaaaa dp4 v1.w, a5, c7
bdaaaaaaabaaaeaeafaaaaoeaaaaaaaaagaaaaoeabaaaaaa dp4 v1.z, a5, c6
bdaaaaaaabaaacaeafaaaaoeaaaaaaaaafaaaaoeabaaaaaa dp4 v1.y, a5, c5
bdaaaaaaabaaabaeafaaaaoeaaaaaaaaaeaaaaoeabaaaaaa dp4 v1.x, a5, c4
aaaaaaaaacaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v2.w, c0
aaaaaaaaadaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v3.w, c0
"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
#define gl_Normal (normalize(_glesNormal))
in vec3 _glesNormal;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;
#define TANGENT vec4(normalize(_glesTANGENT.xyz), _glesTANGENT.w)
in vec4 _glesTANGENT;

#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 330
struct FragmentInput {
    highp vec4 sv_position;
    highp vec4 uvcoords;
};
#line 324
struct VertexInput {
    highp vec4 position;
    highp vec4 uvcoords;
};
#line 336
struct GouradFragment {
    highp vec4 pos;
    highp vec4 texcoord;
    highp vec4 color;
};
#line 52
struct appdata_base {
    highp vec4 vertex;
    highp vec3 normal;
    highp vec4 texcoord;
};
#line 343
struct FullonFragment {
    highp vec4 vertex;
    highp vec4 tangent;
    highp vec3 normal;
    highp vec4 texcoord;
    highp vec3 viewDir;
};
#line 59
struct appdata_tan {
    highp vec4 vertex;
    highp vec4 tangent;
    highp vec3 normal;
    highp vec4 texcoord;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 315
uniform highp vec4 _LightColor0;
uniform highp vec4 _Color;
uniform highp vec4 _SpecColor;
uniform highp vec4 _ReflectColor;
#line 319
uniform highp float _Shininess;
uniform highp float _AmbientWhiteLight;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
#line 323
uniform samplerCube _Cube;
#line 352
#line 377
#line 398
#line 403
#line 367
FullonFragment fullonVert( in appdata_tan xlat_varinput ) {
    #line 369
    FullonFragment xlat_varoutput;
    xlat_varoutput.vertex = (glstate_matrix_mvp * xlat_varinput.vertex);
    xlat_varoutput.texcoord = xlat_varinput.texcoord;
    xlat_varoutput.normal = vec3( (vec4( xlat_varinput.normal, 0.0) * _World2Object));
    #line 373
    xlat_varoutput.tangent = (_Object2World * xlat_varinput.tangent);
    xlat_varoutput.viewDir = vec3( ((_Object2World * xlat_varinput.vertex) - vec4( _WorldSpaceCameraPos, 1.0)));
    return xlat_varoutput;
}

out highp vec4 xlv_TEXCOORD1;
out highp vec3 xlv_TEXCOORD2;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD3;
void main() {
    FullonFragment xl_retval;
    appdata_tan xlt_xlat_varinput;
    xlt_xlat_varinput.vertex = vec4(gl_Vertex);
    xlt_xlat_varinput.tangent = vec4(TANGENT);
    xlt_xlat_varinput.normal = vec3(gl_Normal);
    xlt_xlat_varinput.texcoord = vec4(gl_MultiTexCoord0);
    xl_retval = fullonVert( xlt_xlat_varinput);
    gl_Position = vec4(xl_retval.vertex);
    xlv_TEXCOORD1 = vec4(xl_retval.tangent);
    xlv_TEXCOORD2 = vec3(xl_retval.normal);
    xlv_TEXCOORD0 = vec4(xl_retval.texcoord);
    xlv_TEXCOORD3 = vec3(xl_retval.viewDir);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];

#line 151
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 187
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 181
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 330
struct FragmentInput {
    highp vec4 sv_position;
    highp vec4 uvcoords;
};
#line 324
struct VertexInput {
    highp vec4 position;
    highp vec4 uvcoords;
};
#line 336
struct GouradFragment {
    highp vec4 pos;
    highp vec4 texcoord;
    highp vec4 color;
};
#line 52
struct appdata_base {
    highp vec4 vertex;
    highp vec3 normal;
    highp vec4 texcoord;
};
#line 343
struct FullonFragment {
    highp vec4 vertex;
    highp vec4 tangent;
    highp vec3 normal;
    highp vec4 texcoord;
    highp vec3 viewDir;
};
#line 59
struct appdata_tan {
    highp vec4 vertex;
    highp vec4 tangent;
    highp vec3 normal;
    highp vec4 texcoord;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_LightPosition[8];
uniform highp vec4 unity_LightAtten[8];
#line 19
uniform highp vec4 unity_SpotDirection[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
#line 23
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
#line 27
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
#line 31
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
#line 35
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
#line 39
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
#line 43
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
#line 47
uniform highp mat4 glstate_matrix_projection;
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
#line 51
uniform lowp vec4 unity_ColorSpaceGrey;
#line 77
#line 82
#line 87
#line 91
#line 96
#line 120
#line 137
#line 158
#line 166
#line 193
#line 206
#line 215
#line 220
#line 229
#line 234
#line 243
#line 260
#line 265
#line 291
#line 299
#line 307
#line 311
#line 315
uniform highp vec4 _LightColor0;
uniform highp vec4 _Color;
uniform highp vec4 _SpecColor;
uniform highp vec4 _ReflectColor;
#line 319
uniform highp float _Shininess;
uniform highp float _AmbientWhiteLight;
uniform sampler2D _MainTex;
uniform sampler2D _BumpMap;
#line 323
uniform samplerCube _Cube;
#line 352
#line 377
#line 398
#line 403
#line 377
highp vec4 fullonFrag( in FullonFragment xlat_varinput ) {
    highp vec4 xlat_varoutput = vec4( 0.0, 0.0, 0.0, 0.0);
    highp vec3 lightDir = normalize(vec3( _WorldSpaceLightPos0));
    #line 381
    highp float normalDotLightClipped = max( 0.0, dot( lightDir, xlat_varinput.normal));
    highp float lightAttenuation = normalDotLightClipped;
    highp vec4 diffuseTex = texture( _MainTex, xlat_varinput.texcoord.xy);
    highp vec4 diffuseCombined = (diffuseTex * _Color);
    #line 385
    highp vec4 diffuseAttenuated = (diffuseCombined * lightAttenuation);
    highp vec3 specular = vec3( 0.0);
    if ((normalDotLightClipped > 0.0)){
        #line 389
        highp vec3 reflectedLightDir = reflect( (-lightDir), xlat_varinput.normal);
        highp float reflectedLightDotViewClipped = max( 0.0, dot( reflectedLightDir, (-xlat_varinput.viewDir)));
        specular = ((vec3( _LightColor0) * vec3( _SpecColor)) * pow( reflectedLightDotViewClipped, _Shininess));
    }
    #line 393
    xlat_varoutput += vec4( specular, 0.0);
    highp vec3 reflectedDir = reflect( xlat_varinput.viewDir, normalize(xlat_varinput.normal));
    highp vec4 reflection = texture( _Cube, reflectedDir);
    return xlat_varoutput;
}
in highp vec4 xlv_TEXCOORD1;
in highp vec3 xlv_TEXCOORD2;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD3;
void main() {
    highp vec4 xl_retval;
    FullonFragment xlt_xlat_varinput;
    xlt_xlat_varinput.vertex = vec4(0.0);
    xlt_xlat_varinput.tangent = vec4(xlv_TEXCOORD1);
    xlt_xlat_varinput.normal = vec3(xlv_TEXCOORD2);
    xlt_xlat_varinput.texcoord = vec4(xlv_TEXCOORD0);
    xlt_xlat_varinput.viewDir = vec3(xlv_TEXCOORD3);
    xl_retval = fullonFrag( xlt_xlat_varinput);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 15 to 15, TEX: 0 to 0
//   d3d9 - ALU: 18 to 18
SubProgram "opengl " {
Keywords { }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_SpecColor]
Float 3 [_Shininess]
"!!ARBfp1.0
# 15 ALU, 0 TEX
PARAM c[5] = { program.local[0..3],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
DP3 R0.x, c[0], c[0];
RSQ R0.x, R0.x;
MUL R0.xyz, R0.x, c[0];
DP3 R0.w, R0, fragment.texcoord[2];
MUL R1.xyz, -R0.w, fragment.texcoord[2];
MAD R0.xyz, -R1, c[4].y, -R0;
DP3 R0.x, R0, -fragment.texcoord[3];
MAX R1.x, R0, c[4];
MOV R0.xyz, c[2];
MUL R0.xyz, R0, c[1];
POW R1.x, R1.x, c[3].x;
MUL R1.xyz, R0, R1.x;
MAX R0.x, R0.w, c[4];
CMP result.color.xyz, -R0.x, R1, c[4].x;
MOV result.color.w, c[4].x;
END
# 15 instructions, 2 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_SpecColor]
Float 3 [_Shininess]
"ps_2_0
; 18 ALU
def c4, 0.00000000, 2.00000000, 0, 0
dcl t2.xyz
dcl t3.xyz
dp3 r0.x, c0, c0
rsq r0.x, r0.x
mul r0.xyz, r0.x, c0
dp3 r1.x, r0, t2
mul r2.xyz, -r1.x, t2
mad r0.xyz, -r2, c4.y, -r0
dp3 r0.x, r0, -t3
max r0.x, r0, c4
pow r2.x, r0.x, c3.x
mov r0.xyz, c1
mul r0.xyz, c2, r0
mul r0.xyz, r0, r2.x
max r1.x, r1, c4
mov r0.w, c4.x
cmp r0.xyz, -r1.x, c4.x, r0
mov oC0, r0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

SubProgram "flash " {
Keywords { }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_SpecColor]
Float 3 [_Shininess]
"agal_ps
c4 0.0 2.0 0.0 0.0
[bc]
aaaaaaaaadaaapacaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r3, c0
aaaaaaaaaaaaapacaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0, c0
bcaaaaaaaaaaabacadaaaakeacaaaaaaaaaaaakeacaaaaaa dp3 r0.x, r3.xyzz, r0.xyzz
akaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rsq r0.x, r0.x
adaaaaaaaaaaahacaaaaaaaaacaaaaaaaaaaaaoeabaaaaaa mul r0.xyz, r0.x, c0
bcaaaaaaabaaabacaaaaaakeacaaaaaaacaaaaoeaeaaaaaa dp3 r1.x, r0.xyzz, v2
bfaaaaaaacaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r2.x, r1.x
adaaaaaaacaaahacacaaaaaaacaaaaaaacaaaaoeaeaaaaaa mul r2.xyz, r2.x, v2
bfaaaaaaabaaaoacacaaaakeacaaaaaaaaaaaaaaaaaaaaaa neg r1.yzw, r2.xyzz
adaaaaaaabaaaoacabaaaapjacaaaaaaaeaaaaffabaaaaaa mul r1.yzw, r1.yzww, c4.y
acaaaaaaaaaaahacabaaaapjacaaaaaaaaaaaakeacaaaaaa sub r0.xyz, r1.yzww, r0.xyzz
bfaaaaaaadaaahacadaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa neg r3.xyz, v3
bcaaaaaaaaaaabacaaaaaakeacaaaaaaadaaaakeacaaaaaa dp3 r0.x, r0.xyzz, r3.xyzz
ahaaaaaaaaaaabacaaaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r0.x, r0.x, c4
alaaaaaaacaaapacaaaaaaaaacaaaaaaadaaaaaaabaaaaaa pow r2, r0.x, c3.x
aaaaaaaaaaaaahacabaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.xyz, c1
adaaaaaaaaaaahacacaaaaoeabaaaaaaaaaaaakeacaaaaaa mul r0.xyz, c2, r0.xyzz
adaaaaaaaaaaahacaaaaaakeacaaaaaaacaaaaaaacaaaaaa mul r0.xyz, r0.xyzz, r2.x
ahaaaaaaabaaabacabaaaaaaacaaaaaaaeaaaaoeabaaaaaa max r1.x, r1.x, c4
aaaaaaaaaaaaaiacaeaaaaaaabaaaaaaaaaaaaaaaaaaaaaa mov r0.w, c4.x
bfaaaaaaadaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r3.x, r1.x
ckaaaaaaadaaahacadaaaaaaacaaaaaaaeaaaaaaabaaaaaa slt r3.xyz, r3.x, c4.x
acaaaaaaabaaahacaaaaaakeacaaaaaaaeaaaaaaabaaaaaa sub r1.xyz, r0.xyzz, c4.x
adaaaaaaaaaaahacabaaaakeacaaaaaaadaaaakeacaaaaaa mul r0.xyz, r1.xyzz, r3.xyzz
abaaaaaaaaaaahacaaaaaakeacaaaaaaaeaaaaaaabaaaaaa add r0.xyz, r0.xyzz, c4.x
aaaaaaaaaaaaapadaaaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r0
"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3"
}

}

#LINE 206

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
