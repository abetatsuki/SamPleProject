using UnityEngine;

namespace ActionSample.Weapon
{
    /// <summary>
    /// 発射された弾丸を制御するクラス。
    /// 移動、寿命管理、および衝突判定を行います。
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        /// <summary>
        /// 弾速。
        /// </summary>
        public float Speed = 50f;
        
        /// <summary>
        /// 生存時間（秒）。この時間が経過すると自動的に消滅します。
        /// </summary>
        public float LifeTime = 3f;
        
        /// <summary>
        /// 弾のダメージ量。
        /// </summary>
        public float Damage = 10f;

        /// <summary>
        /// 弾丸を初期化して発射します。
        /// </summary>
        /// <param name="direction">進行方向</param>
        /// <param name="damage">ダメージ量</param>
        public void Initialize(Vector3 direction, float damage)
        {
            _direction = direction;
            Damage = damage;
            
            // 一定時間後に自動破壊
            // なぜこの処理が必要なのか: 画面外に出た弾が永遠に残ってメモリを圧迫するのを防ぐため
            Destroy(gameObject, LifeTime);
        }

        private Vector3 _direction;

        private void Update()
        {
            // フレームごとの移動距離を計算
            float step = Speed * Time.deltaTime;
            
            // 位置を更新
            transform.position += _direction * step;

            // 簡易的な当たり判定
            // なぜこの処理が必要なのか: 高速で移動する弾丸はColliderの物理演算だけではすり抜けてしまうことがあるため
            // 前のフレームからの移動分をRaycastで検査し、衝突を確実に検知する
            if (Physics.Raycast(transform.position - _direction * step, _direction, out RaycastHit hit, step))
            {
                OnHit(hit);
            }
        }

        // ---------------------------------------------------------
        // Private Methods
        // ---------------------------------------------------------

        /// <summary>
        /// 何かに衝突した際の処理。
        /// </summary>
        /// <param name="hit">衝突情報</param>
        private void OnHit(RaycastHit hit)
        {
            // 敵に当たった場合の処理
            // コンポーネント取得を試みる
            var enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // 敵を気絶状態にする（本来はダメージ処理などを呼ぶ）
                // なぜこの処理が必要なのか: ヒット時のインタラクションを実行するため
                enemy.StateMachine.ChangeState(enemy.StunState);
            }

            // 何かに当たったら弾丸を消滅させる
            // なぜこの処理が必要なのか: 貫通弾でない限り、衝突後は役割を終えるため
            Destroy(gameObject);
        }
    }
}
