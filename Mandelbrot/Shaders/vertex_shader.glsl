#version 330 core

layout (location = 0) in vec3 vertexPosition;

varying vec2 pixelPosition;

void main()
{
	pixelPosition = vertexPosition.xy;
	gl_Position = vec4(vertexPosition, 1.0);
}