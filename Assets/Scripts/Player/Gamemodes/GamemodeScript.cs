using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;

namespace Game.Player
{
    /// <summary>
    /// The class all gameplay scripts inherit from.
    /// </summary>
    public class GamemodeScript
    {
        //-- Component references
        [HideInInspector] public PlayerGamemodeHandler gh;
        [HideInInspector] public Rigidbody rb;

        /// <summary>
        /// Shortcut for setting and getting "rb.velocity.y"
        /// </summary>
        public float YVelocity
        {
            set
            {
                gh.YVelocity = value;
            }
            get
            {
                return gh.YVelocity;
            }
        }

        /// <summary>
        /// Shortcut for getting "onGround"
        /// </summary>
        public bool OnGround
        {
            get
            {
                return gh.OnGround;
            }
        }

        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public virtual void OnEnable()
        {

        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public virtual void OnDisable()
        {

        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public virtual void Update()
        {

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
using Game.Input;

namespace Game.Player
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class INSERTNAME : GamemodeScript
    {
        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
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

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void OnClick(PressMode mode)
        {
            base.OnClick(mode);
        }
    }
}
*/
