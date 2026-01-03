using UnityEngine;

namespace ActionSample
{
    public class PlayerAiming : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (lineRenderer == null)
                lineRenderer = GetComponentInChildren<LineRenderer>();
        }

        public void UpdateAiming(Vector3 mousePosition)
        {
            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
            
            // 画面上のマウス位置へレイを飛ばす
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
            {
                Vector3 startPosition = transform.position + Vector3.up * 0.5f;
                Vector3 endPosition = hitInfo.point;

                if (lineRenderer != null)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, startPosition);
                    lineRenderer.SetPosition(1, endPosition);
                }
            }
            else
            {
                if (lineRenderer != null) lineRenderer.enabled = false;
            }
        }
    }
}
