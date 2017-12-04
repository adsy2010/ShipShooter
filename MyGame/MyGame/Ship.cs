using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame
{
    class Ship
    {
        private float PosX;
        private float PosY;
        private float Speed;
        private int Weapon = 0;

        public void IncreaseSpeed()
        {
            Speed += 10;
        }

        public void DecreaseSpeed()
        {
            Speed -= 10;
        }

        public void MoveLeft()
        {
            PosX -= 10;
        }

        public void MoveRight()
        {
            PosX += 10;
        }

        public void MoveForward()
        {
            PosY -= 10;
        }
        public void MoveBackward()
        {
            PosY += 10;
        }
        public void SetWeapon(int weapontype)
        {
            Weapon = weapontype;
        }
    }

    class Fighter : Ship
    {
        private int ammo = 20;

        public void FireCannon()
        {
            ammo--;
        }

    }

    class Frigate : Ship
    {
        private int ammo = 20;

        public void FireLaser()
        {
            ammo--;
        }

    }

    class Battlecruiser : Ship
    {
        private int ammo = 20;

        public void FireTorpedo()
        {
            ammo--;
        }

    }
}
