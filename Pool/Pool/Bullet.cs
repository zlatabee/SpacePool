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
    class Bullet
    {
        public Texture2D sprite;
        public Vector2 position, velocity;
        public int radius, weaponMode;
        public bool owner; // True for P1, False for P2
        public Bullet(Vector2 position, Vector2 velocity, int weaponMode, bool owner)
        {
            this.position = position;
            this.velocity = velocity;
            this.radius = Constants.BULLET_RADIUS;
            this.owner = owner;
            this.weaponMode = weaponMode;
            switch (weaponMode)
            {
                case 3:
                    this.sprite = Game1.bullet3;
                    break;
                case 4:
                    this.sprite = Game1.bullet4;
                    break;
                case 5:
                    this.sprite = Game1.bullet5;
                    break;
                case 6:
                    this.sprite = Game1.bullet6;
                    break;
            }
        }
        public bool updatePosition(GameTime gameTime) // Returns true if bullet has escaped world bounds, false if not
        {
            position += gameTime.ElapsedGameTime.Ticks * velocity;
            if (position.X < Constants.BULLET_RADIUS ||
                position.X > Constants.WORLD_WIDTH - Constants.BULLET_RADIUS - 1 ||
                position.Y < Constants.BULLET_RADIUS ||
                position.Y > Constants.WORLD_HEIGHT - Constants.BULLET_RADIUS - 1)
                return true;
            return false;
        }
        public bool handleCollisions(LinkedList<Planet> planets, Ship opponent) // Return true if the bullet hit something
        {
            LinkedListNode<Planet> planetNode, nextPlanet;
            switch (weaponMode)
            {
                case 3: // Slow opponent's ship down
                    if (this.intersects(opponent))
                    {
                        opponent.slowDown();
                        return true;
                    }
                    return false;
                case 4: // Gravity polarizer
                    planetNode = planets.First;
                    while (planetNode != null)
                    {
                        nextPlanet = planetNode.Next;
                        if (this.intersects(planetNode.Value))
                        {
                            planetNode.Value.polarized = !planetNode.Value.polarized;
                            return true;
                        }
                        planetNode = nextPlanet;
                    }
                    return false;
                case 5: // Phase inducer, makes the planet immune to collisions
                    planetNode = planets.First;
                    while (planetNode != null)
                    {
                        nextPlanet = planetNode.Next;
                        if (this.intersects(planetNode.Value))
                        {
                            planetNode.Value.inducePhase();
                            return true;
                        }
                        planetNode = nextPlanet;
                    }
                    return false;
                case 6: // Turn gravitational effects (to and from other planets) off for a planet
                    planetNode = planets.First;
                    while (planetNode != null)
                    {
                        nextPlanet = planetNode.Next;
                        if (this.intersects(planetNode.Value))
                        {
                            planetNode.Value.turnGravityOff();
                            return true;
                        }
                        planetNode = nextPlanet;
                    }
                    return false;
            }
            return false;
        }
        public bool intersects(Planet p)
        {
            if (Vector2.DistanceSquared(this.position, p.position) <= Math.Pow((double)(this.radius + p.radius), 2))
                return true;
            return false;
        }
        public bool intersects(Ship opponent)
        {
            if (Vector2.DistanceSquared(
                this.position, 
                opponent.position + Constants.SHIP_WIDTH * new Vector2(opponent.laser.Direction.X, opponent.laser.Direction.Y)) 
                <= Math.Pow((double)(this.radius + Constants.SHIP_WIDTH), 2))
                return true;
            return false;
        }
    }
}
