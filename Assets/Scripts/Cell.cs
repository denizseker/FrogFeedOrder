using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Entity entityOnCell;
    [HideInInspector] public GameManager.Colour cellColour;
    [HideInInspector] public Cell_Default ownerDefaultCell;


    private void OnEnable()
    {
        //if(bottomCell.GetComponent<Cell_Default>().activeCell == gameObject)
        //{
        //    Debug.Log("Bu");
        //}
    }

}
