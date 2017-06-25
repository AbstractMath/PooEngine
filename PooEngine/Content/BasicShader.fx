float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 WorldInverseTranspose;
float4 AmbientColor = float4(0.5, 0.5, 0.5, 1);
float AmbientIntensity = 0.6;
float3 lightDirection = float3(1, -1, 0.5);//Can eventually be replaced

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float4 pos : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.pos = worldPosition;

	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float intensity = -dot(normal, lightDirection);
	output.Color = saturate(AmbientColor * (AmbientIntensity + intensity / 4));

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float depth = sqrt(input.pos.x * input.pos.x + input.pos.y * input.pos.y + input.pos.z * input.pos.z);
	float multiplier = 1;
	if (depth >= 4) 
	{
		multiplier = 1 / depth;
	}
	return input.Color;
}

technique Ambient
{
	pass Pass1
	{
		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
	}
}