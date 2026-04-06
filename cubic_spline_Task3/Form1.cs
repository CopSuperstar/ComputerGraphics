namespace cubic_spline_Task3
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

                List<PointF> spline = ComputeCatmullRom(points, 100);
                if (spline.Count >= 2)
                {
                    g.DrawLines(Pens.Red, spline.ToArray());
                }
            }
        }

        private List<PointF> ComputeCatmullRom(List<Point> pts, int stepsPerSegment)
        {
            List<PointF> result = new List<PointF>();

            for (int i = 0; i < pts.Count - 1; i++)
            {
                PointF p0 = i == 0 ? pts[0] : pts[i - 1];
                PointF p1 = pts[i];
                PointF p2 = pts[i + 1];
                PointF p3 = i + 2 >= pts.Count ? pts[pts.Count - 1] : pts[i + 2];

                for (int step = 0; step <= stepsPerSegment; step++)
                {
                    float t = (float)step / stepsPerSegment;
                    float t2 = t * t;
                    float t3 = t2 * t;

                    float x = 0.5f * ((2 * p1.X) +
                                      (-p0.X + p2.X) * t +
                                      (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * t2 +
                                      (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3);

                    float y = 0.5f * ((2 * p1.Y) +
                                      (-p0.Y + p2.Y) * t +
                                      (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2 +
                                      (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3);

                    result.Add(new PointF(x, y));
                }
            }

            return result;
        }
    }
}