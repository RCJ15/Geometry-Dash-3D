using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    /// <summary>
    /// Rotates a billboards Z-Rot by a random amount.
    /// </summary>
    public class BillboardRotator : MonoBehaviour
    {
        [SerializeField] private Vector2 rotSpeed;
        private float _speed;
        private Billboard _billboard;

        private void Start()
        {
            _speed = Random.Range(rotSpeed.x, rotSpeed.y);
            _billboard = GetComponent<Billboard>();
        }

        private void Update()
        {
            _billboard.ZRot += _speed * Time.deltaTime;
        }
    }
}
