using UnityEngine;

namespace ActionSample
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Slide Settings")]
        [SerializeField] private float slideSpeed = 15f;
        [SerializeField] private float slideDuration = 0.8f;
        [SerializeField] private float slideDrag = 0.5f;

        private Rigidbody _rigidbody;
        private float _currentSlideTimer;
        
        public bool IsSliding { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Move(Vector3 direction)
        {
            // スライディング中は通常の移動入力を受け付けない
            if (IsSliding) return;

            // Y軸（落下など）の速度は維持しつつ、XZ平面の速度を適用
            Vector3 targetVelocity = direction * moveSpeed;
            targetVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = targetVelocity;
        }

        public void StartSlide(Vector3 direction)
        {
            if (IsSliding) return;

            IsSliding = true;
            _currentSlideTimer = slideDuration;

            // 入力方向があればそちらへ、なければキャラクターの前方へ
            Vector3 slideDir = direction.magnitude > 0.1f ? direction : transform.forward;
            
            _rigidbody.linearVelocity = slideDir * slideSpeed;
            _rigidbody.linearDamping = slideDrag;
        }

        public void HandleSlideUpdate()
        {
            if (!IsSliding) return;

            _currentSlideTimer -= Time.fixedDeltaTime;

            if (_currentSlideTimer <= 0f)
            {
                EndSlide();
            }
        }

        private void EndSlide()
        {
            IsSliding = false;
            _rigidbody.linearDamping = 0f;
        }
    }
}
