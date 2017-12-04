using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    class Player
    {
        Texture2D shipSprite;
        Texture2D FireballTexture, BeamTexture, ArrowTexture;
        Vector2 position;
        Vector2 spriteOrigin;
        Vector2 Velocity;

        
        float buttonFireDelay = 0.0f, changeWeaponDelay = 0.0f;

        List<Fireball> PlayerShotList = new List<Fireball>();

        int windowWidth, windowHeight;
        public int weapon { get; set; }
        int[] ammo = new int[] { 200, 400, 300 };
        float radius = 25.0f;
        int maxweapons=2;

        string[] WeaponName = new string[]{ "Fireball","Beam Laser","Arrow Gun" };
        public void SetContent(Texture2D w1, Texture2D w2, Texture2D w3){
            FireballTexture = w1; BeamTexture = w2; ArrowTexture = w3;
            weapon = 0;
            Velocity.X = 0.0f;
            Velocity.Y = 0.0f;
        }
        public Player(GraphicsDevice device, Vector2 position, Texture2D sprite)
        {
            // The position that is passed in is now set to the position above
            this.position = position;
            // Set the Texture2D
            shipSprite = sprite;
            // Setup origin
            spriteOrigin.X = (float)shipSprite.Width / 2.0f;
            spriteOrigin.Y = (float)shipSprite.Height / 2.0f;

            // Set window dimensions
            windowHeight = device.Viewport.Height;
            windowWidth = device.Viewport.Width;
        }
        public void DrawText(SpriteBatch batch, SpriteFont f, string t, Vector2 l, Color c)
        {
            batch.DrawString(f, t, l, c);
        }
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(shipSprite, position, null, Color.White, 0.0f, spriteOrigin, 1.0f, SpriteEffects.None, 0.0f);
        }

        public int GetAmmo
        {
            get { return ammo[weapon]; }
        }
        public bool Collision(Vector2 point, float radius)
        {
            if ((point - position).Length() < this.radius + radius)
            {
                return true;
            }
            return false;
        }

        public void UpdateAmmo(bool newammo)
        {
            if (newammo == false) ammo[weapon]--;
        }
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < PlayerShotList.Count; i++)
            {
                PlayerShotList[i].Update(gameTime);
                if (PlayerShotList[i].Position.Y < -100.0f)
                    PlayerShotList.RemoveAt(i);
            }

            position.Y += Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Velocity.Y *= 0.97f;

            position.X += Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Velocity.X *= 0.97f;

            if (position.Y < 25.0f) position.Y = 25.0f;
            if (position.Y > windowHeight-25.0f) position.Y = windowHeight -25.0f;
            if (position.X < 25.0f) position.X = 25.0f;
            if (position.X > windowWidth-25.0f) position.X = windowWidth -25.0f;
            
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            buttonFireDelay -= elapsed;
            changeWeaponDelay -= elapsed;
        }
        public void Accelerate()
        {
            Velocity.Y -= 10.0f;
        }
        public void MoveReverse()
        {
            Velocity.Y += 7.0f;
        }
        public void TurnRight()
        {
            Velocity.X += 8.0f;
        }
        public void TurnLeft()
        {
            Velocity.X -= 8.0f;
        }
        /*public void Fire()
        {
            if (buttonFireDelay <= 0.0f)
            {
                Fireball FireballShot = new Fireball(FireballTexture, new Vector2(Position.X, Position.Y - 10.0f), 300.0f);
                Fireball BeamShot = new Fireball(BeamTexture, new Vector2(Position.X, Position.Y - 10.0f), 300.0f);
                Fireball ArrowShot = new Fireball(ArrowTexture, new Vector2(Position.X, Position.Y - 10.0f), 180.0f);
                
                //if (SelectedWeapon == 0)
                //{
                switch (Weapon)
                {
                    default: PlayerShotList.Add(FireballShot); break;
                    case 1: PlayerShotList.Add(BeamShot); break;
                    case 2: PlayerShotList.Add(ArrowShot); break;
                }

                //}
                buttonFireDelay = 0.25f;
            }

        }*/
        public void ChangeWeapon(int direction)
        {
            if (direction == 0)
            {
                if (weapon < maxweapons) weapon++;
                else weapon = 0;
            }
            else
            {
                if (weapon > 0) weapon--;
                else weapon = maxweapons;
            }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public int Weapon
        {
            get { return weapon; }
        }
        public string SelectedWeapon
        {
            get { return WeaponName[weapon]; }
        }
    }

    class Enemy
    {
        Vector2 position;
        Texture2D picture;
        public int ShipType { get; set; }
        float speed = 150.0f;
        float deltaX = 0.0f, xLength = 0.0f, xStart = 0.0f;
        bool firingActive = false;
        bool firing = false;
        float fireSpeed = 1.0f;
        float totalTime = 0.0f;
        float radius = 25.0f;

        public int CollisionBall(List<Fireball> fireballList)
        {
            for (int i = 0; i < fireballList.Count; i++)
            {
                if ((position - fireballList[i].Position).Length() < radius)
                    return i;
            }
            return -1;
        }

        public float Radius
        {
            get { return radius; }
        }
        public bool FiringActive
        {
            set { firingActive = value; }
        }
        public bool Firing
        {
            set { firing = value; }
            get { return firing; }
        }
        public Enemy(Texture2D picture, Vector2 startPosition, float speed)
        {
            this.picture = picture;
            position = startPosition;
            this.speed = speed;
        }
        public void SetAcrossMovement(float deltaX, float xLength)
        {
            this.deltaX = deltaX;
            this.xLength = xLength;
            xStart = position.X;
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(picture, position, null, Color.White, 0.0f, new Vector2(25.0f, 25.0f), 1.0f, SpriteEffects.None, 0.0f);
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.X += deltaX * elapsed;
            if (Position.X < xStart - xLength || Position.X > xStart + xLength)
                deltaX *= -1.0f;

            position.Y += speed * elapsed;
            if (firingActive)
            {
                totalTime += elapsed;
                if (totalTime > fireSpeed)
                {
                    totalTime = 0.0f;
                    firing = true;
                }
            }
        }
    }

    class Fireball
    {
        Vector2 position;
        Texture2D picture;
        float speed;

        public Fireball(Texture2D firePicture, Vector2 startPosition, float updateSpeed)
        {
            picture = firePicture;
            position = startPosition;
            speed = updateSpeed;
        }
        public Vector2 Position { get { return position; } }
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(picture, position, null, Color.White, 0.0f, new Vector2(10.0f, 10.0f), 1.0f, SpriteEffects.None, 1.0f);
        }
        public void AmmoUpdate(GameTime gameTime)
        {
            position.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Update(GameTime gameTime)
        {
            AmmoUpdate(gameTime);
        }
    }
}
