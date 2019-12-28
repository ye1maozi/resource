Shader "UI/ButtonLightShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//遮罩
		_LightTex ("lightTex",2D) = "white" {}
		//间隔时间
		_speed ("Speed",float) = 2
		//颜色 
		_FlashColor ("FlashColor",Color) = (1,1,1,0.8)
		//角度
		_Angle ("Angle",range(0,90)) = 10
		//调整颜色区域的值
		_off ("offColor",range(0,1)) = 1
		//持续时间(速度)
		_LightDur ("lightDuration",float) = 2

		//位置偏移
		_Off ("_Off",float) = 0.75

		//mask不能遮挡设置
		 _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("ColorMask", Float) = 15
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"  }
		Cull Off
		Lighting Off
		ZTest Always
	    ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
		 Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        ColorMask[_ColorMask]
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
				float2 lightuv: TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _LightTex;
			float4 _LightTex_ST;
			fixed _speed;
			fixed4 _FlashColor;
			fixed _Angle;
			fixed _off;
			fixed _LightDur;
			fixed _Off;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				//旋转中心点
				float center = float2(0.5,0.5);
				//角度
				float rote = (_Angle *3.14)/180;
				float sinNum = sin(rote);
				float cosNum = cos(rote);

				//旋转
				float2 lxy = mul(v.uv , float2x2(cosNum , -sinNum,sinNum,cosNum)) ;

				fixed currPass =  fmod(_Time.y,_speed);
				lxy.x += currPass / _LightDur;
				lxy.y += currPass / _LightDur;
				//lxy.x += currPass;
				lxy.y -= _Off;
				o.lightuv = TRANSFORM_TEX(lxy, _LightTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;
				fixed4 col = tex2D(_MainTex, uv);
				
			
				float2 uv2 = i.lightuv;
				uv2.y *= 16;
				//uv2.x *= 2 ;
				//uv2.x += _Time.y * _speed;
				fixed4 c2 = tex2D(_LightTex,uv2);
				//显示区域
	
				col += c2 *c2.a *col.a * _FlashColor;
				return col ;
			}
			ENDCG
		}
	}
}
