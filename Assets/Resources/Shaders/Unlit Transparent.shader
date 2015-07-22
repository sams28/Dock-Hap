﻿Shader "Unlit/Color2" {
Properties {

	_Color ("Main Color", Color) = (1,1,1,1)
	//_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				fixed4 vertex : POSITION;
				//float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				fixed4 vertex : SV_POSITION;
				//half2 texcoord : TEXCOORD0;
				//UNITY_FOG_COORDS(0)
			};

			fixed4 _Color;
			//sampler2D _MainTex;
			//fixed4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
			
				v2f o; // Shader output
				
				
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				//o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			

			fixed4 frag (v2f i) : SV_Target
			{
			fixed4 col = _Color;
				//fixed4 col = tex2D(_MainTex, i.texcoord)*_Color;
				//UNITY_APPLY_FOG(i.fogCoord, col);
				//UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
		ENDCG
	}
}

}