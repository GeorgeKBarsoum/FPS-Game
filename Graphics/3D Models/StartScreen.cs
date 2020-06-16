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
    class StartScreen : Screen
    {

        Shader shader2D;
        Texture start, controls, background;
        uint startID, controlsID, backgroundID;
        mat4 startTransform, controlsTransform, backTransform;

        int loc;

        public StartScreen()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            shader2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");

            start = new Texture(projectPath + "\\Textures\\Start.png", 1, false);
            controls = new Texture(projectPath + "\\Textures\\Controls.png", 2, false);
            background = new Texture(projectPath + "\\Textures\\menuBackground.jpg", 3, false);

            float[] startVerts =
            {
                -0.7f, 0.7f, 0f,        0,0, //Top Left 
                -0.7f, 0.5f, 0f,        0,1, //Bottom Left
                -0.4f, 0.5f, 0f,        1,1, //Bottom Right

                -0.7f, 0.7f, 0f,        0,0, //Top Left
                -0.4f, 0.5f, 0f,        1,1, //Bottom Right
                -0.4f, 0.7f, 0f,        1,0  //Top Right

            };

            float[] controlsVerts =
            {
                -0.7f, 0.3f, 0f,        0,0, //Top Left 
                -0.7f, 0.1f, 0f,        0,1, //Bottom Left
                -0.3f, 0.1f, 0f,        1,1, //Bottom Right

                -0.7f, 0.3f, 0f,        0,0, //Top Left
                -0.3f, 0.1f, 0f,        1,1, //Bottom Right
                -0.3f, 0.3f, 0f,        1,0  //Top Right
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

            startID = GPU.GenerateBuffer(startVerts);
            controlsID = GPU.GenerateBuffer(controlsVerts);
            backgroundID = GPU.GenerateBuffer(backVerts);

            backTransform = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(1f, 1f, 1f))});
            startTransform = MathHelper.MultiplyMatrices(new List<mat4>() {
                glm.scale(new mat4(1), new vec3(1f, 1f, 1))});

            Gl.glClearColor(0, 0, 0, 1);

            shader2D.UseShader();
            loc = Gl.glGetUniformLocation(shader2D.ID, "model");

        }

        public override void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            shader2D.UseShader();

            Gl.glEnableVertexAttribArray(0);
            Gl.glEnableVertexAttribArray(1);

            //------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, backgroundID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glUniformMatrix4fv(loc, 1, Gl.GL_FALSE, backTransform.to_array());

            background.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, startID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glUniformMatrix4fv(loc, 1, Gl.GL_FALSE, startTransform.to_array());

            start.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, controlsID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            controls.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //-------------------------------------------------------------------------

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
        }

        public override void update(float deltaTime)
        {
            //throw new NotImplementedException();
        }

        public override void cleanup()
        {
            shader2D.DestroyShader();
        }
    }
}
