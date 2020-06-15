Shader "Custom/RGBOffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_OffsetAmn("RGB Offset", float) = 0
		_RedAngle("Red Angle", float) = 0
		_GreenAngle("Green Angle", float) = 0
		_BlueAngle("Blue Angle", float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			float _Offset, _RedAngle, _GreenAngle, _BlueAngle;

			fixed4 frag(v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, i.uv);

				float2 rOffset = float2(_Offset * cos(_RedAngle), _Offset * sin(_RedAngle));
				float2 gOffset = float2(_Offset * cos(_GreenAngle), _Offset * sin(_GreenAngle));
				float2 bOffset = float2(_Offset * cos(_BlueAngle), _Offset * sin(_BlueAngle));

				float2 rUV = float2(i.uv.x + rOffset.x, i.uv.y + rOffset.y);
				float2 gUV = float2(i.uv.x + gOffset.x, i.uv.y + gOffset.y);
				float2 bUV = float2(i.uv.x + bOffset.x, i.uv.y + bOffset.y);

				fixed4 colR = tex2D(_MainTex, rUV);
				fixed4 colG = tex2D(_MainTex, gUV);
				fixed4 colB = tex2D(_MainTex, bUV);

				fixed4 newCol = fixed4(colR.x, colG.y, colB.z, 1);

                //col.rgb = 1 - col.rgb;
                return newCol;
            }
            ENDCG
        }
    }
}
