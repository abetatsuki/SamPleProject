using UnityEngine;

namespace ActionSample
{
    /// <summary>
    /// 武器のリコイル（反動）を制御するクラス
    /// </summary>
    public class RecoilController
    {
        public float RecoilRecoverySpeed { get; set; } = 10f;

        /// <summary>
        /// 現在のPitch（縦方向）のリコイル量
        /// </summary>
        public float CurrentRecoilPitch { get; private set; }

        /// <summary>
        /// 現在のYaw（横方向）のリコイル量
        /// </summary>
        public float CurrentRecoilYaw { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RecoilController()
        {
        }

        /// <summary>
        /// フレーム毎の更新処理
        /// </summary>
        public void Update(float deltaTime)
        {
            RecoverRecoil(deltaTime);
        }

        /// <summary>
        /// リコイルを追加します。
        /// </summary>
        /// <param name="vertical">縦方向の跳ね上がり量（正の値）</param>
        /// <param name="horizontal">横方向のブレ量</param>
        public void AddRecoil(float vertical, float horizontal)
        {
            CurrentRecoilPitch += vertical;
            CurrentRecoilYaw += horizontal;
        }

        /// <summary>
        /// 時間経過とともにリコイルを減衰させます。
        /// </summary>
        private void RecoverRecoil(float deltaTime)
        {
            if (CurrentRecoilPitch > 0)
            {
                CurrentRecoilPitch -= deltaTime * RecoilRecoverySpeed;
                if (CurrentRecoilPitch < 0) CurrentRecoilPitch = 0;
            }

            if (Mathf.Abs(CurrentRecoilYaw) > 0)
            {
                // 横方向は0に向かって減衰
                CurrentRecoilYaw = Mathf.Lerp(CurrentRecoilYaw, 0, deltaTime * RecoilRecoverySpeed);
            }
        }
    }
}
