using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MonoGame.Extended;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Game1
{
    //Tiles
    enum TileType { Space, Wall, Floor }
    enum ObjectType { Spawn, Exit, Welder, Pickup, SpareParts, SecurityBot, Bomber, Boss }
    /// <summary>
    /// 
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState gameState;
        MenuCreator menu;


        // fields for future use
        int screenWidth;
        int screenHeight;

        //Level Array
        TileType[,] levelMap;
        int mapWidth;
        int mapHeight;

        bool testing;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // instantiate Camera2D object
            ObjectManager.Instance.Camera = new Camera2D(GraphicsDevice);

            graphics.IsFullScreen = true;
            screenWidth = 1920;
            screenHeight = 1080;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            ObjectManager.Instance.Rng = new Random();

            gameState = GameState.Start;

            IsMouseVisible = false;

            FutureLevels.LevelNum = 2;


            #region Read Level
            //Read in Level
            FileStream Level = File.OpenRead("./Content/Level.lvl");
            BinaryReader ReadLevel = new BinaryReader(Level);
            try
            {
                //Get the width and height of the map
                mapWidth = ReadLevel.ReadInt32();
                FutureLevels.MapWidth = mapWidth;
                mapHeight = ReadLevel.ReadInt32();
                FutureLevels.MapHeight = mapHeight;
                levelMap = new TileType[mapWidth, mapHeight];
                ObjectManager.Instance.FloorGraph = new Floor[mapWidth, mapHeight];
                ObjectManager.Instance.WallGraph = new Wall[mapWidth, mapHeight];
                //Read each tile into the array
                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        levelMap[x, y] = (TileType)ReadLevel.ReadInt32();
                    }
                }
                //Read game objects
                int numOfObjects = ReadLevel.ReadInt32();
                for (int i = 0; i < numOfObjects; i++)
                {
                    int x = ReadLevel.ReadInt32();
                    int y = ReadLevel.ReadInt32();
                    ObjectType type = (ObjectType)ReadLevel.ReadInt32();
                    if (type == ObjectType.Welder)
                    {
                        ObjectManager.Instance.Temp.Add(new Welder((float)(x * 64), (float)(y * 64)));
                    }
                    else if (type == ObjectType.SecurityBot)
                    {
                        ObjectManager.Instance.Temp.Add(new SecurityBot((float)(x * 64), (float)(y * 64)));
                    }
                    else if (type == ObjectType.Bomber)
                    {
                        ObjectManager.Instance.Temp.Add(new Bomber((float)(x * 64), (float)(y * 64), null));
                    }
                    else if (type == ObjectType.Pickup)
                    {
                        ObjectManager.Instance.Temp.Add(new HealthPickUp((float)(x * 64), (float)(y * 64)));
                    }
                    else if(type == ObjectType.SpareParts)
                    {
                        ObjectManager.Instance.Temp.Add(new SpareParts((float)(x * 64), (float)(y * 64)));
                    }
                    else if (type == ObjectType.Boss)
                    {
                        ObjectManager.Instance.Temp.Add(new Boss((float)(x * 64), (float)(y * 64), null));
                    }
                    else if (type == ObjectType.Spawn)
                    {
                        ObjectManager.Instance.Player = new Player(x * 64, y * 64);
                    }
                    else if (type == ObjectType.Exit)
                    {
                        ObjectManager.Instance.Exit = new Exit(x * 64, y * 64);
                    }
                }
                menu = new MenuCreator(GraphicsDevice, 5, .8f, .8f, 15);

            }
            catch
            {
                Console.WriteLine("Failed to Load Level.");
            }
            finally
            {
                ReadLevel.Close();
                Level.Close();
            }
            #endregion

            #region Skill Nodes
            //Instanciate Skill nodes
            Skill DamageSkill = new Skill(statTypes.damage, (float)(0.5));
            Skill EnergySkill = new Skill(statTypes.energy, (float)(0.9));
            Skill CostSkill = new Skill(statTypes.cost, (float)(0.9));
            Skill CooldownSkill = new Skill(statTypes.rate, (float)(1.1));
            #endregion

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO implement file reading to load in the gameobjects
            // TODO: use this.Content to load your game content here
            ObjectManager.Instance.Player.Texture = Content.Load<Texture2D>("PlayerSheet");
            ObjectManager.Instance.Player.Frame = 1;
            ObjectManager.Instance.Player.Moving = false;
            ObjectManager.Instance.Player.SecondsPerFrame = (double)1 / 10;

            ObjectManager.Instance.Exit.Texture = Content.Load<Texture2D>("Projectile_Placeholder");
            ObjectManager.Instance.Exit.SetHitBox();

            ContentManager.Instance.WallTexture = Content.Load<Texture2D>("Walls");
            ContentManager.Instance.GenericTexture = Content.Load<Texture2D>("Wall_Placeholder");

            ContentManager.Instance.ProjectileTexture = Content.Load<Texture2D>("Projectile_Placeholder");
            ContentManager.Instance.EnemyProjectileTexture = Content.Load<Texture2D>("Enemy_Projectile_Placeholder");
            ContentManager.Instance.WelderTexture = Content.Load<Texture2D>("Welder");
            ContentManager.Instance.WelderAttackTexture = Content.Load<Texture2D>("Welder_Attack_Placeholder");

            ContentManager.Instance.MrRobotoTexture = Content.Load<Texture2D>("Boss_Placeholder");
            ContentManager.Instance.MrRobotoZapModeTexture = Content.Load<Texture2D>("Boss_Zapmode_Placeholder");
            ContentManager.Instance.MrRobotoZapAttackTexture = Content.Load<Texture2D>("Boss_Zapattack_Placeholder");
            ContentManager.Instance.BomberTexture = Content.Load<Texture2D>("Bomber_Sprite");
            ContentManager.Instance.BomberAttackTexture = Content.Load<Texture2D>("Explosion");
            ContentManager.Instance.WelderAttackTexture = Content.Load<Texture2D>("Welder_Attack_Placeholder");
            ContentManager.Instance.EnergyBar = Content.Load<Texture2D>("EnergyBarOutline");
            ContentManager.Instance.SecurityBotTexture = Content.Load<Texture2D>("Security_Bot");
            ContentManager.Instance.GunTexture = Content.Load<Texture2D>("Gun");
            ContentManager.Instance.GunBackTexture = Content.Load<Texture2D>("GunBack");

            ContentManager.Instance.DefaultFont = Content.Load<SpriteFont>("Default_Font");
            if (screenWidth <= 960 && screenWidth > 480)
            {
                ContentManager.Instance.ButtonFont = Content.Load<SpriteFont>("ButtonFontHalf");
            }
            else if (screenWidth <= 480)
            {
                ContentManager.Instance.ButtonFont = Content.Load<SpriteFont>("ButtonFont14");
            }
            else if (screenWidth <= 1440 && screenWidth > 960)
            {
                ContentManager.Instance.ButtonFont = Content.Load<SpriteFont>("ButtonFont34");
            }
            else
            {
                ContentManager.Instance.ButtonFont = Content.Load<SpriteFont>("ButtonFont");
            }

            ContentManager.Instance.TitleScreen = Content.Load<Texture2D>("TitleScreen");
            ContentManager.Instance.Space = Content.Load<Texture2D>("Space");
            ContentManager.Instance.UpgradeScreen = Content.Load<Texture2D>("UpgradeScreen");

            ContentManager.Instance.GunNoise = Content.Load<SoundEffect>("Pew");
            ContentManager.Instance.ExplosionNoise = Content.Load<SoundEffect>("PshBoom");
            ContentManager.Instance.BombPlantNoise = Content.Load<SoundEffect>("BombPlant");

            //make a new texture for these. Only using the projectile teporaraly
            ContentManager.Instance.BombTexture = Content.Load<Texture2D>("Projectile_Placeholder");
            ContentManager.Instance.BatteryTexture = Content.Load<Texture2D>("Projectile_Placeholder");
            ContentManager.Instance.FloorTexture = Content.Load<Texture2D>("Floors");

            //Adding textures to enemies and pickups
            foreach (Enemy e in ObjectManager.Instance.Enemies)
            {
                e.Texture = ContentManager.Instance.WelderTexture;//We'll need ifs in here if we add multiple enemey types
            }
            foreach (HealthPickUp p in ObjectManager.Instance.Pickups)
            {
                p.Texture = ContentManager.Instance.BatteryTexture;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Pass input states to the input manager
            InputManager.Instance.KbStatePrev = InputManager.Instance.KbState;
            InputManager.Instance.KbState = Keyboard.GetState();
            InputManager.Instance.MStatePrev = InputManager.Instance.MState;
            InputManager.Instance.MState = Mouse.GetState();
            InputManager.Instance.Time = gameTime;
            InputManager.Instance.Screen = GraphicsDevice;

            //Switch statement for GameState transitions
            #region Transitions
            switch (gameState)
            {
                case GameState.Start:
                    {
                        if ((InputManager.Instance.JustPressedMouse() && menu.StartButtons[(int)StartButtons.Play].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.StartButtons[(int)StartButtons.Play].MoveHover))
                        {
                            ObjectManager.Instance.Reset();
                            ObjectManager.Instance.Player.Health = ObjectManager.Instance.Player.MaxHealth;

                            gameState = GameState.Play;

                            for (int x = 0; x < FutureLevels.MapWidth; x++)
                            {
                                for (int y = 0; y < FutureLevels.MapHeight; y++)
                                {
                                    if (levelMap[x, y] == TileType.Floor)
                                    {
                                        Floor tile = new Floor(x * 64, y * 64);
                                        if(x % 2 == 1 && y % 2 == 1)
                                        {
                                            tile.FloorType = 1;
                                        }
                                        if (x % 5 == 0 && y % 5 == 0)
                                        {
                                            tile.FloorType = 2;
                                        }
                                        ObjectManager.Instance.Floors.Add(tile);
                                        ObjectManager.Instance.FloorGraph[x, y] = (tile);
                                    }
                                    else if (levelMap[x, y] == TileType.Wall)
                                    {
                                        Wall temp = (new Wall(x * 64, y * 64, 64, 64));
                                        //Wall types
                                        if (y > 0 && levelMap[x, y - 1] == TileType.Wall)
                                        {
                                            temp.WallType++;
                                        }
                                        if (x > 0 && levelMap[x - 1, y] == TileType.Wall)
                                        {
                                            temp.WallType += 2;
                                        }
                                        if (x < FutureLevels.MapWidth && levelMap[x + 1, y] == TileType.Wall)
                                        {
                                            temp.WallType += 4;
                                        }
                                        if (y < FutureLevels.MapHeight && levelMap[x, y + 1] == TileType.Wall)
                                        {
                                            temp.WallType += 8;
                                        }
                                        else
                                        {

                                            temp.Hitbox = new Rectangle(temp.Hitbox.X, temp.Hitbox.Y, temp.Hitbox.Width, temp.Hitbox.Height * 2 / 3);
                                        }
                                        ObjectManager.Instance.Walls.Add(temp);
                                        ObjectManager.Instance.WallGraph[x, y] = temp;
                                    }

                                }
                            }
                            //assign neighbor tiles
                            foreach (Floor f in ObjectManager.Instance.Floors)
                            {
                                int x = (int)f.X / 64;
                                int y = (int)f.Y / 64;

                                if (f.X - 1 >= 0 && levelMap[x - 1, y] == TileType.Floor) // left
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x - 1, y]);
                                }
                                if (x + 1 < FutureLevels.MapWidth && levelMap[x + 1, y] == TileType.Floor) // right
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x + 1, y]);
                                }
                                if (y - 1 >= 0 && levelMap[x, y - 1] == TileType.Floor) // up
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x, y - 1]);
                                }
                                if (y + 1 < FutureLevels.MapHeight && levelMap[x, y + 1] == TileType.Floor) // down
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x, y + 1]);
                                }
                            }
                            foreach (Wall w in ObjectManager.Instance.Walls)
                            {
                                int x = (int)w.X / 64;
                                int y = (int)w.Y / 64;

                                if (w.X - 1 >= 0 && levelMap[x - 1, y] == TileType.Floor) // left
                                {
                                    w.Neighbors = ObjectManager.Instance.FloorGraph[x - 1, y];
                                }
                                if (x + 1 < FutureLevels.MapWidth && levelMap[x + 1, y] == TileType.Floor) // right
                                {
                                    w.Neighbors = ObjectManager.Instance.FloorGraph[x + 1, y];
                                }
                                if (y - 1 >= 0 && levelMap[x, y - 1] == TileType.Floor) // up
                                {
                                    w.Neighbors = ObjectManager.Instance.FloorGraph[x, y - 1];
                                }
                                if (y + 1 < FutureLevels.MapHeight && levelMap[x, y + 1] == TileType.Floor) // down
                                {
                                    w.Neighbors = ObjectManager.Instance.FloorGraph[x, y + 1];
                                }
                            }
                            //Spawn enemies and pickups
                            foreach (GameObject o in ObjectManager.Instance.Temp)
                            {
                                if (o.Type == GameObjectType.Enemy)
                                {
                                    if (o is Welder)
                                        ObjectManager.Instance.Enemies.Add(new Welder(o.X, o.Y));//Will need to implement more ifs and enemy types if we add them
                                    else if (o is SecurityBot)
                                        ObjectManager.Instance.Enemies.Add(new SecurityBot(o.X, o.Y));
                                    else if (o is Bomber)
                                        ObjectManager.Instance.Enemies.Add(new Bomber(o.X, o.Y, gameTime));
                                    else if (o is Boss)
                                        ObjectManager.Instance.TheBoss = new Boss(o.X, o.Y, gameTime);
                                }
                                else if (o.Type == GameObjectType.Pickup)
                                {
                                    ObjectManager.Instance.Pickups.Add(new HealthPickUp(o.X, o.Y));
                                }
                                else if(o.Type == GameObjectType.SpareParts)
                                {
                                    ObjectManager.Instance.Parts.Add(new SpareParts(o.X, o.Y));
                                }
                                
                            }
                            //ObjectManager.Instance.Parts.Add(new SpareParts(512, 512));
                            //ObjectManager.Instance.Enemies.Add(new SecurityBot(300, 800));
                            //ObjectManager.Instance.TheBoss = new Boss(300, 20, 2000, 2752, 3200, gameTime);
                            //ObjectManager.Instance.Enemies.Add(new Bomber(70, 40, 0, ObjectManager.Instance.Player.X, ObjectManager.Instance.Player.Y - 200, gameTime));
                        }
                        if (InputManager.Instance.JustPressedButton(Keys.Escape) ||
                            (InputManager.Instance.JustPressedMouse() && menu.StartButtons[(int)StartButtons.Exit].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.StartButtons[(int)StartButtons.Exit].MoveHover))
                        {
                            Exit();
                        }
                        break;
                    }
                case GameState.Pause:
                    {
                        if ((InputManager.Instance.JustPressedMouse() && menu.PauseButtons[(int)PauseButtons.Menu].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.PauseButtons[(int)PauseButtons.Menu].MoveHover))
                        {
                            gameState = GameState.Start;
                            ObjectManager.Instance.Player.X = ObjectManager.Instance.Player.StartX;
                            ObjectManager.Instance.Player.Y = ObjectManager.Instance.Player.StartY;
                            ObjectManager.Instance.Player.Health = ObjectManager.Instance.Player.MaxHealth;
                            menu.ResetHover();
                        }
                        if (InputManager.Instance.JustPressedButton(Keys.Escape) ||
                            (InputManager.Instance.JustPressedMouse() && menu.PauseButtons[(int)PauseButtons.Resume].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.PauseButtons[(int)PauseButtons.Resume].MoveHover))
                        {
                            gameState = GameState.Play;
                        }
                        if ((InputManager.Instance.JustPressedMouse() && menu.PauseButtons[(int)PauseButtons.Exit].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.PauseButtons[(int)PauseButtons.Exit].MoveHover))
                        {
                            Exit();
                        }
                        break;
                    }
                case GameState.Death:
                    {
                        if (InputManager.Instance.JustPressedButton(Keys.Escape) ||
                            (InputManager.Instance.JustPressedMouse() && menu.DeathButtons[(int)DeathButtons.Menu].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.DeathButtons[(int)DeathButtons.Menu].MoveHover))
                        {

                            gameState = GameState.Start;
                            menu.ResetHover();

                            ObjectManager.Instance.Player.X = ObjectManager.Instance.Player.StartX;
                            ObjectManager.Instance.Player.Y = ObjectManager.Instance.Player.StartY;
                            ObjectManager.Instance.Player.Health = ObjectManager.Instance.Player.MaxHealth;

                            foreach (Enemy e in ObjectManager.Instance.Enemies)
                            {
                                e.Health = e.MaxHealth;
                                e.X = e.StartX;
                                e.Y = e.StartY;
                            }
                        }
                        if ((InputManager.Instance.JustPressedMouse() && menu.DeathButtons[(int)DeathButtons.Restart].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.DeathButtons[(int)DeathButtons.Restart].MoveHover))
                        {

                            gameState = GameState.Play;

                            ObjectManager.Instance.Player.X = ObjectManager.Instance.Player.StartX;
                            ObjectManager.Instance.Player.Y = ObjectManager.Instance.Player.StartY;
                            ObjectManager.Instance.Player.Health = ObjectManager.Instance.Player.MaxHealth;

                            foreach (Enemy e in ObjectManager.Instance.Enemies)
                            {
                                e.Health = e.MaxHealth;
                                e.X = e.StartX;
                                e.Y = e.StartY;
                            }

                            ObjectManager.Instance.Reset();
                            ObjectManager.Instance.Player.Health = ObjectManager.Instance.Player.MaxHealth;

                            for (int x = 0; x < FutureLevels.MapWidth; x++)
                            {
                                for (int y = 0; y < FutureLevels.MapHeight; y++)
                                {
                                    if (levelMap[x, y] == TileType.Floor)
                                    {
                                        Floor tile = new Floor(x * 64, y * 64);
                                        if(x % 2 == 1 && y % 2 == 1)
                                        {
                                            tile.FloorType = 1;
                                        }
                                        if (x % 5 == 0 && y % 5 == 0)
                                        {
                                            tile.FloorType = 2;
                                        }
                                        ObjectManager.Instance.Floors.Add(tile);
                                        ObjectManager.Instance.FloorGraph[x, y] = (tile);
                                    }
                                    else if (levelMap[x, y] == TileType.Wall)
                                    {
                                        Wall temp = (new Wall(x * 64, y * 64, 64, 64));
                                        //Wall types
                                        if (y > 0 && levelMap[x, y - 1] == TileType.Wall)
                                        {
                                            temp.WallType++;
                                        }
                                        if (x > 0 && levelMap[x - 1, y] == TileType.Wall)
                                        {
                                            temp.WallType += 2;
                                        }
                                        if (y < FutureLevels.MapHeight && levelMap[x, y + 1] == TileType.Wall)
                                        {
                                            temp.WallType += 8;
                                        }
                                        if (x < FutureLevels.MapWidth && levelMap[x + 1, y] == TileType.Wall)
                                        {
                                            temp.WallType += 4;
                                        }
                                        ObjectManager.Instance.Walls.Add(temp);
                                    }

                                }
                            }
                            //assign neighbor tiles
                            foreach (Floor f in ObjectManager.Instance.Floors)
                            {
                                int x = (int)f.X / 64;
                                int y = (int)f.Y / 64;

                                if (f.X - 1 >= 0 && levelMap[x - 1, y] == TileType.Floor) // left
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x - 1, y]);
                                }
                                if (x + 1 < FutureLevels.MapWidth && levelMap[x + 1, y] == TileType.Floor) // right
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x + 1, y]);
                                }
                                if (y - 1 >= 0 && levelMap[x, y - 1] == TileType.Floor) // up
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x, y - 1]);
                                }
                                if (y + 1 < FutureLevels.MapHeight && levelMap[x, y + 1] == TileType.Floor) // down
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x, y + 1]);
                                }
                            }
                                //Spawn enemies and pickups
                                foreach (GameObject o in ObjectManager.Instance.Temp)
                            {
                                if (o.Type == GameObjectType.Enemy)
                                {
                                    if (o is Welder)
                                        ObjectManager.Instance.Enemies.Add(new Welder(o.X, o.Y));//Will need to implement more ifs and enemy types if we add them
                                    else if (o is SecurityBot)
                                        ObjectManager.Instance.Enemies.Add(new SecurityBot(o.X, o.Y));
                                    else if (o is Bomber)
                                        ObjectManager.Instance.Enemies.Add(new Bomber(o.X, o.Y, gameTime));
                                    else if (o is Boss)
                                        ObjectManager.Instance.TheBoss = new Boss(o.X, o.Y, gameTime);
                                }
                                else if (o.Type == GameObjectType.Pickup)
                                {
                                    ObjectManager.Instance.Pickups.Add(new HealthPickUp(o.X, o.Y));
                                }
                            }


                        }
                        if ((InputManager.Instance.JustPressedMouse() && menu.DeathButtons[(int)DeathButtons.Exit].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter) && menu.DeathButtons[(int)DeathButtons.Exit].MoveHover))
                        {
                            Exit();
                        }
                        break;
                    }
                case GameState.Play:
                    {
                        if (ObjectManager.Instance.Player.Health < 1)
                        {
                            gameState = GameState.Death;
                            menu.ResetHover();
                        }
                        if (InputManager.Instance.JustPressedButton(Keys.Escape))
                        {
                            gameState = GameState.Pause;
                            menu.ResetHover();
                        }
                        if (ObjectManager.Instance.Win)
                        {
                            gameState = GameState.Exit;
                            levelMap = FutureLevels.CreateNextLevel();
                            menu.ResetHover();
                            ObjectManager.Instance.Win = false;
                        }
                        if (ObjectManager.Instance.GetSkills)
                        {
                            menu.Deleted = ObjectManager.Instance.CurrentSpare;
                            gameState = GameState.Upgrade;
                        }
                        break;
                    }
                case GameState.Exit:
                    {
                        if (InputManager.Instance.JustPressedButton(Keys.Escape) ||
                            (InputManager.Instance.JustPressedMouse() && menu.ExitButtons[(int)ExitButtons.Menu].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter) && menu.ExitButtons[(int)ExitButtons.Menu].MoveHover))
                        {
                            gameState = GameState.Start;
                            menu.ResetHover();

                            ObjectManager.Instance.Player.X = ObjectManager.Instance.Player.StartX;
                            ObjectManager.Instance.Player.Y = ObjectManager.Instance.Player.StartY;
                            ObjectManager.Instance.Player.Health = ObjectManager.Instance.Player.MaxHealth;

                            foreach (Enemy e in ObjectManager.Instance.Enemies)
                            {
                                e.Health = e.MaxHealth;
                                e.X = e.StartX;
                                e.Y = e.StartY;
                            }
                        }
                        if ((InputManager.Instance.JustPressedMouse() && menu.ExitButtons[(int)ExitButtons.Restart].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.ExitButtons[(int)ExitButtons.Restart].MoveHover))
                        {

                            gameState = GameState.Play;

                            ObjectManager.Instance.Player.X = ObjectManager.Instance.Player.StartX;
                            ObjectManager.Instance.Player.Y = ObjectManager.Instance.Player.StartY;
                            ObjectManager.Instance.Player.Health = ObjectManager.Instance.Player.MaxHealth;

                            foreach (Enemy e in ObjectManager.Instance.Enemies)
                            {
                                e.Health = e.MaxHealth;
                                e.X = e.StartX;
                                e.Y = e.StartY;
                            }

                            ObjectManager.Instance.Reset();
                            ObjectManager.Instance.Player.Health = ObjectManager.Instance.Player.MaxHealth;

                            for (int x = 0; x < FutureLevels.MapWidth; x++)
                            {
                                for (int y = 0; y < FutureLevels.MapHeight; y++)
                                {
                                    if (levelMap[x, y] == TileType.Floor)
                                    {
                                        Floor tile = new Floor(x * 64, y * 64);
                                        if (x % 2 == 1 && y % 2 == 1)
                                        {
                                            tile.FloorType = 1;
                                        }
                                        if (x % 5 == 0 && y % 5 == 0)
                                        {
                                            tile.FloorType = 2;
                                        }
                                        ObjectManager.Instance.Floors.Add(tile);
                                        ObjectManager.Instance.FloorGraph[x, y] = (tile);
                                    }
                                    else if (levelMap[x, y] == TileType.Wall)
                                    {
                                        Wall temp = (new Wall(x * 64, y * 64, 64, 64));
                                        //Wall types
                                        if (y > 0 && levelMap[x, y - 1] == TileType.Wall)
                                        {
                                            temp.WallType++;
                                        }
                                        if (x > 0 && levelMap[x - 1, y] == TileType.Wall)
                                        {
                                            temp.WallType += 2;
                                        }
                                        if (y < FutureLevels.MapHeight && levelMap[x, y + 1] == TileType.Wall)
                                        {
                                            temp.WallType += 8;
                                        }
                                        if (x < FutureLevels.MapWidth && levelMap[x + 1, y] == TileType.Wall)
                                        {
                                            temp.WallType += 4;
                                        }
                                        ObjectManager.Instance.Walls.Add(temp);
                                    }

                                }
                            }
                            //assign neighbor tiles
                            foreach (Floor f in ObjectManager.Instance.Floors)
                            {
                                int x = (int)f.X / 64;
                                int y = (int)f.Y / 64;

                                if (f.X - 1 >= 0 && levelMap[x - 1, y] == TileType.Floor) // left
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x - 1, y]);
                                }
                                if (x + 1 < FutureLevels.MapWidth && levelMap[x + 1, y] == TileType.Floor) // right
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x + 1, y]);
                                }
                                if (y - 1 >= 0 && levelMap[x, y - 1] == TileType.Floor) // up
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x, y - 1]);
                                }
                                if (y + 1 < FutureLevels.MapHeight && levelMap[x, y + 1] == TileType.Floor) // down
                                {
                                    f.Neighbors.Add(ObjectManager.Instance.FloorGraph[x, y + 1]);
                                }
                            }
                            //Spawn enemies and pickups
                            foreach (GameObject o in ObjectManager.Instance.Temp)
                            {
                                if (o.Type == GameObjectType.Enemy)
                                {
                                    if (o is Welder)
                                        ObjectManager.Instance.Enemies.Add(new Welder(o.X, o.Y));//Will need to implement more ifs and enemy types if we add them
                                    else if (o is SecurityBot)
                                        ObjectManager.Instance.Enemies.Add(new SecurityBot(o.X, o.Y));
                                    else if (o is Bomber)
                                        ObjectManager.Instance.Enemies.Add(new Bomber(o.X, o.Y, gameTime));
                                    else if (o is Boss)
                                        ObjectManager.Instance.TheBoss = new Boss(o.X, o.Y, gameTime);
                                }
                                else if (o.Type == GameObjectType.Pickup)
                                {
                                    ObjectManager.Instance.Pickups.Add(new HealthPickUp(o.X, o.Y));
                                }
                            }

                        }
                        if ((InputManager.Instance.JustPressedMouse() && menu.ExitButtons[(int)ExitButtons.Exit].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter)
                            && menu.ExitButtons[(int)ExitButtons.Exit].MoveHover))
                        {
                            Exit();
                        }
                        break;
                    }
                case GameState.Upgrade:
                    {
                        if (InputManager.Instance.JustPressedButton(Keys.Escape) ||
                            (InputManager.Instance.JustPressedMouse() && menu.UpgradeButtons[(int)UpgradeButtonsEnum.Resume].MouseHover) ||
                            ((InputManager.Instance.JustPressedButton(Keys.Enter)) && menu.UpgradeButtons[(int)UpgradeButtonsEnum.Resume].MoveHover) || MenuCreator.UpgradeFinished)
                        {
                            gameState = GameState.Play;
                            MenuCreator.UpgradeFinished = false;
                        }
                        break;
                    }
            }
            #endregion

            //Second switch statement for updating things within the certain GameState
            #region Updating
            switch (gameState)
            {
                case GameState.Start:
                    {
                        menu.UpdateStartMenu(GraphicsDevice, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Pause:
                    {
                        menu.UpdatePauseMenu(GraphicsDevice, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Death:
                    {
                        menu.UpdateDeathMenu(GraphicsDevice, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Exit:
                    {
                        menu.UpdateExitMenu(GraphicsDevice, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Upgrade:
                    {
                        menu.UpdateUpgradeMenu(GraphicsDevice, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Play:
                    {
                        // TODO: Add your update logic here
                        // go through and update positions according to current trajectories and player inputs
                        ObjectManager.Instance.Player.Update();
                        for(int i = 0; i < ObjectManager.Instance.Projectiles.Count; i++)
                        {
                            ObjectManager.Instance.Projectiles[i].Update();

                        }

                        for (int i = 0; i < ObjectManager.Instance.EnemyProjectiles.Count; i++)
                        {
                            ObjectManager.Instance.EnemyProjectiles[i].Update();

                        }

                        foreach (Enemy e in ObjectManager.Instance.Enemies)
                        {
                            e.Update();
                        }

                        if (ObjectManager.Instance.TheBoss != null)
                        {
                            ObjectManager.Instance.TheBoss.Update();
                        }

                        if (ObjectManager.Instance.Bomb != null)
                        {
                            ObjectManager.Instance.Bomb.Update();
                        }

                        foreach (HealthPickUp h in ObjectManager.Instance.Pickups)
                        {
                            h.Update();
                        }

                        foreach (SpareParts s in ObjectManager.Instance.Parts)
                        {
                            s.Update();
                        }

                        // check for collisions and modify transforms based on collisions
                        CollisionManager.Instance.CheckCollisions();

                        // apply transforms to positions
                        ObjectManager.Instance.Player.ApplyTransform();
                        foreach (Projectile p in ObjectManager.Instance.Projectiles)
                        {
                            p.ApplyTransform();
                        }

                        foreach (Projectile ep in ObjectManager.Instance.EnemyProjectiles)
                        {
                            ep.ApplyTransform();
                        }

                        if (ObjectManager.Instance.TheBoss != null)
                        {
                            ObjectManager.Instance.TheBoss.ApplyTransform();
                            for (int i = 0; i < ObjectManager.Instance.TheBoss.Minions.Count; i++)
                            {
                                ObjectManager.Instance.TheBoss.Minions[i].ApplyTransform();
                            }
                        }

                        foreach (Enemy e in ObjectManager.Instance.Enemies)
                        {
                            e.ApplyTransform();
                        }

                        // set the camera transform to the player position
                        ObjectManager.Instance.Camera.Position = new Vector2(ObjectManager.Instance.Player.X + ObjectManager.Instance.CameraTransform.X - (screenWidth / 2), ObjectManager.Instance.Player.Y + ObjectManager.Instance.CameraTransform.Y - (screenHeight / 2));

                        break;
                    }
            }
            #endregion

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            // TODO: Add your drawing code here

            spriteBatch.Begin(transformMatrix: ObjectManager.Instance.Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);


            switch (gameState)
            {
                case GameState.Start:
                    {
                        menu.DrawStartMenu(spriteBatch, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Pause:
                    {
                        menu.DrawPauseMenu(spriteBatch, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Death:
                    {
                        menu.DrawDeathMenu(spriteBatch, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Upgrade:
                    {
                        menu.DrawUpgradeMenu(spriteBatch, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Exit:
                    {
                        menu.DrawExitMenu(spriteBatch, ContentManager.Instance.ButtonFont);
                        break;
                    }
                case GameState.Play:
                    {
                        Texture2D screen = ContentManager.Instance.Space;
                        spriteBatch.Draw(screen, new Rectangle((int)ObjectManager.Instance.Camera.BoundingRectangle.X, (int)ObjectManager.Instance.Camera.BoundingRectangle.Y, screen.Width, screen.Height), Color.White);

                        //Draw floor
                        foreach (Floor f in ObjectManager.Instance.Floors)
                        {
                            f.Draw(spriteBatch);
                        }
                        
                        /*
                        foreach (Enemy e in ObjectManager.Instance.Enemies)
                        {
                            if (e.Path != null)
                            {
                                foreach (Floor f in e.Path)
                                {
                                    spriteBatch.Draw(f.Texture, new Vector2(f.X, f.Y), null, Color.Green, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0);
                                }
                                spriteBatch.Draw(e.Target.Texture, new Vector2(e.Target.X, e.Target.Y), null, Color.Red, 0f, Vector2.Zero, .25f, SpriteEffects.None, 0);
                                if (!testing)
                                {
                                    testing = true;
                                }
                                else
                                {
                                    ;
                                }
                            }
                        }
                        */

                        //Draw bomb
                        if (ObjectManager.Instance.Bomb != null)
                        {
                            ObjectManager.Instance.Bomb.Draw(spriteBatch);
                            if (ObjectManager.Instance.Bomb.exploded)
                            {
                                ObjectManager.Instance.Bomb.deathTimer--;
                                if(ObjectManager.Instance.Bomb.deathTimer <= 0)
                                {
                                    ObjectManager.Instance.Bomb = null;
                                }
                            }
                        }

                        //Draw pickups
                        foreach (HealthPickUp p in ObjectManager.Instance.Pickups)
                        {
                            if (p.visible)
                            {
                                p.Draw(spriteBatch);
                            }
                        }
                        
                        //Draw spare parts
                        foreach(SpareParts p in ObjectManager.Instance.Parts)
                        {
                            if (p.visible)
                            {
                                p.Draw(spriteBatch);
                            }
                        }

                        //Draw the Exit
                        ObjectManager.Instance.Exit.Draw(spriteBatch);

                        // draw walls
                        foreach (Wall w in ObjectManager.Instance.Walls)
                        {
                            w.Draw(spriteBatch);
                        }

                        // draw player
                        ObjectManager.Instance.Player.Draw(spriteBatch);

                        // Draw enemies
                        foreach (Enemy e in ObjectManager.Instance.Enemies)
                        {
                            e.Draw(spriteBatch);
                            //spriteBatch.DrawString(ContentManager.Instance.DefaultFont, e.XTransform + " : " + e.YTransform, new Vector2(e.X, e.Y), Color.Black);
                        }

                        // Draw boss
                        if (ObjectManager.Instance.TheBoss != null)
                            ObjectManager.Instance.TheBoss.Draw(spriteBatch);

                        // Draw enemy projectiles
                        foreach (Projectile ep in ObjectManager.Instance.EnemyProjectiles)
                        {
                            ep.Draw(spriteBatch);
                        }

                        // draw projectiles
                        foreach (Projectile p in ObjectManager.Instance.Projectiles)
                        {
                            p.Draw(spriteBatch);
                        }

                        //Draw energy bar
                        UICreator.DrawHealth(spriteBatch, 1015, Color.IndianRed);

                        break;
                    }
            }
            //Draw Crosshair
            UICreator.DrawCrosshair(spriteBatch, 6, 18, Color.LawnGreen);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}

