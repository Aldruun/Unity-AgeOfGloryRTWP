Shader "Emissive/Simple" {
    Properties{
        _Emissive("Emissive Color", Color) = (1,1,1,1)
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass {
                ZTest Always
                Lighting On
                Material {
                Emission[_Emissive]
                }
            }
    }
}
