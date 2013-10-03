using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pool
{
    class Constants
    {
        public const int WORLD_HEIGHT = 600;
        public const int WORLD_WIDTH = 1200;
        public const float PLANET_RADIUS = 12.5f;
        public const float G = 0.0001F; // Universal gravitational constant
        public const float PLANET_MASS_DEFAULT = 0.00000035F;
        public const int SHIP_WIDTH = 10;
        public const int SHIP_HEIGHT = 25;
        public const int BULLET_RADIUS = 4;
        public const float BULLET_SPEED = 0.000065f;
        public const double SHIP_ROTATE_ROUGH = 0.08;
        public const double SHIP_ROTATE_FINE = 0.03;
        public const int SHIP_SPEED = 1;
        public const float MAX_SHIP_SPEED = 100.0F;
        public const float MAX_PLANET_SPEED_COEF = 100000.0F;
        public const float LASER_COEF = 0.00000000001F;
        public const double OWNER_TIMEOUT_MS = 2500.0; // Ownership timeout, in milliseconds
        public const int MESSAGE_HEIGHT = 100;
        public const int MESSAGE_WIDTH = 300;
        public const float PLANET_MASS_MULTIPLIER = 1.2f;
        public const float MAX_MASS_INCREASE = 4.0f;
        public const float MAX_MASS_DECREASE = 0.25f;
        public const double TIME_BOMB_COUNTDOWN_MS = 5000.0; // Amount of time that time should go "backwards" for
        public const double SHIP_SLOW_COUNTDOWN_MS = 5000.0; // Amount of time ships are slowed down for when fired at by the opponent
        public const double PHASE_COUNTDOWN_MS = 5000.0; // Amount of time planets pass through other planets w/o colliding for
        public const double GRAVITY_OFF_COUNTDOWN_MS = 5000.0; // Amount of time for which gravity stays off for a planet
        public const float WALL_BOUNCE = 0.0000003F;
        public const int TOTAL_WEAPONS = 7;
        public const int TOTAL_LAYOUTS = 7; // Number of arrangements ("levels") of black holes / white holes we have
        public const int ENDING_SCORE = 30; // Game ends when either player has this score
        public const double SHIP_SLOW_COEF = 0.5f; // Ship move/turn speed, and laser strength, is reduced when the ship is slowed by its opponent
        public const int TIME_BOMBS_ALLOWED = 3; // Only this many time bombs allowed to be set off per game
        public const int NUM_PLANETS_ON_SCREEN = 10; // Number of planets we want around at any one time
    }
}
