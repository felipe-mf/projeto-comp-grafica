Shader "Custom/LaserGrid"
{
    Properties
    {
        _Color ("Grid Color", Color) = (0, 1, 1, 1)
        _EmissionStrength ("Emission Strength", Range(0, 5)) = 2
        _GridScale ("Grid Scale", Range(1, 50)) = 10
        _LineThickness ("Line Thickness", Range(0.01, 0.5)) = 0.05
        _Alpha ("Transparency", Range(0, 1)) = 0.4
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
            
            float4 _Color;
            float _EmissionStrength;
            float _GridScale;
            float _LineThickness;
            float _Alpha;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Calcula a grade
                float2 grid = frac(i.uv * _GridScale);
                
                // Linhas horizontais e verticais
                float lineX = step(1.0 - _LineThickness, grid.x);
                float lineY = step(1.0 - _LineThickness, grid.y);
                
                // Combina as linhas
                float gridPattern = max(lineX, lineY);
                
                // Cor final
                fixed4 col = _Color * _EmissionStrength;
                col.a = gridPattern * _Alpha;
                
                return col;
            }
            ENDCG
        }
    }
}