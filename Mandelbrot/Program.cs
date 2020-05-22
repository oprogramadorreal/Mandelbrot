using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

namespace Mandelbrot
{
    /**
     * Esse eh o codigo do "programa burocratico" que roda na CPU.
     */
    sealed class Program : GameWindow
    {
        private int shaderProgram;
        private int vao;
        private int vbo;
        private int colorsTexture;

        private float scale = 1.0f;
        private Vector2 center = new Vector2(-3.0f/4.0f, 0.0f);
        private int maxIterations = 100;

        // Vertices do retangulo colado na tela (em "normalized device coordinates" ou NDC)
        private readonly float[] vertices =
        {
            -1.0f, -1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 0.0f,
            1.0f, -1.0f, 0.0f
        };

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.ShiftLeft))
            {
                // controla numero maximo de iteracoes
                maxIterations = Math.Max(2, maxIterations + e.Delta * 10);
            }
            else
            {
                // controla zoom da imagem
                scale *= (1.0f - e.Delta * 0.05f);
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            // controla posicao da imagem
            if (e.Mouse.IsButtonDown(MouseButton.Left))
            {
                var translationSpeed = 0.01f * scale;

                center.X += e.XDelta * translationSpeed;
                center.Y -= e.YDelta * translationSpeed;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var windowAspect = ClientSize.Width / (float)ClientSize.Height;

            GL.UseProgram(shaderProgram);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "windowAspect"), windowAspect);
            GL.Uniform2(GL.GetUniformLocation(shaderProgram, "center"), center);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "scale"), scale);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "maxIterations"), maxIterations);
            BindTexture("colorsTexture", 0);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4); // desenha o retangulo colado na tela

            SwapBuffers();
        }

        private void BindTexture(string uniformName, int textureUnit)
        {
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, uniformName), textureUnit);
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, colorsTexture);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            colorsTexture = Texture2dFactory.Create("colors.bmp", false);
            shaderProgram = ShadersFactory.CreateProgram("Shaders/vertex_shader.glsl", "Shaders/fragment_shader.glsl");

            CreateVertexBuffers();
        }

        private void CreateVertexBuffers()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            // envia geometria do retangulo pra placa grafica
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);

            GL.DeleteTexture(colorsTexture);
            GL.DeleteProgram(shaderProgram);
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
        }

        static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}
