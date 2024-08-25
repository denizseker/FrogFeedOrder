using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    public enum Condition
    {
        FinalCell,
        WrongCell,
        ArrowCell,
        AvailableCell,
        OutofBounds,
    }

    public enum Direction
    {
        Right,
        Left,
        Up,
        Down
    }


    public Default_Cell[,] cells = new Default_Cell[5, 5]; // Grid of cells

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
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update logic if needed
    }





    // Method to link adjacent cells

}