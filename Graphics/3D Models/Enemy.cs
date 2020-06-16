using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;
using Graphics._3D_Models;


namespace Graphics
{
    class Enemy
    {

        public float health;
        float detectionRange;
        float attackRange;
        md2LOL model;
        bool hit;
        public int Move;
        float movSpeed;
        vec3 maxPos;
        vec3 minPos;
        vec3 dir;
        vec3 walk;
        mat4 initialrot;
        HealthBar hp;
        public mat4 beforeCollision;
        public bool saveBeforeCollision;
        public bool updatelastTranslationMatrix;
        public float currentAngle;
        public float lastAngleBeforeCollision;
        public bool clear = true;

        public Enemy(md2LOL mod)
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            Shader sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            hp = new HealthBar(sh);
            Move = 1;
            saveBeforeCollision = true;
            movSpeed = 0.01f;
            hit = false;
            health = 5;
            detectionRange = 20;
            attackRange = 1;
            model = mod;
            beforeCollision = model.TranslationMatrix;
            initialrot = mod.rotationMatrix;
            updatelastTranslationMatrix = true;

            maxPos = getMod().getMaxPos();
            minPos = getMod().getMinPos();

            walk = (maxPos + minPos) / 2;

        }

        public void update(Camera cam, ref int playerHealth)
        {
            if (!clear)
            {
                
                return;
            }
            float dis = calcDistance(this, cam);
            if (dis <= getDet())
            {
                //mat4 rot1 = glm.rotate(calcAngle(this, cam) / 180f * 3.14f, new vec3(0, 1, 0));
                //mat4 res1 = rot1 * glm.rotate(270f / 180f * 3.14f, new vec3(1, 0, 0));
                //getMod().rotationMatrix = res1;
                
                if (dis <= getRan())
                {
                    attack(cam, ref playerHealth);
                }
                else
                {
                    move(cam);
                } 

            }

        }

        public void Draw(int id)
        {       
            model.Draw(id);
        }
        public void DrawHP(Camera cam)
        {
            maxPos = getMod().getMaxPos();
            minPos = getMod().getMinPos();

            hp.scaleMatFront = glm.scale(new mat4(1), new vec3(0.48f * (0.2f*health), 0.1f, 1));
            
            hp.transMatFront = glm.translate(new mat4(1), new vec3(maxPos.x, 1.5f, maxPos.z));
            

            float x = (maxPos.x + minPos.x) / 2 - cam.GetCameraPosition().x;
            float z = (maxPos.z + minPos.z) / 2 - cam.GetCameraPosition().z;
            float hyp = (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));
            float angle = hyp == 0 ? 0 : (float)Math.Asin(x / hyp);

            if (cam.GetCameraPosition().z < maxPos.z)
            {
                hp.rotMat = glm.rotate((angle) + 3.14f , new vec3(0, 1, 0));
                hp.transMatBack = glm.translate(new mat4(1), new vec3(maxPos.x, 1.5f, maxPos.z + 0.01f));
            }
            else
            {
                hp.rotMat = glm.rotate(-angle, new vec3(0, 1, 0));
                hp.transMatBack = glm.translate(new mat4(1), new vec3(maxPos.x, 1.5f, maxPos.z - 0.01f));
            }
            hp.Draw3D();
        }

        public void attack(Camera cam, ref int playerHealth)
        {
            animState_t res2;
            anim_t animation = getMod().getAnim()[(int)animType_LOL.ATTACK1];
            res2.startframe = getMod().getAnim()[(int)animType_LOL.ATTACK1].first_frame;
            if (getMod().getCurrentFrame() == (animation.first_frame + animation.last_frame) /2 && !hit)
            {
                playerHealth--;
                hit = true;
            }
            if (getMod().getCurrentFrame() == ((animation.first_frame + animation.last_frame) / 2) + 1)
            {
                hit = false;
            }
            if (playerHealth <= 0)
            {
                cam.Reset(0, 90, 0, 0, 0, 0, 0, 0, 0);
            }
            if (getMod().getCurrentFrame() < res2.startframe || getMod().getCurrentFrame() > getMod().getAnim()[(int)animType_LOL.ATTACK1].last_frame)
            {
                getMod().StartAnimation(animType_LOL.ATTACK1);
            }
        }

        public void move(Camera cam)
        {
            animState_t res;
            res.startframe = getMod().getAnim()[(int)animType_LOL.RUN].first_frame;
            if (getMod().getCurrentFrame() < res.startframe || getMod().getCurrentFrame() > getMod().getAnim()[(int)animType_LOL.RUN].last_frame)
            {
                getMod().StartAnimation(animType_LOL.RUN);
            }

            vec3 maxPos = getMod().getMaxPos();
            vec3 minPos = getMod().getMinPos();

            dir = cam.GetCameraPosition() - (maxPos + minPos) / 2;
            walk += dir * (Move * movSpeed);
            walk.y = 0;

            getMod().TranslationMatrix = glm.translate(new mat4(1), walk);

            maxPos = getMod().getMaxPos();
            minPos = getMod().getMinPos();

            float x =   (maxPos.x + minPos.x) / 2 - cam.GetCameraPosition().x;
            float z =   (maxPos.z + minPos.z) / 2 - cam.GetCameraPosition().z;
            float hyp = (float) Math.Sqrt(Math.Pow(x,2) + Math.Pow(z,2));
            float angle = hyp == 0 ? 0 : (float) Math.Asin(x / hyp);

            if(cam.GetCameraPosition().z < maxPos.z)
            {
                mat4 rot1 = glm.rotate((-angle) + 3.14f, new vec3(0, 0, 1));
                mat4 res1 = rot1 * glm.rotate((float)270f / 180.0f * 3.14f, new vec3(1, 0, 0));
                getMod().rotationMatrix = res1;
            }
            else
            {
                mat4 rot1 = glm.rotate(angle, new vec3(0, 0, 1));
                mat4 res1 = rot1 * glm.rotate((float)270f / 180.0f * 3.14f, new vec3(1, 0, 0));
                getMod().rotationMatrix = res1;
            }

        }

        public void undoMove(Camera cam)
        {
            vec3 maxPos = getMod().getMaxPos();
            vec3 minPos = getMod().getMinPos();

            dir = cam.GetCameraPosition() - (maxPos + minPos) / 2;
            walk -= dir * (Move * movSpeed);
            walk.y = 0;

            getMod().TranslationMatrix = glm.translate(new mat4(1), walk);
            //getMod().TranslationMatrix = beforeCollision;
        }

        public float getHealth()
        {
            return health;
        }
        public float getDet()
        {
            return detectionRange;
        }
        public float getRan()
        {
            return attackRange;
        }
        public md2LOL getMod()
        {
            return model;
        }

        public float calcDistance(Enemy e, Camera cam)
        {
            vec3 maxPos = e.getMod().getMaxPos();
            vec3 minPos = e.getMod().getMinPos();

            return (float)(Math.Sqrt(Math.Pow(cam.GetCameraPosition().x - ((maxPos.x + minPos.x) / 2), 2) +
                                     Math.Pow(cam.GetCameraPosition().z - ((maxPos.z + minPos.z) / 2), 2)));
        }

        public float calcAngle(Enemy e, Camera cam)
        {
            //vec3 maxPos = e.getMod().getMaxPos();
            //vec3 minPos = e.getMod().getMinPos();

            //vec3 dirEnemy = cam.GetCameraPosition() - (maxPos + minPos) / 2;

            //double numerator = (maxPos.x * cam.GetCameraPosition().x) + (maxPos.z * cam.GetCameraPosition().z+0.5f);
            //double denomenator1 = (float)Math.Sqrt(Math.Pow(maxPos.x, 2) + Math.Pow(maxPos.z, 2));
            //double denomenator2 = (float)Math.Sqrt(Math.Pow(cam.GetCameraPosition().x, 2) + Math.Pow(cam.GetCameraPosition().z+0.5f, 2));

            //double res = numerator / (denomenator1 * denomenator2);
            
            return 0;

        }

    }
}
