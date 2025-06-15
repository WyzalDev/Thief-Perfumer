using Game.Scripts.Steps;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] private float footstepRememberRadius = 1f;
        [SerializeField] private FootStepColorController footStepColorController;
        private InputAction _interact;

        private void Start()
        {
            _interact = InputSystem.actions.FindAction("Interact");
            _interact.performed += Interact;
        }

        private void Interact(InputAction.CallbackContext context)
        {
            Debug.Log("Interact");
            var hit = Physics2D.OverlapCircle(transform.position, footstepRememberRadius,
                LayerMask.GetMask("FootStep"));
            if (hit != null)
            {
                footStepColorController.MemorizeFootStep(hit.GetComponent<Renderer>());
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, footstepRememberRadius);
        }

        private void OnDestroy()
        {
             _interact.performed -= Interact;
        }
    }
}