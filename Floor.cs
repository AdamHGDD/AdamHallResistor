using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game1
{
    public class Floor:GameObject
    {
        //Graph related Properties
        public List<Floor> Neighbors { get; set; }
        public int G { get; set; }
        public float H { get; set; }
        public float F { get { return G + H; } }
        public Floor Path { get; set; }
        public int FloorType
        {
            get; set;
        }

        /// <summary>
        /// Provides coordinates for a floor texture
        /// </summary>
        /// <param name="x">x coord</param>
        /// <param name="y">y coord</param>
        public Floor(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.scale = 2f;
            Texture = ContentManager.Instance.FloorTexture;
            Neighbors = new List<Floor>();
        }

        public override void OnCollision(GameObject other)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, new Vector2((int)this.X, (int)this.Y),
               new Rectangle(FloorType * (this.Texture.Width / 3), 0, (int)(this.Texture.Width / 3), (int)this.Texture.Height),
               Color.White, 0.0f, new Vector2(0, 0), new Vector2(scale, scale), SpriteEffects.None, 1.0f);
        }
    }
}
