using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Task4BezierSurface
{
    public partial class Form1 : Form
    {
        private Point3D[,] controlPoints = new Point3D[4, 4];

        private float angleX = 0;
        private float angleY = 0;
        private float angleZ = 0;

        private const float RotationStep = 0.1f;

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "points.txt");
            LoadPoints(path);
            Invalidate();
        }

        private void LoadPoints(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("File not found:\n" + filename);
                return;
            }

            string[] lines = File.ReadAllLines(filename);

            if (lines.Length < 16)
            {
                MessageBox.Show("points.txt must contain 16 lines");
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    string[] parts = lines[i * 4 + j].Split(' ');

                    float x = float.Parse(parts[0]);
                    float y = float.Parse(parts[1]);
                    float z = float.Parse(parts[2]);

                    controlPoints[i, j] = new Point3D(x, y, z);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: angleX -= RotationStep; break;
                case Keys.S: angleX += RotationStep; break;
                case Keys.A: angleY -= RotationStep; break;
                case Keys.D: angleY += RotationStep; break;
                case Keys.Q: angleZ -= RotationStep; break;
                case Keys.E: angleZ += RotationStep; break;
            }

            Invalidate();
        }

        private Point3D RotatePoint(Point3D p)
        {
            float y1 = p.Y * MathF.Cos(angleX) - p.Z * MathF.Sin(angleX);
            float z1 = p.Y * MathF.Sin(angleX) + p.Z * MathF.Cos(angleX);

            float x2 = p.X * MathF.Cos(angleY) + z1 * MathF.Sin(angleY);
            float z2 = -p.X * MathF.Sin(angleY) + z1 * MathF.Cos(angleY);

            float x3 = x2 * MathF.Cos(angleZ) - y1 * MathF.Sin(angleZ);
            float y3 = x2 * MathF.Sin(angleZ) + y1 * MathF.Cos(angleZ);

            return new Point3D(x3, y3, z2);
        }

        private PointF Project(Point3D p)
        {
            float scale = 40;

            float centerX = ClientSize.Width / 2f;
            float centerY = ClientSize.Height / 2f;

            return new PointF(
                p.X * scale + centerX,
                p.Y * scale + centerY
            );
        }

        private Point3D ComputeBezier3D(Point3D[] pts, float t)
        {
            int n = pts.Length - 1;

            float x = 0;
            float y = 0;
            float z = 0;

            for (int i = 0; i <= n; i++)
            {
                float w = (float)(Binomial(n, i) * Math.Pow(t, i) * Math.Pow(1 - t, n - i));

                x += pts[i].X * w;
                y += pts[i].Y * w;
                z += pts[i].Z * w;
            }

            return new Point3D(x, y, z);
        }

        private double Binomial(int n, int k)
        {
            if (k == 0 || k == n) return 1;

            double result = 1;
                
            for (int i = 0; i < k; i++)
                result *= (double)(n - i) / (i + 1);

            return result;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int steps = 30;

            for (int i = 0; i < 4; i++)
            {
                Point3D[] row =
                {
                    controlPoints[i,0],
                    controlPoints[i,1],
                    controlPoints[i,2],
                    controlPoints[i,3]
                };

                List<PointF> curve = new List<PointF>();

                for (int s = 0; s <= steps; s++)
                {
                    float t = (float)s / steps;

                    Point3D p = ComputeBezier3D(row, t);
                    p = RotatePoint(p);

                    curve.Add(Project(p));
                }

                g.DrawLines(Pens.Red, curve.ToArray());
            }

            for (int j = 0; j < 4; j++)
            {
                Point3D[] col =
                {
                    controlPoints[0,j],
                    controlPoints[1,j],
                    controlPoints[2,j],
                    controlPoints[3,j]
                };

                List<PointF> curve = new List<PointF>();

                for (int s = 0; s <= steps; s++)
                {
                    float t = (float)s / steps;

                    Point3D p = ComputeBezier3D(col, t);
                    p = RotatePoint(p);

                    curve.Add(Project(p));
                }

                g.DrawLines(Pens.Red, curve.ToArray());
            }
        }
    }

    struct Point3D
    {
        public float X;
        public float Y;
        public float Z;

        public Point3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}