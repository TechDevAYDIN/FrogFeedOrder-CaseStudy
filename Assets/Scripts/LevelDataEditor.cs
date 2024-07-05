using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private LevelData levelData;
    private SerializedProperty stacksProperty;
    private SerializedProperty textureDataProperty;
    private Vector2 scrollPos;

    private void OnEnable()
    {
        levelData = (LevelData)target;
        stacksProperty = serializedObject.FindProperty("stacks");
        textureDataProperty = serializedObject.FindProperty("textureData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        levelData.maxMoves = EditorGUILayout.IntField("Max Moves", levelData.maxMoves);
        levelData.rows = EditorGUILayout.IntField("Rows", Mathf.Max(1, levelData.rows)); // En az 1 satýr
        levelData.columns = EditorGUILayout.IntField("Columns", Mathf.Max(1, levelData.columns)); // En az 1 sütun

        EditorGUILayout.Space();

        // Stack listesini görüntüleme
        if (levelData.stacks.Count == 0 || levelData.stacks.Count != levelData.rows * levelData.columns)
        {
            InitializeGrid();
        }

        // Kaydýrýlabilir stack listesi görünümü
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DrawStacks();
        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
    }

    private void InitializeGrid()
    {
        levelData.stacks.Clear();

        for (int i = 0; i < levelData.rows * levelData.columns; i++)
        {
            CellStack stack = new CellStack();
            levelData.stacks.Add(stack);
        }
    }

    private void DrawStacks()
    {
        if (levelData.stacks.Count != levelData.rows * levelData.columns)
            return;

        EditorGUILayout.BeginHorizontal();

        int stackIndex = 0;
        for (int i = 0; i < levelData.columns; i++)
        {
            EditorGUILayout.BeginVertical();

            for (int j = 0; j < levelData.rows; j++)
            {
                CellStack stack = levelData.stacks[stackIndex];

                if (stack.cells.Count == 0)
                {
                    stack.cells.Add(new CellProperties { cellType = CellProperties.CellType.Empty });
                }
                
                string buttonText = $"";
                for (int k = 0; k < stack.cells.Count; k++)
                {
                    buttonText += $"{stack.cells[k].cellType}";

                    if (stack.cells[k].cellType == CellProperties.CellType.Frog)
                    {
                        buttonText += $", {stack.cells[k].color}, {stack.cells[k].direction}";
                    }
                    else if (stack.cells[k].cellType == CellProperties.CellType.Arrow)
                    {
                        buttonText += $", {stack.cells[k].direction}";
                    }
                    else if (stack.cells[k].cellType == CellProperties.CellType.Berry)
                    {
                        buttonText += $", {stack.cells[k].color}";
                    }

                    if (k < stack.cells.Count - 1)
                    {
                        buttonText += "\n";
                    }
                }

                if (GUILayout.Button(buttonText, GUILayout.Width(100), GUILayout.Height(100)))
                {
                    ShowCellListWindow(stackIndex);
                }

                stackIndex++;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
    }
    private void ShowCellListWindow(int stackIndex)
    {
        CellListWindow.ShowWindow(levelData.stacks[stackIndex], (updatedStack) =>
        {
            levelData.stacks[stackIndex] = updatedStack; // Stack'i güncelle
            EditorUtility.SetDirty(levelData); // Deðiþiklikleri kaydet
        });
    }

    private class CellListWindow : EditorWindow
    {
        private CellStack cellStack;
        private System.Action<CellStack> onSaveCallback;

        public static void ShowWindow(CellStack stack, System.Action<CellStack> onSave)
        {
            CellListWindow window = GetWindow<CellListWindow>(true, "Cell List");
            window.cellStack = stack;
            window.onSaveCallback = onSave;
            window.Show();
        }

        private void OnGUI()
        {
            if (cellStack == null)
                return;

            // Her hücre için özellikleri düzenleme
            for (int i = 0; i < cellStack.cells.Count; i++)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Cell {i + 1}", GUILayout.Width(50));

                cellStack.cells[i].cellType = (CellProperties.CellType)EditorGUILayout.EnumPopup(cellStack.cells[i].cellType);

                // CellType'a göre hangi alanlarýn görüneceðini ayarla
                switch (cellStack.cells[i].cellType)
                {
                    case CellProperties.CellType.Frog:
                        cellStack.cells[i].color = (CellProperties.Color)EditorGUILayout.EnumPopup(cellStack.cells[i].color);
                        cellStack.cells[i].direction = (CellProperties.Direction)EditorGUILayout.EnumPopup(cellStack.cells[i].direction);
                        break;

                    case CellProperties.CellType.Arrow:
                        cellStack.cells[i].direction = (CellProperties.Direction)EditorGUILayout.EnumPopup(cellStack.cells[i].direction);
                        break;

                    case CellProperties.CellType.Berry:
                        cellStack.cells[i].color = (CellProperties.Color)EditorGUILayout.EnumPopup(cellStack.cells[i].color);
                        break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Cell"))
            {
                cellStack.cells.Add(new CellProperties());
            }

            if (GUILayout.Button("Remove Last Cell") && cellStack.cells.Count > 0)
            {
                cellStack.cells.RemoveAt(cellStack.cells.Count - 1);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
                onSaveCallback?.Invoke(cellStack); // Deðiþiklikleri kaydet
                Close();
            }
        }
    }
}
