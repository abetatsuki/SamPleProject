using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// 敵キャラクターのステート（状態）の基底クラス。
    /// 全ての敵用ステートはこのクラスを継承します。
    /// </summary>
    public abstract class EnemyState : IState
    {
        /// <summary>
        /// 敵コントローラーへの参照。
        /// 派生クラスから敵のメソッドやプロパティにアクセスするために使用します。
        /// </summary>
        protected EnemyController Context { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">操作対象の敵コントローラー</param>
        public EnemyState(EnemyController context)
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
