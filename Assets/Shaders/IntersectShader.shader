Shader "2D Overlap Outline"
{
    Properties
    {
        _ColorMain("Main Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _ColorBorder("Border", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex("Texture to blend", 2D) = "" {}
    _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
    {
        Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }



        Pass
        {
        Cull Off
        Lighting Off
        ZWrite On
        ZTest Always
        Offset -1, -1
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            half4 _ColorMain;
            half4 _ColorBorder;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };


            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy;
                return o;
            }



            half4 frag(v2f i) : COLOR
            {

                half4 color = tex2D(_MainTex, i.uv);


                half4 delta = _ColorBorder - _ColorMain;


                half4 main = _ColorMain + (color * delta);


                main.a = color.a;

                return main;
            }

            ENDCG
        }



        Pass
        {
            Offset -1, -1

            Blend SrcAlpha OneMinusSrcAlpha


                AlphaTest Greater 0.5

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                uniform sampler2D _MainTex;
                half4 _ColorMain;
                half4 _ColorBorder;

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                };

                v2f vert(appdata_base v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.texcoord.xy;
                    return o;
                }

                half4 frag(v2f i) : COLOR
                {

                    half4 color = tex2D(_MainTex, i.uv);


                    half4 main = _ColorMain;


                    main.a = min(color.a, 1 - color.r);

                    return main;
                }

                ENDCG
            }
    }
}