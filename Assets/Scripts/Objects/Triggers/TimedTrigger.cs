using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Objects
{
    /// <summary>
    /// A timed trigger is a trigger that will change a something over a given amount of time. <para/>
    /// Inherit from this class instead of <see cref="Trigger"/> to modify a value over a given amount of time. <para/>
    /// This trigger can also be saved in a checkpoint state.
    /// </summary>
    public abstract class TimedTrigger : Trigger
    {
        [Header("Timed Trigger Options")]
        public float TargetTime;
        public float CurrentTime;

        protected bool calledFinish;

        public override void Update()
        {
            base.Update();

            // Call OnUpdate if this trigger is running
            if (CurrentTime < TargetTime && HasBeenTriggered)
            {
                OnUpdate(CurrentTime);

                CurrentTime += Time.deltaTime;
            }

            // Call on finish when the timer is finished
            if (CurrentTime >= TargetTime && HasBeenTriggered && !calledFinish)
            {
                calledFinish = true;

                CurrentTime = TargetTime;

                OnFinish();
            }
        }

        public abstract override void OnTriggered();

        /// <summary>
        /// Implement this to determine what will happen at the given time
        /// </summary>
        public abstract void OnUpdate(float time);

        /// <summary>
        /// Implement this to determine what will happen when the timer is finished
        /// </summary>
        public abstract void OnFinish();


#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            DrawDurationLine(TargetTime);
        }
#endif
    }
}
