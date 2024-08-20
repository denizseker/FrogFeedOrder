using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cell_Default : Cell
{
    [HideInInspector] public List<Cell> adjacentCells = new List<Cell>();
    public List<Cell> cellStack = new List<Cell>();
    public GameObject[] cellPrefabs;
    public GameObject[] frogPrefabs;
    public GameObject[] grapePrefabs;
    public GameObject[] arrowPrefabs;

    public Cell activeCell;

    private void OnValidate()
    {
        activeCell = cellStack.Count > 0 ? cellStack[cellStack.Count - 1] : this;
    }

    private void Start()
    {
        CellManager.Instance.cells[(int)-transform.position.x, (int)transform.position.z] = this;
        ownerDefaultCell = this;
    }


    public void AddCell(GameObject prefab, string entityType)
    {
        if (prefab == null) return;

        float height = (cellStack.Count + 1) / 10.0f;
        GameObject newCell = Instantiate(prefab, transform.position + Vector3.up * height, Quaternion.identity);
        newCell.transform.SetParent(transform);
        newCell.GetComponent<Cell>().ownerDefaultCell = this;
        activeCell = newCell.GetComponent<Cell>();
        cellStack.Add(newCell.GetComponent<Cell>());

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
    }

    public void UpdateEntityStates()
    {
        // Deactivate entities in all cells
        foreach (Cell cell in cellStack)
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

    private void DeactivateEntities<T>(Cell cell) where T : Component
    {
        foreach (var entity in cell.GetComponentsInChildren<T>(true)) // Include inactive objects
        {
            if (entity != null)
            {
                entity.gameObject.SetActive(false);
            }
        }
    }

    private void ActivateEntities<T>(Cell cell) where T : Component
    {
        foreach (var entity in cell.GetComponentsInChildren<T>(true)) // Include inactive objects
        {
            if (entity != null)
            {
                entity.gameObject.SetActive(true);
            }
        }
    }

    public void DeleteCell()
    {
        if (cellStack.Count == 0)
        {
            Debug.Log("No cells to delete.");
            return;
        }

        Cell topCell = cellStack[cellStack.Count - 1];
        cellStack.RemoveAt(cellStack.Count - 1);
        activeCell = cellStack.Count > 0 ? cellStack[cellStack.Count - 1] : this;
        DestroyImmediate(topCell.gameObject);

        // Ensure activeCell is updated and entities are updated
        UpdateEntityStates();
    }

    public void DeleteAllCells()
    {
        while (cellStack.Count > 0)
        {
            DeleteCell();
        }

        Debug.Log("All cells deleted.");
    }

    public void AddAdjacentCell(Cell cell)
    {
        if (cell != null && !adjacentCells.Contains(cell))
        {
            adjacentCells.Add(cell);
        }
    }

    private void AddEntityToActiveCell<T>(GameObject[] prefabs, System.Func<T, GameManager.Colour> getColour) where T : Component
    {
        if (activeCell == null || activeCell == gameObject) return;

        var activeCellColour = activeCell.GetComponent<Cell>().cellColour;

        foreach (var prefab in prefabs)
        {
            var entity = prefab.GetComponent<T>();
            if (getColour(entity) == activeCellColour && activeCell.GetComponent<Cell>().entityOnCell == null)
            {
                var entityPrefab = prefab;
                var position = activeCell.transform.position + GetEntityPosition<T>(); // Adjust position relative to activeCell
                var instance = Instantiate(entityPrefab, position, Quaternion.identity);
                instance.transform.parent = activeCell.transform;
                activeCell.GetComponent<Cell>().entityOnCell = instance.GetComponent<Entity>();
                instance.GetComponent<Entity>().entityCell = activeCell;
                return;
            }
        }

        Debug.Log($"No matching {typeof(T).Name} prefab found for the active cell color.");
    }

    private Vector3 GetEntityPosition<T>() where T : Component
    {
        // Adjust these values according to the size of your entities
        if (typeof(T) == typeof(Frog)) return new Vector3(0, 0.15f, 0); // Adjust Y position if needed
        if (typeof(T) == typeof(Grape)) return new Vector3(0, 0.25f, 0); // Adjust Y position if needed
        if (typeof(T) == typeof(Arrow)) return new Vector3(0, 0.35f, 0); // Adjust Y position if needed
        return Vector3.zero;
    }

    //Rotating arrow or frog
    public void RotateEntity()
    {
        if (activeCell != null)
        {
            var cell = activeCell.GetComponent<Cell>();
            var entity = cell.entityOnCell;

            if (entity != null && (entity.GetComponent<Frog>() != null || entity.GetComponent<Arrow>() != null))
            {
                entity.gameObject.transform.Rotate(0, 90, 0);
            }
        }
    }

    public void AddFrogToActiveCell() => AddEntityToActiveCell<Frog>(frogPrefabs, frog => frog.entityColour);
    public void DeleteFrogFromActiveCell() => DeleteEntityFromActiveCell<Frog>(frog => frog.entityColour);
    public void AddGrapeToActiveCell() => AddEntityToActiveCell<Grape>(grapePrefabs, grape => grape.entityColour);
    public void DeleteGrapeFromActiveCell() => DeleteEntityFromActiveCell<Grape>(grape => grape.entityColour);
    public void AddArrowToActiveCell() => AddEntityToActiveCell<Arrow>(arrowPrefabs, arrow => arrow.entityColour);
    public void DeleteArrowFromActiveCell() => DeleteEntityFromActiveCell<Arrow>(arrow => arrow.entityColour);

    private void DeleteEntityFromActiveCell<T>(System.Func<T, GameManager.Colour> getColour) where T : Component
    {
        if (activeCell == null || activeCell == gameObject) return;

        foreach (var entity in activeCell.GetComponentsInChildren<T>())
        {
            if (getColour(entity) == activeCell.GetComponent<Cell>().cellColour)
            {
                DestroyImmediate(entity.gameObject);
                activeCell.GetComponent<Cell>().entityOnCell = null;
                entity.GetComponent<Entity>().entityCell = null;
                return;
            }
        }

        Debug.Log($"No matching {typeof(T).Name} found for the active cell color.");
    }



    public Cell CheckNextCell(CellManager.Direction direction)
    {

        Cell_Default nextCell;

        switch (direction)
        {
            case CellManager.Direction.Right:
                nextCell = CellManager.Instance.cells[(int)-transform.position.x + 1, (int)transform.position.z];
                break;
            case CellManager.Direction.Left:
                nextCell = CellManager.Instance.cells[(int)-transform.position.x - 1, (int)transform.position.z];
                break;
            case CellManager.Direction.Up:
                nextCell = CellManager.Instance.cells[(int)-transform.position.x, (int)transform.position.z - 1];
                break;
            case CellManager.Direction.Down:
                nextCell = CellManager.Instance.cells[(int)-transform.position.x, (int)transform.position.z + 1];
                break;
            default:
                nextCell = null;
                break;
        }


        if (nextCell.activeCell.cellColour == activeCell.cellColour)
        {
            return nextCell.activeCell;
        }
        else
        {
            return null;
        }
    }




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
        Handles.Label(position + Vector3.up * 0.1f, coordinates, style);
    }

    private void Update()
    {
        // Placeholder for future updates
    }
}
