using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace areyesram
{
    public sealed partial class MainForm : Form
    {
        private Cube _cube;

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitCube();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            _cube.Draw(e.Graphics);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            InitCube();
            Invalidate();
        }

        private void InitCube()
        {
            float saveAX = 0, saveAY = 0;
            if (_cube != null)
            {
                saveAX = _cube.AngleX;
                saveAY = _cube.AngleY;
            }
            _cube = new Cube(Math.Min(ClientRectangle.Width, ClientRectangle.Height) * 0.5f);
            _cube.AngleX = saveAX;
            _cube.AngleY = saveAY;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _cube.AngleX += 0.01f;
            _cube.AngleY += 0.02f;
            Invalidate();
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)27:    // Escape
                    Application.Exit();
                    break;
                case ' ':
                    _cube.HiddenLines++;
                    if (_cube.HiddenLines > HiddenLinesMode.DrawDashed)
                        _cube.HiddenLines = HiddenLinesMode.DontDraw;
                    break;
            }
        }
    }
}
