using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace areyesram
{
    internal class Cube
    {
        internal float AngleX { get; set; }
        internal float AngleY { get; set; }
        internal HiddenLinesMode HiddenLines { get; set; }

        internal Cube(float side)
        {
            _edges = _vertices
                .SelectMany((a, i) => _vertices
                    .Select((b, j) => new IndexPair(i, j)))
                .Where(o =>
                {
                    var a = _vertices[o.I];
                    var b = _vertices[o.J];
                    return o.I < o.J
                           && (a.X == b.X && a.Y == b.Y && a.Z != b.Z
                               || a.X == b.X && a.Y != b.Y && a.Z == b.Z
                               || a.X != b.X && a.Y == b.Y && a.Z == b.Z);
                })
                .ToList();
            var size2 = side / 2;
            foreach (var vertex in _vertices)
            {
                vertex.X *= size2;
                vertex.Y *= size2;
                vertex.Z *= size2;
            }
        }

        private readonly Voxel[] _vertices = {
            new Voxel(-1,-1,-1),
            new Voxel(+1,-1,-1),
            new Voxel(-1,+1,-1),
            new Voxel(+1,+1,-1),
            new Voxel(-1,-1,+1),
            new Voxel(+1,-1,+1),
            new Voxel(-1,+1,+1),
            new Voxel(+1,+1,+1)
        };

        private readonly List<IndexPair> _edges;
        private readonly Pen _solidPen = new Pen(Brushes.DarkBlue, 2);
        private readonly Pen _dashedPen = new Pen(Brushes.SteelBlue, 1) { DashPattern = new[] { 1f, 1f } };

        internal Voxel[] RotateY(Voxel[] vertices, float theta)
        {
            var res = vertices.Select(o => new Voxel(o.X, o.Y, o.Z)).ToArray();
            var sinT = (float)Math.Sin(theta);
            var cosT = (float)Math.Cos(theta);
            foreach (var voxel in res)
            {
                var x = voxel.X;
                var z = voxel.Z;
                voxel.X = x * cosT - z * sinT;
                voxel.Z = z * cosT + x * sinT;
            }
            return res;
        }

        internal Voxel[] RotateX(Voxel[] vertices, float theta)
        {
            var res = vertices.Select(o => new Voxel(o.X, o.Y, o.Z)).ToArray();
            var sinT = (float)Math.Sin(theta);
            var cosT = (float)Math.Cos(theta);
            foreach (var voxel in res)
            {
                var y = voxel.Y;
                var z = voxel.Z;
                voxel.Y = y * cosT - z * sinT;
                voxel.Z = z * cosT + y * sinT;
            }
            return res;
        }

        public void Draw(Graphics g)
        {
            var rotated = RotateY(_vertices, AngleY);
            rotated = RotateX(rotated, AngleX);
            var v0 = new PointF(g.VisibleClipBounds.Width / 2, g.VisibleClipBounds.Height / 2);
            var z0 = rotated.Select(v => v.Z).Min();
            foreach (var edge in _edges)
            {
                var v1 = rotated[edge.I];
                var v2 = rotated[edge.J];
                var hidden = v1.Z.Equals(z0) || v2.Z.Equals(z0);
                if (hidden && HiddenLines == HiddenLinesMode.DontDraw) continue;
                var pen = hidden && HiddenLines == HiddenLinesMode.DrawDashed ? _dashedPen : _solidPen;
                g.DrawLine(pen, v0.X + v1.X, v0.Y + v1.Y, v0.X + v2.X, v0.Y + v2.Y);
            }
        }
    }
}
