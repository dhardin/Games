using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.TaskManagement;
using Microsoft.Xna.Framework;

namespace PolarRecall
{
    class MoveObjectTask : Task
    {
        GameObject theObject;
        Vector2 target;
        float speed;

        public MoveObjectTask(GameObject obj, Vector2 destination, float pixPerSec)
            : base(TaskType.Sprite)
        {
            theObject = obj;
            target = destination;
            speed = pixPerSec / 1000.0f;
        }

        public override void Update(int milliseconds)
        {
            base.Update(milliseconds);

            float distanceThisTick = speed * milliseconds;
            Vector2 toTarget = target - theObject.Position;
            float distanceToTarget = toTarget.Length();

            if (distanceToTarget <= distanceThisTick)
            {
                theObject.Position = target;
                Kill();
            }
            else
            {
                toTarget.Normalize();
                theObject.Position += toTarget * distanceThisTick;
            }
        }

    }
}
