using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TiledLib;

namespace TriRace
{
    class TrackTri
    {

        public Vector3[] Points = new Vector3[3];
        public Color Tint;
        public float Alpha;

        public TrackTri(Point p1, Point p2, Point p3)
        {
            Points[0] = new Vector3(p1.X, p1.Y, 0f) * 0.1f;
            Points[1] = new Vector3(p2.X, p2.Y, 0f) * 0.1f;
            Points[2] = new Vector3(p3.X, p3.Y, 0f) * 0.1f;

            Tint = new Color(new Vector3(Helper.RandomFloat(0.5f,0.7f)));
            Alpha = 1f;
        }
    }
}
