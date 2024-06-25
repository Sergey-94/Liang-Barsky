using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LR1_AKG
{
    public partial class Form1 : Form
    {
        public PointDoub[] Lines = new PointDoub[16];
        public PointDoub[] LinesCrop = new PointDoub[16];
        PointF BoxLU = new PointF(150, 150);
        PointF BoxRD = new PointF(450, 450);

        public Form1()
        {
            InitializeComponent();
            initGraphics();
        }

        // Нарисовать рандомно линии
        private void initialLines()
        {
            Random Coord = new Random();

            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = new PointDoub(Coord.Next(0, pictureBox1.Width), Coord.Next(0, pictureBox1.Height), Coord.Next(0, pictureBox1.Width), Coord.Next(0, pictureBox1.Height));
            }
        }

        // Инициализация поверхности рисования
        private Graphics initGraphics()
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);
            return g;
        }

        // Рисуем линии из массива
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Graphics g = initGraphics();
            initialLines();
            for (int i = 0; i < Lines.Length; i++)
            {
                g.DrawLine(new Pen(Color.Blue), Lines[i].xStart, Lines[i].yStart, Lines[i].xEnd, Lines[i].yEnd);
            }
        }

        // Кропаем линии по окну
        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g = initGraphics();
            try
            {
                for (int i = 0; i < LinesCrop.Length; i++)
                { LinesCrop[i] = LBalg(new PointF(Lines[i].xStart, Lines[i].yStart), new PointF(Lines[i].xEnd, Lines[i].yEnd), BoxLU, BoxRD); }
            
                for (int i = 0; i < LinesCrop.Length; i++)
                { g.DrawLine(new Pen(Color.Blue), LinesCrop[i].xStart, LinesCrop[i].yStart, LinesCrop[i].xEnd, LinesCrop[i].yEnd); }

                BoxLU = new PointF(float.Parse(this.textBox1.Text), float.Parse(this.textBox2.Text));
                BoxRD = new PointF(float.Parse(this.textBox3.Text), float.Parse(this.textBox4.Text));
                g.DrawRectangle(new Pen(Color.Red), new Rectangle((int)BoxLU.X, (int)BoxLU.Y, (int)(BoxRD.X - BoxLU.X), (int)(BoxRD.Y - BoxLU.Y)));

            }
            catch
            {
                MessageBox.Show("В одном из полей \nвведено не число!", "Ошибка!");
            }
        }

        /* 
         * Часть алгоритма Лианга-Барски. Определяет входимость линии в прямоугольник и
         * задаёт параметры t (minT и maxT)
         * Возвращает false, если линия не входит в прямоугольную область
         * p,q - параметры в алгоритме
         */
        bool VisibleLine(ref float minT, ref float maxT, float p, float q)
        {
            bool blResult = true;
            float t;
            if (p == 0)
            {
                if (q < 0)
                {
                    blResult = false;
                }
            }
            else
            {
                t = q / p;
                if (p < 0)
                {
                    if (t > maxT)
                        blResult = false;
                    else
                    {
                        if (t > minT)
                            minT = t;
                    }
                }
                else
                {
                    if (t < minT)
                        blResult = false;
                    else
                    {
                        if (t < maxT)
                            maxT = t;
                    }
                }
            }
            return blResult;
        }

        /*
         * Алгоритм Лианга-Барски
         * Возвращает координаты отсечённого отрезка
         * lineStart - координаты начало отрезка
         * lineEnd - координаты конца отрезка
         * boxLU - координаты левого верхнего угла прямоугольной области
         * boxRD - координаты правого нижнего угла прямоугольной области
         */
        private PointDoub LBalg(PointF lineStart, PointF lineEnd, PointF boxLU, PointF boxRD)
        {

            float minT = 0, maxT = 1;
            float dx = lineEnd.X - lineStart.X;
            float dy = lineEnd.Y - lineStart.Y;
            bool flagVisible = false;
            PointDoub resultLine = new PointDoub();
            resultLine.xEnd = lineEnd.X;
            resultLine.yEnd = lineEnd.Y;
            resultLine.xStart = lineStart.X;
            resultLine.yStart = lineStart.Y;
            if (VisibleLine(ref minT, ref maxT, -dx, lineStart.X - boxLU.X))
            {
                if (VisibleLine(ref minT, ref maxT, dx, boxRD.X - lineStart.X))
                {
                    if (VisibleLine(ref minT, ref maxT, -dy, lineStart.Y - boxLU.Y))
                    {
                        if (VisibleLine(ref minT, ref maxT, dy, boxRD.Y - lineStart.Y))
                        {
                            flagVisible = true;
                            if (maxT < 1)
                            {
                                resultLine.xEnd = lineStart.X + maxT * dx;
                                resultLine.yEnd = lineStart.Y + maxT * dy;
                            }
                            if (minT > 0)
                            {
                                resultLine.xStart = lineStart.X + minT * dx;
                                resultLine.yStart = lineStart.Y + minT * dy;
                            }
                        }
                    }
                }
            }
            if (!flagVisible)
            {
                resultLine.xEnd = 0;
                resultLine.yEnd = 0;
                resultLine.xStart = 0;
                resultLine.yStart = 0;
            }
            return resultLine;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Graphics g = initGraphics();
            BoxLU = new PointF(PointToClient(MousePosition).X - ((float.Parse(this.textBox3.Text) - float.Parse(this.textBox1.Text)) / 2),
                               PointToClient(MousePosition).Y - ((float.Parse(this.textBox4.Text) - float.Parse(this.textBox2.Text)) / 2));
            BoxRD = new PointF(PointToClient(MousePosition).X + ((float.Parse(this.textBox3.Text) - float.Parse(this.textBox1.Text)) / 2),
                               PointToClient(MousePosition).Y + ((float.Parse(this.textBox4.Text) - float.Parse(this.textBox2.Text)) / 2));

            g.DrawRectangle(new Pen(Color.Red), new Rectangle((int)BoxLU.X, (int)BoxLU.Y, (int)(BoxRD.X - BoxLU.X), (int)(BoxRD.Y - BoxLU.Y)));

            for (int i = 0; i < LinesCrop.Length; i++)
            { LinesCrop[i] = LBalg(new PointF(Lines[i].xStart, Lines[i].yStart), new PointF(Lines[i].xEnd, Lines[i].yEnd), BoxLU, BoxRD); }

            for (int i = 0; i < LinesCrop.Length; i++)
            { g.DrawLine(new Pen(Color.Blue), LinesCrop[i].xStart, LinesCrop[i].yStart, LinesCrop[i].xEnd, LinesCrop[i].yEnd); }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.timer1.Enabled = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            this.timer1.Enabled = false;
        }
    }
}
