using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected CellManager.Direction entityDirection;
    public GameObject entityCell;
    public GameManager.Colour entityColour;
    public abstract void Interact();
}
