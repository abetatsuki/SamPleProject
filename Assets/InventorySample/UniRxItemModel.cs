using UnityEngine;
using UniRx;
namespace InventorySample
{
  public class UniRxItemModel 
  {
        private readonly ReactiveProperty<int> _amount = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<int> Amount => _amount;

        public void AddAmount(int amount)
        {
            if (amount == 0) return;
            _amount.Value += amount;
        }
    }
}