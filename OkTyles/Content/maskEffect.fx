#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D LayerTexture;
sampler2D LayerTextureSampler = sampler_state
{
	Texture = <LayerTexture>;
};

Texture2D SelectionTexture;
sampler2D SelectionTextureSampler = sampler_state
{
	Texture = <SelectionTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	// Überprüft, ob an der aktuellen Texturkoordinate ein Pixel in beiden Texturen vorhanden ist
	float4 layerColor = tex2D(LayerTextureSampler, input.TextureCoordinates);
	float4 selectionColor = tex2D(SelectionTextureSampler, input.TextureCoordinates);

	if (layerColor.a > 0 && selectionColor.a > 0)
	{
		// Wenn beide Texturen eine Farbe haben, gib Weiß zurück
		return float4(1, 1, 1, 1);
	}
	else
	{
		// Ansonsten gib Transparent zurück
		return float4(0, 0, 0, 0);
	}
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
