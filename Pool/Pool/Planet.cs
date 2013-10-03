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
    class Planet
    {
        public Texture2D sprite;
        public Vector2 position, velocity, next_velocity;
        public float mass;
        public float radius;
        public int owner; // 0 for nobody, 1 for P1 (red), 2 for P2 (blue)
        public double ownerTimeLeft; // After having ownership set to red or blue, it goes back to neutral after a specified time
        public bool cue; // Is this planet a cue ball?
        public bool polarized; // Is the planet currently affected by the gravity polarizing effect?
        public bool phased; // If true, this planet just passes through others without colliding
        public double phaseTimeLeft; // Time left until the planet collides like normal again
        public bool gravityOff; // If true, this planet is not affected by others gravitationally, and doesn't affect them either
        public double gravityOffTimeLeft; // Time left until gravitational effects turn back on for this planet
        public bool canExitPhase; // If true, it's safe to exit the collision cancelling effect -- if not, this planet is currently "inside" another one, so don't cancel just yet!
        public Planet(Texture2D sprite, Vector2 position, float mass, float radius, int owner, bool cue)
        {
            this.sprite = sprite;
            this.velocity = new Vector2(0);
            this.next_velocity = new Vector2(0);
            this.position = position;
            this.mass = mass;
            this.radius = radius;
            this.owner = owner;
            this.cue = cue;
            this.polarized = false;
            this.phased = false;
            this.phaseTimeLeft = 0;
            this.gravityOff = false;
            this.gravityOffTimeLeft = 0;
            this.canExitPhase = true;
        }
        public void updateVelocity(GameTime gameTime, LinkedList<Planet> planets, LinkedList<Planet> black_holes, LinkedList<Planet> white_holes)
        {
            float polarization; // To take care of gravity polarization effects
            Vector2 accel = new Vector2(0);
            foreach (Planet planet in planets)
            {
                if (this == planet || planet.gravityOff || this.gravityOff) // If either planet has gravity effects currently off, ignoring their effect on each other
                    continue;
                polarization = (planet.polarized) ? -1.0f : 1.0f; // If other planet has the gravity polarization effect turned on, negate its gravitational effect on this planet
                accel.X += (Constants.G * polarization * planet.mass * (planet.position.X - this.position.X)) / Vector2.DistanceSquared(this.position, planet.position);
                accel.Y += (Constants.G * polarization * planet.mass * (planet.position.Y - this.position.Y)) / Vector2.DistanceSquared(this.position, planet.position);
            }
            if (!this.gravityOff)
            {
                foreach (Planet bh in black_holes)
                {
                    accel.X += (Constants.G * bh.mass * (bh.position.X - this.position.X)) / Vector2.DistanceSquared(this.position, bh.position);
                    accel.Y += (Constants.G * bh.mass * (bh.position.Y - this.position.Y)) / Vector2.DistanceSquared(this.position, bh.position);
                }
                foreach (Planet wh in white_holes)
                {
                    accel.X += (Constants.G * wh.mass * (wh.position.X - this.position.X)) / Vector2.DistanceSquared(this.position, wh.position);
                    accel.Y += (Constants.G * wh.mass * (wh.position.Y - this.position.Y)) / Vector2.DistanceSquared(this.position, wh.position);
                }
            }
            this.velocity = this.next_velocity + (gameTime.ElapsedGameTime.Ticks * accel);
            this.next_velocity = this.velocity;
        }
        public void updatePosition(GameTime gameTime)
        {
            // Simulating a speed limit, i.e. speed of light
            float effective_velocity = (float)(Math.Atan(Constants.MAX_PLANET_SPEED_COEF * velocity.Length()) / Constants.MAX_PLANET_SPEED_COEF);
            Vector2 eff_V = new Vector2(velocity.X, velocity.Y);
            Vector2 eff_V2 = eff_V * (effective_velocity / eff_V.Length());
            position += (gameTime.ElapsedGameTime.Ticks * eff_V2);

            // If the planet is outside the edges of the screen, accelerate it towards the center, instead of just bouncing off the edges
            if (position.X < radius)
                next_velocity.X += Constants.WALL_BOUNCE;
            else if (position.X > Constants.WORLD_WIDTH - radius - 1)
                next_velocity.X -= Constants.WALL_BOUNCE;
            if (position.Y < radius)
                next_velocity.Y += Constants.WALL_BOUNCE;
            else if (position.Y > Constants.WORLD_HEIGHT - radius - 1)
                next_velocity.Y -= Constants.WALL_BOUNCE;
            canExitPhase = true; // Not in a collision (so is allowed to exit the collision-cancelling effect) unless proven otherwise...
        }

        public void handleCountdowns(GameTime gameTime)
        {
            // Take care of the various countdowns of planet effects here
            if (!cue && owner > 0)
                ownerTimeLeft -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!cue && ownerTimeLeft <= 0)
                updateOwner(0);

            if (phased)
            {
                phaseTimeLeft -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (phaseTimeLeft <= 0 && canExitPhase)
                {
                    phased = false;
                    phaseTimeLeft = 0;
                }
            }

            if (gravityOff)
            {
                gravityOffTimeLeft -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (gravityOffTimeLeft <= 0)
                {
                    gravityOff = false;
                    gravityOffTimeLeft = 0;
                }
            }

        }
        public void inducePhase() // For a set period of time, this planet will pass through others without colliding
        {
            phased = true;
            phaseTimeLeft = Constants.PHASE_COUNTDOWN_MS;
        }
        public void turnGravityOff() // For a set period of time, this planet will not affect or be affected by other planets, gravitationally
        {
            gravityOff = true;
            gravityOffTimeLeft = Constants.GRAVITY_OFF_COUNTDOWN_MS;
        }
        public bool intersects(Planet other)
        {
            if (Vector2.DistanceSquared(this.position, other.position) <= Math.Pow((double)(this.radius + other.radius), 2))
                return true;
            return false;
        }
        public void speedUp(GameTime gameTime, Vector2 direction, float shipSlowed) // Only used by the cue balls, when the laser hits them
        {
            next_velocity += Constants.LASER_COEF * shipSlowed * gameTime.ElapsedGameTime.Ticks * direction;
        }
        public void updateOwner(int owner) // 0 for none, 1 for P1 (red), 2 for P2 (blue) -- should NEVER be called on cues
        {
            this.owner = owner;
            if (owner == 1)
                this.sprite = Game1.redPlanet;
            else if (owner == 2)
                this.sprite = Game1.bluePlanet;
            else
                this.sprite = Game1.grayPlanet;
            if (owner > 0)
                this.ownerTimeLeft = Constants.OWNER_TIMEOUT_MS;
            else
                this.ownerTimeLeft = 0;
        }
        public void collideWith(Planet other)
        {
            // Transfer ownership where necessary
            if (this.cue && !other.cue)
                other.updateOwner(this.owner);
            else if (!this.cue && other.cue)
                this.updateOwner(other.owner);
            else if (this.cue && other.cue) { } // Two cues colliding should do nothing
            else if ((this.owner == 1 && other.owner == 2) || (this.owner == 2 && other.owner == 1))
            {
                this.updateOwner(0);
                other.updateOwner(0);
            }
            else if (this.owner == 0 && other.owner == 1)
            {
                this.updateOwner(1);
            }
            else if (this.owner == 1 && other.owner == 0)
            {
                other.updateOwner(1);
            }
            else if (this.owner == 0 && other.owner == 2)
            {
                this.updateOwner(2);
            }
            else if (this.owner == 2 && other.owner == 0)
            {
                other.updateOwner(2);
            }

            // If either planet was hit by the phase inducer, exit out without running any of the collision code
            if (this.phased || other.phased)
            {
                if (this.phased) this.canExitPhase = false; // The two planets are currently colliding, so don't let them cancel their collision-cancelling effect yet
                if (other.phased) other.canExitPhase = false;
                return;
            }

            // Force the planets apart so they're just barely touching
            double H = this.radius + other.radius - Vector2.Distance(this.position, other.position);
            double a = Math.Atan2(this.position.Y - other.position.Y, this.position.X - other.position.X);
            this.position.X += (float)(((H / 2)) * Math.Cos(a));
            this.position.Y += (float)(((H / 2)) * Math.Sin(a));
            other.position.X -= (float)(((H / 2)) * Math.Cos(a));
            other.position.Y -= (float)(((H / 2)) * Math.Sin(a));

            // Then, do the actual collision
            double theta = Math.Atan2(this.position.Y - other.position.Y, this.position.X - other.position.X);

            double thisv1 = (this.velocity).Length();
            double otherv1 = (other.velocity).Length();

            double thisangle = Math.Atan2(this.velocity.Y, this.velocity.X);
            double otherangle = Math.Atan2(other.velocity.Y, other.velocity.X);

            double thisxvA = thisv1 * Math.Cos(thisangle - theta);

            double thisyvF = thisv1 * Math.Sin(thisangle - theta);

            double otherxvA = otherv1 * Math.Cos(otherangle - theta);

            double otheryvF = otherv1 * Math.Sin(otherangle - theta);

            double thisxvF = ((this.mass - other.mass) * thisxvA + (2.0 * other.mass) * otherxvA) / (this.mass + other.mass);
            double otherxvF = ((this.mass * 2.0) * thisxvA + (other.mass - this.mass) * otherxvA) / (this.mass + other.mass);

            this.next_velocity.X = (float)(Math.Cos(theta) * thisxvF + Math.Cos(theta + Math.PI / 2) * thisyvF);
            this.next_velocity.Y = (float)(Math.Sin(theta) * thisxvF + Math.Sin(theta + Math.PI / 2) * thisyvF);

            other.next_velocity.X = (float)(Math.Cos(theta) * otherxvF + Math.Cos(theta + Math.PI / 2) * otheryvF);
            other.next_velocity.Y = (float)(Math.Sin(theta) * otherxvF + Math.Sin(theta + Math.PI / 2) * otheryvF);
        }
    }
}
