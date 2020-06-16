using GlmNet;
using Graphics._3D_Models;
using System;
using System.Collections.Generic;
using System.IO;
using Tao.OpenGl;

namespace Graphics
{
    class Renderer : Screen
    {
        Shader sh;
        public int modelID;
        int viewID;
        int projID;

        int playerHealth;
        Texture hp;
        Texture bhp;
        uint hpID;
        mat4 healthbar;
        mat4 backhealthbar;
        Shader shader2D;
        int mloc;
        float scalef;

        public List<Bullet> bullets;

        int EyePositionID;
        int AmbientLightID;
        int DataID;

        public int lastSize;
        public int currentSize;

        mat4 ProjectionMatrix;
        mat4 ViewMatrix;

        public float Speed = 1;

        List<Model3D> Props;

        public Model3D car2;
        List<Enemy> collided;
        public Model3D building1;
        public Model3D gun;
        public Model3D gun2;
        public bool currentGun = false;
        mat4 initalRot;
        mat4 initialRot2;

        bool playerHasMoved;

        public md2LOL zombie;
        public md2LOL zombie2;
        public md2LOL zombie3;
        public md2LOL zombie4;

        public Model3D light;

        List<Enemy> enemies;
        public List<Enemy> deadEnemies;

        List<Tuple<vec3, vec3>> minMaxPos;

        List<vec3> minPosEnemies;
        List<vec3> maxPosEnemies;


        int transID;

        public Camera cam;

        int carIndex;
        public bool dead = false;
        public bool win = false;

        //-----------------------------------------------------
      
        mat4 scaleMat;

        uint topBufferID;
        uint bottomBufferID;
        uint frontBufferID;
        uint backBufferID;
        uint leftBufferID;
        uint rightBufferID;

        uint groundBufferID;

        GraphicsForm gf;

        Texture fronttex, backtex, uptex, downtex, lefttex, righttex, ground;
        //---------------------------------------------------------------------------

        public void Initialize(GraphicsForm f)
        {
            gf = f;
            playerHealth = 5;
            playerHasMoved = false;
            Props = new List<Model3D>();
            deadEnemies = new List<Enemy>();
            minMaxPos = new List<Tuple<vec3, vec3>>();

            minPosEnemies = new List<vec3>();
            maxPosEnemies = new List<vec3>();

            collided = new List<Enemy>();

            initalRot = glm.rotate(180f / 180f * (float)Math.PI, new vec3(1, 0, 0)) * glm.rotate(270f / 180f * (float)Math.PI, new vec3(0, 1, 0)) * glm.rotate(270f / 180f * (float)Math.PI, new vec3(1, 0, 0));
            //initialRot2 = glm.rotate(90f / 180f * (float)Math.PI, new vec3(1, 0, 0));
            initialRot2 = glm.rotate(180f / 180f * (float)Math.PI, new vec3(0, 1, 0));

            lastSize = currentSize = 0;
            bullets = new List<Bullet>();
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

            f.updateLoadingScreen(0);

            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            shader2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");

            ground = new Texture(projectPath + "\\Textures\\download.png", 1, false);

            downtex = new Texture(projectPath + "\\Textures\\siege_dn.jpg", 2, false);
            uptex = new Texture(projectPath + "\\Textures\\siege_up.jpg", 3, false);
            fronttex = new Texture(projectPath + "\\Textures\\siege_ft.jpg", 4, false);
            backtex = new Texture(projectPath + "\\Textures\\siege_bk.jpg", 5, false);
            lefttex = new Texture(projectPath + "\\Textures\\siege_lf.jpg", 6, false);
            righttex = new Texture(projectPath + "\\Textures\\siege_rt.jpg", 7, false);
            hp = new Texture(projectPath + "\\Textures\\HP.bmp", 8, false);
            bhp = new Texture(projectPath + "\\Textures\\BackHP.bmp", 9, false);

            f.updateLoadingScreen(10);

            float[] groundVerts = {
                //Bottom Face
                -1f,0f,1f,  0f,0.69f,0.31f,    0f,100f,     0,1,0,
                -1f,0f,-1f, 0f,0.69f,0.31f,    100f,100f,   0,1,0,
                1f,0f,1f,   0f,0.69f,0.31f,    0f,0f,       0,1,0,

                1f,0f,-1f,  0f,0.69f,0.31f,    100f,0f,     0,1,0,
                -1f,0f,-1f, 0f,0.69f,0.31f,    100f,100f,   0,1,0,
                1f,0f,1f,   0f,0.69f,0.31f,    0f,0f,       0,1,0,
            };

            float[] top = { 
		        //Top Face 
                -1f,1f,1f, 0f,0.69f,0.31f,      0f,0f,
                -1f,1f,-1f, 0f,0.69f,0.31f,     1f,0f,
                1f,1f,1f, 0f,0.69f,0.31f,       0f,1f,

                1f,1f,-1f, 0f,0.69f,0.31f,      1f,1f,
                -1f,1f,-1f, 0f,0.69f,0.31f,     1f,0f,
                1f,1f,1f, 0f,0.69f,0.31f,       0f,1f,

            };

            float[] bottom = {
                //Bottom Face
                -1f,-1f,1f, 0f,0.69f,0.31f,     0f,1f,
                -1f,-1f,-1f, 0f,0.69f,0.31f,    1f,1f,
                1f,-1f,1f, 0f,0.69f,0.31f,      0f,0f,

                1f,-1f,-1f, 0f,0.69f,0.31f,     1f,0f,
                -1f,-1f,-1f, 0f,0.69f,0.31f,    1f,1f,
                1f,-1f,1f, 0f,0.69f,0.31f,      0f,0f,

            };

            float[] front = {
                //Front Face
                -1f,1f,-1f, 0f,0.69f,0.31f,     1f,0f,
                -1f,-1f,-1f, 0f,-14f,0.75f,     1f,1f,
                1f,1f,-1f, 0f,0.69f,0.31f,      0f,0f,

                1f,-1f,-1f, 0f,-14f,0.75f,      0f,1f,
                -1f,-1f,-1f, 0f,-14f,0.75f,     1f,1f,
                1f,1f,-1f, 0f,0.69f,0.31f,      0f,0f,
            };

            float[] back = {
                //Back Face
                -1f,1f,1f, 0f,0.69f,0.31f,      0f,0f,
                -1f,-1f,1f, 0f,-14f,0.75f,      0f,1f,
                1f,1f,1f, 0f,0.69f,0.31f,       1f,0f,

                1f,-1f,1f, 0f,-14f,0.75f,       1f,1f,
                -1f,-1f,1f, 0f,-14f,0.75f,      0f,1f,
                1f,1f,1f, 0f,0.69f,0.31f,       1f,0f,
            };

            float[] left =
            {
                 //Left Face
                -1f,1f,1f, 0f,0.69f,0.31f,      1f,0f,
                -1f,-1f,1f, 0f,-14f,0.75f,      1f,1f,
                -1f,1f,-1f, 0f,0.69f,0.31f,     0f,0f,

                -1f,-1f,-1f, 0f,-14f,0.75f,     0f,1f,
                -1f,-1f,1f, 0f,-14f,0.75f,      1f,1f,
                -1f,1f,-1f, 0f,0.69f,0.31f,     0f,0f,
            };

            float[] right = {
                //Right Face
                1f,1f,1f, 0f,0.69f,0.31f,       0f,0f,
                1f,-1f,1f, 0f,-14f,0.75f,       0f,1f,
                1f,1f,-1f, 0f,0.69f,0.31f,      1f,0f,

                1f,-1f,-1f, 0f,-14f,0.75f,      1f,1f,
                1f,-1f,1f, 0f,-14f,0.75f,       0f,1f,
                1f,1f,-1f, 0f,0.69f,0.31f,      1f,0f,

            };
           
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

            hpID = GPU.GenerateBuffer(squarevertices);
            frontBufferID = GPU.GenerateBuffer(front);
            backBufferID = GPU.GenerateBuffer(back);
            topBufferID = GPU.GenerateBuffer(top);
            bottomBufferID = GPU.GenerateBuffer(bottom);
            leftBufferID = GPU.GenerateBuffer(left);
            rightBufferID = GPU.GenerateBuffer(right);
            groundBufferID = GPU.GenerateBuffer(groundVerts);

            backhealthbar = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(0.5f,0.1f, 1)), glm.translate(new mat4(1),new vec3(-0.5f,0.9f,0)) });
            healthbar = MathHelper.MultiplyMatrices(new List<mat4>() {
                glm.scale(new mat4(1), new vec3(0.48f, 0.1f, 1)), glm.translate(new mat4(1), new vec3(-0.5f, 0.9f, 0)) });
            shader2D.UseShader();
            mloc = Gl.glGetUniformLocation(shader2D.ID, "model");
            scalef = 1;

            f.updateLoadingScreen(30);

            scaleMat = glm.scale(new mat4(1), new vec3(100f, 100f, 100f));

            cam = new Camera();
            cam.Reset(0f, 1f, 50f, 0, 0, 0, 0, 0, 0);

            light = new Model3D();
            light.LoadFile(projectPath + "\\ModelFiles\\static\\Light Pole", "Light Pole.3ds", 3);
            light.transmatrix = glm.translate(new mat4(1), new vec3(0, 2f, 0));
            mat4 rot1 = glm.rotate(290 / 180 * 3.14f, new vec3(0, 0, 1));
            light.rotmatrix =  rot1 * glm.rotate(270f / 180.0f * 3.14f, new vec3(1, 0, 0));
            light.scalematrix = glm.scale(new mat4(1), new vec3(0.1f, 0.1f, 0.2f));

            car2 = new Model3D();
            car2.LoadFile(projectPath + "\\ModelFiles\\static\\car", "dpv.obj", 3);
            car2.scalematrix = glm.scale(new mat4(1), new vec3(0.007f, 0.007f, 0.007f));
            car2.rotmatrix = glm.rotate(90f / 180.0f * 3.14f,new vec3(0, 1, 0));
            car2.transmatrix = glm.translate(new mat4(1), new vec3(0, 0, 3));

            for (int i = -9; i < 10; i++)
            {
                building1 = new Model3D();
                building1.LoadFile(projectPath + "\\ModelFiles\\static\\building", "Building 02.obj", 3);
                building1.scalematrix = glm.scale(new mat4(1), new vec3(1f, 01f, 1f));
                building1.transmatrix = glm.translate(new mat4(1), new vec3(-70f, 1f, (float)i * 10));
                building1.rotmatrix = glm.rotate(90f / 180.0f * 3.14f, new vec3(0, 1, 0));

                Props.Add(building1);
                minMaxPos.Add(new Tuple<vec3, vec3>(building1.getMinPos(), building1.getMaxPos()));
            }



            for (int i = -9; i < 10; i++)
            {
                building1 = new Model3D();
                building1.LoadFile(projectPath + "\\ModelFiles\\static\\building", "Building 02.obj", 3);
                building1.scalematrix = glm.scale(new mat4(1), new vec3(1f, 01f, 1f));
                building1.transmatrix = glm.translate(new mat4(1), new vec3(-40f, 1f, ((float)i * 10) + 5));
                building1.rotmatrix = glm.rotate(90f / 180.0f * 3.14f, new vec3(0, 1, 0));

                Props.Add(building1);
                minMaxPos.Add(new Tuple<vec3, vec3>(building1.getMinPos(), building1.getMaxPos()));
            }

            f.updateLoadingScreen(40);

            for (int i=-9; i<10; i++)
            {
                building1 = new Model3D();
                building1.LoadFile(projectPath + "\\ModelFiles\\static\\building", "Building 02.obj", 3);
                building1.scalematrix = glm.scale(new mat4(1), new vec3(1f, 01f, 1f));
                building1.transmatrix = glm.translate(new mat4(1), new vec3(-15f, 1f, (float)i*10));
                building1.rotmatrix = glm.rotate(90f / 180.0f * 3.14f, new vec3(0, 1, 0));

                Props.Add(building1);
                minMaxPos.Add(new Tuple<vec3, vec3>(building1.getMinPos(), building1.getMaxPos()));
            }

            for (int i = -9; i < 10; i++)
            {
                building1 = new Model3D();
                building1.LoadFile(projectPath + "\\ModelFiles\\static\\building", "Building 02.obj", 3);
                building1.scalematrix = glm.scale(new mat4(1), new vec3(1f, 01f, 1f));
                building1.transmatrix = glm.translate(new mat4(1), new vec3(15f, 1f, (float)i * 10));
                building1.rotmatrix = glm.rotate(-90f / 180.0f * 3.14f, new vec3(0, 1, 0));

                Props.Add(building1);
                minMaxPos.Add(new Tuple<vec3, vec3>(building1.getMaxPos(), building1.getMinPos()));
            }

            f.updateLoadingScreen(50);

            for (int i = -9; i < 10; i++)
            {
                building1 = new Model3D();
                building1.LoadFile(projectPath + "\\ModelFiles\\static\\building", "Building 02.obj", 3);
                building1.scalematrix = glm.scale(new mat4(1), new vec3(1f, 01f, 1f));
                building1.transmatrix = glm.translate(new mat4(1), new vec3(70f, 1f, ((float)i * 10) + 5));
                building1.rotmatrix = glm.rotate(-90f / 180.0f * 3.14f, new vec3(0, 1, 0));

                Props.Add(building1);
                minMaxPos.Add(new Tuple<vec3, vec3>(building1.getMaxPos(), building1.getMinPos()));
            }

            for (int i = -9; i < 10; i++)
            {
                building1 = new Model3D();
                building1.LoadFile(projectPath + "\\ModelFiles\\static\\building", "Building 02.obj", 3);
                building1.scalematrix = glm.scale(new mat4(1), new vec3(1f, 01f, 1f));
                building1.transmatrix = glm.translate(new mat4(1), new vec3(40f, 1f, (float)i * 10));
                building1.rotmatrix = glm.rotate(-90f / 180.0f * 3.14f, new vec3(0, 1, 0));

                Props.Add(building1);
                minMaxPos.Add(new Tuple<vec3, vec3>(building1.getMaxPos(), building1.getMinPos()));
            }

            f.updateLoadingScreen(60);

            gun = new Model3D();
            gun.LoadFile(projectPath + "\\ModelFiles\\static\\DAE Railgun", "Railgun_Prototype-COLLADA_3.dae", 3);
            gun.scalematrix = glm.scale(new mat4(1), new vec3(1f, 1f, 1f));
            gun.transmatrix = glm.translate(new mat4(1), new vec3(0.01f, -3f, 3f));
            gun.rotmatrix = initalRot;

            gun2 = new Model3D();
            gun2.LoadFile(projectPath + "\\ModelFiles\\static\\SMG_Upload", "M33.obj", 3);
            gun2.scalematrix = glm.scale(new mat4(1), new vec3(0.01f, 0.01f, 0.01f));
            gun2.transmatrix = glm.translate(new mat4(1), new vec3(0.01f, -3f, 3f));
            gun2.rotmatrix = initialRot2;

            f.updateLoadingScreen(70);

            zombie = new md2LOL((projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2"));
            zombie.StartAnimation(animType_LOL.STAND);
            zombie.rotationMatrix = glm.rotate(180f / 180f * 3.14f, new vec3(0, 0, 1)) * glm.rotate((float)270f / 180.0f * 3.14f, new vec3(1, 0, 0)) ;
            zombie.TranslationMatrix = glm.translate(new mat4(1), new vec3(1f, 0.2f, 5f));
            zombie.scaleMatrix = glm.scale(new mat4(1), new vec3(0.02f, 0.02f, 0.02f));

            zombie2 = new md2LOL((projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2"));
            zombie2.StartAnimation(animType_LOL.STAND);
            zombie2.rotationMatrix = glm.rotate((float)270f / 180.0f * 3.14f, new vec3(1, 0, 0));
            zombie2.TranslationMatrix = glm.translate(new mat4(1), new vec3(-1f, 0.2f, 5f));
            zombie2.scaleMatrix = glm.scale(new mat4(1), new vec3(0.02f, 0.02f, 0.02f));

            zombie3 = new md2LOL((projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2"));
            zombie3.StartAnimation(animType_LOL.STAND);
            zombie3.rotationMatrix = glm.rotate((float)270f / 180.0f * 3.14f, new vec3(1, 0, 0));
            zombie3.TranslationMatrix = glm.translate(new mat4(1), new vec3(4f, 0.2f, 5f));
            zombie3.scaleMatrix = glm.scale(new mat4(1), new vec3(0.02f, 0.02f, 0.02f));

            zombie4 = new md2LOL((projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2"));
            zombie4.StartAnimation(animType_LOL.STAND);
            zombie4.rotationMatrix = glm.rotate((float)270f / 180.0f * 3.14f, new vec3(1, 0, 0));
            zombie4.TranslationMatrix = glm.translate(new mat4(1), new vec3(-4f, 0.2f, 5f));
            zombie4.scaleMatrix = glm.scale(new mat4(1), new vec3(0.02f, 0.02f, 0.02f));

            f.updateLoadingScreen(80);

            enemies = new List<Enemy>();

            for (int i=0; i < 0; i++)
            {
                Random rnd = new Random();
                float x = rnd.Next(-90, 90);
                float z = rnd.Next(-90, 0);

                md2LOL zombie = new md2LOL((projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2"));
                zombie.StartAnimation(animType_LOL.STAND);
                zombie.rotationMatrix = glm.rotate((float)270f / 180.0f * 3.14f, new vec3(1, 0, 0));
                zombie.TranslationMatrix = glm.translate(new mat4(1), new vec3(x, 0.2f, z));
                zombie.scaleMatrix = glm.scale(new mat4(1), new vec3(0.02f, 0.02f, 0.02f));

                enemies.Add(new Enemy(zombie));
            }

            
            enemies.Add(new Enemy(zombie));
            enemies.Add(new Enemy(zombie2));
            enemies.Add(new Enemy(zombie3));
            enemies.Add(new Enemy(zombie4));           

            for (int i = 0; i < enemies.Count; i++)
            {
                minPosEnemies.Add(enemies[i].getMod().getMinPos());
                maxPosEnemies.Add(enemies[i].getMod().getMaxPos());
            }

            carIndex = minMaxPos.Count;
            minMaxPos.Add(new Tuple<vec3, vec3>(car2.getMinPos(), car2.getMaxPos()));
            minMaxPos.Add(new Tuple<vec3, vec3>(light.getMaxPos(), light.getMinPos()));

            f.updateLoadingScreen(90);


            Gl.glClearColor(0, 0, 0.4f, 1);

            sh.UseShader();
            
            modelID = Gl.glGetUniformLocation(sh.ID, "model");

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            transID = Gl.glGetUniformLocation(sh.ID, "model");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            DataID = Gl.glGetUniformLocation(sh.ID, "data");
            vec2 data = new vec2(100, 50);
            Gl.glUniform2fv(DataID, 1, data.to_array());

            //ambient light
            AmbientLightID = Gl.glGetUniformLocation(sh.ID, "ambientLight");
            vec3 ambientLight = new vec3(0.2f, 0.2f, 0.2f);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
            //SendLightData(0.2f, 0.2f, 0.2f, 1f, 0);

            //diffuse light
            int LightPositionID = Gl.glGetUniformLocation(sh.ID, "light_position");
            vec3 light_pos = new vec3(0, 1.9f, 0);
            Gl.glUniform3fv(LightPositionID, 1, light_pos.to_array());

            //eye position
            EyePositionID = Gl.glGetUniformLocation(sh.ID, "EyePosition_worldspace");

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);

            f.initialized = true;
            f.updateLoadingScreen(100);
        }

        public override void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            sh.UseShader(); 

            Gl.glEnableVertexAttribArray(0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glEnableVertexAttribArray(2);
            Gl.glEnableVertexAttribArray(3);


            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, scaleMat.to_array());
            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniform3fv(EyePositionID, 1, cam.GetCameraPosition().to_array());

            //------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, groundBufferID);
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, scaleMat.to_array());
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(6 * sizeof(float)));
            Gl.glVertexAttribPointer(3, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(8 * sizeof(float)));
            ground.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            Gl.glDisableVertexAttribArray(3);

            //------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, topBufferID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            uptex.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //----------------------------------------------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, bottomBufferID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            downtex.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //----------------------------------------------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, frontBufferID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            fronttex.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //----------------------------------------------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, backBufferID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            backtex.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //----------------------------------------------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, leftBufferID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            lefttex.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //----------------------------------------------------------------------------------------------------------------

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, rightBufferID);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), IntPtr.Zero);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            righttex.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            //----------------------------------------------------------------------------------------------------------------

            foreach (Bullet b in bullets)
            {
                b.Draw(modelID);
            }

            car2.Draw(modelID);
            light.Draw(modelID);
            if(!currentGun)
            {
                gun.Draw(modelID);
            }
            else
            {
                gun2.Draw(modelID);
            }


            //building1.Draw(modelID);
            //building2.Draw(modelID);
            //building3.Draw(modelID);
            //building4.Draw(modelID);
            //building5.Draw(modelID);
            //building6.Draw(modelID);
            //building7.Draw(modelID);
            //building8.Draw(modelID);
            //building9.Draw(modelID);
            //building10.Draw(modelID);

            foreach(Model3D m in Props)
            {
                m.Draw(modelID);
            }

            foreach (Enemy e in enemies)
            {
                e.Draw(modelID);
                e.DrawHP(cam);
            }
            foreach (Enemy e in deadEnemies)
            {
                e.Draw(modelID);
            }

            
            /********************************************************************************************/

            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
            Gl.glDisableVertexAttribArray(3);

            //############################################### ----- 2D ----- ###########################################
            
            scalef = (playerHealth) * 0.2f;
            HealthBar playerBar = new HealthBar(shader2D);
            playerBar.scaleMatFront = glm.scale(new mat4(1), new vec3(0.48f * scalef, 0.1f, 1));
            playerBar.transMatFront = glm.translate(new mat4(1), new vec3(-0.5f - ((1 - scalef) * 0.48f), 0.9f, 0));
            
            playerBar.Draw2D();

        }
        public override void update(float deltaTime)
        {

            if(playerHealth <= 0)
            {
                dead = true;
            }

            for(int i=0; i<enemies.Count; i++)
            {
                minPosEnemies[i] = enemies[i].getMod().getMinPos();
                maxPosEnemies[i] = enemies[i].getMod().getMaxPos();

            }

            playerHasMoved = false;

            collided.Clear();
            vec3 lastPos = cam.getLastPos();
            vec3 currPos = cam.GetCameraPosition();
            List<int> removeBullets = new List<int>();
            List<int> removeEnemies = new List<int>();

            //Handle collision between player and static objects
            foreach(Tuple<vec3, vec3> minMax in minMaxPos)
            {
                vec3 Max = minMax.Item2;
                vec3 Min = minMax.Item1;

                bool clear = false;
                if (cam.GetCameraPosition().x > Max.x || cam.GetCameraPosition().x < Min.x ||
                    cam.GetCameraPosition().z < Max.z || cam.GetCameraPosition().z > Min.z)
                {
                    clear = true;                    
                    //cam.lastPosBeforeCamCollision = cam.GetCameraPosition();
                    
                    cam.collision = false;
                }
                if (!clear)
                {
                    cam.collision = true;
                    cam.undoMove();
                    
                    if(minMaxPos.IndexOf(minMax) == carIndex && enemies.Count == 0)
                    {
                        win = true;
                    }
                    
                    break;
                }
            }


            cam.UpdateViewMatrix();
            gun.transmatrix = glm.translate(new mat4(1), new vec3(cam.GetCameraPosition().x, cam.GetCameraPosition().y - 0.4f, cam.GetCameraPosition().z));
            gun.rotmatrix = initalRot * glm.rotate(new mat4(1), cam.mAngleX, new vec3(0, 1, 0));

            gun2.transmatrix = glm.translate(new mat4(1), new vec3(cam.GetCameraPosition().x, cam.GetCameraPosition().y - 0.4f, cam.GetCameraPosition().z));
            gun2.rotmatrix = initialRot2 * glm.rotate(new mat4(1), cam.mAngleX, new vec3(0, 1, 0));

            //Stop bullets on collision with static objects
            for (int i=0; i<bullets.Count; i++)
            {
                for(int j=0; j<minMaxPos.Count; j++)
                {
                    if (staticBulletHit(minMaxPos[j], bullets[i]))
                    {
                        removeBullets.Add(i);
                        //bullets.RemoveAt(i);
                    }
                    else
                    {
                        bullets[i].update();
                    }
                }
            }

            //Handle Enemy-bullet collision
            for(int i=0; i<enemies.Count;i++)
            {
                for(int j=0; j<bullets.Count; j++)
                {
                    if(enemyBulletHit(i, bullets[j]) <= 0.7f)
                    {
                        if (currentGun)
                        {
                            enemies[i].health-=0.3f;
                        }else if (!currentGun)
                        {
                            enemies[i].health --;
                        }
                        
                        if(enemies[i].health <= 0)
                        {
                            removeEnemies.Add(i);
                        }
                        bullets.RemoveAt(j);
                        j--;
                        
                    }
                }
            }
            //Remove bullets out of the skybox
            for(int i=0; i<bullets.Count; i++)
            {
                if (bullets[i].position.x >= 99 || bullets[i].position.x <= -99 || bullets[i].position.z >= 99 || bullets[i].position.z <= -99)
                {
                    bullets.RemoveAt(i);
                }
            }
            //Remove dead enemies from enemies list and add them to another list for dead enemies
            foreach (int i in removeEnemies)
            {
                try
                {
                    enemies[i].getMod().StartAnimation(animType_LOL.DEATH);
                    enemies[i].getMod().Loop = false;
                    deadEnemies.Add(enemies[i]);
                    enemies.RemoveAt(i);
                    gf.updateScore();
                }
                catch (Exception e)
                {

                }
            }

            //handle collision between enemies, eachother and static objects

            if (lastPos.x != currPos.x || lastPos.y != currPos.y || lastPos.z != currPos.z)
            {
                playerHasMoved = true;
                
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                bool clear = true;
                enemies[i].clear = true;
                //Enemy X Enemy
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (j != i)
                    {
                        if (enemyEnemyCollision(i, j))
                        {
                            if (!collided.Contains(enemies[j])){
                                collided.Add(enemies[i]);
                                if (enemies[i].saveBeforeCollision)
                                {
                                    //enemies[i].undoMove(cam);
                                    enemies[i].beforeCollision = enemies[i].getMod().TranslationMatrix;
                                    //enemies[i].saveBeforeCollision = false;
                                    //enemies[i].Move = 0;
                                    enemies[i].clear = false;
                                }
                                clear = false;
                                
                            }
                            cam.LastPos = cam.GetCameraPosition();
                        }
                        else
                        {
                            enemies[i].clear = true;
                            enemies[j].clear = true;
                        }
                    }
                }
                //--------------------------------------------------
                //Enemy X Static
                foreach (Tuple<vec3, vec3> staticPos in minMaxPos)
                {
                    if (staticEnemyCollision(staticPos, i))
                    {
                        if (enemies[i].saveBeforeCollision)
                        {
                            enemies[i].undoMove(cam);
                            enemies[i].beforeCollision = enemies[i].getMod().TranslationMatrix;
                            enemies[i].saveBeforeCollision = false;
                            //enemies[i].Move = 0;
                        }

                        clear = false;
                        cam.LastPos = cam.GetCameraPosition();
                        //break;
                    }
                }
                if (clear || playerHasMoved)
                {
                    enemies[i].Move = 1;
                    enemies[i].saveBeforeCollision = true;
                    enemies[i].update(cam, ref playerHealth);
                    continue;
                }
                //---------------------------------------------------
                if (!clear)
                {
                    //enemies[i].update(cam, ref playerHealth);
                    enemies[i].getMod().TranslationMatrix = enemies[i].beforeCollision;

                }
                else
                {
                    int startFrame = enemies[i].getMod().getAnim()[(int)animType_LOL.STAND].first_frame;
                    int lastFrame = enemies[i].getMod().getAnim()[(int)animType_LOL.STAND].last_frame;
                    if (enemies[i].getMod().getCurrentFrame() < startFrame || enemies[i].getMod().getCurrentFrame() > lastFrame)
                        enemies[i].getMod().StartAnimation(animType_LOL.STAND);
                }
            }


            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            foreach(int i in removeBullets)
            {
                try
                {
                    bullets.RemoveAt(i);
                }catch(Exception e)
                {

                }
                
            }

            foreach(Enemy e in enemies)
            {
                e.getMod().UpdateAnimation();
            }
            foreach (Enemy e in deadEnemies)
            {
                e.getMod().UpdateAnimation();
                
            }

        }
        public override void cleanup()
        {
            sh.DestroyShader();
            shader2D.DestroyShader();
        }
        //public float calcDistance(int i)
        //{
        //    vec3 emax = maxPosEnemies[i];
        //    vec3 emin = minPosEnemies[i];

        //    return (float)(Math.Sqrt(Math.Pow(cam.GetCameraPosition().x - ((emax.x + emin.x) / 2), 2) +
        //                             Math.Pow(cam.GetCameraPosition().z - ((emax.z + emin.z) / 2), 2)));
        //}
        public float enemyBulletHit(int i, Bullet b)
        {
            vec3 emax = maxPosEnemies[i];
            vec3 emin = minPosEnemies[i];

            float firstE = (emax.x + emin.x) / 2;
            float secondE = (emax.z + emin.z) / 2;

            float firstB = (b.maxPos().x + b.minPos().x) / 2;
            float secondB = (b.maxPos().z + b.minPos().z) / 2;

            return (float)(Math.Sqrt( (firstE - firstB) * (firstE - firstB) + (secondE - secondB) * (secondE - secondB) ));
           
        }

        public bool staticBulletHit(Tuple<vec3, vec3> minMaxPos, Bullet b)
        {


            float bulletX = (b.maxPos().x + b.minPos().x) / 2;
            float bulletZ = (b.maxPos().z + b.minPos().z) / 2;

            if (minMaxPos.Item2.x < 0 && minMaxPos.Item1.x < 0)
                return (bulletX <= minMaxPos.Item2.x && bulletX >= minMaxPos.Item1.x && bulletZ >= minMaxPos.Item2.z && bulletZ <= minMaxPos.Item1.z);
            else
                return (bulletX <= minMaxPos.Item2.x && bulletX >= minMaxPos.Item1.x && bulletZ >= minMaxPos.Item2.z && bulletZ <= minMaxPos.Item1.z);

        }

        public bool staticEnemyCollision(Tuple<vec3, vec3> minMaxPos, int i)
        {
            vec3 maxPos = maxPosEnemies[i];
            vec3 minPos = minPosEnemies[i];

            float enemyX = (maxPos.x + minPos.x) / 2;
            float enemyZ = (maxPos.z + minPos.z) / 2;

            return (enemyX <= minMaxPos.Item2.x && enemyX >= minMaxPos.Item1.x && enemyZ >= minMaxPos.Item2.z && enemyZ <= minMaxPos.Item1.z);

        }

        public bool enemyEnemyCollision(int e1, int e2)
        {
            vec3 maxPos1 = maxPosEnemies[e1];
            vec3 minPos1 = minPosEnemies[e1];

            vec3 maxPos2 = maxPosEnemies[e2];
            vec3 minPos2 = minPosEnemies[e2];

            float enemy1X = (maxPos1.x + minPos1.x) / 2;
            float enemy1Z = (maxPos1.z + minPos1.z) / 2;

            float enemy2X = (maxPos2.x + minPos2.x) / 2;
            float enemy2Z = (maxPos2.z + minPos2.z) / 2;

            //return (maxPos1.x <= maxPos2.x && enemy1X >= minPos2.x && enemy1Z >= maxPos2.z && enemy1Z <= minPos2.z);
            return (maxPos1.x <= maxPos2.x+0.5f && maxPos1.z >= maxPos2.z-0.5f );

        }

    }
}
