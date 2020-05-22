#version 330 core

uniform float windowAspect; // largura dividido pela altura da janela

uniform vec2 center;
uniform float scale;
uniform int maxIterations;

uniform sampler2D colorsTexture;

varying vec2 pixelPosition;

out vec4 fragColor;

// Calcula o quadrado de um numero complexo.
// Lembre-se: sqrt(-1)^2 == -1
vec2 complexSquared(vec2 c)
{
	return vec2(
		c.x * c.x - c.y * c.y,
		2.0 * c.x * c.y
	);
}

int runMandelbrotIterations(vec2 c)
{
	// z0 = 0 (por definicao)
	// z1 = z0 + c ==> z1 = c
	vec2 z = c;

	int n;

	for (n = 0; n < maxIterations; ++n)
	{
		z = complexSquared(z) + c; // Essa eh a equacao de Mandelbrot!

		if (length(z) > 2.0)
		{
			break; // c esta fora do conjunto! Podemos parar de iterar.
		}
	}

	return n;
}

void main()
{
	vec2 c = vec2(
		windowAspect * pixelPosition.x * scale + center.x,
		pixelPosition.y * scale + center.y
	);

	int n = runMandelbrotIterations(c);

	if (n == maxIterations)
	{
		// Nao chegamos em Z com tamanho maior que 2 dentro do numero maximo de iteracoes.
		// Portanto, assumimos que c esta dentro do conjunto de Mandelbrot.
		fragColor = vec4(0.0, 0.0, 0.0, 0.0);
	}
	else
	{
		// Chegamos num Z com tamanho maior que 2.
		// Portanto, 'c' esta fora do conjunto de Mandelbrot.
		// Vamos dar pra esse pixel uma cor bonita, baseado no numero de iteracoes que levamos.
		fragColor = texture(colorsTexture, vec2(n / float(maxIterations), 0.0));
	}
}