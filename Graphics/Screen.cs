using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    abstract class Screen
    {
        public abstract void Draw();
        public abstract void update(float deltaTime);
        public abstract void cleanup();
    }
}
