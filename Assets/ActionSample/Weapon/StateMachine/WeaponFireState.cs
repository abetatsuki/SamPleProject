using UnityEngine;

namespace ActionSample.Weapon.StateMachine
{
    public class WeaponFireState : WeaponState
    {
        private float nextFireTime;

        public WeaponFireState(WeaponController context) : base(context) { }

        public override void Enter()
        {
            base.Enter();
            Fire();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            // Wait for fire rate
            if (Time.time >= nextFireTime)
            {
                // If button held (Automatic) or just finished (Semi-Auto logic handled by transitions)
                // For now, simple return to Idle to re-check input
                ctx.StateMachine.ChangeState(ctx.IdleState);
            }
        }

        private void Fire()
        {
            ctx.CurrentAmmo--;
            nextFireTime = Time.time + ctx.FireRate;

            // リコイル適用
            ctx.ApplyRecoil();

            // 画面真ん中（カメラの前方）へのレイキャスト
            Ray ray = ctx.MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 targetPoint;
            
            // 実際にレイを飛ばして着弾点を計算
            if (Physics.Raycast(ray, out RaycastHit hit, ctx.Range))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.origin + ray.direction * ctx.Range;
            }

            // 弾の生成 (Muzzleがある場合のみ)
            if (ctx.BulletPrefab != null && ctx.Muzzle != null)
            {
                GameObject bulletObj = Object.Instantiate(ctx.BulletPrefab, ctx.Muzzle.position, Quaternion.identity);
                Bullet bulletScript = bulletObj.GetComponent<Bullet>();
                
                // Muzzleから着弾点への方向ベクトル
                Vector3 shootDir = (targetPoint - ctx.Muzzle.position).normalized;

                if (bulletScript != null)
                {
                    bulletScript.Initialize(shootDir, ctx.Damage);
                }
            }
            else
            {
                // Instant Hit Logic if no bullet prefab (Previous Raycast Logic kept as fallback or combined)
                if (hit.collider != null)
                {
                    EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                         enemy.StateMachine.ChangeState(enemy.StunState);
                    }
                }
                
                if (ctx.Muzzle != null)
                {
                    Debug.DrawLine(ctx.Muzzle.position, targetPoint, Color.yellow, 0.1f);
                }
            }

            Debug.Log($"Bang! Ammo: {ctx.CurrentAmmo}");
        }
    }
}
