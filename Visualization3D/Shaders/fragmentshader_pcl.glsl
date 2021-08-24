#version 330

layout(location=0) in vec3 vec_color;
out vec4 FragColor;

void main(void)
{
	FragColor = vec4(vec_color, 1.0);
}
