using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Game1
{
    class ContentManager
    {
        // another singleton manager. I've been making a lot of these. Maybe I need a managerManager

        private static ContentManager instance;
        
        public Texture2D ProjectileTexture { get; set; }
        public Texture2D EnemyProjectileTexture { get; set; }

        public Texture2D GenericTexture { get; set; }
        public Texture2D WallTexture { get; set; }
        public Texture2D FloorTexture { get; set; }

        public Texture2D WelderTexture { get; set; }
        public Texture2D WelderAttackTexture { get; set; }

        public Texture2D MrRobotoTexture { get; set; }
        public Texture2D MrRobotoZapModeTexture { get; set; }
        public Texture2D MrRobotoZapAttackTexture { get; set; }

        public Texture2D BomberTexture { get; set; }
        public Texture2D BomberAttackTexture { get; set; }

        public Texture2D SecurityBotTexture { get; set; }

        public Texture2D BombTexture { get; set; }
        public Texture2D BatteryTexture { get; set; }
        public Texture2D GunTexture { get; set; }
        public Texture2D GunBackTexture { get; set; }
        public Texture2D EnergyBar { get; set; }

        public Texture2D TitleScreen { get; set; }
        public Texture2D Space { get; set; }
        public Texture2D UpgradeScreen { get; set; }

        public SpriteFont DefaultFont { get; set; }
        public SpriteFont ButtonFont { get; set; }

        public SoundEffect GunNoise { get; set; }
        public SoundEffect ExplosionNoise { get; set; }
        public SoundEffect BombPlantNoise { get; set; }

        private ContentManager() { }

        public static ContentManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContentManager();
                }
                return instance;
            }
        }
    }
}
