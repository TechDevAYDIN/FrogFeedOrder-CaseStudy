using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridStack : MonoBehaviour
{
    public bool canClickable = true;
    private List<Cell> cells = new List<Cell>();
    public List<CellProperties> cellProperties = new List<CellProperties>();
    [SerializeField] private GameObject cellPrefab; // SerializeField ile tanýmlanmýþ prefab

    public Cell ActiveCell;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (ActiveCell != null && canClickable && GameManager.Instance.canPlayerMove)
        {
            canClickable = false;
            ActiveCell.GetClicked(true);
            if (ActiveCell.cellProperties.cellType == CellProperties.CellType.Frog)
            {
                GameManager.Instance.FrogAction(this, ActiveCell.TriggerFrog());
            }
        }
    }
    public void AddAndStackPrefab()
    {
        float stackHeight = 0;
        // Her bir hücre için prefab oluþtur
        foreach (CellProperties cellProperty in cellProperties)
        {
            GameObject instance = Instantiate(cellPrefab,transform.position + Vector3.up * stackHeight, Quaternion.identity, transform); // Prefab örneði oluþtur
            Cell cell = instance.GetComponent<Cell>();
            cell.cellProperties = cellProperty;
            cells.Add(cell);
            stackHeight += .1f;
        }
    }

    public void ActivateTopCell()
    {
        if (cells.Count > 0)
        {
            Cell topCell = cells[cells.Count - 1];
            topCell.ActivateCell();
            ActiveCell = topCell;
            cells.Remove(topCell);
        }
    }
}
