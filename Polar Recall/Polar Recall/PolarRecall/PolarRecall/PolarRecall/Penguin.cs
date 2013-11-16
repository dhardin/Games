using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Sprites;
using Microsoft.Xna.Framework.Content;
using Engine.Extensions;
using Engine.Map;
using Microsoft.Xna.Framework.Audio;
using Engine.Audio;

namespace PolarRecall
{
    class Penguin : GameObject
    {
        #region Fields

        bool isActive;
        bool isCracked;
        bool isCaught = false;
        bool isVisible = true;
        float moveSpeed = 6f;
        Vector2 Orgin;
        Color color;
        RotatingRectangle lineOfSight;
        //int pathIndex;
        //bool usingPath;

        public List<Vector2> Path { get; set; }
        public int PathIndex { get; set; }
        public bool UsingPath { get; set; }

        AnimatedSpriteFromSpriteSheet _sprite;
        AnimatedSpriteFromSpriteSheet _shirt;
        private const float Speed = 100.0f;
        private SoundEffectInstance _walkingSound;
    
        // this value controls the distance at which the penguin will start to chase the target
        const float penguinChaseDistance = 250.0f;

        //  controls the distance at which the penguin will stop chasing the target
        const float penguinCaughtDistance = 5.0f;

        // this constant is used to avoid hysteresis, which is common in ai programming.
        // see the doc for more details.
        const float penguinHysteresis = 15.0f;

        const float followDistance = 50.0f;

        PenguinAiState penguinState;

        float penguinOrientation;

        float turnSpeed = float.MaxValue;

        BoundingBox objectBounds;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public float FollowDistance
        {
            get { return followDistance; }
        }
        enum PenguinAiState
        {
            // chasing target
            Chasing,
            // has got close enough to target to stop chasing
            Caught,
            // can't detect target so "wanders" around
            Wander,
            //flee from target
            Flee,
            //Evade objects
            Evade
        }

        #endregion
       
        #region Properties
    
        #endregion

        public Penguin()
        {
            objectBounds = new BoundingBox();
            Color = Color.LawnGreen;
            isVisible = true;
            Position = new Vector2(50.0f, 50.0f);
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public bool IsCaught
        {
            get { return isCaught; }
            set { isCaught = value; }
        }

        public bool IsCracked
        {
            get { return isCracked; }
            set { isVisible = value; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }

        public BoundingBox ObjectBounds
        {
            get { return objectBounds; }
        }

        public RotatingRectangle LineOfSight
        {
            get { return lineOfSight; }
        }
        
        public void LoadContent(ContentManager content)
        {
            

            SpriteSheet spriteSheet = content.Load<SpriteSheet>("Penguin");
            _sprite = new AnimatedSpriteFromSpriteSheet(spriteSheet, "PGF1", 18, 8);
            //_shirt = new AnimatedSpriteFromSpriteSheet(spriteSheet, "PGF1s", 18, 8);
            _walkingSound = AudioEngine.Instance.GetSoundEffect(@"Sounds\mario_footstep1");
           Orgin = new Vector2(Position.X + _sprite.SpriteSheet.Texture.Width / 2, Position.Y + _sprite.SpriteSheet.Texture.Height);
           objectBounds = new BoundingBox(new Vector3(new Vector2(Position.X - 15, Position.Y - 15), 0.0f), new Vector3(new Vector2(Position.X + 15, Position.Y + 15), 0.0f));
           lineOfSight = new RotatingRectangle(
                                   new Vector2(Position.X - (float)(_sprite.SpriteSheet.Texture.Width * .5), Position.Y),
                                   new Vector2(Position.X - (float)(_sprite.SpriteSheet.Texture.Width * .5), Position.Y + (float)_sprite.SpriteSheet.Texture.Height),
                                   new Vector2(Position.X + (float)(_sprite.SpriteSheet.Texture.Width), Position.Y),
                                   new Vector2(Position.X + (float)(_sprite.SpriteSheet.Texture.Width), Position.Y + (float)_sprite.SpriteSheet.Texture.Height),
                                    RotatingRectangle.AnchorPoint.Top);

        }

        //All we need is this static method, here generically called update.
        public bool Detect(List<BoundingBox> obstacles)
        {
            for (int i = 0; i < obstacles.Count; ++i)
            {

                if (objectBounds.Intersects(obstacles[i]))
                {
                    Color = Color.Red;
                    return true;
                }

            }
            Color = Color.LawnGreen;
            return false;
        }

        public void Chase(Vector2 targetVector)
        {
            // First we have to use the current state to decide what the thresholds are
            // for changing state
            float penguinChaseThreshold = penguinChaseDistance;
            float penguinCaughtThreshold = penguinCaughtDistance;

            // if the penguin is idle, he prefers to stay idle. we do this by making the
            // chase distance smaller, so the penguin will be less likely to begin chasing
            // the target.
            if (penguinState == PenguinAiState.Wander)
            {
                penguinChaseThreshold -= penguinHysteresis / 2;
            }
            // similarly, if the penguin is active, he prefers to stay active. we
            // accomplish this by increasing the range of values that will cause the
            // penguin to go into the active state.
            else if (penguinState == PenguinAiState.Chasing)
            {
                penguinChaseThreshold += penguinHysteresis / 2;
                penguinCaughtThreshold -= penguinHysteresis / 2;
            }
            // the same logic is applied to the finished state.
            else if (penguinState == PenguinAiState.Caught)
            {
                penguinCaughtThreshold += penguinHysteresis / 2;
            }

            // Second, now that we know what the thresholds are, we compare the penguin's 
            // distance from the target against the thresholds to decide what the penguin's
            // current state is.
            float distanceFromTarget = Vector2.Distance(Position, targetVector);

            // Third, once we know what state we're in, act on that state.
            float currentPenguinSpeed;
            if (penguinState == PenguinAiState.Chasing)
            {
                // the penguin wants to chase the target, so it will just use the TurnToFace
                // function to turn towards the target's position. Then, when the target
                // moves forward, he will chase the target.
                penguinOrientation = TurnToFace(Position, targetVector, penguinOrientation,
                    turnSpeed);
                currentPenguinSpeed = moveSpeed;
            }
            else
            {
                // if the penguin catches the target, it should stop. otherwise it will run right by, then spin around and
                // try to catch it all over again. The end result is that it will kind
                // of "run laps" around the target, which looks funny, but is not what
                // we're after.
                currentPenguinSpeed = moveSpeed;
            }

            // we construct a heading vector based on the penguin's orientation, and then make the penguin move along
            // that heading.
            Vector2 heading = new Vector2(
                (float)Math.Cos(penguinOrientation), (float)Math.Sin(penguinOrientation));
            Position += heading * currentPenguinSpeed;
            Velocity = heading;
            //Now rotate the rectangle's vertices based on the penguin's orientation
            LineOfSight.Rotate(targetVector);

            //Set the rotating rectangle object's orgin to the position of the penguin
            LineOfSight.Orgin = Position;

            //Update the penguin's bounding box min and max values
            objectBounds.Min += new Vector3(heading * currentPenguinSpeed, 0.0f);
            objectBounds.Max += new Vector3(heading * currentPenguinSpeed, 0.0f);
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

        public override void Update(GameTime gameTime)
        {
            SetDirection();
            _sprite.Update(gameTime);
        }

        private void SetDirection()
        {
            CardinalDirection direction = Velocity.ToCardinalDirection();
            switch (direction)
            {
                case CardinalDirection.North:
                    _sprite.FirstFrame = "PGF1";
                    break;
                case CardinalDirection.NorthEast:
                    _sprite.FirstFrame = "PGSE1";
                    break;
                case CardinalDirection.East:
                    _sprite.FirstFrame = "PGR1";
                    break;
                case CardinalDirection.SouthEast:
                    _sprite.FirstFrame = "PGNE1";
                    break;
                case CardinalDirection.South:
                    _sprite.FirstFrame = "PGB1";
                    break;
                case CardinalDirection.SouthWest:
                    _sprite.FirstFrame = "PGNW1";
                    break;
                case CardinalDirection.West:
                    _sprite.FirstFrame = "PGL1";
                    break;
                case CardinalDirection.NorthWest:
                    _sprite.FirstFrame = "PGSW1";
                    break;
                default:
                    break;
            }
        }

        public void Move(Vector2 end, GameObject target)
        {
            if (Position.X != end.X || Position.Y != end.Y)
            {
                if (Position.X > end.X)
                {
                    Position.X -= moveSpeed;

                    if (Position.X <= end.X)
                        Position.X = end.X;
                }
                else if (Position.X < end.X)
                {
                    Position.X += moveSpeed;
                    if (Position.X > end.X)
                        Position.X = end.X;
                }
                if (Position.Y > end.Y)
                {
                    Position.Y -= moveSpeed;
                    if (Position.Y <= end.Y)
                        Position.Y = end.Y;
                }
                else if (Position.Y < end.Y)
                {
                    Position.Y += moveSpeed;
                    if (Position.Y > end.Y)
                        Position.Y = end.Y;
                }
            }
            //Velocity = target.Position - Position;
            objectBounds.Min.X = Position.X - 15;
            objectBounds.Min.Y = Position.Y - 15;
            objectBounds.Max.X = Position.X + 15;
            objectBounds.Max.Y = Position.Y + 15;

            //penguinOrientation = TurnToFace(Position, target.Position, penguinOrientation,
            //           turnSpeed);
            //Vector2 heading = new Vector2(
            //       (float)Math.Cos(penguinOrientation), (float)Math.Sin(penguinOrientation));
            //Velocity = heading;
            //LineOfSight.Orgin = Position;
            //LineOfSight.Rotate(target.Position);
        }

        public override Rectangle CollsionBounds
        {
            get
            {
                return new Rectangle((int)Position.X - 5, (int)Position.Y - 5, 10, 10);
            }
        }

        public override void Draw(DrawContext ctx)
        {
            _sprite.Draw(ctx.Batch, Position + ctx.Offset, Color.White, AnchorType.BottomCenter);
        }

    }
}
