using UnityEngine;

namespace ActionSample.Weapon.StateMachine
{
    /// <summary>
    /// 武器のリロード状態を表すステート。
    /// 一定時間待機した後、弾薬を補充します。
    /// </summary>
    public class WeaponReloadState : WeaponState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">武器コントローラー</param>
        public WeaponReloadState(WeaponController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            // リロードタイマーをセット
            _reloadTimer = Context.ReloadTime;
            
            Debug.Log("Reloading...");
            // 将来の実装: リロードアニメーションの再生、効果音の再生など
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // タイマー減算
            _reloadTimer -= Time.deltaTime;

            // リロード完了判定
            // 設定されたリロード時間が経過したら弾薬補充処理を行うため
            if (_reloadTimer <= 0f)
            {
                // 補充に必要な弾数を計算（マガジン容量 - 現在の弾数）
                int ammoNeeded = Context.MaxAmmo - Context.CurrentAmmo;
                
                // 実際に補充する弾数を決定
                // 予備弾薬（TotalAmmo）が必要数より少ない場合に、持っている分だけを装填するため
                int ammoToLoad = Mathf.Min(ammoNeeded, Context.TotalAmmo);

                // 弾薬の移動
                Context.CurrentAmmo += ammoToLoad;
                Context.TotalAmmo -= ammoToLoad;

                Debug.Log($"Reload Complete! Ammo: {Context.CurrentAmmo}/{Context.TotalAmmo}");
                
                // リロード完了後は待機状態に戻る
                Context.StateMachine.ChangeState(Context.IdleState);
            }
        }

        private float _reloadTimer;
    }
}
