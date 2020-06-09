#version 330

smooth in vec4 theColor;
in vec2 theUVPos;

out vec3 outputColor;

uniform sampler2D tex;

void main()
{
	outputColor = texture(tex, theUVPos).rgb;
}
