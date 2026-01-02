using InventorySample;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// インベントリの1スロット分のUIを管理するクラス
/// </summary>
public sealed class InventorySlotUI : MonoBehaviour
{

    private void Awake()
    {
        _itemImage = GetComponent<Image>();
        _outline = GetComponent<Outline>();
    }
    /// <summary>
    /// スロットにアイテムのSpriteを表示する
    /// </summary>
    public void SetItem(ItemDataSO itemData)
    {
        _currentItemData = itemData;
        _itemImage.sprite = _currentItemData.Icon;
        _itemImage.enabled = true;
    }



    /// <summary>
    /// スロットを空状態にする
    /// </summary>
    public void ClearItem()
    {
        _itemImage.sprite = null;
        _itemImage.enabled = false;
    }

    /// <summary>
    /// スロットが空かどうかを返す
    /// </summary>
    public bool IsEmpty()
    {
        return _itemImage.enabled == false;
    }

    /// <summary>
    /// 現在のアイテムが指定のアイテムかどうかを返す
    /// </summary>
    public bool IsItem(ItemDataSO itemData)
    {
        return _currentItemData == itemData;
    }

    public void SetSelected(bool isSelected)
    {
        // オブジェクトが破棄されていたら何もしない
        if (this == null || _outline == null) return;
        _outline.enabled = isSelected;
    }

    private Image _itemImage;
    private Outline _outline;
    private ItemDataSO _currentItemData;
}
