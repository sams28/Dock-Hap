Shader "Custom/Low shader" {
	Properties {
		_Color ("Color", Color) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass { // some shaders require multiple passes
        
         Cull Off
           GLSLPROGRAM // here begins the part in Unity's GLSL
 
 

 		uniform fixed4 _Color;
 		
         #ifdef VERTEX // here begins the vertex shader
 		
         void main() // all vertex shaders define a main() function
         {
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
               // this line transforms the predefined attribute 
               // gl_Vertex of type vec4 with the predefined
               // uniform gl_ModelViewProjectionMatrix of type mat4
               // and stores the result in the predefined output 
               // variable gl_Position of type vec4.
           
         }
 
         #endif // here ends the definition of the vertex shader
 
 
         #ifdef FRAGMENT // here begins the fragment shader
 		
 		
 		
         void main() // all fragment shaders define a main() function
         {
         
         	
           gl_FragColor = _Color; 

               // this fragment shader just sets the output color 
               // to opaque red (red = 1.0, green = 0.0, blue = 0.0, 
               // alpha = 1.0)
         }
 
         #endif // here ends the definition of the fragment shader
 
         ENDGLSL // here ends the part in GLSL 
      }
	} 
	
}
