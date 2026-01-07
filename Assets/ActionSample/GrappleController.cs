using UnityEngine;

namespace ActionSample
{
    /// <summary>
    /// グラップリングフックの機能を管理するクラス。
    /// レイキャストによる着弾点の検出、移動速度の計算、ロープの描画を担当します。
    /// </summary>
    public class GrappleController : MonoBehaviour
    {
        [Header("Grappling Settings")]
        /// <summary>
        /// グラップルの最大距離
        /// </summary>
        public float MaxGrappleDistance = 100f;

        /// <summary>
        /// グラップル可能なレイヤー
        /// </summary>
        public LayerMask WhatIsGrappleable;

        /// <summary>
        /// ターゲット位置よりどれだけ高く飛び上がるか（放物線の高さ）
        /// </summary>
        public float OvershootYAxis = 2f;

        /// <summary>
        /// グラップルのクールダウン時間
        /// </summary>
        public float GrapplingCooldown = 2.0f;

        /// <summary>
        /// グラップル着弾から移動開始までの遅延時間
        /// </summary>
        public float GrappleDelayTime = 0.2f;

        [Header("References")]
        /// <summary>
        /// 発射位置（銃口など）
        /// </summary>
        public Transform GunTip;

        /// <summary>
        /// プレイヤーのカメラ（視線判定用）
        /// </summary>
        public Transform PlayerCamera;

        /// <summary>
        /// ロープ描画用のLineRenderer
        /// </summary>
        public LineRenderer LineRenderer;

        /// <summary>
        /// 現在クールダウン中かどうか
        /// </summary>
        public bool IsCoolingDown { get; private set; }

        private float _cooldownTimer;
        private Vector3 _grapplePoint;

        private void Awake()
        {
            if (LineRenderer != null)
            {
                // ロープの描画座標がずれないようにワールド座標系を使用する設定を強制
                LineRenderer.useWorldSpace = true;
                LineRenderer.enabled = false;
            }
        }

        private void Update()
        {
            // クールダウンタイマーの更新
            // なぜこの処理が必要なのか: 連続してグラップルを使用できないように制限時間を管理するため
            if (IsCoolingDown)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0)
                {
                    IsCoolingDown = false;
                }
            }
        }

        private void LateUpdate()
        {
            // ロープの開始位置を更新
            // なぜこの処理が必要なのか: プレイヤーが動いてもロープの根元が銃口の位置に追従するようにするため
            if (LineRenderer.enabled)
            {
                // GunTipが設定されていない場合はとりあえず現在位置を使うなどのフォールバックがあると良いが、
                // ここでは設定されている前提で進める
                Vector3 startPos = GunTip != null ? GunTip.position : transform.position;
                LineRenderer.SetPosition(0, startPos);
            }
        }

        /// <summary>
        /// グラップル可能な地点を探し、見つかればその座標を返します。
        /// </summary>
        /// <param name="hitPoint">着弾点（出力）</param>
        /// <returns>グラップル可能ならtrue</returns>
        public bool TryGetGrapplePoint(out Vector3 hitPoint)
        {
            hitPoint = Vector3.zero;

            // クールダウン中は使用不可
            // なぜこの処理が必要なのか: ゲームバランスを保つため
            if (IsCoolingDown) 
            {
                // Debug.Log("Grapple is cooling down.");
                return false;
            }

            // カメラの中心（画面中央）からレイを飛ばす
            // なぜこの処理が必要なのか: プレイヤーが見ている画面の正確な中心（レティクル位置）をターゲットにするため
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            
            // 自身のコライダーとの衝突を避けるため、レイキャストの設定には注意が必要
            if (Physics.Raycast(ray, out hit, MaxGrappleDistance, WhatIsGrappleable))
            {
                hitPoint = hit.point;
                Debug.Log($"Grapple Hit: {hit.collider.name} at distance {hit.distance}");
                return true;
            }

            Debug.Log("Grapple Raycast Missed.");
            return false;
        }

        /// <summary>
        /// 目標地点へ到達するための初期速度ベクトルを計算します。
        /// </summary>
        /// <param name="startPoint">開始地点</param>
        /// <param name="endPoint">目標地点</param>
        /// <returns>計算された速度ベクトル</returns>
        public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint)
        {
            float gravity = Physics.gravity.y;
            
            // 最低地点からの高さを考慮して放物線の頂点を決定
            // なぜこの処理が必要なのか: 常に目標地点より少し上を通るような軌道を描き、障害物を越えやすくするため
            float displacementY = endPoint.y - startPoint.y;
            
            // ターゲットが自分より高いか低いかで頂点の高さを調整するロジック
            // 元のコードのロジックを踏襲
            float trajectoryHeight = displacementY + OvershootYAxis;
            if (displacementY < 0) trajectoryHeight = OvershootYAxis;

            // 平面距離の計算
            Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

            // 物理方程式に基づいて必要な速度を逆算
            // 公式: v_y = sqrt(-2 * g * h)
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
            
            // 公式: v_xz = dist_xz / (sqrt(-2h/g) + sqrt(2(d_y - h)/g))
            // 上昇時間 + 下降時間 で水平距離を割ることで水平速度を求める
            Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
                + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

            // NaNチェック（ルートの中が負になる場合などの安全策）
            if (float.IsNaN(velocityXZ.x) || float.IsNaN(velocityXZ.z) || float.IsNaN(velocityY.y))
            {
                Debug.LogWarning("Grapple calculation failed (NaN).");
                return Vector3.zero;
            }
            
            Vector3 resultVelocity = velocityXZ + velocityY;
            Debug.Log($"Grapple Velocity Calculated: {resultVelocity}");

            return resultVelocity;
        }

        /// <summary>
        /// ロープの描画を開始します。
        /// </summary>
        /// <param name="targetPoint">ロープの先端位置</param>
        public void StartGrappleVisual(Vector3 targetPoint)
        {
            // クールダウン開始
            IsCoolingDown = true;
            _cooldownTimer = GrapplingCooldown;

            // ロープ描画有効化
            // なぜこの処理が必要なのか: プレイヤーにグラップル中であることを視覚的に伝えるため
            LineRenderer.enabled = true;
            LineRenderer.SetPosition(1, targetPoint);
        }

        /// <summary>
        /// ロープの描画を終了します。
        /// </summary>
        public void StopGrappleVisual()
        {
            // ロープ描画無効化
            // なぜこの処理が必要なのか: グラップル動作が終了したため
            LineRenderer.enabled = false;
        }
    }
}
