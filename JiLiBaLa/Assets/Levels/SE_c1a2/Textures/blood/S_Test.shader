Shader "Unlit/S_Test"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex VERT			//*指定顶点着色器
			#pragma fragment FRAG		//*指定片段着色器
			#include "UnityCG.cginc"	//*引入Unity内置定义

			struct appdata	//*定义顶点着色器输入结构体
			{	//*顶点着色器是对模型每一个顶点进行操作的代码，所以输入输出自然是一个顶点的结构数据。
				float4 vertex : POSITION;//*模型坐标系的坐标值
				float2 uv : TEXCOORD;
				//float3 pos : POSITION;
				//float4 color : COLOR;
			};

			struct v2f	//*定义顶点着色器输出结构体
			{
				float4 vertex : SV_POSITION;	//*屏幕坐标，SV是干什么的
				float2 uv : TEXCOORD;
				//float4 posH : SV_POSITION;	//*输出的顶点是矩阵计算后的float4
			};

			v2f VERT(appdata v)	//*顶点函数
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);//*将传进来的模型坐标系的坐标值转换成屏幕坐标
				o.uv = v.uv;
				return o;//*将计算后的结果输出给渲染引擎，底层会根据具体语义去做对应处理
			}

			sampler2D _MainTex;

			fixed4 FRAG(v2f i) : SV_Target	//*片元函数
			{
				fixed4 color = tex2D(_MainTex,i.uv);
				color.rgb = 1-color.rgb;//*反色？
				return color;
			}

			ENDCG
		}
	}
}
