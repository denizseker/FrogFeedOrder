using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Default_Cell : MonoBehaviour
{
    [HideInInspector] public List<Entity_Cell> adjacentCells = new List<Entity_Cell>(); // List of adjacent cells
    public List<Entity_Cell> cellStack = new List<Entity_Cell>(); // Stack of cells in this position
    public GameObject[] cellPrefabs; // Array of cell prefabs
    public GameObject[] frogPrefabs; // Array of frog prefabs
    public GameObject[] grapePrefabs; // Array of grape prefabs
    public GameObject[] arrowPrefabs; // Array of arrow prefabs
    public GameManager.Colour cellColour; // Color of the cell

    public Entity_Cell activeCell; // Currently active cell

    private void OnValidate()
    {
        // Set active cell to the top cell in the stack
        activeCell = cellStack.Count > 0 ? cellStack[cellStack.Count - 1] : null;
    }

    private void Start()
    {
        // Register this cell in the CellManager
        CellManager.Instance.cells[(int)-transform.position.x, (int)transform.position.z] = this;
    }

    // Add a new cell to the stack
    public void AddCell(GameObject prefab, string entityType)
    {
        if (prefab == null) return;

        float height = (cellStack.Count + 1) / 10.0f;
        GameObject newCell = Instantiate(prefab, transform.position + Vector3.up * height, Quaternion.identity);
        newCell.transform.SetParent(transform);
        newCell.GetComponent<Entity_Cell>().ownerDefaultCell = this;
        activeCell = newCell.GetComponent<Entity_Cell>();
        cellStack.Add(newCell.GetComponent<Entity_Cell>());

        UpdateEntityStates();

        // Add selected entity
        switch (entityType)
        {
            case "Frog":
                AddEntityToActiveCell<Frog>(frogPrefabs, frog => frog.entityColour);
                break;
            case "Grape":
                AddEntityToActiveCell<Grape>(grapePrefabs, grape => grape.entityColour);
                break;
            case "Arrow":
                AddEntityToActiveCell<Arrow>(arrowPrefabs, arrow => arrow.entityColour);
                break;
        }
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(gameObject);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            EditorSceneManager.SaveOpenScenes();
        }
    }

    // Update the states of entities in the cells
    public void UpdateEntityStates()
    {
        // Deactivate entities in all cells
        foreach (Entity_Cell cell in cellStack)
        {
            if (cell == activeCell) continue;

            DeactivateEntities<Frog>(cell);
            DeactivateEntities<Grape>(cell);
            DeactivateEntities<Arrow>(cell);
        }

        // Activate entities in the active cell
        if (activeCell != null)
        {
            ActivateEntities<Frog>(activeCell);
            ActivateEntities<Grape>(activeCell);
            ActivateEntities<Arrow>(activeCell);
        }
    }

    // Deactivate entities of type T in the given cell
    private void DeactivateEntities<T>(Entity_Cell cell) where T : Component
    {
        foreach (var entity in cell.GetComponentsInChildren<T>(true)) // Include inactive objects
        {
            if (entity != null)
            {
                entity.gameObject.SetActive(false);
            }
        }
    }

    // Activate entities of type T in the given cell
    private void ActivateEntities<T>(Entity_Cell cell) where T : Component
    {
        foreach (var entity in cell.GetComponentsInChildren<T>(true)) // Include inactive objects
        {
            if (entity != null)
            {
                entity.gameObject.SetActive(true);

                if (Application.isPlaying)
                {
                    entity.gameObject.transform.localScale = new Vector3(0, 0, 0);
                    entity.GetComponent<Entity>().TriggerSpawnAnimation();
                }
            }
        }
    }

    // Delete the top cell in the stack
    public void DeleteCell()
    {
        if (cellStack.Count == 0)
        {
            Debug.Log("No cells to delete.");
            return;
        }

        Entity_Cell topCell = cellStack[cellStack.Count - 1];
        cellStack.RemoveAt(cellStack.Count - 1);
        activeCell = cellStack.Count > 0 ? cellStack[cellStack.Count - 1] : null;
        if (!Application.isPlaying)
        {
            DestroyImmediate(topCell.gameObject);
            // Save changes to the scene
            EditorUtility.SetDirty(gameObject);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            EditorSceneManager.SaveOpenScenes();
        }
        else
        {
            topCell.StartDestory();
        }
        // Ensure activeCell is updated and entities are updated
        UpdateEntityStates();
    }

    // Delete all cells in the stack
    public void DeleteAllCells()
    {
        while (cellStack.Count > 0)
        {
            DeleteCell();
        }

        Debug.Log("All cells deleted.");
    }

    // Add an entity to the active cell
    private void AddEntityToActiveCell<T>(GameObject[] prefabs, System.Func<T, GameManager.Colour> getColour) where T : Component
    {
        if (activeCell == null || activeCell == gameObject) return;

        var activeCellColour = activeCell.GetComponent<Entity_Cell>().cellColour;

        foreach (var prefab in prefabs)
        {
            var entity = prefab.GetComponent<T>();
            if (getColour(entity) == activeCellColour && activeCell.GetComponent<Entity_Cell>().entityOnCell == null)
            {
                var entityPrefab = prefab;
                var position = activeCell.transform.position + GetEntityPosition<T>(); // Adjust position relative to activeCell
                var instance = Instantiate(entityPrefab, position, Quaternion.identity);
                instance.transform.parent = activeCell.transform;
                activeCell.GetComponent<Entity_Cell>().entityOnCell = instance.GetComponent<Entity>();
                instance.GetComponent<Entity>().entityCell = activeCell;
                return;
            }
        }

        Debug.Log($"No matching {typeof(T).Name} prefab found for the active cell color.");
    }

    // Get the position offset for the entity type
    private Vector3 GetEntityPosition<T>() where T : Component
    {
        // Adjust these values according to the size of your entities
        if (typeof(T) == typeof(Frog)) return new Vector3(0, 0.15f, 0); // Adjust Y position if needed
        if (typeof(T) == typeof(Grape)) return new Vector3(0, 0.25f, 0); // Adjust Y position if needed
        if (typeof(T) == typeof(Arrow)) return new Vector3(0, 0.30f, 0); // Adjust Y position if needed
        return Vector3.zero;
    }

    // Rotate the entity in the active cell
    public void RotateEntity()
    {
        if (activeCell != null)
        {
            var cell = activeCell.GetComponent<Entity_Cell>();
            var entity = cell.entityOnCell;

            if (entity != null && (entity.GetComponent<Frog>() != null || entity.GetComponent<Arrow>() != null))
            {
                entity.gameObject.transform.Rotate(0, 90, 0);
            }
        }
    }

    // Get the neighboring cell in the specified direction
    public Default_Cell GetNeigborCellAtDirection(CellManager.Direction direction)
    {
        int nextX = (int)-transform.position.x;
        int nextZ = (int)transform.position.z;

        switch (direction)
        {
            case CellManager.Direction.Right:
                nextX += 1;
                break;
            case CellManager.Direction.Left:
                nextX -= 1;
                break;
            case CellManager.Direction.Up:
                nextZ -= 1;
                break;
            case CellManager.Direction.Down:
                nextZ += 1;
                break;
            default:
                return null;
        }

        // Boundary check (5x5 grid)
        if (nextX < 0 || nextX >= 5 || nextZ < 0 || nextZ >= 5)
        {
            return null;
        }

        Default_Cell nextCell = CellManager.Instance.cells[nextX, nextZ];

        return nextCell;
    }

    // Draw gizmos in the editor
    private void OnDrawGizmos()
    {
        var position = transform.position;
        var coordinates = $"({-position.x}, {position.z})";

        // Set Gizmos color to red for other Gizmos elements
        Gizmos.color = Color.red;

        // Create a new GUIStyle with red text color
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 15; // Adjust font size as needed
        style.alignment = TextAnchor.MiddleCenter; // Center text alignment

        // Draw the label using Handles with the created GUIStyle
#if UNITY_EDITOR
        Handles.Label(position + Vector3.up * 0.1f, coordinates, style);
#endif
    }

    private void Update()
    {
        // Placeholder for future updates
    }
}
