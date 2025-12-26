Shader "Custom/2DWaterWiggle"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Speed ("Tốc độ Sóng", Range(0, 10)) = 2
        _Frequency ("Độ Dày Sóng", Range(0, 20)) = 10
        _Amplitude ("Độ Mạnh Sóng", Range(0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float _Speed;
            float _Frequency;
            float _Amplitude;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // --- PHẦN QUAN TRỌNG NHẤT: LÀM MÉO HÌNH ---
                
                float2 uv = IN.texcoord;

                // Công thức sóng hình Sin:
                // Sóng sẽ uốn lượn theo trục Y (độ cao)
                // uv.y * _Frequency: Tạo nhiều khúc uốn
                // _Time.y * _Speed: Làm sóng di chuyển
                float wave = sin(uv.y * _Frequency + _Time.y * _Speed) * _Amplitude;

                // Cộng độ lệch sóng vào trục X (ngang) của ảnh
                uv.x += wave;

                // -------------------------------------------

                // Lấy màu từ ảnh gốc theo UV đã bị làm méo
                fixed4 c = tex2D(_MainTex, uv) * IN.color;
                
                // Áp dụng Alpha (độ trong suốt)
                c.rgb *= c.a;
                
                return c;
            }
            ENDCG
        }
    }
}