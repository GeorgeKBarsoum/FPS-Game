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

    class Bullet
    {
        string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        public vec3 position;
        vec3 direction;
        Model3D m;
        float speed = 0.01f;
        float scalefactor = 0.05f;

        public vec3 boundingBox;

        public Bullet(vec3 camPos, vec3 camDir)
        {
            position = camPos;
            direction = camDir;
            m = new Model3D();
            m.LoadFile(projectPath + "\\ModelFiles\\static", "cube.3ds", 3);
            m.transmatrix = glm.translate(new mat4(1), new vec3(position.x, 0.5f, position.z));
            m.scalematrix = glm.scale(new mat4(1), new vec3(scalefactor, scalefactor, scalefactor));
            vec3 MaxPos = m.getMaxPos();
            vec3 MinPos = m.getMinPos();
            boundingBox.x = MaxPos.x - MinPos.x;
            boundingBox.y = MaxPos.y - MinPos.y;
            boundingBox.z = MaxPos.z - MinPos.z;
            
        }

        public void Draw(int id)
        {
            m.Draw(id);
        }
        public void update()
        {
            if(position.x < 99 && position.x > -99 && position.z < 99 && position.z > -99)
            {
                position += direction*speed;
                m.transmatrix = glm.translate(new mat4(1), new vec3(position.x, 0.5f, position.z));
            }
            
        }
        public vec3 maxPos()
        {
            return m.getMaxPos();
        }
        public vec3 minPos()
        {
            return m.getMinPos();
        }
        public vec3 bb()
        {
            return boundingBox;

        }

    }
}
