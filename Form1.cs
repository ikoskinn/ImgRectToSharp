using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoIt;

namespace ImgRectToSharp
{
    public partial class Form1 : Form
    {
        public static Pen pen = new Pen(Color.Red, 2);
        public static int x = 0, y = 0, w = 0, h = 0;

        public static Rectangle tempRect = new Rectangle();
        public static List<Rectangle> rects = new List<Rectangle>();

        public static bool paint = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AutoItX.WinMove(TeleREG.TeleImaging.TelegramShot.GetHandleByProcess(textBox1.Text), 0, 0, 300, 501);
            pictureBox1.Image = TeleREG.TeleImaging.TelegramShot.Shot(textBox1.Text);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        { 
            paint = true;
            tempRect = new Rectangle(e.X, e.Y, 0, 0);
            pictureBox1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            paint = false;
            rects.Clear();
            pictureBox1.Invalidate();
            textBox2.Text = "";
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                tempRect = new Rectangle(
                    Math.Min(tempRect.X, e.X),
                    Math.Min(tempRect.Y, e.Y),
                    Math.Abs(e.X - tempRect.X),
                    Math.Abs(e.Y - tempRect.Y));

                rects.Add(tempRect);

                Console.WriteLine("Saved coords " + tempRect.ToString());

                string st = "dict = new Dictionary<string, Rectangle>" + Environment.NewLine +
                    "{" + Environment.NewLine;

                for(int i = 0; i < rects.Count; i++)
                {
                    st += "    { \"Button" + i.ToString() + "\", new Rectangle(" + rects[i].X.ToString()
                        + ", " + rects[i].Y.ToString() + ", " +
                        rects[i].Width.ToString() + ", " + rects[i].Height.ToString() + ") }," + Environment.NewLine;
                }

                st += "};";

                textBox2.Text = st;
                AutoItX.ClipPut(st);

                paint = false;

                tempRect = new Rectangle();
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(paint)
            {
                tempRect = new Rectangle(
                    Math.Min(tempRect.X, e.X),
                    Math.Min(tempRect.Y, e.Y),
                    Math.Abs(e.X - tempRect.X),
                    Math.Abs(e.Y - tempRect.Y));

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            if (paint)
            {
                e.Graphics.DrawRectangle(pen, tempRect);
            }
            if(rects.Count > 0) e.Graphics.DrawRectangles(pen, rects.ToArray());
        }
    }
}
