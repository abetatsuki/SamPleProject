using UnityEngine;

namespace ActionSample.Weapon.StateMachine
{
    /// <summary>
    /// 武器の発射状態を表すステート。
    /// 弾丸の発射処理、リコイル適用、連射制御を行います。
    /// </summary>
    public class WeaponFireState : WeaponState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">武器コントローラー</param>
        public WeaponFireState(WeaponController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            // 発射処理を実行
            Fire();
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            // 連射間隔（レート）のチェック
            // 一瞬で弾を撃ち尽くさないように、次の発射までの待機時間を設けるため
            if (Time.time >= _nextFireTime)
            {
                // 連射待機時間が過ぎたら、一度アイドル状態に戻る
                // ここでIdleに戻ることで、Inputチェックを再度行い、押しっぱなしなら再度FireStateへ遷移するループを作るため
                // （セミオートの場合はここでトリガーリセット待ちなどのロジックを追加可能）
                Context.StateMachine.ChangeState(Context.IdleState);
            }
        }

        private float _nextFireTime;

        /// <summary>
        /// 実際の射撃処理を行います。
        /// </summary>
        private void Fire()
        {
            // 弾薬を消費
            Context.CurrentAmmo--;
            
            // 次の発射可能時間を設定
            // 連射速度（FireRate）に基づいてインターバルを計算するため
            _nextFireTime = Time.time + Context.FireRate;

            // リコイル適用
            // 射撃の反動をカメラや武器モデルに反映するため
            Context.ApplyRecoil();

            // 画面中心（照準）に向かってレイを飛ばす準備
            // ViewportPointToRay(0.5, 0.5) は画面中央を指す
            Ray ray = Context.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 targetPoint;
            
            // レイキャストを実行して着弾点を計算
            // 実際に何かに当たった場所に向かって弾を飛ばすため（クロスヘアの指す場所へ飛ばす）
            if (Physics.Raycast(ray, out RaycastHit hit, Context.Range))
            {
                targetPoint = hit.point;
            }
            else
            {
                // 何にも当たらなかった場合は射程距離の限界地点をターゲットにする
                targetPoint = ray.origin + ray.direction * Context.Range;
            }

            // 弾の生成 (Muzzleがある場合のみ)
            if (Context.BulletPrefab != null && Context.Muzzle != null)
            {
                GameObject bulletObj = Object.Instantiate(Context.BulletPrefab, Context.Muzzle.position, Quaternion.identity);
                Bullet bulletScript = bulletObj.GetComponent<Bullet>();
                
                // Muzzleから着弾点への方向ベクトルを計算
                // 銃口からターゲットへ向かって一直線に弾を飛ばすため
                Vector3 shootDir = (targetPoint - Context.Muzzle.position).normalized;

                if (bulletScript != null)
                {
                    bulletScript.Initialize(shootDir, Context.Damage);
                }
            }
            else
            {
                // 弾プレハブがない場合の即着弾ロジック（ヒットスキャン）
                // フォールバック用として実装
                if (hit.collider != null)
                {
                    EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                         // 敵にヒットした処理
                         enemy.StateMachine.ChangeState(enemy.StunState);
                    }
                }
                
                // デバッグ描画
                if (Context.Muzzle != null)
                {
                    Debug.DrawLine(Context.Muzzle.position, targetPoint, Color.yellow, 0.1f);
                }
            }

            // デバッグログ
            Debug.Log($"Bang! Ammo: {Context.CurrentAmmo}");
        }
    }
}
