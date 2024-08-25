using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Default_Cell : MonoBehaviour
{
    [HideInInspector] public List<Entity_Cell> adjacentCells = new List<Entity_Cell>();
    public List<Entity_Cell> cellStack = new List<Entity_Cell>();
    public GameObject[] cellPrefabs;
    public GameObject[] frogPrefabs;
    public GameObject[] grapePrefabs;
    public GameObject[] arrowPrefabs;
    public GameManager.Colour cellColour;

    public Entity_Cell activeCell;

    private void OnValidate()
    {
        activeCell = cellStack.Count > 0 ? cellStack[cellStack.Count - 1] : null;
    }

    private void Start()
    {
        CellManager.Instance.cells[(int)-transform.position.x, (int)transform.position.z] = this;
    }


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
    }

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

    private void ActivateEntities<T>(Entity_Cell cell) where T : Component
    {
        foreach (var entity in cell.GetComponentsInChildren<T>(true)) // Include inactive objects
        {
            if (entity != null)
            {
                entity.gameObject.SetActive(true);
                entity.gameObject.transform.localScale = new Vector3(0, 0, 0);
                entity.GetComponent<Entity>().GrowAnim();
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

        Entity_Cell topCell = cellStack[cellStack.Count - 1];
        cellStack.RemoveAt(cellStack.Count - 1);
        activeCell = cellStack.Count > 0 ? cellStack[cellStack.Count - 1] : null;
        if (!Application.isPlaying)
        {
            DestroyImmediate(topCell.gameObject);
        }
        else
        {
            topCell.StartDestory();
        }
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

    private Vector3 GetEntityPosition<T>() where T : Component
    {
        // Adjust these values according to the size of your entities
        if (typeof(T) == typeof(Frog)) return new Vector3(0, 0.15f, 0); // Adjust Y position if needed
        if (typeof(T) == typeof(Grape)) return new Vector3(0, 0.25f, 0); // Adjust Y position if needed
        if (typeof(T) == typeof(Arrow)) return new Vector3(0, 0.30f, 0); // Adjust Y position if needed
        return Vector3.zero;
    }

    //Rotating arrow or frog
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
            if (getColour(entity) == activeCell.GetComponent<Entity_Cell>().cellColour)
            {
                DestroyImmediate(entity.gameObject);
                activeCell.GetComponent<Entity_Cell>().entityOnCell = null;
                entity.GetComponent<Entity>().entityCell = null;
                return;
            }
        }

        Debug.Log($"No matching {typeof(T).Name} found for the active cell color.");
    }



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

        // Sýnýr kontrolü (5x5'lik bir grid)
        if (nextX < 0 || nextX >= 5 || nextZ < 0 || nextZ >= 5)
        {
            return null;
        }

        Default_Cell nextCell = CellManager.Instance.cells[nextX, nextZ];

        return nextCell;
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
