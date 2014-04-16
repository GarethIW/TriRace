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

        private int pow;

        public TrackTri(Point p1, Point p2, Point p3)
        {
            Points[0] = new Vector3(p1.X, p1.Y, 0f);
            Points[1] = new Vector3(p2.X, p2.Y, 0f);
            Points[2] = new Vector3(p3.X, p3.Y, 0f);

            Tint = new Color(new Vector3(Helper.RandomFloat(0.5f,0.7f)));
            Alpha = 1f;

            pow = Helper.Random.Next(6)+1;
        }

        public void Update(Vector3 pos)
        {
            float mindist = 200f;
            Vector3 center = new Vector3((Points[0].X + Points[1].X + Points[2].X) / 3f, (Points[0].Y + Points[1].Y + Points[2].Y) / 3f, 0f);
            float dist = Vector3.Distance(pos, center);
            if (dist > mindist)
            {
                float mag = (1f / 100f) * MathHelper.Clamp(dist - mindist, 0f, 100f);// * 0.1f;
                mag = (float) Math.Pow(mag, pow);
                for (int i = 0; i < 3; i++)
                {
                    Points[i].Z = -(0.05f*mag);
                }
                Alpha = MathHelper.Clamp(1f - mag, 0f, 1f);


            }
        }
    }
}
