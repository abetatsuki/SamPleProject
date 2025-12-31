using UnityEngine;
using UniRx;
namespace InventorySample
{
  public class UniRxInventory : MonoBehaviour
  {

        ReactiveProperty<int> _itemCount = new ReactiveProperty<int>();
  }
}