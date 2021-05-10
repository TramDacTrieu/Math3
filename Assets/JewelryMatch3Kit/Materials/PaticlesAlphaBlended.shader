
Shader "Hidden/Particles/Alpha Blended + Additive"
{
Properties
{
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category
{
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	ColorMask RGB
	Cull Off
	Lighting Off
	ZWrite Off
	Fog { Color (0,0,0,0) }
	
	SubShader
	{
		UsePass "Hidden/Particles/ALPHA_BLENDED_VCOLOR"
		UsePass "Hidden/Particles/ADDITIVE"
	}	
}
}
