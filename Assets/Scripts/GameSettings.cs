using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    public int BoardSizeX = 5;

    public int BoardSizeY = 5;

    public int MatchesMin = 3;
    [Header("Tray Settings")]
    public int TrayCapacity = 7; // Số lượng ô tối đa trong khay
    public float TraySlotSpacing = 1.1f; // Khoảng cách giữa các ô trong khay
    public Vector3 TrayPosition = new Vector3(0, -5f, 0); // Vị trí khay trên màn hình

    public int LevelMoves = 16;

    public float LevelTime = 30f;

    public float TimeForHint = 5f;
}
