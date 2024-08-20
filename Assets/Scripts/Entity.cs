using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected CellManager.Direction entityDirection;
    public Cell entityCell;
    public GameManager.Colour entityColour;
    public abstract void Interact();
}
