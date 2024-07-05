using UnityEngine;
using System.Collections.Generic;
using static CellProperties;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public int maxMoves;
    public int rows;
    public int columns;
    public List<CellStack> stacks = new List<CellStack>();
}
[System.Serializable]
public class CellProperties
{
    public enum CellType { Empty, Frog, Berry, Arrow}
    public enum Color { Blue, Green, Purple, Red, Yellow }
    public enum Direction { Up, Down, Left, Right }

    public CellType cellType;
    public Color color;
    public Direction direction;
}

[System.Serializable]
public class CellStack
{
    public List<CellProperties> cells = new List<CellProperties>();
}