using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics
{
    public partial class Controls : Form
    {

        Dictionary<int, char> controls;
        FileStream stream;
        BinaryFormatter formatter;

        public Controls(GraphicsForm g)
        {
            InitializeComponent();
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Controls_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            textBox1.Text = controls[0].ToString().ToUpper();
            textBox2.Text = controls[2].ToString().ToUpper();
            textBox3.Text = controls[1].ToString().ToUpper();
            textBox4.Text = controls[3].ToString().ToUpper();
            textBox5.Text = controls[4].ToString().ToUpper();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
            if(textBox1.Text.Length > 1)
            {
                textBox1.Text = textBox1.Text.ToCharArray()[1].ToString();
            }
            textBox1.Text = textBox1.Text.ToUpper();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
            if (textBox2.Text.Length > 1)
            {
                textBox2.Text = textBox2.Text.ToCharArray()[1].ToString();
            }
            textBox2.Text = textBox2.Text.ToUpper();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
            if (textBox3.Text.Length > 1)
            {
                textBox3.Text = textBox3.Text.ToCharArray()[1].ToString();
            }
            textBox3.Text = textBox3.Text.ToUpper();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            
            if (textBox4.Text.Length > 1)
            {
                textBox4.Text = textBox4.Text.ToCharArray()[1].ToString();
            }
            textBox4.Text = textBox4.Text.ToUpper();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            
            if (textBox5.Text.Length > 1)
            {
                textBox5.Text = textBox5.Text.ToCharArray()[1].ToString();
            }
            textBox5.Text = textBox5.Text.ToUpper();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] temp = { textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text };

            if(textBox1.Text.Length == 1 && textBox2.Text.Length == 1 && textBox3.Text.Length == 1 && textBox4.Text.Length == 1 && textBox5.Text.Length == 1 &&
                textBox1.Text != " " && textBox2.Text != " " && textBox3.Text != " " && textBox4.Text != " " && textBox5.Text != " " &&
                temp.Length == temp.Distinct().Count())
            {
                controls[0] = textBox1.Text.ToLower().ToCharArray()[0];
                controls[2] = textBox2.Text.ToLower().ToCharArray()[0];
                controls[1] = textBox3.Text.ToLower().ToCharArray()[0];
                controls[3] = textBox4.Text.ToLower().ToCharArray()[0];
                controls[4] = textBox5.Text.ToLower().ToCharArray()[0];

                stream = new FileStream("controls.txt", FileMode.Create);
                formatter = new BinaryFormatter();
                formatter.Serialize(stream, controls);
                stream.Close();
                //GraphicsForm g = new GraphicsForm();
                //g.Show();
                this.Close();
                GraphicsForm.controls = controls;
            }
            else
            {
                MessageBox.Show("Invalid Input");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //GraphicsForm g = new GraphicsForm();
            //g.Show();
            this.Close();
        }

        
    }
}
