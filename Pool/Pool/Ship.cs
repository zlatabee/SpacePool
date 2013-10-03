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

namespace Pool
{
    class Ship
    {
        public Texture2D sprite;
        public Rectangle body;
        public double angle; // Where the ship is facing
        public Ray laser;
        public bool laserOn;
        public int weaponMode;
        public int weaponsAvailable;
        public Nullable<float> laserDist;
        public Vector2 position, velocity;
        public Matrix transform;
        public int score;
        public bool owner; // True for P1, False for P2
        public double slowDownCountdown;
        public bool slowed;
        public Ship(Texture2D sprite, Rectangle body, Vector2 position, bool owner)
        {
            this.sprite = sprite;
            this.body = body;
            this.angle = 0;
            this.position = position;
            this.velocity = new Vector2(0);
            Vector2 laserPos = new Vector2(Constants.SHIP_WIDTH / 2.0f, 0);
            Matrix transform =
                Matrix.CreateTranslation(new Vector3(-1 * new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 0.0f)) *
                Matrix.CreateRotationZ((float)angle) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));
            Vector2.Transform(ref laserPos, ref transform, out laserPos);
            this.laser = new Ray(
                new Vector3(laserPos, 0),
                new Vector3((float)Math.Sin(angle), -1 * (float)Math.Cos(angle), 0));
            this.laserOn = false;
            this.laserDist = null;
            this.weaponMode = 0;
            this.weaponsAvailable = Constants.TOTAL_WEAPONS;
            this.score = 0;
            this.owner = owner;
            this.slowDownCountdown = 0;
            this.slowed = false;
        }
        public void updatePosition(GameTime gameTime)
        {
            position.X += (slowed ? 0.5f : 1f) * velocity.X;
            position.Y -= (slowed ? 0.5f : 1f) * velocity.Y;
            if (position.X < 0)
                position.X += Constants.WORLD_WIDTH;
            if (position.Y < 0)
                position.Y += Constants.WORLD_HEIGHT;
            position.X %= Constants.WORLD_WIDTH;
            position.Y %= Constants.WORLD_HEIGHT;
            transform =
                Matrix.CreateTranslation(new Vector3(-1 * new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 0.0f)) *
                Matrix.CreateRotationZ((float)angle) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));
            body = CalculateBoundingRectangle(
                new Rectangle(0, 0, Constants.SHIP_WIDTH, Constants.SHIP_HEIGHT),
                transform);
            velocity.X *= 0.8f;
            velocity.Y *= 0.8f;
            Vector2 laserPos = new Vector2(Constants.SHIP_WIDTH / 2.0f, 0);
            Vector2.Transform(ref laserPos, ref transform, out laserPos);
            this.laser = new Ray(
                new Vector3(laserPos, 0),
                new Vector3((float)Math.Sin(angle), -1 * (float)Math.Cos(angle), 0));
            if (slowed)
            {
                slowDownCountdown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (slowDownCountdown <= 0)
                {
                    slowDownCountdown = 0;
                    slowed = false;
                }
            }
        }
        public void rotateRoughCW()
        {
            angle += (slowed ? Constants.SHIP_SLOW_COEF : 1) * Constants.SHIP_ROTATE_ROUGH;
        }
        public void rotateRoughCCW()
        {
            angle -= (slowed ? Constants.SHIP_SLOW_COEF : 1) * Constants.SHIP_ROTATE_ROUGH;
        }
        public void rotateFineCW()
        {
            angle += (slowed ? Constants.SHIP_SLOW_COEF : 1) * Constants.SHIP_ROTATE_FINE;
        }
        public void rotateFineCCW()
        {
            angle -= (slowed ? Constants.SHIP_SLOW_COEF : 1) * Constants.SHIP_ROTATE_FINE;
        }
        public void moveForward()
        {
            velocity.X += (float)((slowed ? Constants.SHIP_SLOW_COEF : 1f) * (Constants.SHIP_SPEED * Math.Sin(angle)));
            velocity.Y += (float)((slowed ? Constants.SHIP_SLOW_COEF : 1f) * (Constants.SHIP_SPEED * Math.Cos(angle)));
        }
        public void switchWeapon(bool direction)
        {
            if (direction)
                weaponMode++;
            else
            {
                weaponMode--;
                if (weaponMode == -1)
                    weaponMode = weaponsAvailable - 1;
            }
            weaponMode %= weaponsAvailable;
        }
        public void slowDown()
        {
            slowed = true;
            slowDownCountdown = Constants.SHIP_SLOW_COUNTDOWN_MS;
        }
        public void fireWeapon(GameTime gameTime, LinkedList<Planet> planets, LinkedList<Bullet> bullets, Planet cue, Ship opponent)
        {
            if (weaponMode < 3) // If the weapon is a laser
                laserOn = true;
            else // If the weapon is a bullet
                laserOn = false;
            float lDist = Vector2.Distance(new Vector2(0.0f), new Vector2(500.0f));
            Nullable<float> pDist;
            Planet pHit = null;
            switch (weaponMode)
            {
                case 0: // Shoot at the cue
                    BoundingSphere planetSphere = new BoundingSphere(
                        new Vector3(cue.position, 0.0f),
                        (float)cue.radius);
                    laserDist = laser.Intersects(planetSphere);
                    if (laserDist != null)
                        cue.speedUp(gameTime, new Vector2(laser.Direction.X, laser.Direction.Y), (float)(slowed ? Constants.SHIP_SLOW_COEF : 1));
                    break;
                case 1: // Make planet heavier
                    foreach (Planet p in planets)
                    {
                        BoundingSphere pSphere = new BoundingSphere(
                            new Vector3(p.position, 0.0f),
                            (float)p.radius);
                        pDist = laser.Intersects(pSphere);
                        if (pDist != null && pDist < lDist)
                        {
                            lDist = (float)pDist;
                            pHit = p;
                        }
                    }
                    laserDist = lDist;
                    if (pHit != null)
                        pHit.mass = Math.Min(Constants.PLANET_MASS_MULTIPLIER * pHit.mass, Constants.MAX_MASS_INCREASE * Constants.PLANET_MASS_DEFAULT);
                    break;
                case 2: // Make planet lighter
                    foreach (Planet p in planets)
                    {
                        BoundingSphere pSphere = new BoundingSphere(
                            new Vector3(p.position, 0.0f),
                            (float)p.radius);
                        pDist = laser.Intersects(pSphere);
                        if (pDist != null && pDist < lDist)
                        {
                            lDist = (float)pDist;
                            pHit = p;
                        }
                    }
                    laserDist = lDist;
                    if (pHit != null)
                        pHit.mass = Math.Max((1.0f / Constants.PLANET_MASS_MULTIPLIER) * pHit.mass, Constants.MAX_MASS_DECREASE * Constants.PLANET_MASS_DEFAULT);
                    break;
                case 3:
                case 4:
                case 5:
                case 6: // Fire a bullet to do... something
                    Bullet b = new Bullet(
                        new Vector2(laser.Position.X, laser.Position.Y), 
                        Constants.BULLET_SPEED * new Vector2(laser.Direction.X, laser.Direction.Y), 
                        weaponMode,
                        owner);
                    bullets.AddLast(b);
                    break;

            }
        }
        public Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}
