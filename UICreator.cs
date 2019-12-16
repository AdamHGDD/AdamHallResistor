using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class UICreator
    {

        public static void DrawCrosshair(SpriteBatch sb, int crossWidth, int crossLength, Color paint)
        {
            int x = InputManager.Instance.MState.X;
            int y = InputManager.Instance.MState.Y;

            int extraX = (int)ObjectManager.Instance.Camera.BoundingRectangle.X;
            int extraY = (int)ObjectManager.Instance.Camera.BoundingRectangle.Y;

            sb.Draw(ContentManager.Instance.GenericTexture, new Rectangle((x - crossWidth / 2) + extraX,(y - crossWidth / 2 - crossLength) + extraY, crossWidth, crossLength), paint);
            sb.Draw(ContentManager.Instance.GenericTexture, new Rectangle((x + crossWidth / 2) + extraX,(y - crossWidth / 2) + extraY, crossLength, crossWidth), paint);
            sb.Draw(ContentManager.Instance.GenericTexture, new Rectangle((x - crossWidth / 2) + extraX,(y + crossWidth / 2) + extraY, crossWidth, crossLength), paint);
            sb.Draw(ContentManager.Instance.GenericTexture, new Rectangle((x - crossWidth / 2 - crossLength) + extraX,(y - crossWidth / 2) + extraY, crossLength, crossWidth), paint);
        }

        public static void DrawHealth(SpriteBatch sb, int y, Color paint)
        {
            float screenWidth = InputManager.Instance.Screen.Viewport.Width;
            float screenHeight = InputManager.Instance.Screen.Viewport.Height;

            int x = (int) (screenWidth / 2 - (int)(1287/2 * (screenWidth * 1f / 1920)));

            float playerHP = ObjectManager.Instance.Player.Health;
            float maxHp = ObjectManager.Instance.Player.MaxHealth;

            int extraX = (int)ObjectManager.Instance.Camera.BoundingRectangle.X;
            int extraY = (int)ObjectManager.Instance.Camera.BoundingRectangle.Y;

            SpriteFont font = ContentManager.Instance.DefaultFont;

            sb.Draw(ContentManager.Instance.GenericTexture, new Rectangle(x + extraX, (int)(y * (screenHeight / 1080) + extraY), (int)((1287) * (screenWidth * 1f / 1920)), (int)(32 * (screenHeight * 1f / 1080))), Color.Black);
            sb.Draw(ContentManager.Instance.GenericTexture, new Rectangle(x + extraX,(int) (y * (screenHeight/1080) + extraY), (int) ((playerHP/maxHp * 1287 ) * (screenWidth * 1f / 1920)) , (int)(32 * (screenHeight * 1f / 1080))), paint);
            sb.Draw(ContentManager.Instance.EnergyBar, new Rectangle((int) (x + extraX - 6 * (screenHeight / 1080)), (int)(y * (screenHeight / 1080) + extraY - 6 * (screenHeight / 1080)), (int)((1300) * (screenWidth * 1f / 1920)), (int)(44 * (screenHeight * 1f / 1080))), Color.White);

            sb.DrawString(font, "" + playerHP + " / " + maxHp, new Vector2(x + extraX + ((1287) * .5f * (screenWidth * 1f / 1920)) - (.5f * font.MeasureString("" + playerHP + " / " + maxHp).X), (int)(y * (screenHeight / 1080) + extraY) + (32 * .5f * (screenHeight * 1f / 1080)) - (.5f * font.MeasureString("" + playerHP + " / " + maxHp).Y)), Color.White);

        }
    }
}
