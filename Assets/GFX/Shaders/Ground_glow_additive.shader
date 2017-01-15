// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|emission-5441-OUT;n:type:ShaderForge.SFN_Tex2d,id:4127,x:32310,y:32816,ptovrint:False,ptlb:node_6009,ptin:_node_6009,varname:node_6009,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:22b79950539f7d7418b26cf1e758bd7b,ntxv:0,isnm:False|UVIN-8655-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:7762,x:31486,y:32766,varname:node_7762,prsc:2;n:type:ShaderForge.SFN_Append,id:4350,x:31848,y:32846,varname:node_4350,prsc:2|A-3007-OUT,B-9188-OUT;n:type:ShaderForge.SFN_Color,id:9886,x:32356,y:32635,ptovrint:False,ptlb:color,ptin:_color,varname:node_4207,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7176471,c2:1,c3:0.9882354,c4:1;n:type:ShaderForge.SFN_Tex2d,id:3883,x:32242,y:33009,ptovrint:False,ptlb:node_7235,ptin:_node_7235,varname:node_7235,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:51c4afa1743bfe3458f491aa75e54b91,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:8655,x:32021,y:32859,varname:node_8655,prsc:2|A-4350-OUT,B-7166-OUT;n:type:ShaderForge.SFN_Vector1,id:7166,x:31405,y:33142,varname:node_7166,prsc:2,v1:0.1432;n:type:ShaderForge.SFN_Add,id:3007,x:31704,y:32723,varname:node_3007,prsc:2|A-7762-X,B-6822-OUT;n:type:ShaderForge.SFN_Add,id:9188,x:31704,y:32901,varname:node_9188,prsc:2|A-7762-Z,B-6822-OUT;n:type:ShaderForge.SFN_Vector1,id:3122,x:31302,y:32969,varname:node_3122,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Divide,id:6822,x:31506,y:32935,varname:node_6822,prsc:2|A-3122-OUT,B-7166-OUT;n:type:ShaderForge.SFN_Multiply,id:8270,x:32488,y:32909,varname:node_8270,prsc:2|A-4127-A,B-3883-A;n:type:ShaderForge.SFN_Multiply,id:5441,x:32554,y:32718,varname:node_5441,prsc:2|A-9886-RGB,B-8270-OUT;proporder:4127-3883-9886;pass:END;sub:END;*/

Shader "Shader Forge/Ground_glow_additive" {
    Properties {
        _node_6009 ("node_6009", 2D) = "white" {}
        _node_7235 ("node_7235", 2D) = "white" {}
        _color ("color", Color) = (0.7176471,1,0.9882354,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _node_6009; uniform float4 _node_6009_ST;
            uniform float4 _color;
            uniform sampler2D _node_7235; uniform float4 _node_7235_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_7166 = 0.1432;
                float node_6822 = (0.5/node_7166);
                float2 node_8655 = (float2((i.posWorld.r+node_6822),(i.posWorld.b+node_6822))*node_7166);
                float4 _node_6009_var = tex2D(_node_6009,TRANSFORM_TEX(node_8655, _node_6009));
                float4 _node_7235_var = tex2D(_node_7235,TRANSFORM_TEX(i.uv0, _node_7235));
                float3 emissive = (_color.rgb*(_node_6009_var.a*_node_7235_var.a));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
