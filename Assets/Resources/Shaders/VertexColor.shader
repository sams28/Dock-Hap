

Shader "Vertex Colored" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
    Pass {
        ColorMaterial AmbientAndDiffuse

        SetTexture [_MainTex] {
            Combine texture * primary, texture * primary
        }
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine previous * constant DOUBLE, previous * constant
        } 

    } //end pass
}  // end sub shader

Fallback " VertexLit", 1
}