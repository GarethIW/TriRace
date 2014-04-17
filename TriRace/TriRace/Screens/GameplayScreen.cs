using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using TriRace.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using TimersAndTweens;

namespace TriRace.Screens
{
    class GameplayScreen : GameScreen
    {
        private Camera camera;
        private Map map;

        private ParticleController particleController = new ParticleController();

        List<TrackTri> tris = new List<TrackTri>();

        public List<VertexPositionNormalColor> Vertices = new List<VertexPositionNormalColor>();
        public List<short> Indexes = new List<short>();

        public VertexPositionNormalColor[] VertexArray;
        public short[] IndexArray;

        public Matrix worldMatrix;
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        private BasicEffect drawEffect;

        private RasterizerState rastState;

        private Racer racer;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            map = content.Load<Map>("map");

            TrackGenerator.Generate(map);

            var spawn = ((MapObjectLayer)map.GetLayer("Spawn")).Objects[0];
            racer = new Racer(content.Load<Texture2D>("racer"), new Rectangle(0, 0, 10, 10), new Vector2(0, 0));  
            racer.Position = new Vector2(spawn.Location.Center.X, spawn.Location.Center.Y);
            racer.Rotation = float.Parse(spawn.Properties["rot"]);
            racer.Active = true;

            var track = (MapObjectLayer)map.GetLayer("Tris");

            foreach(MapObject o in track.Objects.Where(ob=>ob.PolyPoints!=null && ob.PolyPoints.Count==3))
                tris.Add(new TrackTri(o.PolyPoints[0], o.PolyPoints[1], o.PolyPoints[2]));

            
            //projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi, ScreenManager.Game.GraphicsDevice.Viewport.AspectRatio, 0.1f, 200f);
            Vector2 center;
            center.X = ScreenManager.Game.GraphicsDevice.Viewport.Width * 0.5f;
            center.Y = ScreenManager.Game.GraphicsDevice.Viewport.Height * 0.5f;
            viewMatrix = Matrix.CreateLookAt(new Vector3(center, -0.1f), new Vector3(center, 10f), new Vector3(0, -1, 0));
            projectionMatrix = Matrix.CreatePerspective(center.X * 2, center.Y * 2, 0.1f, 100f);

            drawEffect = new BasicEffect(ScreenManager.Game.GraphicsDevice) {VertexColorEnabled = true, };
            rastState = new RasterizerState()
            {
                CullMode = CullMode.None
            };

            camera = new Camera(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight, map);

            

            particleController.LoadContent(content);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            racer.Update(gameTime, map, tris);
            camera.Zoom = 2f - (racer.Speed.Length()*0.2f);

            foreach(var t in tris) t.Update(new Vector3(racer.Position.X,racer.Position.Y,0f));

            camera.Target = racer.Position;

            worldMatrix = camera.CameraMatrix; //Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up) * Matrix.CreateScale(1f) * Matrix.CreateTranslation(new Vector3(-20f, -20f, 0f));

            drawEffect.World = worldMatrix;
            drawEffect.View = viewMatrix;
            drawEffect.Projection = projectionMatrix;

            camera.Update(gameTime);
           
            particleController.Update(gameTime, map, tris);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputState input)
        {
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Up)) racer.Boost();
            //if (input.CurrentKeyboardState.IsKeyDown(Keys.Down)) racer.Position.Y += 0.1f;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Left)) racer.Rotation -= 0.05f;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Right)) racer.Rotation += 0.05f;

            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 center = new Vector2(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight) / 2f;
            SpriteBatch sb = ScreenManager.SpriteBatch;

            ScreenManager.Game.GraphicsDevice.Clear(new Color(50,50,50));

            ScreenManager.Game.GraphicsDevice.RasterizerState = rastState;

            Vertices.Clear();
            Indexes.Clear();

            for (int tri = 0; tri < tris.Count; tri++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vertices.Add(new VertexPositionNormalColor(tris[tri].Points[i], new Vector3(0f, 0f, -1f), tris[tri].Tint * tris[tri].Alpha));
                    Indexes.Add((short)((tri * 3) + i));
                }
            }

            VertexArray = Vertices.ToArray();
            IndexArray = Indexes.ToArray();

            foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                ScreenManager.Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, VertexArray, 0, VertexArray.Length, IndexArray, 0, VertexArray.Length / 3);
            }

            particleController.Draw(sb, camera, 0);

            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.CameraMatrix);
            racer.Draw(sb);
            sb.End();

            //particleController.Draw(sb, camera, 1);

            //sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            //sb.End();

            sb.Begin();
            sb.DrawString(ScreenManager.Font, racer.Position.ToString(), Vector2.One * 10f, Color.White);
            sb.End();

            ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);

            base.Draw(gameTime);

        }
    }
}
