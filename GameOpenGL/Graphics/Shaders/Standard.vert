#version 330

layout (location = 0) in vec4 position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 uvPos;

smooth out vec4 theColor;
out vec2 theUVPos;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main()
{
	vec4 position = (projectionMatrix * viewMatrix * modelMatrix * position);
	position.z *= -1;										//Reverseing z so that high z = depth
	gl_Position = position;
	
	theUVPos = uvPos;
	theColor = color;
}
