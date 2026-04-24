using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using System;

public class TrayManager : MonoBehaviour
{
    private GameManager m_gamemanager;
    private GameSettings m_settings;
    private List<Item> m_itemsInTray = new List<Item>();
    private List<Vector3> m_slotPositions = new List<Vector3>();
    // Lưu danh sách các ô nền để quản lý nếu cần
    private List<GameObject> m_trayBackgrounds = new List<GameObject>();
    public bool IsBusy { get; private set; }

    // Hàm Setup tương tự BoardController.StartGame
    public void Setup(GameManager gamemanager, GameSettings settings)
    {
        m_gamemanager = gamemanager;
        m_settings = settings;

        // 1. Tải prefab nền từ Resources (giống cách làm trong Board.cs)
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);

        // 2. Tính toán vị trí bắt đầu để căn giữa khay
        float spacing = 1.1f; // Khoảng cách giữa các ô
        float startX = -(m_settings.TrayCapacity - 1) * spacing * 0.5f;

        for (int i = 0; i < m_settings.TrayCapacity; i++)
        {
            // Vị trí của ô thứ i
            Vector3 slotPos = new Vector3(startX + (i * spacing), m_settings.TrayPosition.y, 0);
            m_slotPositions.Add(slotPos);

            // 3. Tạo GameObject nền cho ô này
            if (prefabBG != null)
            {
                GameObject bg = Instantiate(prefabBG, slotPos, Quaternion.identity, this.transform);
                bg.name = $"TraySlot_{i}";

                // Đảm bảo nền luôn nằm phía sau Item (Z > 0 hoặc SortingLayer thấp)
                bg.transform.localPosition = new Vector3(bg.transform.localPosition.x, bg.transform.localPosition.y, 0.1f);

                // Nếu bạn muốn cái nền khay mờ hơn hoặc khác màu một chút để phân biệt với Board:
                var renderer = bg.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = new Color(1f, 1f, 1f, 0.6f); // Làm mờ 40%
                }

                m_trayBackgrounds.Add(bg);
            }
        }
    }


    public bool CanAddItem() => m_itemsInTray.Count < m_settings.TrayCapacity && !IsBusy;

    public void AddItem(Item item, Action onComplete)
    {
        IsBusy = true;

        // Tìm vị trí chèn để gom nhóm các Item cùng loại
        int insertIndex = GetInsertIndex(item);
        m_itemsInTray.Insert(insertIndex, item);

        UpdateTrayVisuals(() =>
        {
            CheckMatches();
            IsBusy = false;
            onComplete?.Invoke();
        });
    }
    public void RemoveItem(Item item)
    {
        if (m_itemsInTray.Contains(item))
        {
            m_itemsInTray.Remove(item);
            UpdateTrayVisuals(null); // Sắp xếp lại các viên còn lại trong khay
        }
    }
    // Thêm hàm này vào TrayManager.cs
    // Hàm mới để lấy Item dựa trên GameObject bị click (có thể là Slot hoặc chính Item)
    public Item GetItemFromHit(Cell hitObject)
    {
        // 1. Kiểm tra xem hitObject có phải là một trong các ô nền không
        int slotIndex = m_trayBackgrounds.IndexOf(hitObject.gameObject);
        
        if (slotIndex != -1)
        {
            // Nếu bấm vào ô nền, kiểm tra xem tại Index đó có Item không
            if (slotIndex < m_itemsInTray.Count)
            {
                return m_itemsInTray[slotIndex];
            }
        }

        // 2. Dự phòng: Nếu người chơi bấm trúng chính xác cái Item (View) 
        // chứ không phải cái nền phía sau
        return m_itemsInTray.Find(it => it.View.gameObject == hitObject);
    }

    private int GetInsertIndex(Item newItem)
    {
        if (!(newItem is NormalItem normalNew)) return m_itemsInTray.Count;

        for (int i = 0; i < m_itemsInTray.Count; i++)
        {
            if (m_itemsInTray[i] is NormalItem existing && existing.ItemType == normalNew.ItemType)
            {
                // Trả về vị trí sau item cùng loại cuối cùng tìm thấy
                int lastIdx = i;
                for (int j = i; j < m_itemsInTray.Count; j++)
                {
                    if (m_itemsInTray[j] is NormalItem next && next.ItemType == normalNew.ItemType)
                        lastIdx = j + 1;
                    else break;
                }
                return lastIdx;
            }
        }
        return m_itemsInTray.Count;
    }

    private void UpdateTrayVisuals(Action onComplete)
    {
        for (int i = 0; i < m_itemsInTray.Count; i++)
        {
            // Sử dụng DOMove của DOTween để di chuyển
            m_itemsInTray[i].View.DOMove(m_slotPositions[i], 0.3f).SetEase(Ease.OutQuad);
        }
        DOVirtual.DelayedCall(0.3f, () => onComplete?.Invoke());
    }

    private void CheckMatches()
    {
        if (m_gamemanager.CurrentMode == GameManager.eLevelMode.TIMER)
        {
            return;
        }
        bool hasMatch = false;
        for (int i = 0; i <= m_itemsInTray.Count - 3; i++)
        {
            var item1 = m_itemsInTray[i] as NormalItem;
            var item2 = m_itemsInTray[i + 1] as NormalItem;
            var item3 = m_itemsInTray[i + 2] as NormalItem;

            if (item1 != null && item2 != null && item3 != null &&
                item1.ItemType == item2.ItemType && item2.ItemType == item3.ItemType)
            {
                hasMatch = true;
                RemoveTriple(i);
                return;
            }
        }
        // Nếu khay đã đầy mà không có Match nào vừa diễn ra -> THUA
        if (!hasMatch && m_itemsInTray.Count >= m_settings.TrayCapacity)
        {
            Debug.Log("Game Over! Khay đã đầy.");
            // Bạn có thể gọi một sự kiện GameOver ở đây
            m_gamemanager.GameOver();
        }
    }

    private void RemoveTriple(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            Item item = m_itemsInTray[index];
            m_itemsInTray.RemoveAt(index);
            item.ExplodeView(); // Sử dụng hàm nổ có sẵn
        }
        UpdateTrayVisuals(null);
    }
}