using UnityEngine;

namespace ActionSample.Weapon
{
    public class Bullet : MonoBehaviour
    {
        public float Speed = 50f;
        public float LifeTime = 3f;
        public float Damage = 10f;

        private Vector3 _direction;

        public void Initialize(Vector3 direction, float damage)
        {
            _direction = direction;
            Damage = damage;
            Destroy(gameObject, LifeTime);
        }

        private void Update()
        {
            float step = Speed * Time.deltaTime;
            transform.position += _direction * step;

            // 簡易的な当たり判定（本来はRaycastやCollider推奨）
            // ここでは前フレームからの移動分でRayを飛ばして判定
            if (Physics.Raycast(transform.position - _direction * step, _direction, out RaycastHit hit, step))
            {
                OnHit(hit);
            }
        }

        private void OnHit(RaycastHit hit)
        {
            // 敵に当たった場合の処理
            var enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.StateMachine.ChangeState(enemy.StunState);
            }

            // 何かに当たったら消滅
            Destroy(gameObject);
        }
    }
}
