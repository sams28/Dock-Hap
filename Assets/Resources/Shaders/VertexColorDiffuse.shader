Shader "Custom/VertexColorDiffuse" {
	Properties {
		_PointSize("PointSize", Float) = 1
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_Color ("Color", Color) = (0,0,0,0)
	}
	SubShader {
	Tags { "RenderType"="Opaque" }
	Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // make sure that all uniforms are correctly set
		Cull Off
         GLSLPROGRAM
         

 		 #extension GL_EXT_gpu_shader4 : require
		varying vec4 xlv_COLOR;

		#ifdef VERTEX
		uniform mat4 _Object2World;
uniform mat4 _World2Object;
uniform vec4 _MainTex_ST;
varying vec2 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
varying vec3 xlv_TEXCOORD2;
void main ()
{
  vec4 v_1;
  v_1.x = _World2Object[0].x;
  v_1.y = _World2Object[1].x;
  v_1.z = _World2Object[2].x;
  v_1.w = _World2Object[3].x;
  vec4 v_2;
  v_2.x = _World2Object[0].y;
  v_2.y = _World2Object[1].y;
  v_2.z = _World2Object[2].y;
  v_2.w = _World2Object[3].y;
  vec4 v_3;
  v_3.x = _World2Object[0].z;
  v_3.y = _World2Object[1].z;
  v_3.z = _World2Object[2].z;
  v_3.w = _World2Object[3].z;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_COLOR = gl_Color;
  xlv_TEXCOORD0 = ((gl_MultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = normalize(((
    (v_1.xyz * gl_Normal.x)
   + 
    (v_2.xyz * gl_Normal.y)
  ) + (v_3.xyz * gl_Normal.z)));
  xlv_TEXCOORD2 = (_Object2World * gl_Vertex).xyz;
}


#endif
#ifdef FRAGMENT
uniform vec4 _WorldSpaceLightPos0;
uniform vec4 _LightColor0;
uniform sampler2D _MainTex;
uniform vec4 _Color;
varying vec2 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD1;
void main ()
{
  vec4 c_1;
  vec4 tmpvar_2;
  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR* _Color);
  vec4 c_3;
  vec4 c_4;
  c_4.xyz = ((tmpvar_2.xyz * _LightColor0.xyz) * max (0.0, dot (xlv_TEXCOORD1, _WorldSpaceLightPos0.xyz)));
  c_4.w = tmpvar_2.w;
  c_3.w = c_4.w;
  c_3.xyz = c_4.xyz;
  c_1.xyz = c_3.xyz;
  c_1.w = 1.0;
  gl_FragData[0] = c_1;
}
#endif
 ENDGLSL
}
}
}
