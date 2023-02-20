// TA.Kim 

Shader "MatCap/PBR Outline(CutOff)"
{
    Properties
    {
        [Toggle(OUTLINE)] _ApplyOutline ("Apply Outline", Int) = 0
        [Toggle(CUTOFF)] _ApplyCutOff ("Apply CutOff", Int) = 0
        [Toggle(PBRSHADER)] _ApplyPBR ("Apply PBR", Int) = 0
        _Cutoff ("Alpha cutoff", Range (0,1)) = 0.5
        _MatCap ("Matcap", 2D) = "white" {}
        _MatCapRotate ("MatCap Rotate(deg)", float) = 0
        _OutlineWidth("Outline", Float) = 0.1
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _MainCol ("Base Color (RGB)", Color) = (1,1,1,1)
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _LightDir ("Light Direction", Color) = (1,1,0,0)
        _NormalStrength("Normal Strength", Float) = 1
        _MetalicMap ("Metallic (RGB)", 2D) = "white" {}
        _Metalicness("Metalicness", Float) = 0
        _AOMap ("AO (RGB)", 2D) = "white" {}
        _AOFactor ("AO Factor", Float) = 1
        _EmissiveColor ("_Emissive Color", Color) = (0,0,0,1)
        _EmissiveTex ("Emissive (RGB)", 2D) = "white" {}
        _EmissiveIntensity("Emissive Intensity", Float) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            Name "OUTLINE"
            Cull Front
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature OUTLINE
            #include "UnityCG.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
 
            float _OutlineWidth;
            sampler2D _MainTex;
			float4 _MainTex_ST;
 
            v2f vert(appdata_base v) {
                v2f o;

                // Convert vertex to clip space
                o.pos = UnityObjectToClipPos(v.vertex);

            # if OUTLINE
                // Convert normal to view space (camera space)
                float3 normal = mul((float3x3) UNITY_MATRIX_IT_MV, v.normal);
 
                // Compute normal value in clip space
                normal.x *= UNITY_MATRIX_P[0][0];
                normal.y *= UNITY_MATRIX_P[1][1];
 
                // Scale the model depending the previous computed normal and outline value
                o.pos.xy += _OutlineWidth * normal.xy * .005;

            # else 
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
            # endif

                return o;
            }
 
            fixed4 _OutlineColor;
 
            fixed4 frag(v2f i) : SV_Target {
                float4 col = tex2D(_MainTex, i.uv.xy);
            # if OUTLINE
                return _OutlineColor;
            # else
                return col;
            # endif
            }
 
            ENDCG
        }

        // Matcap Pass
        Pass {
            Name "PBR Matcap"
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma shader_feature CUTOFF PBRSHADER
             
            #include "UnityCG.cginc"
            struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

            struct v2f
            {
                float4 vertex: SV_POSITION;
                float2 matcap_uv: TEXCOORD0;
                float3 T : TEXCOORD1;
				float3 B : TEXCOORD2;
				float3 N : TEXCOORD3;
				half3 viewDir : TEXCOORD4;

                float2 uv : TEXCOORD5;
            };

            sampler2D _MatCap;
            float _MatCapRotate;
            
            sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _BumpTex;
			float4 _BumpTex_ST;

            void Fuc_LocalNormal2TBN(half3 localnormal, float4 tangent, inout half3 T, inout half3  B, inout half3 N)
			{
				half fTangentSign = tangent.w * unity_WorldTransformParams.w;
				N = normalize(UnityObjectToWorldNormal(localnormal));
				T = normalize(UnityObjectToWorldDir(tangent.xyz));
				B = normalize(cross(N, T) * fTangentSign);
			}

			half3 Fuc_TangentNormal2WorldNormal(half3 fTangnetNormal, half3 T, half3  B, half3 N)
			{
				float3x3 TBN = float3x3(T, B, N);
				TBN = transpose(TBN);
				return mul(TBN, fTangnetNormal);
			}

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //Multiply by The inverse transpose matrix transforms the normal to the view space
                float3 viewnormal = mul(UNITY_MATRIX_IT_MV, v.normal);
                //Need to normalize, otherwise, ensure that the normal is in the (-1,1) interval, otherwise the effect of the scaled object is not correct
                viewnormal = normalize(viewnormal);
                // o.viewDir = viewnormal;
                o.matcap_uv.xy = viewnormal.xy * .5 + .5;
                
                float2 pivot = float2(0.5, 0.5);
                float uv_rotate = _MatCapRotate / 180 * 3.14;
                float c = cos(uv_rotate);
                float s = sin(uv_rotate);
                float2x2 mat = float2x2(c,-s,
                                        s,c);
                float2 cap_uv = o.matcap_uv - pivot;
                o.matcap_uv = mul(mat, cap_uv);
                o.matcap_uv += pivot;
    
                Fuc_LocalNormal2TBN(v.normal, v.tangent, o.T, o.B, o.N);

                return o;
            }
            uniform sampler2D _MetalicMap;
            uniform sampler2D _EmissiveTex;
            uniform sampler2D _AOMap;
            float _NormalStrength;
            float _AOFactor;
            float _Metalicness;
            float _EmissiveIntensity;
            float _Cutoff;
            float4 _EmissiveColor;
            float4 _LightDir;
            float4 _MainCol;
            
            fixed4 frag (v2f i): SV_Target {
                fixed4 mat = tex2D(_MatCap, i.matcap_uv) * _MainCol;
                fixed4 tex = tex2D(_MainTex, i.uv.xy);
                fixed4 ao = tex2D(_AOMap, i.uv.xy);
                fixed4 metalic = tex2D(_MetalicMap, i.uv.xy) * _Metalicness;
                fixed4 emiss = tex2D(_EmissiveTex, i.uv.xy) * _EmissiveColor * _EmissiveIntensity;

                half3 fTangnetNormal = UnpackNormal(tex2D(_BumpTex, i.uv * _BumpTex_ST.rg));
                fTangnetNormal.xy *= _NormalStrength;
                float3 worldNormal = Fuc_TangentNormal2WorldNormal(fTangnetNormal, i.T, i.B, i.N);
                fixed fNDotL = dot(_LightDir, worldNormal);
                float3 fReflection = reflect(_LightDir, worldNormal);
				fixed fSpec_Phong = lerp( tex, fReflection, saturate(float4(metalic.rgb * _Metalicness, 1)));
				fSpec_Phong = pow(fSpec_Phong, 20.0f);
                float4 mc;
            # if PBRSHADER
                mc = mat * tex * fNDotL + fSpec_Phong * lerp( 1, ao, _AOFactor) + emiss;
            # else
                mc = mat * tex;
            # endif

            # if CUTOFF
                clip(tex.a - _Cutoff);
            # endif

				return float4(mc.rgb, tex.a);

            }
            ENDCG
        }
    }
    Fallback "Texture"
}