using UnityEngine;

namespace ActionSample
{
    /// <summary>
    /// プレイヤーの視点操作（Aiming）を制御するクラス。
    /// マウス入力に基づく水平・垂直回転と、リコイル（反動）の適用を担当します。
    /// </summary>
    public class PlayerAiming
    {
        /// <summary>
        /// マウス感度。視点移動の速さを調整します。
        /// </summary>
        public float MouseSensitivity { get; set; } = 2.0f;

        /// <summary>
        /// 上下の最大視点角度（度数法）。真上・真下を向く制限を設定します。
        /// </summary>
        public float MaxLookAngle { get; set; } = 80f;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="playerBody">プレイヤーのTransform（水平回転用）</param>
        /// <param name="mainCamera">カメラのTransform（垂直回転用）</param>
        /// <param name="recoilController">リコイル情報を取得するためのコントローラー</param>
        public PlayerAiming(Transform playerBody, Transform mainCamera, RecoilController recoilController)
        {
            _playerBody = playerBody;
            _mainCamera = mainCamera;
            _recoilController = recoilController;

            Initialize();
        }

        /// <summary>
        /// 視点を更新します。毎フレーム（Update内）呼び出すことを想定しています。
        /// </summary>
        /// <param name="lookInput">入力ベクトル（X: 水平, Y: 垂直）</param>
        public void UpdateLook(Vector2 lookInput)
        {
            // 必要な参照がなければ処理を中断
            // なぜこの処理が必要なのか: ゲーム終了時やオブジェクト破棄後にアクセスしてエラーになるのを防ぐため
            if (_mainCamera == null || _playerBody == null) return;

            // 水平回転 (Player Body)
            // なぜこの処理が必要なのか: キャラクターの向き自体を変えることで、移動方向も視点に合わせるため
            // リコイルのYaw成分（横ブレ）も加算して、射撃時のブレを表現する
            float yaw = lookInput.x * MouseSensitivity;
            _playerBody.Rotate(0, yaw + _recoilController.CurrentRecoilYaw, 0);

            // 垂直回転 (Camera)
            // なぜこの処理が必要なのか: 首の上下運動を表現するため。体全体は回さず、カメラだけを回転させる
            float pitchDelta = -lookInput.y * MouseSensitivity;
            _currentPitch += pitchDelta;
            
            // 角度制限
            // なぜこの処理が必要なのか: 首が一周したり、不自然な角度まで曲がらないようにするため
            _currentPitch = Mathf.Clamp(_currentPitch, -MaxLookAngle, MaxLookAngle);

            // リコイル適用後の最終的なPitch
            // なぜこの処理が必要なのか: 銃のリコイル（跳ね上がり）は画面を上に向かせるが、
            // プレイヤーの意図した視点操作（_currentPitch）とは別で一時的なオフセットとして扱いたい場合があるため
            // ここでは現在の視点角度からリコイル分を引く（UnityのX軸回転はマイナスが上向き）
            float finalPitch = _currentPitch - _recoilController.CurrentRecoilPitch;

            _mainCamera.localEulerAngles = new Vector3(finalPitch, 0, 0);
        }

        /// <summary>
        /// リコイル（反動）を追加します。
        /// </summary>
        /// <param name="vertical">縦方向の跳ね上がり量</param>
        /// <param name="horizontal">横方向のブレ量</param>
        public void AddRecoil(float vertical, float horizontal)
        {
            // RecoilControllerへ処理を委譲
            // なぜこの処理が必要なのか: リコイルの計算ロジックを一箇所（RecoilController）に集約するため
            _recoilController?.AddRecoil(vertical, horizontal);
        }

        private Transform _playerBody;
        private Transform _mainCamera;
        private RecoilController _recoilController;

        private float _currentPitch = 0f;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            // FPS視点ではカーソルをロックするのが一般的
            // なぜこの処理が必要なのか: マウスカーソルが画面外に出たり、クリックで他のウィンドウにフォーカスが移るのを防ぐため
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}