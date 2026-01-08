using UnityEngine;

namespace ActionSample.Weapon.StateMachine
{
    /// <summary>
    /// 武器の待機状態を表すステート。
    /// 入力を監視し、射撃（Fire）またはリロード（Reload）ステートへの遷移を制御します。
    /// </summary>
    public class WeaponIdleState : WeaponState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">武器コントローラー</param>
        public WeaponIdleState(WeaponController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // リロード入力の判定
            // プレイヤーがRキーを押した時に手動リロードを実行するため
            if (Context.InputHandler != null && Context.InputHandler.ReloadInput)
            {
                // 弾が減っていて、かつ予備弾薬がある場合のみリロード可能
                if (Context.CurrentAmmo < Context.MaxAmmo && Context.TotalAmmo > 0)
                {
                    Context.StateMachine.ChangeState(Context.ReloadState);
                    return;
                }
            }

            // 射撃入力の判定
            // プレイヤーが攻撃ボタンを押した時に射撃を実行するため
            if (Context.InputHandler != null && Context.InputHandler.FireInput)
            {
                // マガジンに弾がある場合は射撃へ
                if (Context.CurrentAmmo > 0)
                {
                    Context.StateMachine.ChangeState(Context.FireState);
                }
                // 弾切れだが予備弾薬はある場合
                else if (Context.TotalAmmo > 0)
                {
                    // 空撃ち音（カチッ）を鳴らす処理をここに入れるか、
                    // または自動リロードを行う
                    // ここでは自動リロードへの遷移を採用
                    Context.StateMachine.ChangeState(Context.ReloadState);
                }
            }
        }
    }
}
