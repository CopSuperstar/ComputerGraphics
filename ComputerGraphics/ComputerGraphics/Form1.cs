namespace ComputerGraphics
{
    public partial class Form1 : Form
    {
        private List<Point> points = new List<Point>();
        private bool shouldDraw = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                points.Add(e.Location);
                Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                shouldDraw = true;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Point p in points)
            {
                g.FillEllipse(Brushes.Black, p.X - 4, p.Y - 4, 8, 8);
            }

            if (shouldDraw && points.Count >= 2)
            {
                g.DrawLines(Pens.Blue, points.ToArray());

                using (Pen thickGreen = new Pen(Color.Green, 14))
                using (Pen thinRed = new Pen(Color.Red, 4))
                {
                    List<PointF> curve1 = ComputeBezier(points, 1000);
                    g.DrawLines(thickGreen, curve1.ToArray());

                    List<PointF> curve2 = ComputeBezierDeCasteljau(points, 1000);
                    g.DrawLines(thinRed, curve2.ToArray());
                }
            }
        }

        private List<PointF> ComputeBezier(List<Point> pts, int steps)//бернштейн
        {
            List<PointF> result = new List<PointF>();
            int n = pts.Count - 1;

            for (int step = 0; step <= steps; step++)
            {
                double t = (double)step / steps;
                double x = 0;
                double y = 0;

                for (int i = 0; i <= n; i++)
                {
                    double weight = Binomial(n, i) * Math.Pow(t, i) * Math.Pow(1 - t, n - i);
                    x += weight * pts[i].X;
                    y += weight * pts[i].Y;
                }

                result.Add(new PointF((float)x, (float)y));
            }

            return result;
        }

        private double Binomial(int n, int k)
        {
            if (k == 0 || k == n) return 1;
            double result = 1;
            for (int i = 0; i < k; i++)
            {
                result *= (double)(n - i) / (i + 1);
            }
            return result;
        }

        private List<PointF> ComputeBezierDeCasteljau(List<Point> pts, int steps)
        {
            List<PointF> result = new List<PointF>();

            for (int step = 0; step <= steps; step++)
            {
                double t = (double)step / steps;

                PointF[] temp = pts.Select(p => new PointF(p.X, p.Y)).ToArray();

                for (int round = 1; round < temp.Length; round++)
                {
                    for (int i = 0; i < temp.Length - round; i++)
                    {
                        temp[i].X = (float)(temp[i].X * (1 - t) + temp[i + 1].X * t);
                        temp[i].Y = (float)(temp[i].Y * (1 - t) + temp[i + 1].Y * t);
                    }
                }

                result.Add(temp[0]);
            }

            return result;
        }
    }
}