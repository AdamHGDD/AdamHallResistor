using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game1
{
    public class Projectile : GameObject
    {
        private int timeTracker;
        public int Timer { get; set; }

        public Projectile(float xPos, float yPos, float xTrans, float yTrans, float projectileSpeed)
        {
            scale = .25f;
            Hitbox = new Rectangle((int)xPos, (int)yPos, (int)(64 * scale), (int)(64 * scale));
            this.X = xPos;
            this.Y = yPos;
            XTransform = xTrans * projectileSpeed;
            YTransform = yTrans * projectileSpeed;
            Type = GameObjectType.Projectile;
            Texture = ContentManager.Instance.ProjectileTexture;
            Timer = int.MaxValue;
            timeTracker = 0;
        }

        public Projectile(float xPos, float yPos, float xTrans, float yTrans, float projectileSpeed, int burnout)
        {
            scale = .25f;
            Hitbox = new Rectangle((int)xPos, (int)yPos, (int)(64 * scale), (int)(64 * scale));
            this.X = xPos;
            this.Y = yPos;
            XTransform = xTrans * projectileSpeed;
            YTransform = yTrans * projectileSpeed;
            Type = GameObjectType.Projectile;
            Texture = ContentManager.Instance.ProjectileTexture;
            Timer = burnout;
            timeTracker = 0;
        }

        public override void Update()
        {
            // TODO: add a burnout timer to delete the projectile after a given time/distance
            timeTracker++;
            if(timeTracker >= Timer)
            {
                if (this.Type == GameObjectType.Projectile)
                    ObjectManager.Instance.Projectiles.Remove(this);
                else if (this.Type == GameObjectType.EnemyProjectile)
                    ObjectManager.Instance.EnemyProjectiles.Remove(this);
            }
        }

        public override void OnCollision(GameObject other)
        {
            if (this.Type == GameObjectType.Projectile)
                ObjectManager.Instance.Projectiles.Remove(this);
            else if (this.Type == GameObjectType.EnemyProjectile)
                ObjectManager.Instance.EnemyProjectiles.Remove(this);
        }
    }
}
