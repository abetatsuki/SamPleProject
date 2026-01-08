using UnityEngine;

namespace ActionSample
{
    /// <summary>
    /// 武器のリコイル（反動）を制御するクラス。
    /// 射撃時に発生するPitch（跳ね上がり）とYaw（横ブレ）を管理し、時間経過による減衰計算を行います。
    /// </summary>
    public class RecoilController
    {
        /// <summary>
        /// リコイルの回復速度。値が大きいほど早く元の照準に戻ります。
        /// </summary>
        public float RecoilRecoverySpeed { get; set; } = 10f;

        /// <summary>
        /// 現在のPitch（縦方向）のリコイル量。
        /// </summary>
        public float CurrentRecoilPitch { get; private set; }

        /// <summary>
        /// 現在のYaw（横方向）のリコイル量。
        /// </summary>
        public float CurrentRecoilYaw { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RecoilController()
        {
        }

        /// <summary>
        /// フレーム毎の更新処理を行います。
        /// </summary>
        /// <param name="deltaTime">前フレームからの経過時間</param>
        public void Update(float deltaTime)
        {
            // リコイルの減衰処理を実行
            // 射撃後に照準を自動的に元の位置に戻すため
            RecoverRecoil(deltaTime);
        }

        /// <summary>
        /// リコイルを追加します。
        /// </summary>
        /// <param name="vertical">縦方向の跳ね上がり量（正の値）</param>
        /// <param name="horizontal">横方向のブレ量</param>
        public void AddRecoil(float vertical, float horizontal)
        {
            // 縦方向のリコイルを加算
            // 銃の発射反動による跳ね上がりを表現するため
            CurrentRecoilPitch += vertical;
            
            // 横方向のリコイルを加算
            // 銃の左右へのブレを表現するため
            CurrentRecoilYaw += horizontal;
        }

        /// <summary>
        /// 時間経過とともにリコイルを減衰させます。
        /// </summary>
        /// <param name="deltaTime">経過時間</param>
        private void RecoverRecoil(float deltaTime)
        {
            if (CurrentRecoilPitch > 0)
            {
                // 縦方向の減衰（線形補間ではなく減算）
                // 一定速度でスムーズに照準を戻すため
                CurrentRecoilPitch -= deltaTime * RecoilRecoverySpeed;
                
                // 負の値にならないようにクランプ
                if (CurrentRecoilPitch < 0) CurrentRecoilPitch = 0;
            }

            if (Mathf.Abs(CurrentRecoilYaw) > 0)
            {
                // 横方向の減衰（Lerpによる補間）
                // 横ブレは中心（0）に向かって収束するように滑らかに戻すため
                CurrentRecoilYaw = Mathf.Lerp(CurrentRecoilYaw, 0, deltaTime * RecoilRecoverySpeed);
            }
        }
    }
}
