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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        KeyboardState previousKeyboard = Keyboard.GetState();

        static LinkedList<Planet> planets = new LinkedList<Planet>();
        static LinkedList<Planet> new_planets = new LinkedList<Planet>();
        static LinkedList<Planet> black_holes = new LinkedList<Planet>();
        static LinkedList<Planet> white_holes = new LinkedList<Planet>();
        static LinkedList<Bullet> bullets = new LinkedList<Bullet>();

        static Vector2[][] black_hole_layouts, white_hole_layouts;
        static Vector2[] P1_start_positions, P2_start_positions;

        static bool running = true; // Is the game running?
        static bool P1_won = false; // Did Player One win?
        static bool P2_won = false; // Did Player Two win?
        static bool tie = false; // Was there a tie?

        static double timeBombTimeLeft = 0; // Time left in the Time Bomb effect, if it's currently on
        static int timeBombsSoFar; // Time bombs set off so far in this game

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Sprites
        static public Texture2D redPlanet, bluePlanet, grayPlanet, redCue, blueCue, blackHoleSprite, whiteHoleSprite, redShip, blueShip, laserSprite, bulletSprite;
        static public Texture2D bullet3, bullet4, bullet5, bullet6;

        // Sprite effects/overlays
        static public Texture2D heavierEffect, lighterEffect, greenOutline, yellowOutline;

        // Ship auras (indicate which weapon is currently selected)
        static public Texture2D aura1, aura2, aura3, aura4, aura5, aura6;

        // Message boxes
        static public Texture2D P1Win, P2Win, Tie;

        // Players and their cue balls
        static Ship P1, P2;
        static Planet P1Cue, P2Cue;

        // Current "level" or arrangement of black holes and white holes
        static int currentLevel = -1;

        // Random number generator for random positions/velocities/etc;
        static Random ran;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = Constants.WORLD_WIDTH;
            graphics.PreferredBackBufferHeight = Constants.WORLD_HEIGHT;

            IsMouseVisible = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            ran = new Random();

            black_hole_layouts = new Vector2[Constants.TOTAL_LAYOUTS][];
            white_hole_layouts = new Vector2[Constants.TOTAL_LAYOUTS][];
            P1_start_positions = new Vector2[Constants.TOTAL_LAYOUTS];
            P2_start_positions = new Vector2[Constants.TOTAL_LAYOUTS];

            // Level 1
            P1_start_positions[0] = new Vector2(50, 300);
            P2_start_positions[0] = new Vector2(1150, 300);
            black_hole_layouts[0] = new Vector2[1];
            white_hole_layouts[0] = new Vector2[0];

            black_hole_layouts[0][0] = new Vector2(600, 300);

            // Level 2
            P1_start_positions[1] = new Vector2(500, 300);
            P2_start_positions[1] = new Vector2(700, 300);
            black_hole_layouts[1] = new Vector2[6];
            white_hole_layouts[1] = new Vector2[0];

            black_hole_layouts[1][0] = new Vector2(50, 50);
            black_hole_layouts[1][1] = new Vector2(50, 550);
            black_hole_layouts[1][2] = new Vector2(600, 50);
            black_hole_layouts[1][3] = new Vector2(1150, 50);
            black_hole_layouts[1][4] = new Vector2(1150, 550);
            black_hole_layouts[1][5] = new Vector2(600, 550);

            // Level 3
            P1_start_positions[2] = new Vector2(50, 300);
            P2_start_positions[2] = new Vector2(1150, 300);
            black_hole_layouts[2] = new Vector2[5];
            white_hole_layouts[2] = new Vector2[16];

            black_hole_layouts[2][0] = new Vector2(50, 50);
            black_hole_layouts[2][1] = new Vector2(50, 550);
            black_hole_layouts[2][2] = new Vector2(600, 300);
            black_hole_layouts[2][3] = new Vector2(1150, 50);
            black_hole_layouts[2][4] = new Vector2(1150, 550);

            white_hole_layouts[2][0] = new Vector2(50, 100);
            white_hole_layouts[2][1] = new Vector2(80, 80);
            white_hole_layouts[2][2] = new Vector2(100, 50);
            white_hole_layouts[2][3] = new Vector2(1150, 100);
            white_hole_layouts[2][4] = new Vector2(1120, 80);
            white_hole_layouts[2][5] = new Vector2(1100, 50);
            white_hole_layouts[2][6] = new Vector2(100, 550);
            white_hole_layouts[2][7] = new Vector2(50, 500);
            white_hole_layouts[2][8] = new Vector2(80, 520);
            white_hole_layouts[2][9] = new Vector2(1150, 500);
            white_hole_layouts[2][10] = new Vector2(1100, 550);
            white_hole_layouts[2][11] = new Vector2(1120, 520);
            white_hole_layouts[2][12] = new Vector2(600, 250);
            white_hole_layouts[2][13] = new Vector2(600, 350);
            white_hole_layouts[2][14] = new Vector2(650, 300);
            white_hole_layouts[2][15] = new Vector2(550, 300);

            // Level 4
            P1_start_positions[3] = new Vector2(600, 100);
            P2_start_positions[3] = new Vector2(600, 500);
            black_hole_layouts[3] = new Vector2[2];
            white_hole_layouts[3] = new Vector2[6];

            black_hole_layouts[3][0] = new Vector2(200, 300);
            black_hole_layouts[3][1] = new Vector2(1000, 300);

            white_hole_layouts[3][0] = new Vector2(300, 300);
            white_hole_layouts[3][1] = new Vector2(900, 300);
            white_hole_layouts[3][2] = new Vector2(1100, 200);
            white_hole_layouts[3][3] = new Vector2(1100, 400);
            white_hole_layouts[3][4] = new Vector2(100, 400);
            white_hole_layouts[3][5] = new Vector2(100, 200);

            // Level 5
            P1_start_positions[4] = new Vector2(1050, 150);
            P2_start_positions[4] = new Vector2(1050, 450);
            black_hole_layouts[4] = new Vector2[3];
            white_hole_layouts[4] = new Vector2[52];

            int white_index = 0;
            int black_index = 0;
            for (int x = 100; x <= 1100; x += 100)
            {
                for (int y = 100; y <= 500; y += 100)
                {
                    if ((x == 100 && y == 100) || (x == 100 && y == 500) || (x == 1100 && y == 300))
                    {
                        black_hole_layouts[4][black_index] = new Vector2(x, y);
                        black_index++;
                    }
                    else
                    {
                        white_hole_layouts[4][white_index] = new Vector2(x, y);
                        white_index++;
                    }
                }
            }

            // Level 6
            P1_start_positions[5] = new Vector2(150, 300);
            P2_start_positions[5] = new Vector2(1050, 300);
            black_hole_layouts[5] = new Vector2[2];
            white_hole_layouts[5] = new Vector2[9];

            black_hole_layouts[5][0] = new Vector2(300, 300);
            black_hole_layouts[5][1] = new Vector2(900, 300);

            white_hole_layouts[5][0] = new Vector2(600, 60);
            white_hole_layouts[5][1] = new Vector2(600, 120);
            white_hole_layouts[5][2] = new Vector2(600, 180);
            white_hole_layouts[5][3] = new Vector2(600, 240);
            white_hole_layouts[5][4] = new Vector2(600, 300);
            white_hole_layouts[5][5] = new Vector2(600, 360);
            white_hole_layouts[5][6] = new Vector2(600, 420);
            white_hole_layouts[5][7] = new Vector2(600, 480);
            white_hole_layouts[5][8] = new Vector2(600, 540);

            // Level 7
            P1_start_positions[6] = new Vector2(600, 150);
            P2_start_positions[6] = new Vector2(600, 450);
            black_hole_layouts[6] = new Vector2[9];
            white_hole_layouts[6] = new Vector2[0];

            black_hole_layouts[6][0] = new Vector2(300, 300);
            black_hole_layouts[6][1] = new Vector2(600, 300);
            black_hole_layouts[6][2] = new Vector2(900, 300);
            black_hole_layouts[6][3] = new Vector2(25, 25);
            black_hole_layouts[6][4] = new Vector2(600, 25);
            black_hole_layouts[6][5] = new Vector2(1175, 25);
            black_hole_layouts[6][6] = new Vector2(25, 575);
            black_hole_layouts[6][7] = new Vector2(600, 575);
            black_hole_layouts[6][8] = new Vector2(1175, 575);

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

            // Load sprites
            redPlanet = Content.Load<Texture2D>(@"Sprites/redPlanet");
            bluePlanet = Content.Load<Texture2D>(@"Sprites/bluePlanet");
            grayPlanet = Content.Load<Texture2D>(@"Sprites/grayPlanet");
            redCue = Content.Load<Texture2D>(@"Sprites/redCue");
            blueCue = Content.Load<Texture2D>(@"Sprites/blueCue");
            blackHoleSprite = Content.Load<Texture2D>(@"Sprites/blackHoleSprite");
            whiteHoleSprite = Content.Load<Texture2D>(@"Sprites/whiteHoleSprite");
            redShip = Content.Load<Texture2D>(@"Sprites/redShip");
            blueShip = Content.Load<Texture2D>(@"Sprites/blueShip");
            laserSprite = Content.Load<Texture2D>(@"Sprites/laserSprite");

            bullet3 = Content.Load<Texture2D>(@"Sprites/bullet3");
            bullet4 = Content.Load<Texture2D>(@"Sprites/bullet4");
            bullet5 = Content.Load<Texture2D>(@"Sprites/bullet5");
            bullet6 = Content.Load<Texture2D>(@"Sprites/bullet6");

            // Load sprite effects/overlays
            heavierEffect = Content.Load<Texture2D>(@"Effects/heavierEffect");
            lighterEffect = Content.Load<Texture2D>(@"Effects/lighterEffect");
            greenOutline = Content.Load<Texture2D>(@"Effects/greenOutline");
            yellowOutline = Content.Load<Texture2D>(@"Effects/yellowOutline");

            // Load ship auras
            aura1 = Content.Load<Texture2D>(@"Effects/Aura1");
            aura2 = Content.Load<Texture2D>(@"Effects/Aura2");
            aura3 = Content.Load<Texture2D>(@"Effects/Aura3");
            aura4 = Content.Load<Texture2D>(@"Effects/Aura4");
            aura5 = Content.Load<Texture2D>(@"Effects/Aura5");
            aura6 = Content.Load<Texture2D>(@"Effects/Aura6");

            // Load text (message boxes)
            P1Win = Content.Load<Texture2D>(@"Text/P1Win");
            P2Win = Content.Load<Texture2D>(@"Text/P2Win");
            Tie = Content.Load<Texture2D>(@"Text/Tie");

            // Set up the game for the first time
            Reset();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
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
            KeyboardState keyboard = Keyboard.GetState();

            // Allows the game to exit
            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keyboard.IsKeyDown(Keys.R) && previousKeyboard.IsKeyUp(Keys.R)) // Reset the game
                Reset();
            if (!running) // No need to calculate anything if the game isn't running
                return;

            // Interpret key presses

            // For the time bomb...
            if (keyboard.IsKeyDown(Keys.T) && previousKeyboard.IsKeyUp(Keys.T) && timeBombTimeLeft == 0)
                timeBomb(true);

            // For player two...
            if (keyboard.IsKeyDown(Keys.J)) // Rotate Player Two's ship counter-clockwise
                P2.rotateRoughCCW();
            if (keyboard.IsKeyDown(Keys.L)) // Rotate Player Two's ship clockwise
                P2.rotateRoughCW();
            if (keyboard.IsKeyDown(Keys.U)) // Rotate Player Two's ship counter-clockwise (finely)
                P2.rotateFineCCW();
            if (keyboard.IsKeyDown(Keys.O)) // Rotate Player Two's ship clockwise (finely)
                P2.rotateFineCW();
            if (keyboard.IsKeyDown(Keys.OemQuestion) && (P2.weaponMode < 3 || previousKeyboard.IsKeyUp(Keys.OemQuestion))) // Fire Player Two's weapon
                P2.fireWeapon(gameTime, planets, bullets, P2Cue, P1);
            P2.laserOn = (keyboard.IsKeyDown(Keys.OemQuestion) && P2.weaponMode < 3) ? true : false;
            if (keyboard.IsKeyDown(Keys.I) && P2.velocity.Length() < Constants.MAX_SHIP_SPEED) // Move Player Two's ship forward
                P2.moveForward();
            if (keyboard.IsKeyDown(Keys.H) && previousKeyboard.IsKeyUp(Keys.H)) // Flip through possible weapons in one direction...
                P2.switchWeapon(false);
            if (keyboard.IsKeyDown(Keys.OemSemicolon) && previousKeyboard.IsKeyUp(Keys.OemSemicolon)) // ...and in the other direction
                P2.switchWeapon(true);

            // ...and for player one
            if (keyboard.IsKeyDown(Keys.A)) // Rotate Player One's ship counter-clockwise
                P1.rotateRoughCCW();
            if (keyboard.IsKeyDown(Keys.D)) // Rotate Player One's ship clockwise
                P1.rotateRoughCW();
            if (keyboard.IsKeyDown(Keys.Q)) // Rotate Player One's ship counter-clockwise (finely)
                P1.rotateFineCCW();
            if (keyboard.IsKeyDown(Keys.E)) // Rotate Player One's ship clockwise (finely)
                P1.rotateFineCW();
            if (keyboard.IsKeyDown(Keys.Space) && (P1.weaponMode < 3 || previousKeyboard.IsKeyUp(Keys.Space))) // Fire Player One's weapon
                P1.fireWeapon(gameTime, planets, bullets, P1Cue, P2);
            P1.laserOn = (keyboard.IsKeyDown(Keys.Space) && P1.weaponMode < 3) ? true : false;
            if (keyboard.IsKeyDown(Keys.W) && P1.velocity.Length() < Constants.MAX_SHIP_SPEED) // Move Player One's ship forward
                P1.moveForward();
            if (keyboard.IsKeyDown(Keys.CapsLock) && previousKeyboard.IsKeyUp(Keys.CapsLock)) // Flip through possible weapons in one direction...
                P1.switchWeapon(false);
            if (keyboard.IsKeyDown(Keys.F) && previousKeyboard.IsKeyUp(Keys.F)) // ...and in the other direction
                P1.switchWeapon(true);

            // Take care of the Time Bomb countdown, if relevant
            if (timeBombTimeLeft > 0)
            {
                timeBombTimeLeft -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timeBombTimeLeft <= 0) // If the countdown is complete, turn off the effect
                {
                    timeBombTimeLeft = 0;
                    timeBomb(false);
                }
            }

            // Update accel, velocity, and positions

            P1.updatePosition(gameTime);
            P2.updatePosition(gameTime);

            foreach (Planet planet in planets)
                planet.updateVelocity(gameTime, planets, black_holes, white_holes);

            foreach (Planet planet in planets)
                planet.updatePosition(gameTime);

            // Did either player win the game yet?
            if (P1.score >= Constants.ENDING_SCORE || P2.score >= Constants.ENDING_SCORE)
            {
                running = false;
                if (P1.score > P2.score)
                    P1_won = true;
                else if (P1.score < P2.score)
                    P2_won = true;
                else
                    tie = true;
            }

            // Check for collisions
            LinkedListNode<Planet> planetNode = planets.First;
            LinkedListNode<Planet> blackHoleNode, otherPlanetNode, nextPlanet, nextBlackHole, nextOtherPlanet;
            while (planetNode != null)
            {
                // Check if this planet hit any black holes or other planets
                nextPlanet = planetNode.Next;
                blackHoleNode = black_holes.First;
                otherPlanetNode = planetNode.Next;
                while (otherPlanetNode != null)
                {
                    nextOtherPlanet = otherPlanetNode.Next;
                    if (planetNode.Value.intersects(otherPlanetNode.Value)) // Planet hit other planet
                        planetNode.Value.collideWith(otherPlanetNode.Value);
                    otherPlanetNode = nextOtherPlanet;
                }
                while (!planetNode.Value.cue && blackHoleNode != null)
                {
                    nextBlackHole = blackHoleNode.Next;
                    if (planetNode.Value.intersects(blackHoleNode.Value)) // Planet hit the black hole, remove it from existence
                    {
                        if (planetNode.Value.owner == 1)
                            P1.score++;
                        else if (planetNode.Value.owner == 2)
                            P2.score++;
                        planets.Remove(planetNode);
                        double ran_angle = 2 * Math.PI * ran.NextDouble();
                        float ran_x = (float)(600 * Math.Cos(ran_angle) + Constants.WORLD_WIDTH / 2.0);
                        float ran_y = (float)(600 * Math.Sin(ran_angle) + Constants.WORLD_HEIGHT / 2.0);
                        Planet p = new Planet(
                            grayPlanet,
                            new Vector2(ran_x, ran_y),
                            Constants.PLANET_MASS_DEFAULT,
                            Constants.PLANET_RADIUS,
                            0,
                            false);
                        new_planets.AddLast(p);
                        break;
                    }
                    blackHoleNode = nextBlackHole;
                }
                planetNode = nextPlanet;
            }

            // Any new planets that need to be added, due to old planets being sucked in to black holes?
            foreach (Planet p in new_planets)
                planets.AddLast(p);
            new_planets.Clear();

            // Bullet collisions
            LinkedListNode<Bullet> bulletNode, nextBullet;
            bulletNode = bullets.First;
            while (bulletNode != null)
            {
                nextBullet = bulletNode.Next;
                // Returns true if the bullet escaped past the edge of the screen, or collided with the object it's supposed to collide with (depending on weapon mode)
                if (bulletNode.Value.updatePosition(gameTime) || bulletNode.Value.handleCollisions(planets, bulletNode.Value.owner ? P2 : P1))
                    bullets.Remove(bulletNode);
                bulletNode = nextBullet;
            }

            // Take care of countdowns for various planet effects (such as collisions being off, gravity being off, etc)
            foreach (Planet planet in planets)
                planet.handleCountdowns(gameTime);

            previousKeyboard = keyboard;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (timeBombTimeLeft > 0) // If the Time Bomb is in effect, have a different background color
                GraphicsDevice.Clear(Color.DarkGray); 
            else
                GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // Draw the planets, black holes, and bullets
            foreach (Planet blackHole in black_holes)
                spriteBatch.Draw(blackHole.sprite, new Vector2(blackHole.position.X - blackHole.radius, blackHole.position.Y - blackHole.radius), Color.White * 0.4f);
            foreach (Planet whiteHole in white_holes)
                spriteBatch.Draw(whiteHole.sprite, new Vector2(whiteHole.position.X - whiteHole.radius, whiteHole.position.Y - whiteHole.radius), Color.White * 0.4f);
            foreach (Planet planet in planets)
            {
                spriteBatch.Draw(planet.sprite, new Vector2(planet.position.X - planet.radius, planet.position.Y - planet.radius), Color.White * (planet.phased ? 0.5f : 1.0f));
                if (planet.mass > Constants.PLANET_MASS_DEFAULT)
                    spriteBatch.Draw(heavierEffect, new Vector2(planet.position.X - planet.radius, planet.position.Y - planet.radius), Color.White * 0.5f);
                else if (planet.mass < Constants.PLANET_MASS_DEFAULT)
                    spriteBatch.Draw(lighterEffect, new Vector2(planet.position.X - planet.radius, planet.position.Y - planet.radius), Color.White * 0.5f);
                if (planet.gravityOff)
                    spriteBatch.Draw(greenOutline, new Vector2(planet.position.X - planet.radius, planet.position.Y - planet.radius), Color.White);
                if (planet.polarized)
                    spriteBatch.Draw(yellowOutline, new Vector2(planet.position.X - planet.radius, planet.position.Y - planet.radius), Color.White);
            }
            foreach (Bullet bullet in bullets)
                spriteBatch.Draw(bullet.sprite, new Vector2(bullet.position.X - bullet.radius, bullet.position.Y - bullet.radius), Color.White);
            
            // Draw the players' ships
            spriteBatch.Draw(P1.sprite, P1.position, null, Color.White, (float)P1.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            if (P1.weaponMode == 1)
                spriteBatch.Draw(aura1, P1.position, null, Color.White, (float)P1.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P1.weaponMode == 2)
                spriteBatch.Draw(aura2, P1.position, null, Color.White, (float)P1.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P1.weaponMode == 3)
                spriteBatch.Draw(aura3, P1.position, null, Color.White, (float)P1.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P1.weaponMode == 4)
                spriteBatch.Draw(aura4, P1.position, null, Color.White, (float)P1.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P1.weaponMode == 5)
                spriteBatch.Draw(aura5, P1.position, null, Color.White, (float)P1.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P1.weaponMode == 6)
                spriteBatch.Draw(aura6, P1.position, null, Color.White, (float)P1.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.Draw(P2.sprite, P2.position, null, Color.White, (float)P2.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            if (P2.weaponMode == 1)
                spriteBatch.Draw(aura1, P2.position, null, Color.White, (float)P2.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P2.weaponMode == 2)
                spriteBatch.Draw(aura2, P2.position, null, Color.White, (float)P2.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P2.weaponMode == 3)
                spriteBatch.Draw(aura3, P2.position, null, Color.White, (float)P2.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P2.weaponMode == 4)
                spriteBatch.Draw(aura4, P2.position, null, Color.White, (float)P2.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P2.weaponMode == 5)
                spriteBatch.Draw(aura5, P2.position, null, Color.White, (float)P2.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);
            else if (P2.weaponMode == 6)
                spriteBatch.Draw(aura6, P2.position, null, Color.White, (float)P2.angle, new Vector2(Constants.SHIP_WIDTH / 2, Constants.SHIP_HEIGHT / 2), 1.0f, SpriteEffects.None, 0.0f);

            // Display the lasers (if they're on)
            if (P1.laserOn)
            {
                float laserDist = (P1.laserDist == null) ? Vector2.Distance(new Vector2(0), new Vector2(Constants.WORLD_WIDTH, Constants.WORLD_HEIGHT)) : (float)P1.laserDist;
                spriteBatch.Draw(laserSprite, new Vector2(P1.laser.Position.X, P1.laser.Position.Y), null, Color.White, (float)(P1.angle - (Math.PI / 2.0f)), new Vector2(0.0f, 0.5f), new Vector2(laserDist, 3.0f), SpriteEffects.None, 0.0f);
            }
            if (P2.laserOn)
            {
                float laserDist = (P2.laserDist == null) ? Vector2.Distance(new Vector2(0), new Vector2(Constants.WORLD_WIDTH, Constants.WORLD_HEIGHT)) : (float)P2.laserDist;
                spriteBatch.Draw(laserSprite, new Vector2(P2.laser.Position.X, P2.laser.Position.Y), null, Color.White, (float)(P2.angle - (Math.PI / 2.0f)), new Vector2(0.0f, 0.5f), new Vector2(laserDist, 3.0f), SpriteEffects.None, 0.0f);
            }

            // Display text boxes, if the game is over
            if (P1_won)
                spriteBatch.Draw(
                    P1Win,
                    new Vector2(0.5f * (Constants.WORLD_WIDTH - Constants.MESSAGE_WIDTH), 0.5f * (Constants.WORLD_HEIGHT - Constants.MESSAGE_HEIGHT)),
                    Color.White);
            if (P2_won)
                spriteBatch.Draw(
                    P2Win,
                    new Vector2(0.5f * (Constants.WORLD_WIDTH - Constants.MESSAGE_WIDTH), 0.5f * (Constants.WORLD_HEIGHT - Constants.MESSAGE_HEIGHT)),
                    Color.White);
            if (tie)
                spriteBatch.Draw(
                    Tie,
                    new Vector2(0.5f * (Constants.WORLD_WIDTH - Constants.MESSAGE_WIDTH), 0.5f * (Constants.WORLD_HEIGHT - Constants.MESSAGE_HEIGHT)),
                    Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public static void timeBomb(bool turnOn) // Make time go "backwards" for a few seconds, or turn off the effect
        {
            if (turnOn && timeBombsSoFar >= Constants.TIME_BOMBS_ALLOWED)
                return; // We've already set off the maximum # of allowed time bombs this game!
            if (turnOn)
            {
                timeBombTimeLeft = Constants.TIME_BOMB_COUNTDOWN_MS;
                timeBombsSoFar++;
            }
            else
                timeBombTimeLeft = 0;
            foreach (Planet p in planets) // Fake time going backwards by negating velocities
                p.next_velocity *= -1;
        }

        public static void Reset() // Reset the game
        {
            planets.Clear();
            new_planets.Clear();
            black_holes.Clear();
            white_holes.Clear();
            bullets.Clear();

            P1_won = false;
            P2_won = false;
            tie = false;
            running = true;

            timeBombsSoFar = 0;

            currentLevel = (currentLevel + 1) % Constants.TOTAL_LAYOUTS;

            // Populate the world with randomly-spaced planets
            
            for (int i = 0; i < Constants.NUM_PLANETS_ON_SCREEN; i++)
            {
                float ran_x = (float)(((float)Constants.WORLD_WIDTH - 2.0f * Constants.PLANET_RADIUS - 1) * ran.NextDouble()) + Constants.PLANET_RADIUS;
                float ran_y = (float)(((float)Constants.WORLD_HEIGHT - 2.0f * Constants.PLANET_RADIUS - 1) * ran.NextDouble()) + Constants.PLANET_RADIUS;
                Planet p = new Planet(
                    grayPlanet,
                    new Vector2(ran_x, ran_y),
                    Constants.PLANET_MASS_DEFAULT,
                    Constants.PLANET_RADIUS,
                    0,
                    false);
                planets.AddLast(p);
            }

            // ...and read in black holes and white holes
            for (int i = 0; i < black_hole_layouts[currentLevel].Length; i++)
            {
                Planet bh = new Planet(
                    blackHoleSprite,
                    black_hole_layouts[currentLevel][i],
                    4*Constants.PLANET_MASS_DEFAULT,
                    Constants.PLANET_RADIUS,
                    0,
                    false);
                black_holes.AddLast(bh);
            }
            for (int i = 0; i < white_hole_layouts[currentLevel].Length; i++)
            {
                Planet wh = new Planet(
                    whiteHoleSprite,
                    white_hole_layouts[currentLevel][i],
                    (currentLevel == 4 ? -0.15f : -1f) * 2 * Constants.PLANET_MASS_DEFAULT, // Make Level 5's white holes really weak
                    Constants.PLANET_RADIUS,
                    0,
                    false);
                white_holes.AddLast(wh);
            }
            
            // Set up the players' ships and cue balls
            Rectangle shipBody = new Rectangle(0, 0, Constants.SHIP_WIDTH, Constants.SHIP_HEIGHT);
            P1 = new Ship(redShip, shipBody, P1_start_positions[currentLevel], true);
            P2 = new Ship(blueShip, shipBody, P2_start_positions[currentLevel], false);

            P1Cue = new Planet(
                    redCue,
                    new Vector2(
                        (float)(((float)Constants.WORLD_WIDTH - 2.0f * Constants.PLANET_RADIUS - 1) * ran.NextDouble()) + Constants.PLANET_RADIUS,
                        (float)(((float)Constants.WORLD_HEIGHT - 2.0f * Constants.PLANET_RADIUS - 1) * ran.NextDouble()) + Constants.PLANET_RADIUS),
                    Constants.PLANET_MASS_DEFAULT,
                    Constants.PLANET_RADIUS,
                    1,
                    true);
            P2Cue = new Planet(
                    blueCue,
                    new Vector2(
                        (float)(((float)Constants.WORLD_WIDTH - 2.0f * Constants.PLANET_RADIUS - 1) * ran.NextDouble()) + Constants.PLANET_RADIUS,
                        (float)(((float)Constants.WORLD_HEIGHT - 2.0f * Constants.PLANET_RADIUS - 1) * ran.NextDouble()) + Constants.PLANET_RADIUS),
                    Constants.PLANET_MASS_DEFAULT,
                    Constants.PLANET_RADIUS,
                    2,
                    true);

            planets.AddLast(P1Cue);
            planets.AddLast(P2Cue);
        }
    }
}
