using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    class ObjectManager
    {
        //singleton

        private static ObjectManager instance;

        public Player Player { get; set; }
        public Bomb Bomb { get; set; }
        public Exit Exit { get; set; }
        public List<Wall> Walls { get; set; }
        public Wall[,] WallGraph { get; set; }
        public List<Projectile> Projectiles { get; set; }
        public List<Projectile> EnemyProjectiles { get; set; }
        public List<HealthPickUp> Pickups { get; set; }
        public List<SpareParts> Parts { get; set; }
        public List<Enemy> Enemies { get; set; }
        public Boss TheBoss { get; set; }
        public List<GameObject> Temp { get; set; }//A temporary list for objects that are read in before they are created
        public Floor[,] FloorGraph { get; set; }
        public List<Floor> Floors { get; set; }
        public Camera2D Camera { get; set; }
        public Vector2 CameraTransform { get; set; }
        public Random Rng { get; set; }
        //If the player has won
        public bool Win { get; set; }
        public bool GetSkills { get; set; }
        public SpareParts CurrentSpare { get; set; }

        private ObjectManager()
        {
            Walls = new List<Wall>();
            Projectiles = new List<Projectile>();
            EnemyProjectiles = new List<Projectile>();
            Enemies = new List<Enemy>();
            Pickups = new List<HealthPickUp>();
            Parts = new List<SpareParts>();
            Floors = new List<Floor>();
            Temp = new List<GameObject>();
            Win = false;
            GetSkills = false;
        }

        public static ObjectManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ObjectManager();
                }
                return instance;
            }
        }

        public void Reset()
        {
            Walls = new List<Wall>();
            Projectiles = new List<Projectile>();
            Enemies = new List<Enemy>();
            Pickups = new List<HealthPickUp>();
            Floors = new List<Floor>();
            Bomb = null;
        }
    }
}

