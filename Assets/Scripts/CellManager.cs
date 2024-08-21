using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{


    public enum Condition
    {
        FinalCell,
        WrongCell,
        ArrowCell,
        NextCell,
    }

    public enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }


    public Cell_Default[,] cells = new Cell_Default[5, 5]; // Grid of cells

    public static CellManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LinkCells();       // Link adjacent cells
    }

    // Update is called once per frame
    void Update()
    {
        // Update logic if needed
    }





    // Method to link adjacent cells
    public void LinkCells()
    {
        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Cell_Default currentCell = cells[x, z];
                if (currentCell != null)
                {
                    // Check adjacent cells and link them
                    if (x > 0 && cells[x - 1, z] != null)
                    {
                        currentCell.AddAdjacentCell(cells[x - 1, z]); // Link left
                    }
                    if (x < width - 1 && cells[x + 1, z] != null)
                    {
                        currentCell.AddAdjacentCell(cells[x + 1, z]); // Link right
                    }
                    if (z > 0 && cells[x, z - 1] != null)
                    {
                        currentCell.AddAdjacentCell(cells[x, z - 1]); // Link below
                    }
                    if (z < height - 1 && cells[x, z + 1] != null)
                    {
                        currentCell.AddAdjacentCell(cells[x, z + 1]); // Link above
                    }
                }
            }
        }
    }
}