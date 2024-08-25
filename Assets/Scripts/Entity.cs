using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public CellManager.Direction entityDirection;
    public Entity_Cell entityCell;
    public GameManager.Colour entityColour;
    public abstract void HitByTongue(Frog frog);


    public void GrowAnim()
    {
        transform.DOScale(1f, 0.45f).SetEase(Ease.Linear);
    }

}
