Shader "Custom/glsl sphere" {
	Properties {
		_Color ("Color", Color) = (0,0,0,0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" "DisableBatching"="True"}
		Pass { // some shaders require multiple passes
        
         Cull Off
         Blend SrcAlpha OneMinusSrcAlpha
         AlphaTest Greater 0.01
  		ColorMask RGB
           GLSLPROGRAM // here begins the part in Unity's GLSL
 
 

 		uniform fixed4 _Color;
 		uniform sampler2D _MainTex;

 		varying vec4 xlv_COLOR;
 
 
         varying vec4 textureCoordinates;
 
         #ifdef VERTEX         
 
         void main()
         {
         	

            gl_Position = gl_ProjectionMatrix 
               * (gl_ModelViewMatrix * vec4(0.0, 0.0, 0.0, 1.0) 
               + vec4(gl_Vertex.x, gl_Vertex.y, 0.0, 0.0));
            
            
            xlv_COLOR = gl_Color;
            textureCoordinates = gl_MultiTexCoord0;
               
               
         }
 
         #endif
 
 

 
 
 
 
 
 
         #ifdef FRAGMENT
 
         void main()
         {
         
        
         gl_FragColor = (xlv_COLOR *_Color) * texture2D (_MainTex,vec2(textureCoordinates));
          
         
        
          
         }
 
         #endif
 
         ENDGLSL
 		
 		
 		
 		
 		
      }
	} 
}
