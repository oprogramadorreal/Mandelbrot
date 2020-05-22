using OpenTK.Graphics.OpenGL;
using System.IO;

namespace Mandelbrot
{
    /**
     * Classe utilitaria para criar e compilar os shaders.
     **/
    public static class ShadersFactory
    {
        public static int CreateProgram(string vertexShaderFile, string fragmentShaderFile)
        {
            var shaderProgram = GL.CreateProgram();

            var vertexShader = CreateShader(ShaderType.VertexShader, ReadShaderCode(vertexShaderFile));
            var fragmentShader = CreateShader(ShaderType.FragmentShader, ReadShaderCode(fragmentShaderFile));

            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);

            GL.LinkProgram(shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        private static int CreateShader(ShaderType type, string shaderCode)
        {
            var shaderId = GL.CreateShader(type);

            GL.ShaderSource(shaderId, shaderCode);
            GL.CompileShader(shaderId);

            return shaderId;
        }

        private static string ReadShaderCode(string shaderFile)
        {
            using (var reader = new StreamReader(shaderFile))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
