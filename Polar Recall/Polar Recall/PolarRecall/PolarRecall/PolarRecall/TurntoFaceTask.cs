using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.TaskManagement;
using Microsoft.Xna.Framework;

namespace PolarRecall
{
    class TurntoFaceTask : Task
    {
        GameObject Obj1; //Object 1: This is the object that looks around
        GameObject Obj2; //Object 2: This is the object to be found
        Vector2 Target; //Target: A location to be turned to
        /* Obj2 and Target will both be used for the turn. 
        While the Target is directly used, Object 2 is used when you want to chase a specific thing.*/
        float TurnSpeed; //Radians to turn per Second
        float objOrientation;

        #region Constructors
        /// <summary>
        /// This is the first constructor, which takes in Two Game Objects
        /// </summary>
        /// <param name="ObjectLooking">The First Object, which is the one turning</param>
        /// <param name="ObjectDestination">The Second Object, the one to look for</param>
        /// <param name="Speed">The Maximum Turn Speed, in Radians per Second</param>
        public TurntoFaceTask(GameObject ObjectLooking, GameObject ObjectDestination, float Speed)
            : base(TaskType.Sprite)
        {
            Obj1 = ObjectLooking;
            Obj2 = ObjectDestination;
            TurnSpeed = Speed;
        }

        /// <summary>
        /// This is the second constructor, which takes in a Game Object and a Vector 2 that describes location
        /// </summary>
        /// <param name="ObjectLooking">The Object, which we will turn</param>
        /// <param name="Destination">A cartesian coordinate (X,Y) to be looked for on the screen</param>
        /// <param name="Speed">The Maximum Turn Speed, in Radians per Second</param>
        public TurntoFaceTask(GameObject ObjectLooking, Vector2 Destination, float Speed)
            : base(TaskType.Sprite)
        {
            Obj1 = ObjectLooking;
            Target = Destination;
            TurnSpeed = Speed;
            Obj2 = null;
        }
        #endregion

        public override void Initialize()
        {
            if (Obj2 != null)
                Target = Obj2.Position;
        }

        public override void Update(int milliseconds)
        {
            base.Update(milliseconds);
            Initialize();

            float desiredAngle;

            #region Where I'm derriving my formulas. In ASCII art!
            // Consider the following:
            //         D 
            //        /|
            //      /  |
            //    /    | y
            //  / Θ    |
            // T--------
            //     x
            // 
            // where T is the position of the Looking Object, D is the position of the Destination,
            // and Θ is the angle that the Looking Object should be facing in order to 
            // point at the Destination. We need to Θ. And we can use trig to do it!
            //      tan(theta)       = opposite / adjacent
            //      tan(Θ)           = y / x
            // If we take the arctan of both sides of this equation:
            //      arctan( tan(Θ) ) = arctan( y / x )
            //      Θ                = arctan( y / x ) *This should be a DUH for anyone who knows trig!*
            // So, then we can use x and y to find Θ, our "desiredAngle."
            // X and Y are just the differences in position between the two objects.
            #endregion

            float x = Target.X - Obj1.Position.X;
            float y = Target.Y - Obj1.Position.Y;

            y = -y;

            desiredAngle = (float)Math.Atan2(y, x);


            objOrientation = TurnToFace(Obj1.Position, Obj2.Position, objOrientation,
                       TurnSpeed);
            // we construct a heading vector based on the penguin's orientation, and then make the penguin move along
            // that heading.
            Vector2 heading = new Vector2(
                (float)Math.Cos(objOrientation), (float)Math.Sin(objOrientation));

            float difference = MathHelper.WrapAngle(desiredAngle - objOrientation/*Obj1.angleFacing*/);
            
            float RadPerMil = TurnSpeed / 1000f; //Convert TurnSpeed, which is in Radians per Second, into Radians per Millisecond
            RadPerMil = RadPerMil * milliseconds; //Now, set that value to the amount of time that passed 

            if (Math.Abs(difference) > Math.Abs(RadPerMil))
            {
                difference = MathHelper.Clamp(difference, -RadPerMil, RadPerMil);

                //Obj1.angleFacing = MathHelper.WrapAngle(Obj1.angleFacing + difference);
                objOrientation = MathHelper.WrapAngle(objOrientation + difference);
            }
            else
            {
                //Obj1.angleFacing = MathHelper.WrapAngle(Obj1.angleFacing + difference);
                objOrientation = MathHelper.WrapAngle(objOrientation + difference);

                Kill();
            }
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

    }
}
