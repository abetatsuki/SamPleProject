using UnityEngine;
using UnityEngine.InputSystem;

namespace ActionSample
{
  public class PlayerAction : MonoBehaviour
  {

        private LineRenderer _lineRenderer;
        private Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _lineRenderer = GetComponentInChildren<LineRenderer>();
        }

        private void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            _rigidbody.linearVelocity = new Vector3(x, 0, z) * 5f;
            CameraDirection();

        }

        private void CameraDirection()
        {
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 10f))
            {
                Vector3 startPosition = transform.position + Vector3.up * 0.5f;
                Vector3 endPosition = hitInfo.point;
                Vector3 direction = (hitInfo.point - _rigidbody.position).normalized;

                _rigidbody.AddForce(direction * 100f, ForceMode.Acceleration);

                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(0, startPosition);
                _lineRenderer.SetPosition(1, endPosition);
                _lineRenderer.enabled = true;
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }

       
    }
}