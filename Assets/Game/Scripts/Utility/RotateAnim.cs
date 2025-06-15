using System;
using UnityEngine;

namespace Game.Scripts.Utility
{
    public class RotateAnim : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed;

        private void Update()
        {
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
        }
    }
}