using UnityEngine;

namespace ActionSample
{
    public class EnemyController : MonoBehaviour
    {


        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Player")) return;
            Debug.Log("Enemy collided with Player!");
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.StateMachine.CurrentState == player.IdleState)
            {
                Debug.Log("Player is in Idle State. Enemy takes action!");
            }
        }
    }
}