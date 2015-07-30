Shader "Custom/VertexCol" {
	Properties {
         _PointSize("PointSize", Float) = 1
     }
	
 SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // make sure that all uniforms are correctly set

         GLSLPROGRAM
         
         

		 varying vec4 xlv_COLOR;
         
         
         #ifdef VERTEX

         
         void main()
         {				
            
            xlv_COLOR = gl_Color;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         
         #endif

         #ifdef FRAGMENT
         
         void main()
         {
            gl_FragColor = xlv_COLOR;
         }
         
         #endif

         ENDGLSL
      }
   } 




	

}
