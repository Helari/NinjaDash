// Shader created with Shader Forge v1.27 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.27;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32717,y:32718,varname:node_3138,prsc:2|emission-4207-RGB,alpha-2856-OUT;n:type:ShaderForge.SFN_Tex2d,id:6009,x:32360,y:32853,ptovrint:False,ptlb:node_6009,ptin:_node_6009,varname:node_6009,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:22b79950539f7d7418b26cf1e758bd7b,ntxv:0,isnm:False|UVIN-3005-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4677,x:31422,y:32702,varname:node_4677,prsc:2;n:type:ShaderForge.SFN_Append,id:500,x:31784,y:32782,varname:node_500,prsc:2|A-9129-OUT,B-7517-OUT;n:type:ShaderForge.SFN_Color,id:4207,x:32461,y:32666,ptovrint:False,ptlb:color,ptin:_color,varname:node_4207,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7176471,c2:1,c3:0.9882354,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7235,x:32292,y:33046,ptovrint:False,ptlb:node_7235,ptin:_node_7235,varname:node_7235,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:51c4afa1743bfe3458f491aa75e54b91,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3005,x:31957,y:32795,varname:node_3005,prsc:2|A-500-OUT,B-9133-OUT;n:type:ShaderForge.SFN_Vector1,id:9133,x:31341,y:33078,varname:node_9133,prsc:2,v1:0.1432;n:type:ShaderForge.SFN_Add,id:9129,x:31640,y:32659,varname:node_9129,prsc:2|A-4677-X,B-2618-OUT;n:type:ShaderForge.SFN_Add,id:7517,x:31640,y:32837,varname:node_7517,prsc:2|A-4677-Z,B-2618-OUT;n:type:ShaderForge.SFN_Vector1,id:4830,x:31238,y:32905,varname:node_4830,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Divide,id:2618,x:31442,y:32871,varname:node_2618,prsc:2|A-4830-OUT,B-9133-OUT;n:type:ShaderForge.SFN_Multiply,id:2856,x:32538,y:32946,varname:node_2856,prsc:2|A-6009-A,B-7235-A;proporder:6009-4207-7235;pass:END;sub:END;*/

Shader "Shader Forge/Ground_glow" {
    Properties {
        _node_6009 ("node_6009", 2D) = "white" {}
        _color ("color", Color) = (0.7176471,1,0.9882354,1)
        _node_7235 ("node_7235", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float3 emissive = _color.rgb;
                float3 finalColor = emissive;
                float node_9133 = 0.1432;
                float node_2618 = (0.5/node_9133);
                float2 node_3005 = (float2((i.posWorld.r+node_2618),(i.posWorld.b+node_2618))*node_9133);
                float4 _node_6009_var = tex2D(_node_6009,TRANSFORM_TEX(node_3005, _node_6009));
                float4 _node_7235_var = tex2D(_node_7235,TRANSFORM_TEX(i.uv0, _node_7235));
                return fixed4(finalColor,(_node_6009_var.a*_node_7235_var.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
