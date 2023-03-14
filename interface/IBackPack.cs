using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 具有保存itempage功能的都实现此接口
/// </summary>
public interface IBackPack
{
    void ClearItems(int page = 0);
    void SetSize(int x, int y, int page=0);

    void SetItems(Item[] items, int[,] placements, int page=0);

    Item[] GetItems(int page=0);
    Item[,] GetBigItems(int page = 0);

    int[,] GetPlacements(int page=0);

    #region  set
    bool SetItemAt(Item i, int x, int y, int page = 0);
    bool DeleteItemAt(int x, int y, int page = 0);

    int AddItem(Item i, int page = 0);
    int AddItemAt(Item i, int x, int y, int page = 0);
    int AddItemNumAt(int i, int x, int y, int page = 0);
    int SubItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum);
    int SubItemAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum);
    int SetItemNumAt(int num, int x, int y, int page = 0);

    #endregion

    #region  get
    int Contain(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum);
    int ContainAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum);
    Item GetItemAt(int x, int y, int page = 0);
    int CountItem(Item i, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum);
    bool CanPlaceAt(Item item, int x, int y, int page = 0, ItemCompareMode mode = ItemCompareMode.excludeNum);
    int[] GetItemLeftUp(int x, int y, int page = 0);
    bool CanPlaceIgnoreCurrent(Item it, int x, int y, int area = 0);
    bool IsBigChest();
    #endregion

   void init();
}

public interface IBackPack_AddItem
{
    int AddItem(Item i, int page = 0);
}
public interface IBackPack_AddItemAt
{
    int AddItemAt(Item i, int x, int y, int page = 0);
}