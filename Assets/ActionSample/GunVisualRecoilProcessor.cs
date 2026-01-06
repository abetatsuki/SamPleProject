using UnityEngine;

namespace ActionSample
{
    /// <summary>
    /// 銃の見た目上のリコイル（反動）位置を計算する Pure Class。
    /// MonoBehaviour から分離し、ロジックのみをテスト可能にするために独立させています。
    /// Transform への依存を持たず、計算結果としての Vector3 オフセットのみを提供します。
    /// </summary>
    public class GunVisualRecoilProcessor
    {
        /// <summary>
        /// コンストラクタ。リコイルの挙動設定を行います。
        /// </summary>
        /// <param name="returnSpeed">リコイルが元の位置に戻る速度（減衰速度）。値が大きいほど早く戻ります。</param>
        /// <param name="snappiness">リコイル発生時の反応速度。値が大きいほどキビキビと反応し、小さいとゴムのような挙動になります。</param>
        /// <param name="maxRecoil">リコイルの最大許容値（各軸の絶対値の上限）。</param>
        public GunVisualRecoilProcessor(float returnSpeed, float snappiness, Vector3 maxRecoil)
        {
            _returnSpeed = returnSpeed;
            _snappiness = snappiness;
            _maxRecoil = maxRecoil;
        }

        /// <summary>
        /// 現在適用すべきリコイルのオフセット位置。
        /// 毎フレーム変動するため、この値を Transform.localPosition 等に加算して使用してください。
        /// </summary>
        public Vector3 CurrentOffset { get; private set; }

        /// <summary>
        /// リコイル（反動）を発生させます。
        /// </summary>
        /// <param name="recoilForce">発生させる反動のベクトル（例: 後ろ方向への Vector3.back * 0.1f 等）。</param>
        public void PlayRecoil(Vector3 recoilForce)
        {
            // なぜこの処理が必要か:
            // 瞬間的にターゲット位置（_targetRecoil）をずらすことで、
            // 次の Tick 処理にて CurrentOffset がそこへ向かって補間移動を開始するため。
            _targetRecoil += recoilForce;

            // なぜこの処理が必要か:
            // 連続射撃時に反動が累積しすぎて、銃が画面外に消えたり不自然な位置まで移動するのを防ぐため。
            // 各軸ごとに設定された最大値（絶対値）の範囲内に制限します。
            _targetRecoil.x = Mathf.Clamp(_targetRecoil.x, -_maxRecoil.x, _maxRecoil.x);
            _targetRecoil.y = Mathf.Clamp(_targetRecoil.y, -_maxRecoil.y, _maxRecoil.y);
            _targetRecoil.z = Mathf.Clamp(_targetRecoil.z, -_maxRecoil.z, _maxRecoil.z);
        }

        /// <summary>
        /// 時間経過によるリコイル状態の更新を行います。
        /// MonoBehaviour の Update メソッド内で呼び出してください。
        /// </summary>
        /// <param name="deltaTime">前のフレームからの経過時間（Time.deltaTime）。</param>
        public void Tick(float deltaTime)
        {
            // 1. ターゲット位置の減衰（元に戻ろうとする力）
            // なぜこの処理が必要か:
            // 反動でずれたターゲット位置を、時間経過とともに初期位置（Vector3.zero）へ戻すため。
            _targetRecoil = Vector3.Lerp(_targetRecoil, Vector3.zero, _returnSpeed * deltaTime);

            // 2. 現在位置の追従（バネのような動き）
            // なぜこの処理が必要か:
            // 現在の表示位置（CurrentOffset）を、反動が加算されたターゲット位置（_targetRecoil）へ滑らかに追従させるため。
            CurrentOffset = Vector3.Lerp(CurrentOffset, _targetRecoil, _snappiness * deltaTime);
        }

        private readonly float _returnSpeed;

        private readonly float _snappiness;

        private readonly Vector3 _maxRecoil;

        private Vector3 _targetRecoil;
    }
}