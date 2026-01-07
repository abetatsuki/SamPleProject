using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// グラップリングアクション中のステート。
    /// 目標地点への物理移動とステート遷移を管理します。
    /// </summary>
    public class PlayerGrappleState : PlayerState
    {
        private Vector3 _targetPoint;
        private bool _isActive;
        private bool _isMoving; // 移動を開始したかどうかのフラグ
        private float _elapsedTime;
        private const float GrappleDuration = 1.0f; // 移動制限時間

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerGrappleState(PlayerController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// グラップル判定、ビジュアル開始を行います。移動は遅延後に開始します。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            _elapsedTime = 0f;
            _isActive = false;
            _isMoving = false;

            Debug.Log("Enter Grapple State");

            // グラップル地点の探索
            if (Context.GrappleController.TryGetGrapplePoint(out _targetPoint))
            {
                _isActive = true;

                // まずはラインだけ描画（タメを作る）
                Context.GrappleController.StartGrappleVisual(_targetPoint);

                // 重力以外の速度を一旦リセットして空中で止まるような演出にするか、
                // あるいは慣性を残すかは選択によるが、ここでは「掴んだ瞬間」感を出すため少し減速させる
                Context.Rigidbody.linearVelocity *= 0.5f;
            }
            else
            {
                Debug.Log("No valid grapple point found. Exiting state.");
                Context.StateMachine.ChangeState(Context.IdleState);
            }
        }

        /// <summary>
        /// ステート終了時の処理。
        /// ビジュアルエフェクトを停止します。
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit Grapple State");
            Context.GrappleController.StopGrappleVisual();
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// 遅延後の移動開始処理と終了判定を行います。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!_isActive) return;

            _elapsedTime += Time.deltaTime;

            // 遅延時間が経過したら移動を開始
            if (!_isMoving && _elapsedTime >= Context.GrappleController.GrappleDelayTime)
            {
                _isMoving = true;
                StartMovement();
            }

            // 移動中の終了判定
            if (_isMoving)
            {
                // 時間経過での終了（遅延時間分を考慮して延長するか、全体時間で切るか）
                // ここでは全体時間で切る
                if (_elapsedTime >= GrappleDuration + Context.GrappleController.GrappleDelayTime)
                {
                    Debug.Log("Grapple time limit reached.");
                    Context.StateMachine.ChangeState(Context.IdleState);
                }

                // 距離判定（ターゲットに接近したら終了）
                if (Vector3.Distance(Context.transform.position, _targetPoint) < 0.1f)
                {
                    Debug.Log("Reached grapple target.");
                    Context.StateMachine.ChangeState(Context.IdleState);
                }
            }
        }

        private void StartMovement()
        {
            // ジャンプ速度の計算と適用
            Vector3 velocity = Context.GrappleController.CalculateJumpVelocity(Context.transform.position, _targetPoint);

            if (velocity == Vector3.zero)
            {
                Debug.Log("Grapple velocity is zero. Exiting state.");
                Context.StateMachine.ChangeState(Context.IdleState);
                return;
            }

            // 移動開始
            Context.Rigidbody.linearVelocity = velocity;
        }

        /// <summary>
        /// 外部（衝突検知など）からこのステートを強制終了させるためのメソッド。
        /// </summary>

    }
}
