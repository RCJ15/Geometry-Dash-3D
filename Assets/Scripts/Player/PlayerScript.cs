using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;

namespace Game.Player
{
    /// <summary>
    /// The class all player scripts inherit from
    /// </summary>
    public class PlayerScript : MonoBehaviour
    {
        //-- Input
        internal Key clickKey;

        //-- Component references
        internal PlayerMain p;
        internal Rigidbody rb;
        internal MeshRenderer mr;
        internal BoxCollider boxCol;

        /// <summary>
        /// Shortcut for setting and getting "rb.velocity.y"
        /// </summary>
        public float YVelocity
        {
            set
            {
                rb.velocity = new Vector3(rb.velocity.x, value, rb.velocity.z);
            }
            get
            {
                return rb.velocity.y;
            }
        }

        /// <summary>
        /// Shortcut for getting "onGround"
        /// </summary>
        public bool OnGround
        {
            get
            {
                return p.movement.onGround;
            }
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public virtual void Start()
        {
            // Get input key
            clickKey = PlayerInput.GetKey("Click");

            GetComponents();
        }

        /// <summary>
        /// Gets components
        /// </summary>
        private void GetComponents()
        {
            p = GetChildComponent<PlayerMain>();
            rb = GetChildComponent<Rigidbody>();
            mr = GetChildComponent<MeshRenderer>();
            boxCol = GetChildComponent<BoxCollider>();
        }

        /// <summary>
        /// Same as GetComponent<>(), but if the method return null, GetComponentInChildren() is used instead
        /// </summary>
        public T GetChildComponent<T>()
        {
            // Get component regularly
            T component = GetComponent<T>();

            // If it's null, use get component in children
            if (component.Equals(null))
            {
                component = GetComponentInChildren<T>();
            }

            return component;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public virtual void Update()
        {
            // Loop through all press modes (there are only 3)
            foreach (PressMode mode in System.Enum.GetValues(typeof(PressMode)))
            {
                // Check if the key is pressed with this press mode
                if (clickKey.Pressed(mode))
                {
                    // Call on click with this press mode
                    OnClick(mode);
                }
            }
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public virtual void FixedUpdate()
        {

        }

        /// <summary>
        /// OnClick is called when the player presses the main gameplay button. <para/>
        /// <paramref name="mode"/> determines whether the button was just pressed, held or just released.
        /// </summary>
        public virtual void OnClick(PressMode mode)
        {

        }
    }
}

// ############
// # Template #
// ############
/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// 
    /// </summary>
    public class INSERTNAME : PlayerScript
    {


        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}
 */
