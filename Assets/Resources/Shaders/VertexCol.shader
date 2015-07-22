Shader "Custom/VertexCol" {
	Properties {
         _PointSize("PointSize", Float) = 1
     }
	
 SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // make sure that all uniforms are correctly set

         GLSLPROGRAM
         
         
 		 #extension GL_EXT_gpu_shader4 : require
		 varying vec4 xlv_COLOR;
         
         
         #ifdef VERTEX
         uniform float _PointSize;
         
         void main()
         {				
            
            xlv_COLOR = gl_Color;
            gl_PointSize = _PointSize;
            

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
