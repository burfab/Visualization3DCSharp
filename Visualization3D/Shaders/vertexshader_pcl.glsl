#version 330

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 color;
layout(location = 0) out vec3 vec_color;

uniform mat4 MVP;

void main(void)
{
	gl_Position = MVP * vec4(position,1);
	gl_PointSize = 2;
	vec_color = color;
}
