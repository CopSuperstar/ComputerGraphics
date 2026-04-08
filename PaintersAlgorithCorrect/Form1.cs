using System.Linq;
namespace PaintersAlgorithCorrect
{
    public partial class Form1 : Form
    {
        private float angleX = 0;
        private float angleY = 0;
        private float angleZ = 0;
        private const float RotationStep = 0.02f;

        private List<Triangle> triangles = new List<Triangle>();

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            DoubleBuffered = true;
            BuildCube();
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void BuildCube()
        {
            float s = 1f;

            Point3D lbf = new Point3D(-s, -s, -s); 
            Point3D rbf = new Point3D(s, -s, -s); 
            Point3D ltf = new Point3D(-s, s, -s); 
            Point3D rtf = new Point3D(s, s, -s); 
            Point3D lbb = new Point3D(-s, -s, s);
            Point3D rbb = new Point3D(s, -s, s);
            Point3D ltb = new Point3D(-s, s, s); 
            Point3D rtb = new Point3D(s, s, s); 

            triangles.Add(new Triangle(lbf, rbf, rtf));
            triangles.Add(new Triangle(lbf, rtf, ltf));
        
            triangles.Add(new Triangle(rbb, lbb, ltb));
            triangles.Add(new Triangle(rbb, ltb, rtb));
            triangles.Add(new Triangle(lbb, lbf, ltf));
            triangles.Add(new Triangle(lbb, ltf, ltb));
            triangles.Add(new Triangle(rbf, rbb, rtb));
            triangles.Add(new Triangle(rbf, rtb, rtf));
            triangles.Add(new Triangle(ltf, rtf, rtb));
            triangles.Add(new Triangle(lbb, rbb, rbf));
            triangles.Add(new Triangle(lbb, rbf, lbf));
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
            float scale = 100f;
            float centerX = ClientSize.Width / 2f;
            float centerY = ClientSize.Height / 2f;
            return new PointF(p.X * scale + centerX, p.Y * scale + centerY);
        }

        private float GetDepth(Triangle t)
        {
            Point3D ra = RotatePoint(t.A);
            Point3D rb = RotatePoint(t.B);
            Point3D rc = RotatePoint(t.C);
            return (ra.Z + rb.Z + rc.Z) / 3f;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            List<Triangle> sorted = triangles
                .OrderBy(t => GetDepth(t))
                .ToList();

            foreach (Triangle t in sorted)
            {
                Point3D ra = RotatePoint(t.A);
                Point3D rb = RotatePoint(t.B);
                Point3D rc = RotatePoint(t.C);

                PointF[] pts = {
                    Project(ra),
                    Project(rb),
                    Project(rc)
                };

                g.FillPolygon(Brushes.LightBlue, pts);
                g.DrawPolygon(Pens.Black, pts);
            }
        }
    }

    struct Point3D
    {
        public float X, Y, Z;
        public Point3D(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }
    }

    struct Triangle
    {
        public Point3D A, B, C;
        public Triangle(Point3D a, Point3D b, Point3D c)
        {
            A = a; B = b; C = c;
        }
    }
}