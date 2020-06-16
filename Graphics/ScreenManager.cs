using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class ScreenManager
    {
        public List<Screen> screens;
        public ScreenManager()
        { 
            screens = new List<Screen>();
        }
        public void Draw()
        {
            try
            {
                screens[screens.Count - 1].Draw();
            }
            catch
            {

            }
            
        }
        public void update(float deltaTime)
        {
            try
            {
                screens[screens.Count - 1].update(deltaTime);
            }catch
            {

            }            
        }
        public void addScreen(Screen s)
        {
            try
            {
                screens.Add(s);
            }
            catch
            {

            }
            
        }
        public void removeScreen()
        {
            try
            {
                screens.RemoveAt(screens.Count - 1);
            }
            catch
            {

            }
            
        }
        public void cleanup()
        {
            try
            {
                screens[screens.Count - 1].cleanup();
            }
            catch
            {

            }
            
        }
        public void clear()
        {
            screens.Clear();
        }
    }
}
