using System;
using UnityEngine.InputSystem;

namespace ActionSample.Input
{
    /// <summary>
    /// 入力アクションのイベントラッパークラス。
    /// UnityのInputActionをラップし、C#標準のイベントとして各種状態変化を提供します。
    /// </summary>
    /// <typeparam name="T">入力値の型（Vector2, float等の構造体）。</typeparam>
    public class InputActionEntity<T> : IDisposable
        where T : struct
    {
        /// <summary>
        /// <see cref="InputActionEntity{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="inputAction">ラップするUnityのInputAction。</param>
        public InputActionEntity(InputAction inputAction)
        {
            _inputAction = inputAction;
            // UnityのInput Systemのイベントを購読し、このクラスのイベントとして再配信するため
            inputAction.started += StartedHandler;
            inputAction.performed += PerformedHandler;
            inputAction.canceled += CanceledHandler;
        }



        /// <summary>
        /// 入力が開始された時のイベント。
        /// </summary>
        public event Action<T> Started;

        /// <summary>
        /// 入力が実行された時のイベント。
        /// </summary>
        public event Action<T> Performed;

        /// <summary>
        /// 入力がキャンセルされた時のイベント。
        /// </summary>
        public event Action<T> Canceled;




        /// <summary>
        /// 登録されている全てのStartedイベントハンドラーを手動で呼び出します。
        /// </summary>
        /// <param name="value">イベントハンドラーに渡す値。</param>
        public void InvokeStarted(T value)
        {
            Started?.Invoke(value);
        }

        /// <summary>
        /// 登録されている全てのPerformedイベントハンドラーを手動で呼び出します。
        /// </summary>
        /// <param name="value">イベントハンドラーに渡す値。</param>
        public void InvokePerformed(T value)
        {
            Performed?.Invoke(value);
        }

        /// <summary>
        /// 登録されている全てのCanceledイベントハンドラーを手動で呼び出します。
        /// </summary>
        /// <param name="value">イベントハンドラーに渡す値。</param>
        public void InvokeCanceled(T value)
        {
            Canceled?.Invoke(value);
        }

        /// <summary>
        /// アクションを有効化します。
        /// </summary>
        public void Enable()
        {
            // InputActionはデフォルトで無効になっている場合があるため、明示的に有効化する必要がある
            _inputAction.Enable();
        }

        /// <summary>
        /// アクションを無効化します。
        /// </summary>
        public void Disable()
        {
            // 不要な入力処理を停止するため
            _inputAction.Disable();
        }



        /// <summary>
        /// このインスタンスによって使用されているリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            // イベント購読を解除し、メモリリークを防ぐため
            _inputAction.started -= StartedHandler;
            _inputAction.performed -= PerformedHandler;
            _inputAction.canceled -= CanceledHandler;
            _inputAction.Disable();
        }





        /// <summary> ラップ対象のUnity InputAction。 </summary>
        private readonly InputAction _inputAction;



        /// <summary>
        /// InputActionのstartedイベントのハンドラー。
        /// </summary>
        /// <param name="ctx">コールバックコンテキスト。</param>
        private void StartedHandler(InputAction.CallbackContext ctx)
        {
            // コンテキストから値を読み取り、型安全なイベントとして配信するため
            T value = ctx.ReadValue<T>();
            InvokeStarted(value);
        }

        /// <summary>
        /// InputActionのperformedイベントのハンドラー。
        /// </summary>
        /// <param name="ctx">コールバックコンテキスト。</param>
        private void PerformedHandler(InputAction.CallbackContext ctx)
        {
            // コンテキストから値を読み取り、型安全なイベントとして配信するため
            T value = ctx.ReadValue<T>();
            InvokePerformed(value);
        }

        /// <summary>
        /// InputActionのcanceledイベントのハンドラー。
        /// </summary>
        /// <param name="ctx">コールバックコンテキスト。</param>
        private void CanceledHandler(InputAction.CallbackContext ctx)
        {
            // コンテキストから値を読み取り、型安全なイベントとして配信するため
            T value = ctx.ReadValue<T>();
            InvokeCanceled(value);
        }

    }
}
