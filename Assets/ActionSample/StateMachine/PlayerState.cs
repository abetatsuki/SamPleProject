using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// プレイヤーのステート（状態）の基底クラス。
    /// 全てのプレイヤー用ステートはこのクラスを継承します。
    /// </summary>
    public abstract class PlayerState : IState
    {
        /// <summary>
        /// プレイヤーコントローラーへの参照。
        /// 派生クラスからプレイヤーのメソッドやプロパティにアクセスするために使用します。
        /// </summary>
        protected PlayerController Context { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">操作対象のプレイヤーコントローラー</param>
        public PlayerState(PlayerController context)
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
        public virtual void LogicUpdate() 
        {
            // 共通の入力処理や状態遷移チェックがあればここに書く
            // なぜこの処理が必要なのか: 全ステートで共通して行いたい処理（例えばグローバルなキー入力チェックなど）を一箇所にまとめるため
        }

        /// <summary>
        /// 物理演算の更新。
        /// 必要に応じて派生クラスでオーバーライドします。
        /// </summary>
        public virtual void PhysicsUpdate() { }
    }
}
