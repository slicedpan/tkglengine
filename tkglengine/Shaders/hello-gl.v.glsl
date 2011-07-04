#version 330

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;

uniform mat4 WVP;
uniform vec3 LightPos;

out float amount;
out vec3 normal;

void main()
{
	amount = min(dot(normalize(LightPos - Position), normalize(Normal)), 0.0);
	normal = normalize(Normal);
	gl_Position = WVP * vec4(Position, 1.0);
}
