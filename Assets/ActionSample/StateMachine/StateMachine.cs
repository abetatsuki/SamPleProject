namespace ActionSample.StateMachine
{
    /// <summary>
    /// ステート（状態）を表すインターフェース。
    /// 全ての具体的なステートクラスはこのインターフェースを実装する必要があります。
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// ステート開始時に一度だけ呼ばれる処理。
        /// </summary>
        void Enter();

        /// <summary>
        /// ステート終了時に一度だけ呼ばれる処理。
        /// </summary>
        void Exit();

        /// <summary>
        /// 毎フレーム実行されるロジック更新処理（Update内）。
        /// </summary>
        void LogicUpdate();

        /// <summary>
        /// 一定時間ごとに実行される物理更新処理（FixedUpdate内）。
        /// </summary>
        void PhysicsUpdate();
    }

    /// <summary>
    /// ステートマシン本体。
    /// 現在のステートを保持し、ステートの初期化と遷移を管理します。
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// 現在アクティブなステート。
        /// 外部から読み取り可能ですが、変更はChangeStateメソッドを通して行います。
        /// </summary>
        public IState CurrentState { get; private set; }

        /// <summary>
        /// ステートマシンを初期化します。
        /// </summary>
        /// <param name="startingState">初期ステート</param>
        public void Initialize(IState startingState)
        {
            // 初期ステートを設定
            CurrentState = startingState;
            
            // ステート開始処理を実行
            // 最初のステートのセットアップ（アニメーション再生やパラメータ初期化など）を行うため
            startingState.Enter();
        }

        /// <summary>
        /// ステートを遷移させます。
        /// </summary>
        /// <param name="newState">遷移先の新しいステート</param>
        public void ChangeState(IState newState)
        {
            // 現在のステートの終了処理を実行
            // ステートを抜ける際のクリーンアップ（イベント解除やフラグのリセットなど）を行うため
            CurrentState.Exit();

            // ステートを更新
            CurrentState = newState;

            // 新しいステートの開始処理を実行
            // 次のステートとして動作を開始するための準備を行うため
            CurrentState.Enter();
        }
    }
}
