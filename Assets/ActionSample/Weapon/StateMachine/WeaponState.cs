using UnityEngine;
using ActionSample.StateMachine;

namespace ActionSample.Weapon.StateMachine
{
    /// <summary>
    /// 武器のステート（状態）の基底クラス。
    /// 全ての武器用ステートはこのクラスを継承します。
    /// </summary>
    public abstract class WeaponState : IState
    {
        /// <summary>
        /// 武器コントローラーへの参照。
        /// 派生クラスから武器のメソッドやプロパティにアクセスするために使用します。
        /// </summary>
        protected WeaponController Context { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">操作対象の武器コントローラー</param>
        public WeaponState(WeaponController context)
        {
            this.Context = context;
        }

        /// <summary>
        /// ステート開始時の処理。
        /// 必要に応じて派生クラスでオーバーライドします。
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// ステート終了時の処理。
        /// 必要に応じて派生クラスでオーバーライドします。
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// 必要に応じて派生クラスでオーバーライドします。
        /// </summary>
        public virtual void LogicUpdate() { }

        /// <summary>
        /// 物理演算の更新。
        /// 必要に応じて派生クラスでオーバーライドします。
        /// </summary>
        public virtual void PhysicsUpdate() { }
    }
}
