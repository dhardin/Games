using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Shooter
{
    class Enemy : GameObject
    {
        GraphicsDeviceManager graphics;
        // Animation representing the enemy
        public Animation EnemyAnimation;

        // The position of the enemy ship relative to the top left corner of thescreen
        public Vector2 Position;

        // The state of the Enemy Ship
        public bool Active;

        // The hit points of the enemy, if this goes to zero the enemy dies
        public int Health;

        // The amount of damage the enemy inflicts on the player ship
        public int Damage;

        // The amount of score the enemy will give to the player
        public int Value;

        // The speed at which the enemy moves
        float enemyMoveSpeed;

        // how fast can he turn?
        const float enemyTurnSpeed = 0.10f;

        // this value controls the distance at which the enemy will start to chase the target
        const float enemyChaseDistance = 250.0f;

        //  controls the distance at which the enemy will stop chasing the target
        const float enemyCaughtDistance = 10f;

        // this constant is used to avoid hysteresis, which is common in ai programming.
        // see the doc for more details.
        const float enemyHysteresis = 15.0f;

        EnemyAiState enemyState;

        float enemyOrientation;

        Vector2 enemyWanderDirection;

        // Get the width of the enemy ship
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        // Get the height of the enemy ship
        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }

        

        enum EnemyAiState
        {
            // chasing target
            Chasing,
            // has got close enough to target to stop chasing
            Caught,
            // can't detect target so "wanders" around
            Wander
        }

        public Enemy(GraphicsDeviceManager g)
        {
            graphics = g;
        }
        public Enemy()
        {
        }
        public void Initialize(Animation animation, Vector2 position)
        {
            // Load the enemy ship texture
            EnemyAnimation = animation;

            // Set the position of the enemy
            Position = position;

            // We initialize the enemy to be active so it will be update in the game
            Active = true;


            // Set the health of the enemy
            Health = 10;

            // Set the amount of damage the enemy can do
            Damage = 10;

            // Set how fast the enemy moves
            enemyMoveSpeed = 6f;


            // Set the score value of the enemy
            Value = 100;

        }


        public void Update(GameTime gameTime)
        {
            // The enemy always moves to the left so decrement it's xposition
            //Position.X -= enemyMoveSpeed;

            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            // Update Animation
            EnemyAnimation.Update(gameTime);

            // If the enemy is past the screen or its health reaches 0 then deactivateit
            if (Position.X < -Width || Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the 
                // active game list
                Active = false;
            }
        }

        public void Update(GameTime gameTime, Vector2 targetVector)
        {
            //Currently, enemy always moves to the left.  Change so that they may wander and chase
            //Position.X -= enemyMoveSpeed;

           

            // First we have to use the current state to decide what the thresholds are
            // for changing state
            float enemyChaseThreshold = enemyChaseDistance;
            float enemyCaughtThreshold = enemyCaughtDistance;
            // if the enemy is idle, he prefers to stay idle. we do this by making the
            // chase distance smaller, so the enemy will be less likely to begin chasing
            // the target.
            if (enemyState == EnemyAiState.Wander)
            {
                enemyChaseThreshold -= enemyHysteresis / 2;
            }
            // similarly, if the enemy is active, he prefers to stay active. we
            // accomplish this by increasing the range of values that will cause the
            // enemy to go into the active state.
            else if (enemyState == EnemyAiState.Chasing)
            {
                enemyChaseThreshold += enemyHysteresis / 2;
                enemyCaughtThreshold -= enemyHysteresis / 2;
            }
            // the same logic is applied to the finished state.
            else if (enemyState == EnemyAiState.Caught)
            {
                enemyCaughtThreshold += enemyHysteresis / 2;
            }

            // Second, now that we know what the thresholds are, we compare the enemy's 
            // distance from the target against the thresholds to decide what the enemy's
            // current state is.
            float distanceFromTarget = Vector2.Distance(Position, targetVector);
            if (distanceFromTarget > enemyChaseThreshold)
            {
                // if the enemy is far away from the target, it should
                // idle.
                enemyState = EnemyAiState.Wander;
            }
            else if (distanceFromTarget > enemyCaughtThreshold)
            {
                enemyState = EnemyAiState.Chasing;
            }
            else
            {
                enemyState = EnemyAiState.Caught;
            }

            // Third, once we know what state we're in, act on that state.
            float currentEnemySpeed;
            if (enemyState == EnemyAiState.Chasing)
            {
                // the enemy wants to chase the target, so it will just use the TurnToFace
                // function to turn towards the cat's position. Then, when the enemy
                // moves forward, he will chase the target.
                enemyOrientation = TurnToFace(Position, targetVector, enemyOrientation,
                    enemyTurnSpeed);
                currentEnemySpeed = .5f * enemyMoveSpeed;
            }
            else if (enemyState == EnemyAiState.Wander)
            {
                //wander randomly
                Wander(Position, ref enemyWanderDirection, ref enemyOrientation,
                    enemyTurnSpeed);
                currentEnemySpeed = .25f * enemyMoveSpeed;
            }
            else
            {
                // if the enemy catches the target, it should stop. otherwise it will run right by, then spin around and
                // try to catch it all over again. The end result is that it will kind
                // of "run laps" around the target, which looks funny, but is not what
                // we're after.
                currentEnemySpeed = 0.0f;
            }

            // we construct a heading vector based on the enemy's orientation, and then make the enemy move along
            // that heading.
            Vector2 heading = new Vector2(
                (float)Math.Cos(enemyOrientation), (float)Math.Sin(enemyOrientation));
            Position += heading * currentEnemySpeed;


            // Update the position of the Animation
            EnemyAnimation.Position = Position;

            // Update Animation
            EnemyAnimation.Update(gameTime);

            // If the enemy is past the screen or its health reaches 0 then deactivate it
            if (/*.X < -Width ||*/ Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the 
                // active game list
                Active = false;
            }
        }

        /// <summary>
        /// Wander contains functionality that is shared between both the mouse and the
        /// tank, and does just what its name implies: makes them wander around the
        /// screen. The specifics of the function are described in more detail in the
        /// accompanying doc.
        /// </summary>
        /// <param name="position">the position of the character that is wandering
        /// </param>
        /// <param name="wanderDirection">the direction that the character is currently
        /// wandering. this parameter is passed by reference because it is an input and
        /// output parameter: Wander accepts it as input, and will update it as well.
        /// </param>
        /// <param name="orientation">the character's orientation. this parameter is
        /// also passed by reference and is an input/output parameter.</param>
        /// <param name="turnSpeed">the character's maximum turning speed.</param>
        private void Wander(Vector2 position, ref Vector2 wanderDirection,
            ref float orientation, float turnSpeed)
        {
            // The wander effect is accomplished by having the character aim in a random
            // direction. Every frame, this random direction is slightly modified.
            // Finally, to keep the characters on the center of the screen, we have them
            // turn to face the screen center. The further they are from the screen
            // center, the more they will aim back towards it.

            // the first step of the wander behavior is to use the random number
            // generator to offset the current wanderDirection by some random amount.
            // .25 is a bit of a magic number, but it controls how erratic the wander
            // behavior is. Larger numbers will make the characters "wobble" more,
            // smaller numbers will make them more stable. we want just enough
            // wobbliness to be interesting without looking odd.
            Random random;

            // Initialize our random number generator
            random = new Random();

            wanderDirection.X +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
            wanderDirection.Y +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());

            // we'll renormalize the wander direction, ...
            if (wanderDirection != Vector2.Zero)
            {
                wanderDirection.Normalize();
            }
            // ... and then turn to face in the wander direction. We don't turn at the
            // maximum turning speed, but at 15% of it. Again, this is a bit of a magic
            // number: it works well for this sample, but feel free to tweak it.
            orientation = TurnToFace(position, position + wanderDirection, orientation,
                .15f * turnSpeed);


            // next, we'll turn the characters back towards the center of the screen, to
            // prevent them from getting stuck on the edges of the screen.
            Vector2 screenCenter = Vector2.Zero;
            screenCenter.X = graphics.GraphicsDevice.Viewport.Width / 2;
            screenCenter.Y = graphics.GraphicsDevice.Viewport.Height / 2;

            // Here we are creating a curve that we can apply to the turnSpeed. This
            // curve will make it so that if we are close to the center of the screen,
            // we won't turn very much. However, the further we are from the screen
            // center, the more we turn. At most, we will turn at 30% of our maximum
            // turn speed. This too is a "magic number" which works well for the sample.
            // Feel free to play around with this one as well: smaller values will make
            // the characters explore further away from the center, but they may get
            // stuck on the walls. Larger numbers will hold the characters to center of
            // the screen. If the number is too large, the characters may end up
            // "orbiting" the center.
            float distanceFromScreenCenter = Vector2.Distance(screenCenter, position);
            float MaxDistanceFromScreenCenter =
                Math.Min(screenCenter.Y, screenCenter.X);

            float normalizedDistance =
                distanceFromScreenCenter / MaxDistanceFromScreenCenter;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
                turnSpeed;

            // once we've calculated how much we want to turn towards the center, we can
            // use the TurnToFace function to actually do the work.
            orientation = TurnToFace(position, screenCenter, orientation,
                turnToCenterSpeed);
        }

        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        private static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            // consider this diagram:
            //         B 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // A--------
            //     x
            // 
            // where A is the position of the object, B is the position of the target,
            // and "o" is the angle that the object should be facing in order to 
            // point at the target. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            float desiredAngle = (float)Math.Atan2(y, x);

            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return WrapAngle(currentAngle + difference);
        }
        /// <summary>
        /// This function takes a Vector2 as input, and returns that vector "clamped"
        /// to the current graphics viewport. We use this function to make sure that 
        /// no one can go off of the screen.
        /// </summary>
        /// <param name="vector">an input vector</param>
        /// <returns>the input vector, clamped between the minimum and maximum of the
        /// viewport.</returns>
        private Vector2 ClampToViewport(Vector2 vector)
        {
            Viewport vp = graphics.GraphicsDevice.Viewport;
            vector.X = MathHelper.Clamp(vector.X, vp.X, vp.X + vp.Width);
            vector.Y = MathHelper.Clamp(vector.Y, vp.Y, vp.Y + vp.Height);
            return vector;
        }
        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        /*        
        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // handle input will read the controller input, and update the cat
            // to move according to the user's whim.
            HandleInput();

            // UpdateTank will run the AI code that controls the tank's movement...
            UpdateTank();

            // ... and UpdateMouse does the same thing for the mouse.
            UpdateMouse();

            // Once we've finished that, we'll use the ClampToViewport helper function
            // to clamp everyone's position so that they stay on the screen.
            tankPosition = ClampToViewport(tankPosition);
            catPosition = ClampToViewport(catPosition);
            mousePosition = ClampToViewport(mousePosition);

            base.Update(gameTime);
        }
         */
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animation
            EnemyAnimation.Draw(spriteBatch);
        }
    }
}
