using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        Settings s = new Settings();
        
        GraphicsDeviceManager graphics;
        MovingBackground mb;

        Texture2D SpaceBackground;
        Texture2D[] EnemyTexture = new Texture2D[2];
        Texture2D FireballTexture, BeamTexture, ArrowTexture;

        Song BGMusic;
        SoundEffect FiringNoise, ExplodeNoise, LaserNoise, TorpedoNoise, RedAlert;
        SoundEffectInstance RedAlertInstance;

        Keys KeyUp, KeyDown, KeyLeft, KeyRight, KeyFire, KeyWepForward, KeyWepBackward, KeyEsc;
        
        GameState gamestate = GameState.StartScreen;
        MouseState mouseState = Mouse.GetState();

        Vector2 ViewScreen, textLeft, textMiddle, mousePosition;
        Player PlayerShip;
        
        public int SelectedMenuItem { get; set; }
        public int MenuPos { get; set; }
        public int Points { get; set; }
        public float Health { get; set; }
        public float launchingspeed { get; set; }
     
        SpriteBatch spriteBatch;
        SpriteFont CourierNew;

        List<Fireball> PlayerShotList = new List<Fireball>();
        List<Fireball> EnemyShotList = new List<Fireball>();
        List<Enemy> EnemyList = new List<Enemy>();

        Rectangle[] MenuObject = new Rectangle[10];

        float buttonFireDelay = 0.0f, changeWeaponDelay = 0.0f, VolumeDelay = 0.0f;
        float healthtimer = 0.0f, EnemyLauncher = 0.0f;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferMultiSampling = false;
            //graphics.IsFullScreen = true;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        public void PlaySound(SoundEffect SE){
            SoundEffectInstance FiringInstance;
            FiringInstance = SE.CreateInstance();
            FiringInstance.Volume = Controls.Default.EffectNoise;
            FiringInstance.Play();
        }

        public void PlayInstanceSound()
        {
            RedAlertInstance.Volume = Controls.Default.EffectNoise;
            if (RedAlertInstance.State == SoundState.Playing && Health > 38.0f) RedAlertInstance.Stop();
            if (RedAlertInstance.State == SoundState.Stopped && Health < 25.0f) RedAlertInstance.Play();

        }

        protected void UpdateMouse()
        {
            previousMouseState = Mouse.GetState();
            MouseState mouseState = Mouse.GetState(); // The mouse X and Y positions are set relative to the Top, Left of the game window.
            mousePosition.X = mouseState.X;
            mousePosition.Y = mouseState.Y;
        }

        public enum ShipType
        {
            Bandit, Bandit_Elite
        }
        public enum KEYSET
        {
            KeyUp, KeyDown, KeyLeft, KeyRight, KeyFire, KeyWepForward, KeyWepBackward, KeyEsc
        }
        public enum GameState
        {
            StartScreen, Running, Pause, Options, EndScreen
        }


        public void DrawStartScreen()
        {
            //string[] text = new string[] {""};

            string[] mi = new string[] { "Start", "Options", "Quit" };

            for (int x = 0; x < mi.Length; x++)
            {
                Vector2 newwidth = CourierNew.MeasureString(mi[x]);
                if (x == SelectedMenuItem)  DrawMenu(mi[x], Color.Yellow, MenuPos, x);
                else DrawMenu(mi[x], Color.White, MenuPos, x);
                MenuPos = MenuPos + 35;
                
            }
            MenuPos = -15;
        }

        public void DrawOptionsMenu()
        {
            string[] mi = new string[] { "Easy", "Normal", "Hard" };

            for (int x = 0; x < mi.Length; x++)
            {
                Vector2 newwidth = CourierNew.MeasureString(mi[x]);
                if (x == SelectedMenuItem) DrawMenu(mi[x], Color.Yellow, MenuPos, x);
                else DrawMenu(mi[x], Color.White, MenuPos, x);
                MenuPos = MenuPos + 35;

            }
            MenuPos = -15;
        }

        public void DrawMenu(string a, Color c, int h, int m)
        {
            Vector2 newwidth = CourierNew.MeasureString(a);
            Vector2 v = new Vector2(GraphicsDevice.Viewport.Width / 2 - newwidth.X / 2, GraphicsDevice.Viewport.Height / 2 + h);
            MenuObject[m].X = Convert.ToInt16(v.X); MenuObject[m].Y = Convert.ToInt16(v.Y);
            MenuObject[m].Width = Convert.ToInt16(newwidth.X); MenuObject[m].Height = Convert.ToInt16(newwidth.Y);
            if (MouseCollisionDetect(MenuObject[m]) == true)
            {
                c = Color.Yellow; SelectedMenuItem = m;
            }
            else c = Color.White;

            PlayerShip.DrawText(spriteBatch, CourierNew, a, v, c);
        }

        public void DrawEndScreen()
        {
            if (RedAlertInstance.State == SoundState.Playing) RedAlertInstance.Stop();
            string text = "Game Over";
            Vector2 newwidth = CourierNew.MeasureString(text);
            PlayerShip.DrawText(spriteBatch, CourierNew, text, new Vector2(GraphicsDevice.Viewport.Width / 2 - newwidth.X / 2, GraphicsDevice.Viewport.Height / 2), Color.Red);
            if (graphics.IsFullScreen)
                graphics.ToggleFullScreen();
            graphics.ApplyChanges();
        }
        public void DrawPauseScreen()
        {
            string[] mi = new string[] { "Resume", "Options", "Quit" };
            this.IsMouseVisible = true;
            string text = "Paused";
            string text2 = "Press Fire to restart the game.";
            Vector2 newwidth = CourierNew.MeasureString(text);
            Vector2 newwidth2 = CourierNew.MeasureString(text2);
            PlayerShip.DrawText(spriteBatch, CourierNew, text, new Vector2(GraphicsDevice.Viewport.Width / 2 - newwidth.X / 2, GraphicsDevice.Viewport.Height / 2 - 100), Color.LimeGreen);
            PlayerShip.DrawText(spriteBatch, CourierNew, text2, new Vector2(GraphicsDevice.Viewport.Width / 2 - newwidth2.X / 2, GraphicsDevice.Viewport.Height / 2 -75), Color.LimeGreen);
            
            for (int x = 0; x < mi.Length; x++)
            {
                DrawMenu(mi[x], Color.White, MenuPos, x);
                MenuPos += 35;
            }
            MenuPos = -15;
        }

        public void UpdateSplashScreen()
        {
            if (gamestate == GameState.StartScreen || gamestate == GameState.EndScreen || gamestate == GameState.Pause)
            {
                switch (gamestate)
                {
                    case GameState.StartScreen: DrawStartScreen(); break;
                    case GameState.EndScreen: DrawEndScreen(); break;
                    case GameState.Pause: DrawPauseScreen(); break;
                }
            }
        }
        MouseState previousMouseState;
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ViewScreen.X = GraphicsDevice.Viewport.X + 50;
            ViewScreen.Y = GraphicsDevice.Viewport.Height - 75;
            Health = 100.0f;
            previousMouseState = Mouse.GetState();
            launchingspeed = 6.0f;
            //this.s.SFVolume += new System.EventHandler(this.SFVolumeAdjuster);
            //s.SetupSettings();
            SetupKeys();
            base.Initialize();
        }

        public void AdjustVolume()
        {
            Controls.Default.EffectNoise = s.Vol;
            s.SaveSettings();
        }

        public void SetupKeys()
        {
            KeyEsc = Controls.Default.KeyEsc;
            KeyUp = Controls.Default.KeyUp;
            KeyDown = Controls.Default.KeyDown;
            KeyLeft = Controls.Default.KeyLeft;
            KeyRight = Controls.Default.KeyRight;
            KeyFire = Controls.Default.KeyFire;
            KeyWepForward = Controls.Default.KeyWepForward;
            KeyWepBackward = Controls.Default.KeyWepBackward;
            s.Vol = Controls.Default.EffectNoise;
            /*
            KeyEsc = s.keyset[(int)KEYSET.KeyEsc];
            KeyUp = s.keyset[(int)KEYSET.KeyUp];
            KeyDown = s.keyset[(int)KEYSET.KeyDown];
            KeyLeft = s.keyset[(int)KEYSET.KeyLeft];
            KeyRight = s.keyset[(int)KEYSET.KeyRight];
            KeyFire = s.keyset[(int)KEYSET.KeyFire];
            KeyWepForward = s.keyset[(int)KEYSET.KeyWepForward];
            KeyWepBackward = s.keyset[(int)KEYSET.KeyWepBackward];*/
        }

        public void LoadTextures()
        {
            SpaceBackground = Content.Load<Texture2D>("sprites\\space_background");
            FireballTexture = Content.Load<Texture2D>("sprites\\fireball");
            BeamTexture = Content.Load<Texture2D>("sprites\\beam");
            ArrowTexture = Content.Load<Texture2D>("sprites\\arrow");
            EnemyTexture[(int)ShipType.Bandit] = Content.Load<Texture2D>("sprites\\ship_enemy");
            EnemyTexture[(int)ShipType.Bandit_Elite] = Content.Load<Texture2D>("sprites\\ship_enemy1");            
        }
        public void LoadFonts()
        {
            CourierNew = Content.Load<SpriteFont>("MainText");
        }
        public void LoadSoundEffects()
        {
            FiringNoise = Content.Load<SoundEffect>("sounds\\ZINGSPLT");
            ExplodeNoise = Content.Load<SoundEffect>("sounds\\EXPLODE");
            LaserNoise = Content.Load<SoundEffect>("sounds\\LASER21");
            TorpedoNoise = Content.Load<SoundEffect>("sounds\\fmtorp2");
            RedAlert = Content.Load<SoundEffect>("sounds\\redalert");
        }

        protected override void LoadContent()
        {
            // Create the font
            Texture2D playerTexture = Content.Load<Texture2D>("sprites\\ship_main");

            BGSoundControl();
            LoadTextures();
            LoadSoundEffects();
            LoadFonts();

            RedAlertInstance = RedAlert.CreateInstance();
            RedAlertInstance.IsLooped = true;
            // Set text positions
            textMiddle = new Vector2(GraphicsDevice.Viewport.Width / 1.3f,
            graphics.GraphicsDevice.Viewport.Height / 30);
            textLeft = new Vector2(GraphicsDevice.Viewport.Width / 30,
            GraphicsDevice.Viewport.Height / 30);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
           
            mb = new MovingBackground(graphics);
            PlayerShip = new Player(GraphicsDevice, new Vector2(400, 300),playerTexture);
            PlayerShip.SetContent(FireballTexture, BeamTexture, ArrowTexture);
            // TODO: use this.Content to load your game content here
        }

        public void BGSoundControl()
        {
            BGMusic = Content.Load<Song>("Jittery");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(BGMusic);
            MediaPlayer.Volume = 0.15f;
        }
        public void HealthUpdater()
        {
            if (Health < 100.0f) Health += 0.1f;
            else { Health = 100.0f; }
            healthtimer = 0.1f;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            UpdateMouse();
            UpdateInput(gameTime);
            CountdownTimers(gameTime);
            // TODO: Add your update logic here                
            base.Update(gameTime);
        }

        public void CountdownTimers(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            buttonFireDelay -= elapsed;
            changeWeaponDelay -= elapsed;
            EnemyLauncher -= elapsed;
            healthtimer -= elapsed;
            VolumeDelay -= elapsed;
        }
        
        public void EnemyInteraction(GameTime gameTime)
        {
            PlayerShip.Update(gameTime);
            for (int i = 0; i < PlayerShotList.Count; i++)
            {
                PlayerShotList[i].Update(gameTime);
                if (PlayerShotList[i].Position.Y < -100.0f)
                    PlayerShotList.RemoveAt(i);
            }

            

            for (int i = 0; i < EnemyList.Count; i++)
            {
                EnemyList[i].Update(gameTime);
                if (EnemyList[i].Firing)
                {
                    //Fireball fireball = new Fireball(enemyFireballTexture,
                    //new Vector2(EnemyList[i].Position.X + 10.0f, 
                    //EnemyList[i].Position.Y + 30.0f), -300.0f);
                    //enemyFireballList.Add(fireball);
                    EnemyList[i].Firing = false;
                }
                bool ShipCollide = PlayerShip.Collision(EnemyList[i].Position, 20.0f);
                if (ShipCollide == true) //lose HP
                {
                    Health -= 20.0f;
                    Points += 75;
                    EnemyList.RemoveAt(i);
                    PlaySound(ExplodeNoise);
                    if (Health < 0.0f) gamestate = GameState.EndScreen;
                }

                if (PlayerShotList.Count > 0)
                {
                    int FBallCollide = EnemyList[i].CollisionBall(PlayerShotList);
                    if (FBallCollide != -1)
                    {
                        if ((int)ShipType.Bandit == EnemyList[i].ShipType) Points += 200;
                        if ((int)ShipType.Bandit_Elite == EnemyList[i].ShipType) Points += 300;

                        EnemyList.RemoveAt(i);
                        PlayerShotList.RemoveAt(FBallCollide);
                        
                        PlaySound(ExplodeNoise);
                    }
                }
            }
        }

        public void DrawText(SpriteBatch TextBatch)
        {
            Color c;
            if (Health > 85.0f) c = Color.Green;
            else if (Health > 30.0f && Health < 85.0f) c = Color.Orange;
            else c = Color.Red;
            string RA = "RED ALERT";
            if(RedAlertInstance.State == SoundState.Playing) PlayerShip.DrawText(spriteBatch, CourierNew, RA, new Vector2((graphics.PreferredBackBufferWidth/2)  - (CourierNew.MeasureString(RA).X/ 2), graphics.PreferredBackBufferHeight - 60), Color.Red); 
            PlayerShip.DrawText(TextBatch, CourierNew, Health.ToString("0.0"), new Vector2(graphics.PreferredBackBufferWidth - 100, graphics.PreferredBackBufferHeight - 50), c);
            PlayerShip.DrawText(TextBatch, CourierNew, "Points: " + Points.ToString(), textMiddle, Color.White); //Points Position
            PlayerShip.DrawText(TextBatch, CourierNew, PlayerShip.SelectedWeapon, new Vector2(ViewScreen.X + 30, ViewScreen.Y - 10), Color.White);
            PlayerShip.DrawText(TextBatch, CourierNew, PlayerShip.GetAmmo.ToString(), new Vector2(ViewScreen.X + 30, ViewScreen.Y + 10), Color.Orange);

        }

        public bool MouseCollisionDetect(Rectangle a)
        {
            if ((mousePosition.X >= a.X) && mousePosition.X < (a.X + a.Width) &&
                mousePosition.Y >= a.Y && mousePosition.Y < (a.Y + a.Height))
                return true;
            else return false;
        }

        private void UpdateInput(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if ((keyState.IsKeyDown(Keys.LeftAlt) || keyState.IsKeyDown(Keys.RightAlt)) && keyState.IsKeyDown(Keys.Enter))
                graphics.ToggleFullScreen();

            if (gamestate == GameState.StartScreen)
            {
                GameStart(keyState, gamePadState);
            }
            if (gamestate == GameState.Pause)
            {
                GamePaused(keyState, gamePadState);
            }
            if (gamestate == GameState.Running)
            {
                GameRunning(keyState, gamePadState, gameTime); 
            }

        }

        public void GameStart(KeyboardState keyState, GamePadState gamePadState)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                //find the item
                for (int x = 0; x < MenuObject.Length; x++)
                {
                    if (MouseCollisionDetect(MenuObject[x]))
                    {
                        switch (x)
                        {
                            default:
                                IsMouseVisible = false;
                                gamestate = GameState.Running;
                                buttonFireDelay = 0.35f;
                                break;
                            case 1: gamestate = GameState.Options; buttonFireDelay = 0.35f; break;
                            case 2: this.Exit(); break;
                        }
                    }
                }
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start ==
            ButtonState.Pressed || keyState.IsKeyDown(KeyFire))
            {
                switch (SelectedMenuItem)
                {
                    default:
                        IsMouseVisible = false;
                        gamestate = GameState.Running;
                        buttonFireDelay = 0.25f;
                        break;
                    case 1: break;
                    case 2: this.Exit(); break;
                }
            }
            if (buttonFireDelay <= 0.0f)
            {
                if (keyState.IsKeyDown(KeyUp)
                || gamePadState.DPad.Up == ButtonState.Pressed)
                {
                    if (SelectedMenuItem > 0) SelectedMenuItem--;
                }
                if (keyState.IsKeyDown(KeyDown)
                || gamePadState.DPad.Down == ButtonState.Pressed)
                {
                    if (SelectedMenuItem < 2) SelectedMenuItem++;
                }
                buttonFireDelay = 0.2f;
            }
        }
        public void GameRunning(KeyboardState keyState, GamePadState gamePadState, GameTime gameTime)
        {
            EnemyInteraction(gameTime);
            PlayInstanceSound();
            if (healthtimer <= 0.0f) HealthUpdater();
            if (EnemyLauncher <= 0.0f && gamestate == GameState.Running)
            {
                float difficultymultiplier = 1.0f;
                if (Controls.Default.Difficulty == 0) difficultymultiplier = 1.0f;
                if (Controls.Default.Difficulty == 1) difficultymultiplier = 1.2f;
                if (Controls.Default.Difficulty == 2) difficultymultiplier = 1.5f;
                if (Points > 1000) launchingspeed = 5.0f / difficultymultiplier;
                if (Points > 2500) launchingspeed = 4.0f / difficultymultiplier;
                if (Points > 6000) launchingspeed = 3.0f / difficultymultiplier;
                if (Points > 10000) launchingspeed = 2.0f / difficultymultiplier;
                if (Points > 15000) launchingspeed = 1.5f / difficultymultiplier;
                if (Points > 25000) launchingspeed = 1.0f / difficultymultiplier;
                if (Points > 40000) launchingspeed = 0.8f / difficultymultiplier;
                if (Points > 55000) launchingspeed = 0.6f / difficultymultiplier;
                if (Points > 75000) launchingspeed = 0.3f / difficultymultiplier;
                Random r = new Random(2);
                EnemyLauncher = (float)r.NextDouble() * launchingspeed;


                Random R = new Random();
                switch (R.Next(3))
                {
                    default: CreateEnemy(EnemyTexture[(int)ShipType.Bandit], 50.0f, (int)ShipType.Bandit); break;
                    case 2: if (Points > 200) CreateEnemy(EnemyTexture[(int)ShipType.Bandit_Elite], 100.0f, (int)ShipType.Bandit_Elite);
                        else CreateEnemy(EnemyTexture[(int)ShipType.Bandit], 50.0f, (int)ShipType.Bandit); break;
                }
            }
            if (buttonFireDelay <= 0.0f)
            {
                if (keyState.IsKeyDown(KeyEsc)
                    || gamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    gamestate = GameState.Pause;
                    buttonFireDelay = 0.2f;
                }
                if (keyState.IsKeyDown(Keys.C)
                || gamePadState.Buttons.Y == ButtonState.Pressed)
                {
                    //CreateEnemy();
                    buttonFireDelay = 0.2f;
                }
            }

            if (VolumeDelay <= 0.0f)
            {
                if (keyState.IsKeyDown(Keys.F5))
                    if (Controls.Default.EffectNoise > 0.0f)
                    {
                        s.Vol -= 0.01f; AdjustVolume();
                    }
                if (keyState.IsKeyDown(Keys.F6))
                    if (Controls.Default.EffectNoise < 1.0f)
                    {
                        s.Vol += 0.01f; AdjustVolume();
                    }
                //if (Controls.Default.EffectNoise > 1.0f) Controls.Default.EffectNoise = 1.0f;
                //if (Controls.Default.EffectNoise > 0.0f) Controls.Default.EffectNoise = 0.0f;

                VolumeDelay = 0.05f;
            }

            if (keyState.IsKeyDown(KeyUp) || gamePadState.DPad.Up == ButtonState.Pressed)
                PlayerShip.Accelerate();
            if (keyState.IsKeyDown(KeyDown) || gamePadState.DPad.Down == ButtonState.Pressed)
                PlayerShip.MoveReverse();
            if (keyState.IsKeyDown(KeyLeft) || gamePadState.DPad.Left == ButtonState.Pressed)
                PlayerShip.TurnLeft();
            if (keyState.IsKeyDown(KeyRight) || gamePadState.DPad.Right == ButtonState.Pressed)
                PlayerShip.TurnRight();
            if (keyState.IsKeyDown(KeyFire) || gamePadState.Buttons.A == ButtonState.Pressed)
            {
                //PlayerShip.Fire();
                if (buttonFireDelay <= 0.0f)
                {
                    Fireball FireballShot = new Fireball(FireballTexture, new Vector2(PlayerShip.Position.X, PlayerShip.Position.Y - 10.0f), 150.0f);
                    Fireball BeamShot = new Fireball(BeamTexture, new Vector2(PlayerShip.Position.X, PlayerShip.Position.Y - 10.0f), 300.0f);
                    Fireball ArrowShot = new Fireball(ArrowTexture, new Vector2(PlayerShip.Position.X, PlayerShip.Position.Y - 10.0f), 300.0f);

                    //if (SelectedWeapon == 0)
                    //{
                    if (PlayerShip.GetAmmo > 0)
                    {
                        if (PlayerShip.Weapon == 0)
                        {
                            PlayerShotList.Add(FireballShot);
                            buttonFireDelay = 0.4f;
                            PlaySound(TorpedoNoise);
                        }
                        else if (PlayerShip.Weapon == 1)
                        {
                            PlayerShotList.Add(BeamShot);
                            buttonFireDelay = 0.18f;
                            PlaySound(LaserNoise);
                        }
                        else
                        {
                            PlayerShotList.Add(ArrowShot);
                            buttonFireDelay = 0.2f;
                            PlaySound(FiringNoise);
                        }

                        PlayerShip.UpdateAmmo(false);
                    }
                    //}

                }
            }
            if (keyState.IsKeyDown(KeyWepForward)
            || gamePadState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                if (changeWeaponDelay <= 0.0f)
                {
                    PlayerShip.ChangeWeapon(0);
                    changeWeaponDelay = 0.25f;
                }
            }
            if (keyState.IsKeyDown(KeyWepBackward)
            || gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                if (changeWeaponDelay <= 0.0f)
                {
                    PlayerShip.ChangeWeapon(1);
                    changeWeaponDelay = 0.25f;
                }
            }

        }
        public void GamePaused(KeyboardState keyState, GamePadState gamePadState)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                //find the item
                for (int x = 0; x < MenuObject.Length; x++)
                {
                    if (MouseCollisionDetect(MenuObject[x]))
                    {
                        switch (x)
                        {
                            default:
                                IsMouseVisible = false;
                                gamestate = GameState.Running;
                                buttonFireDelay = 0.25f;
                                break;
                            case 1: break;
                            case 2: this.Exit(); break;
                        }
                    }
                }
            }
            if (buttonFireDelay <= 0.0f)
            {
                if (keyState.IsKeyDown(KeyEsc)
                || gamePadState.Buttons.Start == ButtonState.Pressed)
                {
                    gamestate = GameState.Running;
                    buttonFireDelay = 0.2f;
                }
            }
        }

        public void CreateEnemy(Texture2D ET, float speed, int type)
        {
            Random r = new Random();
            int startX = r.Next(GraphicsDevice.Viewport.Width - 50);
                Enemy enemy = new Enemy(ET, new Vector2(startX+50, -100), speed);
                enemy.ShipType = type;
            //looks, startlocation, speed

                enemy.SetAcrossMovement(0.0f, 0.0f);
                EnemyList.Add(enemy);
        }

        public void DrawPlayerStuff()
        {
            Texture2D x;
            PlayerShip.Draw(spriteBatch);
            switch (PlayerShip.Weapon) //This sets up the selected weapon displayer in the corner
            {
                default: { x = FireballTexture; break; }
                case 1: { x = BeamTexture; break; }
                case 2: { x = ArrowTexture; break; }
                //case 3:  { x=; }
                //case 4:  { x=; }
                //case 5:  { x=; }
            }
            Fireball g = new Fireball(x, ViewScreen, 0.0f);
            g.Draw(spriteBatch);
        }

        public void DrawMovingObjects()
        {
            foreach (Fireball f in PlayerShotList) f.Draw(spriteBatch);
            foreach (Enemy e in EnemyList) e.Draw(spriteBatch);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);                
            spriteBatch.Begin();
                // TODO: Add your drawing code here
                spriteBatch.Draw(SpaceBackground, Vector2.Zero, Color.White);
            if (gamestate == GameState.Running || gamestate == GameState.Pause)
            {
                DrawMovingObjects();
                DrawPlayerStuff();
                DrawText(spriteBatch);
                if (gamestate == GameState.Pause) UpdateSplashScreen();
            }
            else UpdateSplashScreen();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
