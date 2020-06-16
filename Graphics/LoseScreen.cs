using GlmNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace Graphics
{
    class LoseScreen : Screen
    {

        public int progress = 0;

        Shader shader2D;
        Texture bar, text, background;
        uint barID, textID, backgroundID;
        mat4 barTransform, textTransform, backTransform;

        int loc;

        public LoseScreen()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            shader2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");

            text = new Texture(projectPath + "\\Textures\\Lose.png", 2, false);
            background = new Texture(projectPath + "\\Textures\\solidBack.png", 3, false);

            float[] textVerts =
            {
                -0.2f,  0.1f, 0f,        0,0, //Top Left 
                -0.2f, -0.1f, 0f,        0,1, //Bottom Left
                 0.2f, -0.1f, 0f,        1,1, //Bottom Right

                -0.2f,  0.1f, 0f,        0,0, //Top Left
                 0.2f, -0.1f, 0f,        1,1, //Bottom Right
                 0.2f,  0.1f, 0f,        1,0  //Top Right

            };


            float[] backVerts =
            {
                -1f,  1f, 0f,        0,0, //Top Left 
                -1f, -1f, 0f,        0,1, //Bottom Left
                 1f, -1f, 0f,        1,1, //Bottom Right

                -1f,  1f, 0f,        0,0, //Top Left
                 1f, -1f, 0f,        1,1, //Bottom Right
                 1f,  1f, 0f,        1,0  //Top Right
            };

            textID = GPU.GenerateBuffer(textVerts);
            backgroundID = GPU.GenerateBuffer(backVerts);

            backTransform = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(1f, 1f, 1f))});
            textTransform = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(1f, 1f, 1f))});

            Gl.glClearColor(0, 0, 0, 1);

            shader2D.UseShader();
            loc = Gl.glGetUniformLocation(shader2D.ID, "model");
        }

        public override void cleanup()
        {
            shader2D.DestroyShader();
        }

        public override void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            shader2D.UseShader();

            Gl.glEnableVertexAttribArray(0);
            Gl.glEnableVertexAttribArray(1);

            //-----------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, backgroundID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glUniformMatrix4fv(loc, 1, Gl.GL_FALSE, backTransform.to_array());

            background.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, textID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glUniformMatrix4fv(loc, 1, Gl.GL_FALSE, textTransform.to_array());

            text.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //------------------------------------------------------------------------


            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

        }

        public override void update(float deltaTime)
        {
            //throw new NotImplementedException();
        }
    }
}
