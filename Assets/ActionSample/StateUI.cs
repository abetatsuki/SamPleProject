using UnityEngine;
using TMPro;
namespace ActionSample
{
  public class StateUI : MonoBehaviour
  {
        [SerializeField] private PlayerController playerController;
        private TMP_Text text;
        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }
        private void Update()
        {
            text.text = playerController.StateMachine.CurrentState.GetType().Name;
        }

    }
}