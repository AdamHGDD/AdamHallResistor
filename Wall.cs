using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class Wall : GameObject
    {
        //Property for just walls
        public int WallType
        {
            get; set;
        }
        public Floor Neighbors { get; set; }

        public Wall(int x, int y, int w, int h)
        {
            this.X = x;
            this.Y = y;
            scale = 2f;
            Type = GameObjectType.Wall;
            Hitbox = new Rectangle(x, y, w, h);
            Texture = ContentManager.Instance.WallTexture;
            WallType = 0;
        }

        public override void OnCollision(GameObject other)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, new Vector2((int)this.X, (int)this.Y), 
                new Rectangle((WallType % 4)*32, (WallType / 4)*32, (int)(this.Texture.Width/4), (int)(this.Texture.Height/4)),
                Color.White, 0.0f, new Vector2(0, 0), new Vector2(scale, scale), SpriteEffects.None, 1.0f);
        }
    }
}
