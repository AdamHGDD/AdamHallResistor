using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    enum statTypes { energy, damage, rate, cost };
    public class Player : GameObject
    {
        //TO DO: implement attack cooldown
        private float speed;
        private float timeSinceLastAttack;
        private float health;
        private float maxHealth;
        private float attackCooldown;
        private float damage;
        private float attackCost;

        //Propterties
        public float Health { get { return health; } set { health = value; } }
        public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
        public float AttackCooldown { get { return attackCooldown; } set { attackCooldown = value; } }
        public float Damage { get { return damage; } set { damage = value; } }
        public float AttackCost { get { return attackCost; } set { attackCost = value; } }
        public float StartX { get; set; }
        public float StartY { get; set; }
        public float XOffset { get; set; }
        public float YOffset { get; set; }

        public float ProjectileSpeed
        {
            get; set;
        }
        public bool RelativeSpeed
        {
            get; set;
        }

        //Properties for animation frames
        public bool Moving
        {
            get; set;
        }
        public int Frame
        {
            get; set;
        }
        public double TimeCounter
        {
            get; set;
        }
        public double SecondsPerFrame
        {
            get; set;
        }
        public float TextureScale
        {
            get; set;
        }

        public Player(float xPos, float yPos)
        {
            X = xPos;
            Y = yPos;
            StartX = xPos;
            StartY = yPos;

            Type = GameObjectType.Player;
            scale = .5f;
            TextureScale = 4.4f;
            speed = 8.0f;
            Hitbox = new Rectangle((int)X, (int)Y, (int)(19 * scale*TextureScale), (int)(28 * scale*TextureScale));

            timeSinceLastAttack = 0;

            // Base Stats
            Damage = 10;
            MaxHealth = 100;
            Health = 50;
            AttackCooldown = 250;
            AttackCost = 1;

            ProjectileSpeed = 10;
            RelativeSpeed = false;
        }

        public override void Update() // needs to change to move the player but stay centered on the player
        {
            XTransform = 0;
            YTransform = 0;
            timeSinceLastAttack += (float)InputManager.Instance.Time.ElapsedGameTime.TotalMilliseconds;

            // set player transform
            if (InputManager.Instance.KbState.IsKeyDown(Keys.W))
            {
                YTransform -= speed;
            }
            if (InputManager.Instance.KbState.IsKeyDown(Keys.A))
            {
                XTransform -= speed;
            }
            if (InputManager.Instance.KbState.IsKeyDown(Keys.S))
            {
                YTransform += speed;
            }
            if (InputManager.Instance.KbState.IsKeyDown(Keys.D))
            {
                XTransform += speed;
            }

            // set camera transfrom back towards the player position
            Vector2 cTransform = ObjectManager.Instance.CameraTransform;
            if(cTransform.X > 0)
            {
                cTransform.X -= .5f;
            }
            if (cTransform.X < 0)
            {
                cTransform.X += .5f;
            }
            if (cTransform.Y > 0)
            {
                cTransform.Y -= .5f;
            }
            if (cTransform.Y < 0)
            {
                cTransform.Y += .5f;
            }
            ObjectManager.Instance.CameraTransform = cTransform;

            //Attack inputs
            if (InputManager.Instance.MState.LeftButton == ButtonState.Pressed)
            {
                if (Health > AttackCost)
                {
                    if (timeSinceLastAttack >= AttackCooldown)
                    {
                        Shoot();
                        ContentManager.Instance.GunNoise.Play();
                        Health -= AttackCost;
                        timeSinceLastAttack = 0;
                    }
                }
                else
                {
                    if (timeSinceLastAttack >= 1500)
                    {
                        Shoot();
                        ContentManager.Instance.GunNoise.Play();
                        timeSinceLastAttack = 0;
                    }
                }
            }
            if (InputManager.Instance.KbState.IsKeyDown(Keys.Space))
            {
                if (Health > AttackCost*3 && ObjectManager.Instance.Bomb == null)
                {
                    DropBomb();
                    timeSinceLastAttack = 0;
                }
            }

            //Frame update
            TimeCounter += InputManager.Instance.Time.ElapsedGameTime.TotalSeconds;

            if (TimeCounter >= SecondsPerFrame)
            {
                if (!Moving && (Math.Abs(XTransform) + Math.Abs(YTransform) > 0))
                {
                    Moving = true;
                    Frame += 4;
                }
                else if (Moving && (Math.Abs(XTransform) + Math.Abs(YTransform) == 0))
                {
                    Moving = false;
                    Frame -= 4;
                }

                Frame++;
                if (Frame % 4 == 0)
                {
                    Frame -= 4;
                }
                TimeCounter -= SecondsPerFrame;
            }

            //Turn player
            Point mouse = InputManager.Instance.MState.Position;
            float width = InputManager.Instance.Screen.Viewport.Width;
            float height = InputManager.Instance.Screen.Viewport.Height;
            if((mouse.X / width < mouse.Y / height) && (mouse.X/width + mouse.Y/height < 1))
            {
                Frame = 16 + Frame % 8;
            }
            else if ((mouse.X / width < mouse.Y / height) && !(mouse.X / width + mouse.Y / height < 1))
            {
                Frame = 0 + Frame % 8;
            }
            else if(!(mouse.X / width < mouse.Y / height) && (mouse.X / width + mouse.Y / height < 1))
            {
                Frame = 8 + Frame % 8;
            }
            else if(!(mouse.X / width < mouse.Y / height) && !(mouse.X / width + mouse.Y / height < 1))
            {
                Frame = 24 + Frame % 8;
            }
            
        }

        public void Shoot() 
        {
            Vector2 worldPos = ObjectManager.Instance.Camera.ScreenToWorld(new Vector2(InputManager.Instance.MState.X, InputManager.Instance.MState.Y));

            float shotX = this.X + (this.Hitbox.Width * .5f);
            float shotY = this.Y + (this.Hitbox.Height * .5f);

            float xDist = worldPos.X - shotX;
            float yDist = worldPos.Y - shotY;

            float magnitude = (float)Math.Sqrt(((xDist * xDist) + (yDist * yDist)));

            if (magnitude > 0)
            {
                xDist /= magnitude;
                yDist /= magnitude;
            }

            //add camera shake
            ObjectManager.Instance.CameraTransform += new Vector2(-xDist * 5, -yDist * 5);

            //add projectile to the list
            int boolMuliply = 0;
            if(RelativeSpeed)
            {
                boolMuliply = 1;
            }
            ObjectManager.Instance.Projectiles.Add(new Projectile((int)(shotX), (int)(shotY), xDist + (XTransform / ProjectileSpeed) * boolMuliply, yDist + (YTransform / ProjectileSpeed) * boolMuliply, ProjectileSpeed));
        }

        public void DropBomb()
        {
            ObjectManager.Instance.Bomb = new Bomb((int)(this.X), (int)(this.Y));
            Health -= AttackCost * 3;
        }

        public override void OnCollision(GameObject other)
        {
            if(other.Type == GameObjectType.Wall)
            {
                WallCollision();
            }

            if(other.Type == GameObjectType.EnemyProjectile)
            {
                health -= 10;
            }

            if(other.Type == GameObjectType.Enemy)
            {
                if (Math.Max(other.X - (this.X + this.Hitbox.Width), this.X - (other.X + other.Hitbox.Width)) > Math.Max(other.Y - (this.Y + this.Hitbox.Height), this.Y - (other.Y + other.Hitbox.Height)))
                {
                    other.XTransform = this.XTransform;
                }
                else
                {
                    other.YTransform = this.YTransform;
                }

                foreach (Enemy e in ObjectManager.Instance.Enemies)
                {
                    if (CollisionManager.Instance.CheckObjectCollision(other, e) && e != other)
                    {
                        e.XTransform = XTransform;
                        e.YTransform = YTransform;
                    }
                }

                foreach (Wall w in ObjectManager.Instance.Walls)
                {
                    if (CollisionManager.Instance.CheckObjectCollision(other, w))
                    {
                        Enemy temp = (Enemy)other;
                        temp.WallCollision();

                        float XIncrement = XTransform / speed;
                        float YIncrement = YTransform / speed;

                        XTransform = 0.0f;
                        YTransform = 0.0f;

                        //change x transform
                        if (!(XIncrement == 0))
                        {
                            XTransform = 0.0f;
                            do
                            {
                                XTransform += XIncrement;
                            } while (!CollisionManager.Instance.CheckObjectCollision(this, other) && Math.Abs(XTransform) < speed + Math.Abs(XIncrement));
                            XTransform -= XIncrement;
                        }

                        //change y transfrom
                        if (!(YIncrement == 0))
                        {
                            YTransform = 0.0f;
                            do
                            {
                                YTransform += YIncrement;
                            } while (!CollisionManager.Instance.CheckObjectCollision(this, other) && Math.Abs(YTransform) < speed + Math.Abs(YIncrement));
                            YTransform -= YIncrement;
                        }
                    }
                }
            }
        }

        public void WallCollision()
        {
            float XIncrement = XTransform / speed;
            float YIncrement = YTransform / speed;

            XTransform = 0.0f;
            YTransform = 0.0f;

            //change x transform
            if (!(XIncrement == 0))
            {
                XTransform = 0.0f;
                do
                {
                    XTransform += XIncrement;
                } while (!CollisionManager.Instance.BlockedByWall(this) && Math.Abs(XTransform) < speed + Math.Abs(XIncrement));
                XTransform -= XIncrement;
            }

            //change y transfrom
            if (!(YIncrement == 0))
            {
                YTransform = 0.0f;
                do
                {
                    YTransform += YIncrement;
                } while (!CollisionManager.Instance.BlockedByWall(this) && Math.Abs(YTransform) < speed + Math.Abs(YIncrement));
                YTransform -= YIncrement;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D gun = ContentManager.Instance.GunTexture;
            Texture2D gunBack = ContentManager.Instance.GunBackTexture;

            float mouseX = InputManager.Instance.MState.Position.X + ObjectManager.Instance.Camera.Position.X;
            float mouseY = InputManager.Instance.MState.Position.Y + ObjectManager.Instance.Camera.Position.Y;
            float rotation = (float) Math.Atan((mouseY - this.Y) / (mouseX - this.X));
           
            if(Frame/8 == 0)
            {
                XOffset = 35;
                YOffset = 10 + (this.Texture.Height / 8);
            }
            if (Frame / 8 == 2)
            {
                XOffset = 27;
                YOffset = 10 + (this.Texture.Height / 8);
            }
            if (Frame / 8 == 3)
            {
                XOffset = 35;
                YOffset = 10 + (this.Texture.Height / 8);
            }

            if (mouseX >= this.X && !(Frame / 8 == 3))
            {
                rotation += (float)(Math.PI);
            }

            if(Frame / 8 == 0 || Frame / 8 == 2)
            {
                spriteBatch.Draw(this.Texture, new Vector2((int)this.X, (int)this.Y),
                    new Rectangle((Frame % 4) * 19, (Frame / 4) * 28, (int)(this.Texture.Width / 4), (int)(this.Texture.Height / 8)),
                    Color.White, 0.0f, new Vector2(0, 0), new Vector2(scale * TextureScale, scale * TextureScale), SpriteEffects.None, 1.0f);

                if (Frame % 2 == 0)
                {
                    spriteBatch.Draw(gun, new Vector2((int)this.X + XOffset, (int)this.Y + YOffset), new Rectangle(0, 0, (gun.Width), (gun.Height)), Color.White, rotation, new Vector2(gun.Width * (((float)4) / 5), gun.Height * (((float)2) / 3)), new Vector2(scale * TextureScale, scale * TextureScale), SpriteEffects.None, 1.0f);

                }
                else
                {
                    spriteBatch.Draw(gun, new Vector2((int)this.X + XOffset, (int)this.Y + YOffset + 2), new Rectangle(0, 0, (gun.Width), (gun.Height)), Color.White, rotation, new Vector2(gun.Width * (((float)4) / 5), gun.Height * (((float)2) / 3)), new Vector2(scale * TextureScale, scale * TextureScale), SpriteEffects.None, 1.0f);

                }
            }
            else if(Frame / 8 == 3)
            {
                if (Frame % 2 == 0)
                {
                    spriteBatch.Draw(gunBack, new Vector2((int)this.X +XOffset, (int)this.Y + YOffset), new Rectangle(0, 0, (gun.Width), (gun.Height)), Color.White, rotation, new Vector2(gun.Width * (((float)1) / 5), gun.Height * (((float)2) / 3)), new Vector2(scale * TextureScale, scale * TextureScale), SpriteEffects.None, 1.0f);
                }
                else
                {
                    spriteBatch.Draw(gunBack, new Vector2((int)this.X + XOffset, (int)this.Y + YOffset + 2), new Rectangle(0, 0, (gun.Width), (gun.Height)), Color.White, rotation, new Vector2(gun.Width * (((float)1) / 5), gun.Height * (((float)2) / 3)), new Vector2(scale * TextureScale, scale * TextureScale), SpriteEffects.None, 1.0f);
                }
                spriteBatch.Draw(this.Texture, new Vector2((int)this.X, (int)this.Y),
                    new Rectangle((Frame % 4) * 19, (Frame / 4) * 28, (int)(this.Texture.Width / 4), (int)(this.Texture.Height / 8)),
                    Color.White, 0.0f, new Vector2(0, 0), new Vector2(scale * TextureScale, scale * TextureScale), SpriteEffects.None, 1.0f);

            }
            else
            {
                spriteBatch.Draw(this.Texture, new Vector2((int)this.X, (int)this.Y),
                    new Rectangle((Frame % 4) * 19, (Frame / 4) * 28, (int)(this.Texture.Width / 4), (int)(this.Texture.Height / 8)),
                    Color.White, 0.0f, new Vector2(0, 0), new Vector2(scale * TextureScale, scale * TextureScale), SpriteEffects.None, 1.0f);
            }


        }
    }
}
