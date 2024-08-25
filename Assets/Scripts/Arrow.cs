using UnityEngine;

public class Arrow :  Entity
{
    public override void HitByTongue(Frog frog)
    {
        //Debug.Log(gameObject + " Hit by tongue" + frog);
    }

    public void SetDirection()
    {
        Vector3 forward = transform.forward;

        if (Vector3.Dot(forward, Vector3.forward) > 0.9f)
        {
            entityDirection = CellManager.Direction.Up;
        }
        else if (Vector3.Dot(forward, Vector3.back) > 0.9f)
        {
            entityDirection = CellManager.Direction.Down;
        }
        else if (Vector3.Dot(forward, Vector3.right) > 0.9f)
        {
            entityDirection = CellManager.Direction.Right;
        }
        else if (Vector3.Dot(forward, Vector3.left) > 0.9f)
        {
            entityDirection = CellManager.Direction.Left;
        }
    }

}