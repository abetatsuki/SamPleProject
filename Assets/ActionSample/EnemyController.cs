using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ActionSample
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float stunDuration = 2.0f;
        private MeshRenderer meshRenderer;
        private Color originalColor;
        
        // CancellationTokenSource to handle manual cancellation (e.g., re-stun)
        private CancellationTokenSource stunCts;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                originalColor = meshRenderer.material.color;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Player")) return;
            
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            
            if (player != null && player.IsSliding)
            {
                // Use Forget() to fire and forget the async method
                StunAsync().Forget();
            }
        }

        private async UniTaskVoid StunAsync()
        {
            if (meshRenderer == null) return;

            // 1. Cancel previous stun if active (reset timer)
            if (stunCts != null)
            {
                stunCts.Cancel();
                stunCts.Dispose();
            }
            stunCts = new CancellationTokenSource();

            // 2. Create a linked token: cancels if 'stunCts' is cancelled OR object is destroyed
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                stunCts.Token, 
                this.GetCancellationTokenOnDestroy()
            );

            try
            {
                // Apply Stun Color
                meshRenderer.material.color = Color.blue;
                Debug.Log("Enemy Stunned!");

                // Wait for duration (converted to milliseconds)
                await UniTask.Delay((int)(stunDuration * 1000), cancellationToken: linkedCts.Token);

                // Revert Color
                meshRenderer.material.color = originalColor;
                Debug.Log("Enemy Recovered!");
            }
            catch (System.OperationCanceledException)
            {
                // Handle cancellation (re-stun or destroy) gracefully
            }
            finally
            {
                linkedCts.Dispose();
            }
        }

        private void OnDestroy()
        {
            // Clean up the CTS if the object is destroyed manually
            if (stunCts != null)
            {
                stunCts.Cancel();
                stunCts.Dispose();
            }
        }
    }
}