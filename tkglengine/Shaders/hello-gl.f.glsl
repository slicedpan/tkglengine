#version 330

out vec4 outputColor;
in float amount;
in vec3 normal;

void main()
{
	outputColor = vec4(normal * (amount + 1.0), 1.0);
}
