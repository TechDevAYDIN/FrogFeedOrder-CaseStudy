using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool canPlayerMove = true;
    public int movesLeft = 0;
    public List<LevelData> levels;

    public GameObject gridCellPrefab;

    private LevelData activeLevelData;

    private List<GridStack> gridStacks = new List<GridStack>();

    private int activeLevel = 0;

    [SerializeField] TextMeshProUGUI movesText;
    [SerializeField] GameObject passScreen, failScreen;

    private void Start()
    {
        LoadLevel();
        RefreshUI();
    }

    public void LoadLevel()
    {
        activeLevel = PlayerPrefs.GetInt("CurrentStage",0);
        int levelIndex = activeLevel % levels.Count;
        if (levelIndex < 0 || levels.Count == 0)
        {
            Debug.LogError($"Level index {levelIndex} is out of range.");
            return;
        }

        activeLevelData = levels[levelIndex];

        movesLeft = activeLevelData.maxMoves;
        float targetOrthographicSize;
        float aspectRatio = (float)Screen.width / Screen.height;

        targetOrthographicSize = Mathf.Max((activeLevelData.columns + 1) / 2f / aspectRatio, (activeLevelData.rows + 1) / 2f);

        Camera.main.orthographicSize = targetOrthographicSize;

        CreateGrid(activeLevelData.rows, activeLevelData.columns, activeLevelData.stacks);
    }

    private void CreateGrid(int rows, int columns, List<CellStack> stacks)
    {
        // Önce mevcut grid'i temizle
        ClearGrid();

        Vector3 midPoint = new Vector3((columns - 1) / 2f, 0, -(rows-1) / 2f);//Orta nokta tespiti

        // Grid'i oluþtur
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int stackIndex = i * rows + j;
                CellStack stack = stacks[stackIndex];

                Vector3 position = new Vector3(i * 1, 0, -j * 1) - midPoint; // konum (1 birim aralýklarla)

                // Prefab'ý sahneye ekle
                GameObject gridCell = Instantiate(gridCellPrefab, position, Quaternion.identity);
                gridCell.transform.parent = transform; // LevelManager altýnda olacak þekilde ata
                gridCell.name = i + "_" + j;
                gridStacks.Add(gridCell.GetComponent<GridStack>());
                gridCell.GetComponent<GridStack>().cellProperties = stack.cells;
            }
        }
        foreach (GridStack gridStack in gridStacks)
        {
            gridStack.AddAndStackPrefab();
            gridStack.ActivateTopCell();
        }
    }
    public void FrogAction(GridStack stack, LineRenderer tongue)
    {
        movesLeft--;
        RefreshUI();
        if(movesLeft <= 0)
        {
            canPlayerMove = false;
        }
        int startLoc = gridStacks.IndexOf(stack);
        CellProperties.Direction checkDirection = stack.ActiveCell.cellProperties.direction;
        List<GridStack> stackstoVisit = new List<GridStack>();

        int nextIndex = startLoc;
        bool continueTraversal = true;

        stackstoVisit.Add(stack);

        while (continueTraversal && nextIndex >= 0 && nextIndex < gridStacks.Count)
        {
            switch (checkDirection)
            {
                case CellProperties.Direction.Up:
                    nextIndex = startLoc - 1;
                    if (Mathf.FloorToInt(nextIndex / activeLevelData.rows) == Mathf.FloorToInt(startLoc / activeLevelData.rows))
                    {
                        if (nextIndex >= 0 && nextIndex < gridStacks.Count)
                        {
                            GridStack nextStack = gridStacks[nextIndex];
                            if (!stackstoVisit.Contains(nextStack) && (nextStack.ActiveCell.cellProperties.cellType != CellProperties.CellType.Empty))
                            {
                                stackstoVisit.Add(nextStack);
                                startLoc = nextIndex;
                                if (nextStack.ActiveCell.cellProperties.cellType == CellProperties.CellType.Arrow)
                                {
                                    checkDirection = nextStack.ActiveCell.cellProperties.direction;
                                }
                            }
                            else
                            {
                                continueTraversal = false;
                            }
                        }
                        else
                        {
                            continueTraversal = false;
                        }
                    }
                    else
                    {
                        continueTraversal = false;
                    }
                    break;
                case CellProperties.Direction.Down:
                    nextIndex = startLoc + 1;
                    if (Mathf.FloorToInt(nextIndex / activeLevelData.rows) == Mathf.FloorToInt(startLoc / activeLevelData.rows))
                    {
                        if (nextIndex >= 0 && nextIndex < gridStacks.Count)
                        {
                            GridStack nextStack = gridStacks[nextIndex];
                            if (!stackstoVisit.Contains(nextStack) && (nextStack.ActiveCell.cellProperties.cellType != CellProperties.CellType.Empty))
                            {
                                stackstoVisit.Add(nextStack);
                                startLoc = nextIndex;
                                if (nextStack.ActiveCell.cellProperties.cellType == CellProperties.CellType.Arrow)
                                {
                                    checkDirection = nextStack.ActiveCell.cellProperties.direction;
                                }
                            }
                            else
                            {
                                continueTraversal = false;
                            }
                        }
                        else
                        {
                            continueTraversal = false;
                        }
                    }
                    else
                    {
                        continueTraversal = false;
                    }
                    break;
                case CellProperties.Direction.Left:
                    nextIndex = startLoc - activeLevelData.rows;
                    if (Mathf.CeilToInt(nextIndex % activeLevelData.rows) == Mathf.CeilToInt(startLoc % activeLevelData.rows))
                    {
                        if (nextIndex >= 0 && nextIndex < gridStacks.Count)
                        {
                            GridStack nextStack = gridStacks[nextIndex];
                            if (!stackstoVisit.Contains(nextStack) && (nextStack.ActiveCell.cellProperties.cellType != CellProperties.CellType.Empty))
                            {
                                stackstoVisit.Add(nextStack);
                                startLoc = nextIndex;
                                if (nextStack.ActiveCell.cellProperties.cellType == CellProperties.CellType.Arrow)
                                {
                                    checkDirection = nextStack.ActiveCell.cellProperties.direction;
                                }
                            }
                            else
                            {
                                continueTraversal = false;
                            }
                        }
                        else
                        {
                            continueTraversal = false;
                        }

                    }
                    else
                    {
                        continueTraversal = false;
                    }
                    break;
                case CellProperties.Direction.Right:
                    nextIndex = startLoc + activeLevelData.rows;
                    if (Mathf.CeilToInt(nextIndex % activeLevelData.rows) == Mathf.CeilToInt(startLoc % activeLevelData.rows))
                    {
                        if (nextIndex >= 0 && nextIndex < gridStacks.Count)
                        {
                            GridStack nextStack = gridStacks[nextIndex];
                            if (!stackstoVisit.Contains(nextStack) && (nextStack.ActiveCell.cellProperties.cellType != CellProperties.CellType.Empty))
                            {
                                stackstoVisit.Add(nextStack);
                                startLoc = nextIndex;
                                if (nextStack.ActiveCell.cellProperties.cellType == CellProperties.CellType.Arrow)
                                {
                                    checkDirection = nextStack.ActiveCell.cellProperties.direction;
                                }
                            }
                            else
                            {
                                continueTraversal = false;
                            }
                        }
                        else
                        {
                            continueTraversal = false;
                        }
                    }
                    else
                    {
                        continueTraversal = false;
                    }
                    break;
            }
        }
        StartCoroutine(CollectBerries(tongue, stackstoVisit));
    }

    IEnumerator CollectBerries(LineRenderer tongue, List<GridStack> stackstoVisit)
    {
        List<Cell> collectedCells = new List<Cell>();
        bool legalMove = true;
        float timePerPosition = 0.2f; // Her bir pozisyon için geçecek süre
        float elapsedTime = 0f; // Geçen süreyi tutacak deðiþken
        collectedCells.Add(stackstoVisit[0].ActiveCell);
        // Her bir pozisyon için lerp iþlemi yap
        tongue.positionCount = 1;
        tongue.SetPosition(0, stackstoVisit[0].transform.position + Vector3.up * tongue.transform.position.y);
        for (int i = 1; i < stackstoVisit.Count; i++)
        {
            tongue.positionCount++;
            tongue.SetPosition(i, stackstoVisit[i-1].transform.position + Vector3.up * tongue.transform.position.y);
            Vector3 startPos = tongue.GetPosition(i);
            Vector3 targetPos = stackstoVisit[i].transform.position + Vector3.up * tongue.transform.position.y;

            while (elapsedTime < timePerPosition)
            {
                tongue.SetPosition(i, Vector3.Lerp(startPos, targetPos, elapsedTime / timePerPosition));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            tongue.SetPosition(i, targetPos);
            elapsedTime = 0f;
            if(stackstoVisit[i].ActiveCell.cellProperties.cellType == CellProperties.CellType.Berry)
            {
                if (stackstoVisit[0].ActiveCell.cellProperties.color == stackstoVisit[i].ActiveCell.cellProperties.color)
                {
                    stackstoVisit[i].ActiveCell.GetClicked(true);
                    collectedCells.Add(stackstoVisit[i].ActiveCell);
                }
                else
                {
                    legalMove = false;
                    stackstoVisit[i].ActiveCell.GetClicked(false);
                    break;
                }
            }
            if (stackstoVisit[i].ActiveCell.cellProperties.cellType == CellProperties.CellType.Arrow)
            {
                stackstoVisit[i].ActiveCell.GetClicked(true);
                    collectedCells.Add(stackstoVisit[i].ActiveCell);
            }
        }
        timePerPosition = .1f;
        if (legalMove)
        {
            for (int a = 1; a < collectedCells.Count; a++)
            {
                collectedCells[collectedCells.Count - a].DeactivateCell(timePerPosition * a);
            }
        }
        for (int i = tongue.positionCount - 1; i > 0; i--)
        {
            Vector3 startPos = tongue.GetPosition(i);
            Vector3 targetPos = tongue.GetPosition(i-1);
            
            while (elapsedTime <= timePerPosition)
            {
                // Lerp ile pozisyonu güncelle
                tongue.SetPosition(i, Vector3.Lerp(startPos, targetPos, elapsedTime / timePerPosition));
                if (legalMove)
                {
                    for (int j = 1; j < collectedCells.Count; j++)
                    {
                        if(collectedCells[j].cellProperties.cellType == CellProperties.CellType.Berry)
                        {
                            Vector3 cellStartPos;
                            cellStartPos = tongue.GetPosition(j);
                            Vector3 cellTargetPos = tongue.GetPosition(j - 1);
                            collectedCells[j].childObject.transform.position = Vector3.Lerp(cellStartPos, cellTargetPos, elapsedTime / timePerPosition);
                            if (j == 1)
                            {
                                collectedCells[j].childObject.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, elapsedTime / timePerPosition);
                            }
                        }
                    }
                }
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            tongue.SetPosition(i, targetPos);
            if(collectedCells.Count > 1 && legalMove)
            {
                Destroy(collectedCells[1].childObject);
                collectedCells.RemoveAt(1);
            }
            tongue.positionCount--;
            elapsedTime = 0f;
        }
        if (legalMove)
        {
            collectedCells[0].DeactivateCell(timePerPosition);
        }
        else
        {
            stackstoVisit[0].canClickable = true;
        }
        yield return new WaitForSeconds(.5f);
        CheckPassedOrFailed();
    }

    public void CheckPassedOrFailed()
    {
        GridStack firstNonEmptyStack = gridStacks.Find(stack => stack.ActiveCell.cellProperties.cellType != CellProperties.CellType.Empty);
        if(firstNonEmptyStack == null)
        {
            LevelPassed();
        }
        else
        {
            if (movesLeft == 0)
                LevelFailed();
        }
    }
    private void LevelPassed()
    {
        PlayerPrefs.SetInt("CurrentStage", activeLevel + 1);
        passScreen.SetActive(true);
    }
    private void LevelFailed()
    {
        failScreen.SetActive(true);
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(0);
    }
    private void ClearGrid()
    {
        // LevelManager altýnda bulunan tüm child objeleri sil
        foreach (Transform child in transform)
        {
            gridStacks = new List<GridStack>();
            Destroy(child.gameObject);
        }
    }
    private void RefreshUI()
    {
        movesText.text = "Moves Left: " + movesLeft.ToString(); ;
    }
}
