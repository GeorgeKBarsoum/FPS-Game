using System;
using System.Collections.Generic;
using System.IO;
using GlmNet;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

namespace Graphics
{
    class HealthBar
    {
        uint hpID;
        Shader shader;
        Texture bhp;
        Texture hp;
        mat4 healthbar;
        mat4 backhealthbar;
        int mloc;
        public mat4 scaleMatBack;
        public mat4 transMatBack;

        public mat4 scaleMatFront;
        public mat4 transMatFront;

        public mat4 rotMat;

        float[] squarevertices = {
                //-1,1,0,
                //0,1,

                //1,-1,0,
                //1,0,

                //-1,-1,0,
                //0,0,

                //1,1,0,
                //1,1,

                //-1,1,0,
                //0,1,

                //1,-1,0,
                //1,0
                -1,1,0,
                0,0,

                1,-1,0,
                1,1,

                -1,-1,0,
                0,1,

                1,1,0,
                1,0,

                -1,1,0,
                0,0,

                1,-1,0,
                1,1
        };

        public HealthBar(Shader sh)
        {
            scaleMatBack = glm.scale(new mat4(1), new vec3(0.5f, 0.1f, 1));
            transMatBack = glm.translate(new mat4(1), new vec3(-0.5f, 0.9f, 0.1f));

            scaleMatFront = glm.scale(new mat4(1), new vec3(0.48f, 0.1f, 1));
            transMatFront = glm.translate(new mat4(1), new vec3(-0.5f, 0.9f, 0));

            rotMat = new mat4(1);
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            hpID = GPU.GenerateBuffer(squarevertices);

            shader = sh;

            backhealthbar = MathHelper.MultiplyMatrices(new List<mat4>(){
                scaleMatBack, transMatBack });
            healthbar = MathHelper.MultiplyMatrices(new List<mat4>() {
                scaleMatFront, transMatFront });
            shader.UseShader();
            mloc = Gl.glGetUniformLocation(shader.ID, "model");

            hp = new Texture(projectPath + "\\Textures\\HP.bmp", 8, false);
            bhp = new Texture(projectPath + "\\Textures\\BackHP.bmp", 9, false);
            
        }

        public void Draw2D()
        {
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            shader.UseShader();
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));




            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, backhealthbar.to_array());
            bhp.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            
            healthbar = MathHelper.MultiplyMatrices(new List<mat4>() {
                 scaleMatFront, transMatFront });
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
            hp.Bind();

            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            Gl.glEnable(Gl.GL_DEPTH_TEST);
        }
        public void Draw3D()
        {
            //Gl.glDisable(Gl.GL_DEPTH_TEST);
            //shader.UseShader();
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));


            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, backhealthbar.to_array());
            bhp.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            
            backhealthbar = MathHelper.MultiplyMatrices(new List<mat4>(){
                scaleMatBack, rotMat, transMatBack });
            healthbar = MathHelper.MultiplyMatrices(new List<mat4>() {
                 scaleMatFront, rotMat, transMatFront });
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
            hp.Bind();

            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            Gl.glEnable(Gl.GL_DEPTH_TEST);
        }


    }
}
