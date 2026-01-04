using UnityEngine;
using ActionSample.StateMachine;

namespace ActionSample
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Settings")]
        public float StunDuration = 2.0f;

        // Components
        public MeshRenderer MeshRenderer { get; private set; }
        public Color OriginalColor { get; private set; }

        // State Machine
        public StateMachine.StateMachine StateMachine { get; private set; }
        public EnemyIdleState IdleState { get; private set; }
        public EnemyStunState StunState { get; private set; }

        private void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
            if (MeshRenderer != null)
            {
                OriginalColor = MeshRenderer.material.color;
            }

            // Initialize State Machine
            StateMachine = new StateMachine.StateMachine();
            IdleState = new EnemyIdleState(this);
            StunState = new EnemyStunState(this);
        }

        private void Start()
        {
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            StateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            StateMachine.CurrentState.PhysicsUpdate();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.CompareTag("Player")) return;
            
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            
            // If hit by sliding player, transition to StunState
            if (player != null && player.IsSliding)
            {
                StateMachine.ChangeState(StunState);
            }
        }
    }
}
