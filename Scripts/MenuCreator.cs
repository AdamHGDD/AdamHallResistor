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
    //Enumerators for the menu's buttons
    enum StartButtons
    {
        Play , Exit
    }

    enum DeathButtons
    {
        Restart , Menu , Exit
    }

    enum PauseButtons
    {
        Resume , Menu , Exit
    }

    enum ExitButtons
    {
        Restart, Menu, Exit
    }
    
    enum UpgradeButtonsEnum
    {
        Damage, Cost, Cooldown, Lifepool, Resume
    }


    //Enumerator for different GameStates
    enum GameState
    {
        Start, Play, Pause, Death, Exit, Upgrade
    }

    class MenuCreator
    {
        //Fields
        List<Button> startButtons;
        List<Button> deathButtons;
        List<Button> pauseButtons;
        List<Button> exitButtons;
        List<Button> upgradeButtons;


        //Properties
        public List<Button> StartButtons
        {
            get
            {
                return startButtons;
            }
        }
        public List<Button> DeathButtons
        {
            get
            {
                return deathButtons;
            }
        }
        public List<Button> PauseButtons
        {
            get
            {
                return pauseButtons;
            }
        }
        public List<Button> ExitButtons
        {
            get
            {
                return exitButtons;
            }
        }
        public List<Button> UpgradeButtons
        {
            get
            {
                return upgradeButtons;
            }
        }
        public Point newMouse
        {
            get; set;
        }
        public bool Moved
        {
            get; set;
        }
        public static bool UpgradeFinished
        {
            get; set;
        }

        public Skill DamageSkill
        {
            get; set;
        }
        public Skill CostSkill
        {
            get; set;
        }
        public Skill CooldownSkill
        {
            get; set;
        }
        public Skill LifepoolSkill
        {
            get; set;
        }
        public SpareParts Deleted
        {
            get; set;
        }

        //Constuctor 
        public MenuCreator(GraphicsDevice graphicsDevice, float damageChange, float costChange, float cooldownChange, float lifepoolChange)
        {
            //Creating button list
            startButtons = new List<Button>();
            deathButtons = new List<Button>();
            pauseButtons = new List<Button>();
            exitButtons = new List<Button>();
            upgradeButtons = new List<Button>();

            //Play Game Button
            startButtons.Add(new Button("Play", new Rectangle(0, 0, 0, 0), true));
            //Exit Button
            startButtons.Add(new Button("Exit", new Rectangle(0, 0, 0, 0), false));



            //Restart Button
            deathButtons.Add(new Button("Restart", new Rectangle(0, 0, 0, 0), true));
            //To Menu Button
            deathButtons.Add(new Button("Menu", new Rectangle(0, 0, 0, 0), false));
            //Exit Button
            deathButtons.Add(new Button("Exit", new Rectangle(0, 0, 0, 0), false));



            //Resume Button
            pauseButtons.Add(new Button("Resume", new Rectangle(0, 0, 0, 0),true));
            //To Menu Button
            pauseButtons.Add(new Button("Main Menu", new Rectangle(0, 0, 0, 0),false));
            //Exit Button
            pauseButtons.Add(new Button("Exit", new Rectangle(0, 0, 0, 0),false));


            //Restart Button
            exitButtons.Add(new Button("Next Level", new Rectangle(0, 0, 0, 0),true));
            //To Menu Button
            exitButtons.Add(new Button("Main Menu", new Rectangle(0, 0, 0, 0), false));
            //Exit Button
            exitButtons.Add(new Button("Exit", new Rectangle(0, 0, 0, 0), false));

            DamageSkill = new Skill(statTypes.damage, damageChange);
            CostSkill = new Skill(statTypes.cost, costChange);
            CooldownSkill = new Skill(statTypes.rate, cooldownChange);
            LifepoolSkill = new Skill(statTypes.energy, lifepoolChange);

            //Damage Button
            upgradeButtons.Add(new Button("", new Rectangle(0, 0, 0, 0), true));
            //Cost Button
            upgradeButtons.Add(new Button("", new Rectangle(0, 0, 0, 0), false));
            //Cooldown Button
            upgradeButtons.Add(new Button("", new Rectangle(0, 0, 0, 0), false));
            //Lifepool Button
            upgradeButtons.Add(new Button("", new Rectangle(0, 0, 0, 0), false));
            //Resume Button
            upgradeButtons.Add(new Button("Resume", new Rectangle(0, 0, 0, 0), false));
        }


        //Methods

        //Update Methods

        public void UpdateStartMenu(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            Moved = false;
            foreach (Button item in startButtons)
            {
                newMouse = new Point(InputManager.Instance.MState.Position.X + (int)ObjectManager.Instance.Camera.BoundingRectangle.X, InputManager.Instance.MState.Position.Y + (int)ObjectManager.Instance.Camera.BoundingRectangle.Y);

                if (item.HitBox.Contains(newMouse))
                {
                    item.MouseHover = true;
                }
                else
                {
                    item.MouseHover = false;
                }

                if(item.MoveHover && !Moved)
                {
                    Moved = true;
                    if (InputManager.Instance.JustPressedButton(Keys.S))
                    {
                        item.MoveHover = false;
                        startButtons[(startButtons.IndexOf(item) + 1) % startButtons.Count].MoveHover = true;
                    }
                    if (InputManager.Instance.JustPressedButton(Keys.W))
                    {
                        item.MoveHover = false;
                        startButtons[((startButtons.IndexOf(item) - 1) + startButtons.Count) % startButtons.Count].MoveHover = true;
                    }
                }
            }

            


            int extraX = (int)ObjectManager.Instance.Camera.BoundingRectangle.X;
            int extraY = (int)ObjectManager.Instance.Camera.BoundingRectangle.Y;
            
            Vector2 dimensions = font.MeasureString("Max Length");
            int height = (int)(dimensions.Y * 1.5f);
            int width = (int)dimensions.X;

            startButtons[0].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width / 2 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 + height +extraY, width, height);
            startButtons[1].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width / 2 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 +  height * 3 + extraY, width, height);
        }

        public void UpdateUpgradeMenu(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            Moved = false;
            foreach (Button item in upgradeButtons)
            {
                newMouse = new Point(InputManager.Instance.MState.Position.X + (int)ObjectManager.Instance.Camera.BoundingRectangle.X, InputManager.Instance.MState.Position.Y + (int)ObjectManager.Instance.Camera.BoundingRectangle.Y);

                if (item.HitBox.Contains(newMouse))
                {
                    item.MouseHover = true;
                }
                else
                {
                    item.MouseHover = false;
                }

                if (item.MoveHover && !Moved)
                {
                    Moved = true;
                    if (InputManager.Instance.JustPressedButton(Keys.D))
                    {
                        item.MoveHover = false;
                        upgradeButtons[(upgradeButtons.IndexOf(item) + 1) % upgradeButtons.Count].MoveHover = true;
                    }
                    if (InputManager.Instance.JustPressedButton(Keys.A))
                    {
                        item.MoveHover = false;
                        upgradeButtons[(upgradeButtons.IndexOf(item) - 1 + upgradeButtons.Count) % upgradeButtons.Count].MoveHover = true;
                    }
                    if (InputManager.Instance.JustPressedButton(Keys.W))
                    {
                        item.MoveHover = false;
                        upgradeButtons[0].MoveHover = true;
                    }
                    if (InputManager.Instance.JustPressedButton(Keys.S))
                    {
                        item.MoveHover = false;
                        upgradeButtons[upgradeButtons.Count-1].MoveHover = true;
                    }
                }
            }

            int extraX = (int)ObjectManager.Instance.Camera.BoundingRectangle.X;
            int extraY = (int)ObjectManager.Instance.Camera.BoundingRectangle.Y;

            Vector2 dimensions = font.MeasureString("Max Length");
            int height = (int)(dimensions.Y * 1.5f);
            int width = (int)dimensions.X;

            upgradeButtons[0].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 1 / 5 - width / 2 + extraX, graphicsDevice.Viewport.Height * 2 / 3 + extraY, width, height);
            upgradeButtons[1].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 2 / 5 - width / 2 + extraX, graphicsDevice.Viewport.Height * 2 / 3 + extraY, width, height);
            upgradeButtons[2].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 3 / 5 - width / 2 + extraX, graphicsDevice.Viewport.Height * 2 / 3 + extraY, width, height);
            upgradeButtons[3].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 4 / 5 - width / 2 + extraX, graphicsDevice.Viewport.Height * 2 / 3 + extraY, width, height);
            upgradeButtons[4].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 1 / 5 - width / 2 + extraX, graphicsDevice.Viewport.Height * 5 / 6  + extraY, width, height);

            UpgradeNames();
            ObjectManager.Instance.GetSkills = false;
            SkillUpgrade(Deleted);
        }

        public void UpgradeNames()
        {
            upgradeButtons[0].Message = "" + DamageSkill.Current() + " - " + (DamageSkill.Current() + DamageSkill.Change);
            upgradeButtons[1].Message = "" + CostSkill.Current() + " - " + (CostSkill.Current() * CostSkill.Change);
            upgradeButtons[2].Message = "" + CooldownSkill.Current() + " - " + (CooldownSkill.Current() * CooldownSkill.Change);
            upgradeButtons[3].Message = "" + LifepoolSkill.Current() + " - " + (LifepoolSkill.Current() + LifepoolSkill.Change);
        }

        public void SkillUpgrade(SpareParts delete)
        {
            if((InputManager.Instance.JustPressedMouse() && upgradeButtons[(int)UpgradeButtonsEnum.Damage].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter) && upgradeButtons[(int)UpgradeButtonsEnum.Damage].MoveHover))
            {
                DamageSkill.Acquire(delete);
                UpgradeFinished = true;
            }
            else if ((InputManager.Instance.JustPressedMouse() && upgradeButtons[(int)UpgradeButtonsEnum.Cost].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter) && upgradeButtons[(int)UpgradeButtonsEnum.Cost].MoveHover))
            {
                CostSkill.Acquire(delete);
                UpgradeFinished = true;
            }
            else if ((InputManager.Instance.JustPressedMouse() && upgradeButtons[(int)UpgradeButtonsEnum.Cooldown].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter) && upgradeButtons[(int)UpgradeButtonsEnum.Cooldown].MoveHover))
            {
                CooldownSkill.Acquire(delete);
                UpgradeFinished = true;
            }
            else if ((InputManager.Instance.JustPressedMouse() && upgradeButtons[(int)UpgradeButtonsEnum.Lifepool].MouseHover) ||
                            (InputManager.Instance.JustPressedButton(Keys.Enter) && upgradeButtons[(int)UpgradeButtonsEnum.Lifepool].MoveHover))
            {
                LifepoolSkill.Acquire(delete);
                UpgradeFinished = true;
            }
        }

        public void UpdateDeathMenu(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            Moved = false;
            foreach (Button item in deathButtons)
            {
                newMouse = new Point(InputManager.Instance.MState.Position.X + (int)ObjectManager.Instance.Camera.BoundingRectangle.X, InputManager.Instance.MState.Position.Y + (int)ObjectManager.Instance.Camera.BoundingRectangle.Y);
                if (item.HitBox.Contains(newMouse))
                {
                    item.MouseHover = true;
                }
                else
                {
                    item.MouseHover = false;
                }

                if (item.MoveHover && !Moved)
                {
                    Moved = true;
                    if (InputManager.Instance.JustPressedButton(Keys.S))
                    {
                        item.MoveHover = false;
                        deathButtons[(deathButtons.IndexOf(item) + 1) % deathButtons.Count].MoveHover = true;
                    }
                    if (InputManager.Instance.JustPressedButton(Keys.W))
                    {
                        item.MoveHover = false;
                        deathButtons[(deathButtons.IndexOf(item) - 1 + deathButtons.Count) % deathButtons.Count].MoveHover = true;
                    }
                }
            }

            int extraX = (int)ObjectManager.Instance.Camera.BoundingRectangle.X;
            int extraY = (int)ObjectManager.Instance.Camera.BoundingRectangle.Y;

            Vector2 dimensions = font.MeasureString("Max Length");
            int height = (int)(dimensions.Y * 1.5f);
            int width = (int)dimensions.X;
            
            deathButtons[0].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 2 / 3 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 - height + extraY, width, height);
            deathButtons[1].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 2 / 3 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 + height  + extraY, width, height);
            deathButtons[2].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 2 / 3 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 + height * 3 + extraY, width, height);
        }

        public void UpdatePauseMenu(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            Moved = false;
            foreach (Button item in pauseButtons)
            {
                newMouse = new Point(InputManager.Instance.MState.Position.X + (int)ObjectManager.Instance.Camera.BoundingRectangle.X, InputManager.Instance.MState.Position.Y + (int)ObjectManager.Instance.Camera.BoundingRectangle.Y);
                if (item.HitBox.Contains(newMouse))
                {
                    item.MouseHover = true;
                }
                else
                {
                    item.MouseHover = false;
                }

                if (item.MoveHover && !Moved)
                {
                    Moved = true;
                    if (InputManager.Instance.JustPressedButton(Keys.S))
                    {
                        item.MoveHover = false;
                        pauseButtons[(pauseButtons.IndexOf(item) + 1) % pauseButtons.Count].MoveHover = true;
                    }
                    if (InputManager.Instance.JustPressedButton(Keys.W))
                    {
                        item.MoveHover = false;
                        pauseButtons[(pauseButtons.IndexOf(item) - 1 + pauseButtons.Count) % pauseButtons.Count].MoveHover = true;
                    }
                }
            }

            int extraX = (int)ObjectManager.Instance.Camera.BoundingRectangle.X;
            int extraY = (int)ObjectManager.Instance.Camera.BoundingRectangle.Y;

            Vector2 dimensions = font.MeasureString("Max Length");
            int height = (int)(dimensions.Y * 1.5f);
            int width = (int)dimensions.X;

            pauseButtons[0].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width / 2 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 + height + extraY, width, height);
            pauseButtons[1].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width / 2 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 + height * 3 +extraY, width, height);
            pauseButtons[2].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width / 2 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 + height * 5 + extraY, width, height);
        }

        public void UpdateExitMenu(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            Moved = false;
            foreach (Button item in exitButtons)
            {
                newMouse = new Point(InputManager.Instance.MState.Position.X + (int)ObjectManager.Instance.Camera.BoundingRectangle.X, InputManager.Instance.MState.Position.Y + (int)ObjectManager.Instance.Camera.BoundingRectangle.Y);
                if (item.HitBox.Contains(newMouse))
                {
                    item.MouseHover = true;
                }
                else
                {
                    item.MouseHover = false;
                }

                if (item.MoveHover && !Moved)
                {
                    Moved = true;
                    if (InputManager.Instance.JustPressedButton(Keys.S))
                    {
                        item.MoveHover = false;
                        exitButtons[(exitButtons.IndexOf(item) + 1) % exitButtons.Count].MoveHover = true;
                    }
                    if (InputManager.Instance.JustPressedButton(Keys.W))
                    {
                        item.MoveHover = false;
                        exitButtons[(exitButtons.IndexOf(item) - 1 + exitButtons.Count) % exitButtons.Count].MoveHover = true;
                    }
                }
            }

            int extraX = (int)ObjectManager.Instance.Camera.BoundingRectangle.X;
            int extraY = (int)ObjectManager.Instance.Camera.BoundingRectangle.Y;

            Vector2 dimensions = font.MeasureString("Max Length");
            int height = (int)(dimensions.Y * 1.5f);
            int width = (int)dimensions.X;

            exitButtons[0].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 2 / 3 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 - height + extraY, width, height);
            exitButtons[1].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 2 / 3 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 + height + extraY, width, height);
            exitButtons[2].DisplayBox = new Rectangle(graphicsDevice.Viewport.Width * 2 / 3 - width / 2 + extraX, graphicsDevice.Viewport.Height / 2 + height * 3 + extraY, width, height);
        }


        //Draw Methods

        public void DrawStartMenu(SpriteBatch spriteBatch, SpriteFont font)
        {
            Texture2D screen = ContentManager.Instance.TitleScreen;
            spriteBatch.Draw(screen, new Rectangle((int)ObjectManager.Instance.Camera.BoundingRectangle.X, (int)ObjectManager.Instance.Camera.BoundingRectangle.Y, screen.Width, screen.Height), Color.White);
            foreach (Button item in startButtons)
            {
                if (item.MoveHover)
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.Black);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.White);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.White);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.Black);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.White);
                }
            }
        }

        public void DrawDeathMenu(SpriteBatch spriteBatch, SpriteFont font)
        {
            foreach (Button item in deathButtons)
            {
                if (item.MoveHover)
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.Black);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.White);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.White);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.Black);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.White);
                }
            }
        }

        public void DrawExitMenu(SpriteBatch spriteBatch, SpriteFont font)
        {
            foreach (Button item in exitButtons)
            {
                if (item.MoveHover)
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.Black);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.White);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.White);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.Black);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.White);
                }
            }
        }

        public void DrawUpgradeMenu(SpriteBatch spriteBatch, SpriteFont font)
        {
            Texture2D screen = ContentManager.Instance.UpgradeScreen;
            spriteBatch.Draw(screen, new Rectangle((int)ObjectManager.Instance.Camera.BoundingRectangle.X, (int)ObjectManager.Instance.Camera.BoundingRectangle.Y, screen.Width, screen.Height), Color.White);
            foreach (Button item in upgradeButtons)
            {
                if (item.MoveHover)
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.Black);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.White);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.White);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.Black);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.White);
                }
            }
        }

        public void DrawPauseMenu(SpriteBatch spriteBatch, SpriteFont font)
        {
            foreach (Button item in pauseButtons)
            {
                if (item.MoveHover)
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.Black);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.White);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, item.HitBox, Color.White);
                    Rectangle temp = new Rectangle((item.HitBox.X + item.DisplayBox.X) / 2, (item.HitBox.Y + item.DisplayBox.Y) / 2, (item.HitBox.Width + item.DisplayBox.Width) / 2, (item.HitBox.Height + item.DisplayBox.Height) / 2);
                    spriteBatch.Draw(ContentManager.Instance.GenericTexture, temp, Color.Black);
                    spriteBatch.DrawString(font, item.Message, new Vector2(item.DisplayBox.X, item.DisplayBox.Y), Color.White);
                }
            }
        }
        public void ResetHover()
        {
            foreach (Button item in deathButtons)
            {
                item.MoveHover = false;
            }
            foreach (Button item in exitButtons)
            {
                item.MoveHover = false;
            }
            foreach (Button item in pauseButtons)
            {
                item.MoveHover = false;
            }
            foreach (Button item in upgradeButtons)
            {
                item.MoveHover = false;
            }
            foreach (Button item in startButtons)
            {
                item.MoveHover = false;
            }

            deathButtons[0].MoveHover = true;
            startButtons[0].MoveHover = true;
            exitButtons[0].MoveHover = true;
            pauseButtons[0].MoveHover = true;
            upgradeButtons[0].MoveHover = true;
            InputManager.Instance.ResetKeys();
        }
    }
}
