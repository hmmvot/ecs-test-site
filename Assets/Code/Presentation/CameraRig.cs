using UnityEngine;
using UnityEngine.InputSystem;

namespace EcsTestSite.Presentation
{
    public sealed class CameraRig : MonoBehaviour
    {
        private const float RAYCAST_HEIGHT = 100f;
        
        public Camera Camera;
        public LayerMask GroundMask;
        public float MoveSpeed = 10f;
        public float RotateSpeed = 0.15f;

        private Keyboard _keyboard;
        private Mouse _mouse;

        private void Start()
        {
            _keyboard = Keyboard.current;
            _mouse = Mouse.current;
        }

        private void Update()
        {
            if (_keyboard == null || _mouse == null)
                return;
            
            HandleRotate();
            HandleMove();
        }    
        
        private void HandleRotate()
        {
            if (_mouse == null)
                return;

            bool rotating = _mouse.middleButton.isPressed;
            Cursor.lockState = rotating ? CursorLockMode.Locked : CursorLockMode.None;
            if (!rotating)
                return;

            var d = _mouse.delta.ReadValue();
            transform.Rotate(0f, d.x * RotateSpeed, 0f, Space.World);
        }
        
        private void HandleMove()
        {
            if (_keyboard == null)
                return;
            
            // WASD
            float x = (_keyboard.dKey.isPressed ? 1 : 0) - (_keyboard.aKey.isPressed ? 1 : 0);
            float z = (_keyboard.wKey.isPressed ? 1 : 0) - (_keyboard.sKey.isPressed ? 1 : 0);
            
            var input = new Vector3(x, 0f, z).normalized;

            var up = Vector3.up;
            var fwd = Vector3.ProjectOnPlane(transform.forward, up).normalized;
            var right = Vector3.Cross(up, fwd).normalized;

            var delta = (fwd * input.z + right * input.x) * MoveSpeed * Time.deltaTime;

            // Едем только по Ground (проекция точки на коллайдер)
            var desired = transform.position + delta;
            if (TryProjectToGround(desired, out var snapped))
                transform.position = snapped;
        }

        private bool TryProjectToGround(Vector3 pos, out Vector3 snapped)
        {
            var origin = pos + Vector3.up * RAYCAST_HEIGHT;
            if (Physics.Raycast(origin, Vector3.down, out var hit, RAYCAST_HEIGHT * 2f, GroundMask, QueryTriggerInteraction.Ignore))
            {
                snapped = hit.point;
                return true;
            }
            snapped = transform.position;
            return false;
        }
    }
}
