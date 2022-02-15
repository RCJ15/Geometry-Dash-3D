using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Player
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerMesh : PlayerScript
    {
        [SerializeField] private GameObject meshOutline;

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

        /// <summary>
        /// Toggles the mesh on/off based on <paramref name="enable"/>
        /// </summary>
        public void ToggleMesh(bool enable)
        {
            _meshRenderer.enabled = enable;
            meshOutline.SetActive(enable);
        }
    }
}
