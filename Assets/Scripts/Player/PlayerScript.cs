using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CustomInput;

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
        /// The primary player color
        /// </summary>
        public Color PlayerColor1
        {
            get
            {
                return p.colorer.GetColors[0];
            }
        }

        /// <summary>
        /// The secondary player color
        /// </summary>
        public Color PlayerColor2
        {
            get
            {
                return p.colorer.GetColors[1];
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
        /// Same as GetComponent<>(), but if the method return null, GetComponentInChildren() is used instead. If that also fails then GetComponentInParent() is used instead.
        /// </summary>
        public T GetChildComponent<T>()
        {
            // Get component regularly
            T component = GetComponent<T>();

            // If it's null, use get component in children
            if (component == null || component.Equals(null))
            {
                component = GetComponentInChildren<T>();
            }

            // If it's null again, use get component in parent
            if (component == null || component.Equals(null))
            {
                component = GetComponentInParent<T>();
            }
            
            // Return
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

        /// <summary>
        /// Basically the same as <see cref="CloneMaterial(Material, Color, bool, bool)"/> but with a player color id instead
        /// </summary>
        public Material CloneMaterial(Material original, int playerColor = 1, bool changeEmission = false, bool changeSpecular = false)
        {
            return CloneMaterial(original, playerColor == 2 ? PlayerColor2 : PlayerColor1, changeEmission, changeSpecular);
        }

        /// <summary>
        /// Clones the given material and changes the new clone materials color to match the given color. <para/>
        /// Will also change the emission and specular colors if <paramref name="changeEmission"/> and <paramref name="changeSpecular"/> are true.
        /// </summary>
        /// <param name="original">The material to clone.</param>
        /// <param name="newColor">The new color of the cloned material.</param>
        /// <param name="changeEmission">Wheter to update the emission color.</param>
        /// <param name="changeSpecular">Wheter to update the specular color.</param>
        /// <returns></returns>
        public Material CloneMaterial(Material original, Color newColor, bool changeEmission = false, bool changeSpecular =false)
        {
            // Copy original material
            Material newMaterial = new Material(original);
            newColor.a = original.color.a;

            // Update material color
            newMaterial.color = newColor;

            // Update emission color if we should
            if (changeEmission)
            {
                newMaterial.SetColor("_EmissionColor", newColor);
            }

            // Update specular color if we should
            if (changeSpecular)
            {
                newMaterial.SetColor("_SpecColor", newColor);
            }

            // Return the new copied material
            return newMaterial;
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
