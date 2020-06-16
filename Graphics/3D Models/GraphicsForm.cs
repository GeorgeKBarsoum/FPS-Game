using System.Windows.Forms;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Timers;

namespace Graphics
{
    public partial class GraphicsForm : Form
    {
 
        List<Bullet> bullets;
        Renderer renderer = new Renderer();
        StartScreen s;
        LoadingScreen l;
        WinScreen ws;
        LoseScreen ls;
        ScreenManager sc;

        string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

        System.Media.SoundPlayer Walkplayer;

        System.Media.SoundPlayer WeakShot;
        System.Media.SoundPlayer StrongShot;
        System.Media.SoundPlayer Reload1;
        System.Media.SoundPlayer Reload2;


        FileStream stream;
        BinaryFormatter formatter;

        public static Dictionary<int, char> controls;

        public bool initialized = false;

        public int progress = 0;

        Screen currentScreen;

        Stopwatch sw = new Stopwatch();
        Stopwatch soundsw = new Stopwatch();

        float deltaTime;
        public GraphicsForm()
        {
            InitializeComponent();
            bullets = new List<Bullet>();
            simpleOpenGlControl1.InitializeContexts();
            //Cursor.Hide();
            MoveCursor();

            Walkplayer = new System.Media.SoundPlayer();
            Walkplayer.SoundLocation = projectPath + "\\Sounds\\Footsteps.wav";
            WeakShot = new System.Media.SoundPlayer();
            WeakShot.SoundLocation = projectPath + "\\Sounds\\Weak Shot.wav";
            StrongShot = new System.Media.SoundPlayer();
            StrongShot.SoundLocation = projectPath + "\\Sounds\\Strong Shot.wav";
            Reload1 = new System.Media.SoundPlayer();
            Reload1.SoundLocation = projectPath + "\\Sounds\\Reload 1.wav";
            Reload2 = new System.Media.SoundPlayer();
            Reload2.SoundLocation = projectPath + "\\Sounds\\Reload 2.wav";


            deltaTime = 0.005f;   
            simpleOpenGlControl1.Focus();
            
            sc = new ScreenManager();
            s = new StartScreen();
            l = new LoadingScreen();
            ws = new WinScreen();
            ls = new LoseScreen();

            loadControls();
            

            sc.addScreen(s);
            currentScreen = s;
            //Parallel.Invoke(() => initialize(), () => MainLoop());
            //Task task1 = Task.Factory.StartNew(() => initialize());
            Task task2 = Task.Factory.StartNew(() => MainLoop());
            //Task.WaitAll(task1, task2);

        }

        

        public void updateLoadingScreen(int progress)
        {
            l.progress = progress;
            l.Draw();
            l.update(deltaTime);
            simpleOpenGlControl1.Refresh();
        }
        void initialize()
        {
            renderer.Initialize(this);
        }
        void MainLoop()
        {   
            
            while (true)
            {
                
                if(currentScreen == renderer)
                {
                    if (renderer.dead)
                    {
                        sc.removeScreen();
                        sc.removeScreen();
                        sc.addScreen(ls);
                        currentScreen = ls;
                    }else if (renderer.win)
                    {
                        sc.removeScreen();
                        sc.removeScreen();
                        sc.addScreen(ws);
                        currentScreen = ws;
                    }
                }

                sc.Draw();          
                sc.update(deltaTime);
                simpleOpenGlControl1.Refresh();
             
                if (currentScreen == l && l.progress == 100)
                {                    
                    sc.addScreen(renderer);
                    currentScreen = renderer;
                    Cursor.Hide();
                }             
       
            }
           
        }

        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            simpleOpenGlControl1.Dispose(); 
            //renderer.cleanup();
            sc.cleanup();
            //MainLoopThread.Abort();
            //task2.Dispose();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            //renderer.Draw();
            //renderer.update(deltaTime);
            sc.Draw();
            sc.update(deltaTime);
        }



        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(currentScreen == renderer)
            {
                float speed = 0.3f;
                if (e.KeyChar == controls[1])
                {   
                    if(soundsw.ElapsedMilliseconds >= 800)
                    {
                        Walkplayer.Play();
                        soundsw.Restart();
                    }
                    renderer.cam.Strafe(-speed);
                }
                if (e.KeyChar == controls[3])
                {
                    if (soundsw.ElapsedMilliseconds >= 800)
                    {
                        Walkplayer.Play();
                        soundsw.Restart();
                    }
                    renderer.cam.Strafe(speed);
                }
                if (e.KeyChar == controls[2])
                {
                    if (soundsw.ElapsedMilliseconds >= 800)
                    {
                        Walkplayer.Play();
                        soundsw.Restart();
                    }
                    renderer.cam.Walk(-speed);
                }
                if (e.KeyChar == controls[0])
                {
                    if (soundsw.ElapsedMilliseconds >= 800)
                    {
                        Walkplayer.Play();
                        soundsw.Restart();
                    }
                    renderer.cam.Walk(speed);
                }if (e.KeyChar == controls[4])
                {
                    if (!renderer.currentGun)
                    {     
                        Reload1.Play();
                    }else if (renderer.currentGun)
                    {
                        Reload2.Play();
                    }
                    renderer.currentGun = !renderer.currentGun;
                }
                if (e.KeyChar == 'z')
                {
                    renderer.cam.Fly(-speed);
                }
                if (e.KeyChar == 'c')
                {
                    renderer.cam.Fly(speed);
                }
            }

            if (e.KeyChar == (char)Keys.Escape)
            {
                if(currentScreen == s)
                {
                    this.Close();
                    return;
                }else if(currentScreen == l)
                {
                    return;
                }
                try
                {
                    renderer.dead = false;
                    renderer.win = false;
                }catch{ }
                sc.clear();
                sc.addScreen(s);
                currentScreen = s;
                sw.Reset();
                soundsw.Reset();
                Cursor.Show();
                //this.Close();
            }
        }

        float prevX, prevY;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            float speed = 0.03f;
            float delta = e.X - prevX;
            if(currentScreen == renderer)
            {
                if (delta > 2)
                    renderer.cam.Yaw(-speed);
                else if (delta < -2)
                    renderer.cam.Yaw(speed);

                MoveCursor();
            }



            //delta = e.Y - prevY;
            /* if (delta > 2)
                 renderer.cam.Pitch(-speed);
             else if (delta < -2)
                 renderer.cam.Pitch(speed);*/

            
        }

        private void simpleOpenGlControl1_Load(object sender, EventArgs e)
        {
           // this.simpleOpenGlControl1.Size = new System.Drawing.Size(1920, 1080);
           
        }

        private void simpleOpenGlControl1_Click(object sender, EventArgs e)
        {

            if (currentScreen == s)
            {
                double maxWidth = this.Bounds.Width;
                double maxHeight = this.Bounds.Height;
                Point coordinates = Cursor.Position;

                double glX = (coordinates.X - maxWidth / 2) / (maxWidth / 2);
                double glY = -(coordinates.Y - maxHeight / 2) / (maxHeight / 2);

                if (glX < -0.4f && glX > -0.7f && glY > 0.5f && glY < 0.7f)
                {
                    l.progress = 0;
                    sc.addScreen(l);
                    currentScreen = l;
                    Cursor.Hide();
                    //Task task1 = Task.Factory.StartNew(() => initialize());
                    initialize();
                    sw.Restart();
                    soundsw.Restart();
                    
                }
                else if(glX < -0.3f && glX > -0.7f && glY > 0.1f && glY < 0.3f)
                {
                    Controls c = new Controls(this);
                    c.Show();
                    //loadControls();
                }


            }
            

        }

        private void loadControls()
        {
            try
            {
                stream = new FileStream("controls.txt", FileMode.Open);
                formatter = new BinaryFormatter();
                controls = (Dictionary<int, char>)formatter.Deserialize(stream);
                stream.Close();
            }
            catch
            {
                controls = new Dictionary<int, char>();
                controls[0] = 'w';
                controls[1] = 'a';
                controls[2] = 's';
                controls[3] = 'd';
                controls[4] = 'r';
            }
        }

        private void GraphicsForm_Load(object sender, EventArgs e)
        {
            //this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //simpleOpenGlControl1.PerformAutoScale();
            //simpleOpenGlControl1.AutoScaleDimensions = simpleOpenGlControl1.AutoScaleDimensions;
        }

        private void GraphicsForm_MouseDown(object sender, MouseEventArgs e)
        {

            

        }

        private void simpleOpenGlControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if (!renderer.currentGun)
                {
                    fire1();
                }
                else if (renderer.currentGun)
                {
                    fire2();
                }
            }
            
            
        }

        public void updateScore()
        {
            //score.Visible = true;
            score.Text = "Score : " + (renderer.deadEnemies.Count * 10).ToString();
        }

        public void fire1()
        {
            
            if (currentScreen == renderer)
            {
                
                if(sw.ElapsedMilliseconds >= 700)
                {
                    StrongShot.Play();
                    Bullet b = new Bullet(renderer.cam.GetCameraPosition(), renderer.cam.GetLookDirection());
                    renderer.bullets.Add(b);
                    sw.Restart();
                }
            }
        }
        public void fire2()
        {

            if (currentScreen == renderer)
            {

                if (sw.ElapsedMilliseconds >= 100)
                {              
                    WeakShot.Play();
                    Bullet b = new Bullet(renderer.cam.GetCameraPosition(), renderer.cam.GetLookDirection());
                    renderer.bullets.Add(b);
                    sw.Restart();
                }
            }
        }

        private void simpleOpenGlControl1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void simpleOpenGlControl1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void simpleOpenGlControl1_KeyUp(object sender, KeyEventArgs e)
        {
            soundsw.Restart();
        }

        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width/2+p.X, simpleOpenGlControl1.Size.Height/2+p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X+simpleOpenGlControl1.Size.Width/2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }
    }
}
