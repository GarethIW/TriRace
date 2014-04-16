using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Xna.Framework;
using TiledLib;

namespace TriRace
{
    static class TrackGenerator
    {
        public static void Generate(Map gameMap)
        {
            const float deviation = 40f;

            List<Point> trackLine = new List<Point>();
            List<Point> innerTrack = new List<Point>();
            List<Point> outerTrack = new List<Point>();

            List<Point> controlPoints = new List<Point>();

            int mapWidth = gameMap.Width*gameMap.TileWidth;
            int mapHeight = gameMap.Height*gameMap.TileHeight;

            controlPoints.Add(new Point(0, mapHeight / 2) + new Point(Helper.Random.Next((int)deviation) - ((int)deviation / 2), Helper.Random.Next((int)deviation) - ((int)deviation / 2)));
            if (Helper.Random.Next(2) == 0)
                controlPoints.Add(new Point((mapWidth / 8) + Helper.Random.Next(mapWidth / 6), (mapHeight / 8) + Helper.Random.Next(mapHeight / 6)) + new Point(Helper.Random.Next((int)deviation) - ((int)deviation / 2), Helper.Random.Next((int)deviation) - ((int)deviation / 2)));
            controlPoints.Add(new Point(mapWidth/2, 0) + new Point(Helper.Random.Next((int) deviation) - ((int) deviation/2), Helper.Random.Next((int) deviation) - ((int) deviation/2)));
            if (Helper.Random.Next(2) == 0)
                controlPoints.Add(new Point(((mapWidth / 8) * 7) - Helper.Random.Next(mapWidth / 6), (mapHeight / 8) + Helper.Random.Next(mapHeight /6)) + new Point(Helper.Random.Next((int)deviation) - ((int)deviation / 2), Helper.Random.Next((int)deviation) - ((int)deviation / 2)));
            controlPoints.Add(new Point(mapWidth, mapHeight/2) + new Point(Helper.Random.Next((int) deviation) - ((int) deviation/2), Helper.Random.Next((int) deviation) - ((int) deviation/2)));
            if (Helper.Random.Next(2) == 0)
                controlPoints.Add(new Point(((mapWidth / 8) * 7) - Helper.Random.Next(mapWidth / 6), ((mapHeight / 8)*7) - Helper.Random.Next(mapHeight / 6)) + new Point(Helper.Random.Next((int)deviation) - ((int)deviation / 2), Helper.Random.Next((int)deviation) - ((int)deviation / 2)));
            controlPoints.Add(new Point(mapWidth/2, mapHeight) + new Point(Helper.Random.Next((int) deviation) - ((int) deviation/2), Helper.Random.Next((int) deviation) - ((int) deviation/2)));
            if (Helper.Random.Next(2) == 0)
                controlPoints.Add(new Point((mapWidth / 8) + Helper.Random.Next(mapWidth / 6), ((mapHeight / 8) * 7) - Helper.Random.Next(mapHeight / 6)) + new Point(Helper.Random.Next((int)deviation) - ((int)deviation / 2), Helper.Random.Next((int)deviation) - ((int)deviation / 2)));

            for (int i = 0; i < controlPoints.Count; i++)
            {
                trackLine.Add(controlPoints[i]);
                for (float d = 0.25f; d <= 0.75f; d += 0.25f)
                {
                    int nextPoint = i + 1;
                    if (nextPoint == controlPoints.Count) nextPoint = 0;
                    Vector2 newPoint = Vector2.Lerp(Helper.PtoV(controlPoints[i]), Helper.PtoV(controlPoints[nextPoint]),d);

                    newPoint.X = Helper.RandomFloat(newPoint.X - deviation, newPoint.X + deviation);
                    newPoint.Y = Helper.RandomFloat(newPoint.Y - deviation, newPoint.Y + deviation);

                    trackLine.Add(Helper.VtoP(newPoint));
                }
            }

            foreach (var p in trackLine)
            {
                float width = Helper.RandomFloat(0.8f, 0.95f);
                Point newP =  p - new Point(mapWidth/2,mapHeight/2);
                newP.X = (int)((float)newP.X * width);
                newP.Y = (int)((float)newP.Y * width);
                newP += new Point(mapWidth / 2, mapHeight / 2);
                innerTrack.Add(newP);
            }

            foreach (var p in trackLine)
            {
                float width = Helper.RandomFloat(1.05f, 1.2f);
                Point newP = p - new Point(mapWidth / 2, mapHeight / 2);
                newP.X = (int)((float)newP.X * width);
                newP.Y = (int)((float)newP.Y * width);
                newP += new Point(mapWidth / 2, mapHeight / 2);
                outerTrack.Add(newP);
            }

            var track = (MapObjectLayer)gameMap.GetLayer("Tris");
            for (int i = 0; i < trackLine.Count; i++)
            {
                float triDev = 5f;

                Vector2 start = Helper.PtoV(outerTrack[i]);
                Vector2 end = Helper.PtoV(i + 1 < outerTrack.Count ? outerTrack[i + 1] : outerTrack[0]);

                List<Point> polyPoints;

                for (float d = 0f; d < 1f; d += 0.1f)
                {
                    Vector2 p1 = Vector2.Lerp(start, end, MathHelper.Clamp(d + Helper.RandomFloat(-0.3f, -0.1f), 0f, 1f)) + new Vector2(Helper.RandomFloat(-triDev, triDev), Helper.RandomFloat(-triDev, triDev));
                    Vector2 p2 = Vector2.Lerp(start, end, MathHelper.Clamp(d + Helper.RandomFloat(0.1f,0.3f), 0f, 1f)) + new Vector2(Helper.RandomFloat(-triDev, triDev), Helper.RandomFloat(-triDev, triDev));
                    float a = (float) Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) + Helper.RandomFloat((-MathHelper.PiOver2-MathHelper.PiOver4),-MathHelper.PiOver4);
                    Vector2 off = Helper.PointOnCircle(Vector2.Lerp(p1, p2, 0.5f), Helper.RandomFloat(10f, 60f), a);
                    off += new Vector2(Helper.RandomFloat(-triDev*5f, triDev*5f), Helper.RandomFloat(-triDev*10f, triDev*10f));

                    polyPoints = new List<Point>
                    {
                        Helper.VtoP(p1),
                        Helper.VtoP(p2),
                        Helper.VtoP(off)
                    };
                    track.Objects.Add(new MapObject("", "", new Rectangle(outerTrack[i].X, outerTrack[i].Y, 0, 0), null, polyPoints, new PropertyCollection()));

                    a = (float)Math.Atan2(end.Y - start.Y, end.X - start.X) + Helper.RandomFloat((-MathHelper.PiOver2 - MathHelper.PiOver4), -MathHelper.PiOver4);
                    p1 = Helper.PointOnCircle(Vector2.Lerp(p1, p2, 0.5f), Helper.RandomFloat(0f, 20f), a);// + new Vector2(Helper.RandomFloat(-triDev, triDev), Helper.RandomFloat(-triDev, triDev));
                    a = (float)Math.Atan2(end.Y - start.Y, end.X - start.X) + Helper.RandomFloat((-MathHelper.PiOver2 - MathHelper.PiOver4), -MathHelper.PiOver4);
                    p2 = Helper.PointOnCircle(Vector2.Lerp(p1, p2, 0.5f), Helper.RandomFloat(20f, 150f), a);// + new Vector2(Helper.RandomFloat(-triDev, triDev), Helper.RandomFloat(-triDev, triDev));
                    //p2 += new Vector2(Helper.RandomFloat(-triDev * 5f, triDev * 5f), Helper.RandomFloat(-triDev * 10f, triDev * 10f));
                    a = (float)Math.Atan2(end.Y - start.Y, end.X - start.X) + Helper.RandomFloat((-MathHelper.PiOver2 - MathHelper.PiOver4), -MathHelper.PiOver4);
                    off = Helper.PointOnCircle(Vector2.Lerp(p1, p2, 0.5f), Helper.RandomFloat(20f, 150f), a);
                    //off += new Vector2(Helper.RandomFloat(-triDev * 5f, triDev * 5f), Helper.RandomFloat(-triDev * 10f, triDev * 10f));

                    polyPoints = new List<Point>
                    {
                        Helper.VtoP(p1),
                        Helper.VtoP(p2),
                        Helper.VtoP(off)
                    };
                    track.Objects.Add(new MapObject("", "", new Rectangle(outerTrack[i].X, outerTrack[i].Y, 0, 0), null, polyPoints, new PropertyCollection()));
                }

                start = Helper.PtoV(innerTrack[i]);
                end = Helper.PtoV(i + 1 < innerTrack.Count ? innerTrack[i + 1] : innerTrack[0]);

                for (float d = 0f; d < 1f; d += 0.1f)
                {
                    Vector2 p1 = Vector2.Lerp(start, end, MathHelper.Clamp(d + Helper.RandomFloat(-0.3f, -0.1f), 0f, 1f)) + new Vector2(Helper.RandomFloat(-triDev, triDev), Helper.RandomFloat(-triDev, triDev));
                    Vector2 p2 = Vector2.Lerp(start, end, MathHelper.Clamp(d + Helper.RandomFloat(0.1f, 0.3f), 0f, 1f)) + new Vector2(Helper.RandomFloat(-triDev, triDev), Helper.RandomFloat(-triDev, triDev));
                    float a = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X) + Helper.RandomFloat(MathHelper.PiOver4, MathHelper.PiOver4+MathHelper.PiOver4);
                    Vector2 off = Helper.PointOnCircle(Vector2.Lerp(p1, p2, 0.5f), Helper.RandomFloat(10f, 60f), a);
                    off += new Vector2(Helper.RandomFloat(-triDev * 5f, triDev * 5f), Helper.RandomFloat(-triDev * 10f, triDev * 10f));

                    polyPoints = new List<Point>
                    {
                        Helper.VtoP(p1),
                        Helper.VtoP(p2),
                        Helper.VtoP(off)
                    };
                    track.Objects.Add(new MapObject("", "", new Rectangle(innerTrack[i].X, innerTrack[i].Y, 0, 0), null, polyPoints, new PropertyCollection()));

                    a = (float)Math.Atan2(end.Y - start.Y, end.X - start.X) + Helper.RandomFloat(MathHelper.PiOver2-0.1f, MathHelper.PiOver2 + 0.1f);
                    p1 = Helper.PointOnCircle(Vector2.Lerp(p1, p2, 0.5f), Helper.RandomFloat(0f, 20f), a);// + new Vector2(Helper.RandomFloat(-triDev, triDev), Helper.RandomFloat(-triDev, triDev));
                    a = (float)Math.Atan2(end.Y - start.Y, end.X - start.X) + Helper.RandomFloat(MathHelper.PiOver2 - 0.1f, MathHelper.PiOver2 + 0.1f);
                    p2 = Helper.PointOnCircle(Vector2.Lerp(p1, p2, 0.5f), Helper.RandomFloat(20f, 100f), a);// + new Vector2(Helper.RandomFloat(-triDev, triDev), Helper.RandomFloat(-triDev, triDev));
                    //p2 += new Vector2(Helper.RandomFloat(-triDev * 5f, triDev * 5f), Helper.RandomFloat(-triDev * 10f, triDev * 10f));
                    a = (float)Math.Atan2(end.Y - start.Y, end.X - start.X) + Helper.RandomFloat(MathHelper.PiOver2 - 0.1f, MathHelper.PiOver2 + 0.1f);
                    off = Helper.PointOnCircle(Vector2.Lerp(p1, p2, 0.5f), Helper.RandomFloat(20f, 100f), a);
                    //off += new Vector2(Helper.RandomFloat(-triDev * 5f, triDev * 5f), Helper.RandomFloat(-triDev * 10f, triDev * 10f));

                    polyPoints = new List<Point>
                    {
                        Helper.VtoP(p1),
                        Helper.VtoP(p2),
                        Helper.VtoP(off)
                    };
                    track.Objects.Add(new MapObject("", "", new Rectangle(innerTrack[i].X, innerTrack[i].Y, 0, 0), null, polyPoints, new PropertyCollection()));
                }

                polyPoints = new List<Point>
                {
                    trackLine[i],
                    trackLine[i]+new Point(2,0),
                    i+1<trackLine.Count?trackLine[i+1]:trackLine[0]
                };

                track.Objects.Add(new MapObject("", "", new Rectangle(trackLine[i].X, trackLine[i].Y, 0, 0), null, polyPoints, new PropertyCollection()));

                //polyPoints = new List<Point>
                //{
                //    innerTrack[i],
                //    innerTrack[i]+new Point(2,0),
                //    i+1<innerTrack.Count?innerTrack[i+1]:innerTrack[0]
                //};

                //track.Objects.Add(new MapObject("", "", new Rectangle(innerTrack[i].X, innerTrack[i].Y, 0, 0), null, polyPoints, new PropertyCollection()));

                //polyPoints = new List<Point>
                //{
                //    outerTrack[i],
                //    outerTrack[i]+new Point(2,0),
                //    i+1<outerTrack.Count?outerTrack[i+1]:outerTrack[0]
                //};

                //track.Objects.Add(new MapObject("", "", new Rectangle(outerTrack[i].X, outerTrack[i].Y, 0, 0), null, polyPoints, new PropertyCollection()));
            }

            Vector2 spawn = Vector2.Lerp(Helper.PtoV(trackLine[0]), Helper.PtoV(trackLine[1]), 0.5f);
            float spawnDir = (float)Math.Atan2(Helper.PtoV(trackLine[1]).Y - Helper.PtoV(trackLine[0]).Y, Helper.PtoV(trackLine[1]).X - Helper.PtoV(trackLine[0]).X);
            var spawnLayer = ((MapObjectLayer)gameMap.GetLayer("Spawn"));
            PropertyCollection props = new PropertyCollection();
            props.Add("rot", spawnDir.ToString());
            spawnLayer.Objects.Add(new MapObject("","",new Rectangle((int)spawn.X,(int)spawn.Y,0,0), null,null, props));
        }
    }
}
