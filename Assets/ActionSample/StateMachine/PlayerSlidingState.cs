using UnityEngine;

namespace ActionSample.StateMachine
{
    /// <summary>
    /// プレイヤーのスライディング（滑走）状態を表すステート。
    /// SampleCodes/Sliding.cs を参考に実装。
    /// 斜面では無限に滑り、平地では時間制限付きで滑ります。
    /// </summary>
    public class PlayerSlidingState : PlayerState
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">プレイヤーコントローラー</param>
        public PlayerSlidingState(PlayerController context) : base(context) { }

        /// <summary>
        /// ステート開始時の処理。
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            
            // 参照元のStartSlide()に相当
            
            // スライディングの持続時間を設定
            _slideTimer = Context.SlidingDuration;

            // スケール変更（しゃがみ動作）
            _originalScale = Context.transform.localScale;
            Context.transform.localScale = new Vector3(_originalScale.x, Context.SlidingYScale, _originalScale.z);

            // 接地性を高めるための下方向への力
            // スライディング開始時に体が浮くのを防ぎ、地面に吸い付くようにするため
            Context.Rigidbody.AddForce(Vector3.down * Context.GravityScale, ForceMode.Impulse);
            
            // 空気抵抗（ドラッグ）の設定
            Context.Rigidbody.linearDamping = Context.SlidingDrag;
            
            // 初速の扱いについては、参照元は特に初速セットはしていないが（AddForceのみ）、
            // 既存の動きを引き継ぐ形になる。
        }

        /// <summary>
        /// ステート終了時の処理。
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            
            // 参照元のStopSlide()に相当
            
            // スケール復元時の地面めり込み回避（Super Jump防止）
            // スケールを戻すとコライダーが地面に食い込み、物理演算で大きく弾き飛ばされてしまうのを防ぐため
            float scaleDifference = _originalScale.y - Context.transform.localScale.y;
            if (scaleDifference > 0)
            {
                // 中心ピボットと仮定し、高さの半分だけ上にずらす
                float heightOffset = scaleDifference * Context.PlayerHeight * 0.5f;
                Context.transform.position += Vector3.up * heightOffset;
            }

            // スケールを元に戻す
            Context.transform.localScale = _originalScale;
            
            // 空気抵抗をリセット
            Context.Rigidbody.linearDamping = 0f;
        }

        /// <summary>
        /// フレーム毎のロジック更新。
        /// </summary>
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            // ジャンプ入力があればJumpへ（スライディングジャンプ）
            if (Context.InputHandler.JumpTriggered)
            {
                Context.StateMachine.ChangeState(Context.JumpState);
                return;
            }
            
            // キーを離したらスライディング終了
            // プレイヤーの意思でスライディングを中断できるようにするため
            if (!Context.InputHandler.SlidingInputHeld)
            {
                TransitionToNextState();
            }
        }

        /// <summary>
        /// 物理演算の更新。
        /// </summary>
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            SlidingMovement();
        }

                /// <summary>
                /// スライディングの移動ロジック
                /// </summary>
                private void SlidingMovement()
                {
                    // 入力方向の計算
                    Vector3 inputDirection = Context.transform.forward * Context.InputHandler.MovementInput.z + Context.transform.right * Context.InputHandler.MovementInput.x;
                    
                    // 接地判定
                    bool isGrounded = Context.IsGrounded();
        
                    if (isGrounded)
                    {
                        // 平地または上り坂の場合
                        // なぜこの条件が必要なのか: 斜面以外では摩擦や勢いの減衰を表現するため、時間制限を設ける
                        if (!Context.OnSlope() || Context.Rigidbody.linearVelocity.y > -0.1f)
                        {
                            // 移動力を加える
                            Context.Rigidbody.AddForce(inputDirection.normalized * Context.SlidingSpeed, ForceMode.Force);
                        
                            // タイマーを減らす
                            _slideTimer -= Time.fixedDeltaTime;
                        }
                        // 下り坂の場合
                        // なぜこの条件が必要なのか: 重力を利用して加速しながら無限に滑り落ちる挙動を再現するため
                        else
                        {
                            // 斜面に沿った方向への力を加える
                            Context.Rigidbody.AddForce(Context.GetSlopeMoveDirection(inputDirection) * Context.SlidingSpeed, ForceMode.Force);
                        
                            // タイマーは減らさない（無限スライド）
                        }
                    }
                    else
                    {
                        // 空中の場合
                        // 空中で加速し続ける（飛ぶ）バグを防ぐため、力は加えない。
                        // ただしスライディング状態としてのタイマーは消費させる。
                         _slideTimer -= Time.fixedDeltaTime;
                    }
        
                    // タイマー終了時の遷移
                    if (_slideTimer <= 0)
                    {
                        TransitionToNextState();
                    }
                }        
        /// <summary>
        /// 次のステートへの遷移処理
        /// </summary>
        private void TransitionToNextState()
        {
             if (Context.InputHandler.MovementInput.sqrMagnitude > 0.01f)
             {
                 Context.StateMachine.ChangeState(Context.WalkState);
             }
             else
             {
                 Context.StateMachine.ChangeState(Context.IdleState);
             }
        }

        private float _slideTimer;
        private Vector3 _originalScale;
    }
}