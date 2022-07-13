Shader "Custom/Book"
{
    Properties{
        _WaveLength("WaveLength", Float) = 5.0
        _Speed("Speed", Float) = 5.0
        _Amplitude("Amplitude", Float) = 5.0
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Amount("Height Adjustment", Float) = 1.0
        _Coefficient("Coefficient", Float) = 1.0

        _A("A", Float) = 5.0
        _Theta("Theta", Float) = 5.0
        _Rho("Rho", Float) = 5.0

    }
    SubShader{

        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
        struct Input {
            float2 uv_MainTex;
        };

       // Access the shaderlab properties
        float _Amount;
        sampler2D _MainTex;
        float _WaveLength;
        float _Speed;
        float _Amplitude;
        float _Coefficient;
        float _A;
        float _Theta;
        float _Rho;

       // Vertex modifier function
        void vert(inout appdata_full v) 
        {
           //v.vertex.y = sin(v.vertex.x+_Time[0]*10)* sin(v.vertex.z + _Time[0] * 10);
                //v.vertex.y = _Coefficient*pow(v.vertex.x,2)*_Coefficient*pow(v.vertex.z,2)/15+ _Coefficient* pow(v.vertex.x, 1.5);
            float A = _A;
            float theta = _Theta;
            float rho = _Rho;


            float R = sqrt(v.vertex.x * v.vertex.x + pow(v.vertex.z - A, 2));
            // Now get the radius of the cone cross section intersected by our vertex in 3D space.
            float r = R * sin(theta);
            // Angle subtended by arc |ST| on the cone cross section.
            float beta = asin(v.vertex.x / R) / sin(theta);

            // *** MAGIC!!! ***
            float a = r * sin(beta);
            float c = R + A - r * (1 - cos(beta)) * sin(theta);
            float b = r * (1 - cos(beta)) * cos(theta);

            v.vertex.x = (a * cos(rho) - b * sin(rho)); //orizz
            v.vertex.z = c; //vert
            v.vertex.y = (a * sin(rho) + b * cos(rho)); //prof
        }

       // Surface shader function
        void surf(Input IN, inout SurfaceOutput o) 
        {
            //IN.uv_MainTex = float2(IN.uv_MainTex.x, IN.uv_MainTex.y + sin(IN.uv_MainTex.x*_WaveLength + _Time[0]*_Speed)/_Amplitude);

            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}