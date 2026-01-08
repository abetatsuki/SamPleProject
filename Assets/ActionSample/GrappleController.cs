using UnityEngine;

namespace ActionSample
{
    /// <summary>
    /// 物理的な挙動（振り子運動）を利用したフリースイング型のグラップリングフックを制御するクラス。
    /// RigidbodyとSpringJointを組み合わせて、プレイヤーの移動補助と姿勢制御を実現します。
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class GrappleController : MonoBehaviour
    {
        /// <summary>
        /// コンストラクタ
        /// MonoBehaviourのインスタンス化はUnityが行うため、明示的な初期化は行いません。
        /// </summary>
        public GrappleController()
        {
        }

        /// <summary>
        /// グラップルが現在接続中かどうかを返します。
        /// ステートマシンや外部クラスが現在のグラップル状態を確認し、適切な挙動（アニメーションや遷移）を行うためです。
        /// </summary>
        public bool IsGrappling => _joint != null;

        /// <summary>
        /// グラップルがヒットした地点を返します。
        /// ロープの描画や、他のシステムがターゲット位置を参照できるようにするためです。
        /// </summary>
        public Vector3 GrapplePoint { get; private set; }

        /// <summary>
        /// グラップルが届く最大距離
        /// </summary>
        [Header("Settings")]
        public float MaxGrappleDistance = 25f;

        /// <summary>
        /// スイング中の空中操作による加速力
        /// </summary>
        public float AirControlForce = 45f;

        /// <summary>
        /// プル（直線移動）時の速度
        /// </summary>
        public float PullSpeed = 30f;

        /// <summary>
        /// プル移動完了とみなす距離
        /// </summary>
        public float StopPullDistance = 2.0f;

        /// <summary>
        /// ロープのバネ強度（高いほど硬いロープになる）
        /// </summary>
        public float JointSpring = 4.5f;

        /// <summary>
        /// ロープの揺れを減衰させる力
        /// </summary>
        public float JointDamper = 7f;

        /// <summary>
        /// ロープの質量スケール
        /// </summary>
        public float JointMassScale = 4.5f;

        /// <summary>
        /// グラップル可能なレイヤー
        /// </summary>
        [Header("References")]
        public LayerMask WhatIsGrappleable;

        /// <summary>
        /// 視界判定用のカメラ
        /// </summary>
        public Transform PlayerCamera;

        /// <summary>
        /// グラップル発射口のTransform
        /// </summary>
        public Transform GunTip;

        /// <summary>
        /// ロープ描画用のLineRenderer
        /// </summary>
        public LineRenderer LineRenderer;

        /// <summary>
        /// グラップルの開始を試みます。
        /// プレイヤーの入力に応じて、指定された方向にフックを飛ばし、物理的な接続を作成するためです。
        /// </summary>
        /// <returns>グラップルが成功した場合はtrue</returns>
        public bool StartGrapple()
        {
            // カメラの前方に向かってレイを飛ばし、接続地点を探す
            // プレイヤーが見ている位置にグラップルを固定するため
            RaycastHit hit;
            if (Physics.Raycast(PlayerCamera.position, PlayerCamera.forward, out hit, MaxGrappleDistance, WhatIsGrappleable))
            {
                // ヒット地点を保存
                // ジョイントの接続先や描画の終点として使用するため
                GrapplePoint = hit.point;

                // 物理ジョイントを生成
                // SpringJointを使用することで、振り子のような物理挙動をUnityの物理エンジンに任せるため
                _joint = gameObject.AddComponent<SpringJoint>();
                _joint.autoConfigureConnectedAnchor = false;
                _joint.connectedAnchor = GrapplePoint;

                // 接続時の距離を基準に、ロープの最大・最小長を設定
                // ロープが不自然に伸びすぎるのを防ぎつつ、物理的な張力（テンション）を生み出すため
                float distanceFromPoint = Vector3.Distance(transform.position, GrapplePoint);
                _joint.maxDistance = distanceFromPoint * 0.8f; // 少し短めに設定して張力をかける
                _joint.minDistance = distanceFromPoint * 0.25f;

                // ジョイントの物理パラメータを設定
                // プレイヤーがスイングした際のバウンス感や安定性を調整するため
                _joint.spring = JointSpring;
                _joint.damper = JointDamper;
                _joint.massScale = JointMassScale;

                // ビジュアルの有効化
                // プレイヤーにロープの存在を視覚的に伝えるため
                if (LineRenderer != null)
                {
                    LineRenderer.positionCount = 2;
                    LineRenderer.enabled = true;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// グラップルを解除します。
        /// 入力が離された際、または障害物に衝突した際に、物理的な拘束を解いて自由落下に移行させるためです。
        /// </summary>
        public void StopGrapple()
        {
            // ジョイントの破棄
            // 物理的な接続を解除するため
            if (_joint != null)
            {
                Destroy(_joint);
            }

            // ビジュアルの無効化
            // ロープの表示を消去するため
            if (LineRenderer != null)
            {
                LineRenderer.enabled = false;
            }
        }

        /// <summary>
        /// スイング中の空中操作（移動補助）を適用します。
        /// 物理挙動だけでは操作が難しいため、プレイヤーの入力によって振り子を加速させたり、姿勢を制御したりするためです。
        /// </summary>
        /// <param name="horizontal">横方向入力 (-1.0 ~ 1.0)</param>
        /// <param name="vertical">縦方向入力 (-1.0 ~ 1.0)</param>
        public void ApplyAirControl(float horizontal, float vertical)
        {
            // 入力がない場合は何もしない
            // 不要な力（ゼロベクトル）の加算を防ぎ、慣性を維持するため
            if (horizontal == 0 && vertical == 0) return;

            // カメラの向きに基づいた力の方向を計算
            // プレイヤーが向いている方向に対して直感的に操作できるようにするため
            Vector3 rightForce = PlayerCamera.right * horizontal;
            Vector3 forwardForce = PlayerCamera.forward * vertical;
            Vector3 targetDirection = (rightForce + forwardForce).normalized;

            // 加速力を適用
            // ロープに繋がれた状態でこの力を加えると、振り子の「漕ぐ」動作となり、スイングを加速させることができるため
            _rb.AddForce(targetDirection * AirControlForce, ForceMode.Acceleration);

            // 制約の補足：入力を入れすぎると、進行方向と逆向きになった場合に失速の原因となり、物理法則に則った納得感が生まれる
        }

        /// <summary>
        /// ターゲット地点へ直線的にプレイヤーを引き寄せます（プル挙動）。
        /// スイング（物理）ではなく、ワイヤーの巻き取りによる高速移動（非物理的移動）を実現するためです。
        /// </summary>
        public void ExecutePull()
        {
            // スイング用のジョイントが残っていれば削除
            // 物理的な拘束があると直線移動の妨げになるため、Pullモード移行時に物理挙動を切る必要があるからです。
            if (_joint != null)
            {
                Destroy(_joint);
                _joint = null;
            }

            // ターゲットへの方向ベクトルを計算
            Vector3 direction = (GrapplePoint - transform.position).normalized;

            // 速度を上書きして強制移動
            // 重力や慣性を無視して、ターゲットへ一直線に向かわせるためです。
            _rb.linearVelocity = direction * PullSpeed;
        }

        /// <summary>
        /// プレイヤーがグラップル地点に到達したか判定します。
        /// Pull移動の終了条件（目的地への到達）を判定するためです。
        /// </summary>
        /// <returns>到達していればtrue</returns>
        public bool IsAtGrapplePoint()
        {
            return Vector3.Distance(transform.position, GrapplePoint) <= StopPullDistance;
        }

        private Rigidbody _rb;
        private SpringJoint _joint;

        private void Awake()
        {
            // 必要なコンポーネントのキャッシュ
            // 毎フレームのコンポーネント検索を避け、パフォーマンスを向上させるため
            _rb = GetComponent<Rigidbody>();
            
            if (LineRenderer != null)
            {
                LineRenderer.useWorldSpace = true;
            }
        }

        private void LateUpdate()
        {
            // ロープの描画更新
            // 物理演算によって移動したプレイヤーの最新の位置に合わせてロープを追従させるため
            DrawRope();
        }

        /// <summary>
        /// ロープのビジュアルを更新します。
        /// 接続点とプレイヤー（または銃口）の間に線を引くことで、グラップリングの状態を可視化するためです。
        /// </summary>
        private void DrawRope()
        {
            if (!IsGrappling || LineRenderer == null) return;

            // 始点は銃口、終点は接続点
            // プレイヤーの手に持っている武器からロープが出ているように見せるため
            Vector3 startPos = GunTip != null ? GunTip.position : transform.position;
            LineRenderer.SetPosition(0, startPos);
            LineRenderer.SetPosition(1, GrapplePoint);
        }
    }
}