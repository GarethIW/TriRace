using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using TimersAndTweens;

namespace TriRace.Entities
{
    class Racer : Entity
    {
        public const int MAX_LIFE = 1000;

        public int Life = MAX_LIFE;
        public float Rotation = 0f;

        private const float GRAVITY = 0.03f;

        private SpriteAnim _idleAnim;

        private Color _tint = Color.White;

        public Racer(Texture2D spritesheet, Rectangle hitbox, Vector2 hitboxoffset) 
            : base(spritesheet, hitbox, hitboxoffset)
        {
            _idleAnim = new SpriteAnim(spritesheet, 0, 1, 41,41,0, new Vector2(20f,20f));

            Speed = Vector2.Zero;
        }

        public override void Update(GameTime gameTime, Map gameMap)
        {
            // inertia
            Speed *= 0.99f;

            Position += Speed;

            _idleAnim.Update(gameTime);

            Life--;
            if (Life <= 0) Active = false;

            //CheckMapCollisions(gameMap);

            _tint = Color.White;

            base.Update(gameTime, gameMap);
        }

        public override void OnCollision(Entity collided, Rectangle intersect)
        {
            // Collides with another Hero
            if (collided.GetType() == typeof(Racer)) _tint = Color.Red;
                

            base.OnCollision(collided, intersect);
        }

        private void CheckMapCollisions(Map gameMap)
        {
            // Check downward collision
            if(Speed.Y>0)
                for (int x = HitBox.Left+2; x <= HitBox.Right-2; x += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(x, HitBox.Bottom + Speed.Y));
                    if (coll.HasValue && coll.Value) Speed.Y = 0;
                }

            // Check left collision
            if(Speed.X<0)
                for (int y = HitBox.Top+2; y <= HitBox.Bottom-2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Left - Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = 0;
                    }
                }

            // Check right collision
            if (Speed.X > 0)
                for (int y = HitBox.Top+2; y <= HitBox.Bottom-2; y += 2)
                {
                    bool? coll = gameMap.CheckCollision(new Vector2(HitBox.Right + Speed.X, y));
                    if (coll.HasValue && coll.Value)
                    {
                        Speed.X = 0;
                    }
                }
        }

        public void Boost()
        {
            Vector2 thrust = Helper.AngleToVector(Rotation, 0.04f);
            //thrust.Normalize();

            float maxS = 10f;
            Vector2 maxSpeed = new Vector2(1f, 1f);
            maxSpeed.Normalize();
            maxSpeed *= maxS;

            Speed = Vector2.Clamp(Speed+thrust, -maxSpeed, maxSpeed);
            ParticleController.Instance.Add(Position,
                                            Helper.AngleToVector((Rotation + Helper.RandomFloat(-0.1f,0.1f))+MathHelper.Pi, Helper.RandomFloat(0.2f,2f)),
                                            0, Helper.RandomFloat(500,2000), 500,
                                            false, false, 
                                            new Rectangle(0,0,5,5),
                                            Color.Orange,
                                            ParticleFunctions.FadeInOut,
                                            1f,0f,0, ParticleBlend.Alpha);
        }

        public override void Draw(SpriteBatch sb)
        {
            //_idleAnim.Draw(sb, Position);
            _idleAnim.Draw(sb,Position,SpriteEffects.None, 1f, Rotation, _tint);
            base.Draw(sb);
        }

        public override void Reset()
        {
            Life = MAX_LIFE;
            Speed = Vector2.Zero;

            base.Reset();
        }
    }
}
