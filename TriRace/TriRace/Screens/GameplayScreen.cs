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

        private Vector2 Position;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            map = content.Load<Map>("map");

            var spawn = ((MapObjectLayer)map.GetLayer("Spawn")).Objects[0];
            Position = new Vector2(spawn.Location.Center.X, spawn.Location.Center.Y) * 0.1f;

            var track = (MapObjectLayer)map.GetLayer("Tris");

            //tris.Add(new TrackTri(new Point(0,0), new Point(0,0), new Point(1,1), new Point(-1,2)));

            foreach(MapObject o in track.Objects.Where(ob=>ob.PolyPoints!=null && ob.PolyPoints.Count==3))
                tris.Add(new TrackTri(o.PolyPoints[0], o.PolyPoints[1], o.PolyPoints[2]));

            
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -40f), new Vector3(0, 0, 0f), Vector3.Down);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ScreenManager.Game.GraphicsDevice.Viewport.AspectRatio, 0.1f, 200f);

            drawEffect = new BasicEffect(ScreenManager.Game.GraphicsDevice) {VertexColorEnabled = true, };
            rastState = new RasterizerState()
            {
                CullMode = CullMode.None
            };

            camera = new Camera(ScreenManager.Game.RenderWidth, ScreenManager.Game.RenderHeight, map);

            camera.Zoom = 1f;

            particleController.LoadContent(content);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            camera.Target = Position;

            worldMatrix = camera.CameraMatrix; //Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up) * Matrix.CreateScale(1f) * Matrix.CreateTranslation(new Vector3(-20f, -20f, 0f));

            drawEffect.World = worldMatrix;
            drawEffect.View = viewMatrix;
            drawEffect.Projection = projectionMatrix;

            camera.Update(gameTime);
           
            particleController.Update(gameTime, map);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputState input)
        {
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Up)) Position.Y -= 1f;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Down)) Position.Y += 1f;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Left)) Position.X -= 1f;
            if (input.CurrentKeyboardState.IsKeyDown(Keys.Right)) Position.X += 1f;

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

            for(int tri=0;tri<tris.Count;tri++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vertices.Add(new VertexPositionNormalColor(tris[tri].Points[i], new Vector3(0f,0f,-1f), tris[tri].Tint));
                    Indexes.Add((short)((tri*3) + i));
                }
            }

            VertexArray = Vertices.ToArray();
            IndexArray = Indexes.ToArray();

            foreach (EffectPass pass in drawEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                ScreenManager.Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalColor>(PrimitiveType.TriangleList, VertexArray, 0, VertexArray.Length, IndexArray, 0, VertexArray.Length / 3);
            }

            //sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.CameraMatrix);
          
            //sb.End();

            //particleController.Draw(sb, camera, 1);

            //sb.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null);
            //sb.End();

            ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);

            base.Draw(gameTime);

        }
    }
}
