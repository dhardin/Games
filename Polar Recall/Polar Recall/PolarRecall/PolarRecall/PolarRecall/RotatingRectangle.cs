using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PolarRecall
{
    class RotatingRectangle
    {

        Vector2 topLeft;
        Vector2 topRight;
        Vector2 bottomLeft;
        Vector2 bottomRight;
        Vector2 orgin;
        AnchorPoint currentAnchor;
        float currentAngle;

        public AnchorPoint CurrentAnchor
        {
            get { return currentAnchor; }
            set { currentAnchor = value; }
        }
        //float currentAngle;
        public Vector2 TopLeft { get { return topLeft; } set { topLeft = value; } }
        public Vector2 TopRight { get { return topRight; } set { topRight = value; } }
        public Vector2 BottomLeft { get { return bottomLeft; } set { bottomLeft = value; } }
        public Vector2 BottomRight { get { return bottomRight; } set {bottomRight = value;} }
        public Vector2 Orgin { get { return orgin; } set { orgin = value;} }
        public enum AnchorPoint
        {
            Top,
            Center,
            TopRightCorner,
            TopLeftCorner,
            BottomLeftCorner,
            BottomRightConer
            
        }

        /// <summary>
        /// Represents a rectangle that can be rotated
        /// </summary>
        /// <param name="top_Left">Top left corner of rectangle</param>
        /// <param name="top_Right">Top right corner of rectangle</param>
        /// <param name="bottom_Left">Bottom left corner of rectangle</param>
        /// <param name="bottom_Right">Bottom right corner of rectangle</param>
        public RotatingRectangle(Vector2 top_Left, Vector2 top_Right, Vector2 bottom_Left, Vector2 bottom_Right, AnchorPoint anchor)
        {
            topLeft = top_Left;
            topRight = top_Right;
            bottomLeft = bottom_Left;
            bottomRight = bottom_Right;
            currentAnchor = anchor;
            currentAngle = (float)Math.Atan2(topRight.Y - bottomRight.Y, topRight.X - bottomRight.X);
        }

        /// <summary>
        /// Rotate rectange by the specified amound of radians
        /// </summary>
        /// <param name="radians">The amount of degrees you want to rotate rectangle</param>
        public void Rotate(Vector2 target)
        {
            currentAngle = MathHelper.WrapAngle(currentAngle);
            float cdegrees = currentAngle * (180 / (float)Math.PI);
            float angleDiff =  (float)Math.Atan2(target.Y - orgin.Y, target.X - orgin.X);
            float angleDiffSides = (float)Math.Atan2(bottomLeft.Y - topLeft.Y, bottomLeft.X - topLeft.X);
            float degrees = angleDiff * (180/(float)Math.PI);

            if (/*angleDiff != angleDiffSides && */!Encapsulates(target))
            {
                currentAngle += angleDiff;
                Matrix rotationMatrix = Matrix.CreateRotationZ(angleDiff);
                topLeft = Vector2.Transform(orgin - topLeft, rotationMatrix) + orgin;
                topRight = Vector2.Transform(orgin - topRight, rotationMatrix) + orgin;
                bottomLeft = Vector2.Transform(orgin - bottomLeft, rotationMatrix) + orgin;
                bottomRight = Vector2.Transform(orgin - bottomRight, rotationMatrix) + orgin;
            }
        }

        public bool Encapsulates(Vector2 target)
        {
            Vector2 inBetween = Vector2.Lerp(bottomLeft, bottomRight, 0.5f);

            if (target.Equals(inBetween))
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool Intersects(BoundingBox box)
        {
            return false;
        }

        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        private static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle)
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

            return desiredAngle;
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

        /// Find the angle between two vectors. This will not only give the angle difference, but the direction.
        /// For example, it may give you -1 radian, or 1 radian, depending on the direction. Angle given will be the 
        /// angle from the FromVector to the DestVector, in radians.
        /// </summary>
        /// <param name="FromVector">Vector to start at.</param>
        /// <param name="DestVector">Destination vector.</param>
        /// <param name="DestVectorsRight">Right vector of the destination vector</param>
        /// <returns>Signed angle, in radians</returns>        
        /// <remarks>All three vectors must lie along the same plane.</remarks>
        public static float GetSignedAngleBetween2DVectors(Vector2 FromVector, Vector2 DestVector, Vector2 DestVectorsRight)
        {
            FromVector.Normalize();
            DestVector.Normalize();
            DestVectorsRight.Normalize();

            float forwardDot = Vector2.Dot(FromVector, DestVector);
            float rightDot = Vector2.Dot(FromVector, DestVectorsRight);

            // Keep dot in range to prevent rounding errors
            forwardDot = MathHelper.Clamp(forwardDot, -1.0f, 1.0f);

            float angleBetween = (float)Math.Acos(forwardDot);

            if (rightDot < 0.0f)
                angleBetween *= -1.0f;

            return angleBetween;
        }

        public Vector3 RotateAroundPoint(Vector3 point, Vector3 originPoint, Vector3 rotationAxis, float radiansToRotate)
        {

            Vector3 diffVect = point - originPoint;

            Vector3 rotatedVect = Vector3.Transform(diffVect, Matrix.CreateFromAxisAngle(rotationAxis, radiansToRotate));

            rotatedVect += originPoint;

            return rotatedVect;

        }
    }
}
