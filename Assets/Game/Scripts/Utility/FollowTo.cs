using UnityEngine;

namespace Game.Scripts.Utility
{
    public class FollowTo : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 offset;

        private void LateUpdate()
        {
            transform.position = new Vector3(target.position.x + offset.x, target.position.y + offset.y,
                transform.position.z);
        }
    }
}