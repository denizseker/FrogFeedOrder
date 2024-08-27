using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Frog : Entity
{
    private LineRenderer lineRenderer;
    private Default_Cell _defaultCell;
    private float duration = 0.3f;
    public List<Entity_Cell> visitedCells = new List<Entity_Cell>();
    public Sequence collectSequence;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        _defaultCell = entityCell.ownerDefaultCell;
        collectSequence = DOTween.Sequence().Pause();  // Pause the sequence initially
    }

    // Sets the direction based on the frog's current forward vector
    private void SetDirection()
    {
        Vector3 forward = transform.forward;

        if (Vector3.Dot(forward, Vector3.forward) > 0.9f)
            entityDirection = CellManager.Direction.Up;
        else if (Vector3.Dot(forward, Vector3.back) > 0.9f)
            entityDirection = CellManager.Direction.Down;
        else if (Vector3.Dot(forward, Vector3.right) > 0.9f)
            entityDirection = CellManager.Direction.Right;
        else if (Vector3.Dot(forward, Vector3.left) > 0.9f)
            entityDirection = CellManager.Direction.Left;
    }

    // Handles the logic when the frog's tongue reaches a cell
    private void TongueReachedToThisCell(Default_Cell cell)
    {
        var entity = cell.activeCell.entityOnCell;
        entity.HitByTongue(this);

        if (entity is Arrow arrow)
        {
            if (IsColourSame(cell))
            {
                arrow.SetDirection();
                entityDirection = arrow.entityDirection;
            }
            else
            {
                TriggerFail();
                Debug.Log("Wrong colour arrow");
                return;
            }
        }
        else if (entity is Grape grape)
        {
            if (IsColourSame(cell))
            {
                Vector3[] tempPoints = GetLineRendererPositions();
                grape.SetForCollect(tempPoints, collectSequence);
            }
            else
            {
                TriggerFail();
                Debug.Log("Wrong colour grape");
                return;
            }
        }
        else if (entity is Frog frog)
        {
            TriggerFail();
            Debug.Log("Hit to frog");
        }

        if (IsNextCellValid(cell))
            SetTongueForNextCell(cell);
        else
            DOVirtual.DelayedCall(0.6f,StartReturnAnimation); // Delayed return animation start
    }

    // Validates if the next cell in the direction is available
    private bool IsNextCellValid(Default_Cell currentCell)
    {
        var nextCell = currentCell.GetNeigborCellAtDirection(entityDirection);
        return nextCell?.activeCell != null;
    }

    // Sets the frog's tongue to move towards the next cell
    private void SetTongueForNextCell(Default_Cell currentCell)
    {
        var nextCell = currentCell.GetNeigborCellAtDirection(entityDirection);
        Vector3 newEndPoint = nextCell.activeCell.entityOnCell.transform.position;

        visitedCells.Add(nextCell.activeCell);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, lineRenderer.GetPosition(lineRenderer.positionCount - 2));

        MoveTongueToNextCellPosition(newEndPoint, nextCell, lineRenderer.positionCount - 1);
    }

    // Handles the hit by tongue event for other entities
    public override void HitByTongue(Frog frog)
    {
        Debug.Log($"{gameObject} Hit by tongue {frog}");
    }

    // Moves the tongue to the next cell's position
    private void MoveTongueToNextCellPosition(Vector3 endPoint, Default_Cell currentCell, int newIndex)
    {
        DOTween.To(() => lineRenderer.GetPosition(newIndex - 1),
                   x => lineRenderer.SetPosition(newIndex, x),
                   endPoint, duration)
               .SetEase(Ease.Linear)
               .OnComplete(() => TongueReachedToThisCell(currentCell));
    }

    // Checks if the current cell's color matches the frog's color
    private bool IsColourSame(Default_Cell currentCell)
    {
        return currentCell.activeCell.cellColour == entityCell.cellColour;
    }

    // Plays a hit animation on the frog
    public void Hit()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(1.4f, 0.1f).SetEase(Ease.InOutQuad))
            .Append(transform.DOScale(1f, 0.4f).SetEase(Ease.OutQuad))
            .Play();
    }

    // Starts the animation for returning the tongue to the original position
    private void StartReturnAnimation()
    {
        if (lineRenderer.positionCount < 2)
        {
            Debug.LogWarning("Not enough points to return.");
            return;
        }

        Sequence returnSequenceTongue = DOTween.Sequence();
        for (int i = lineRenderer.positionCount - 1; i > 0; i--)
        {
            int currentIndex = i;
            int nextIndex = i - 1;

            returnSequenceTongue.Append(DOTween.To(() => lineRenderer.GetPosition(currentIndex),
                                                   x => lineRenderer.SetPosition(currentIndex, x),
                                                   lineRenderer.GetPosition(nextIndex), duration)
                                              .SetEase(Ease.Linear)
                                              .OnStart(() => visitedCells[currentIndex].ownerDefaultCell.DeleteCell())
                                              .OnComplete(() => lineRenderer.positionCount--));
        }

        collectSequence.Play(); // Play the collection sequence in parallel
        returnSequenceTongue.OnComplete(() => _defaultCell.DeleteCell()).Play();

    }


    private void TriggerFail()
    {
        collectSequence = DOTween.Sequence();
        collectSequence.Pause();
        visitedCells.Clear();
        lineRenderer.positionCount = 0;


        // Mevcut SkinnedMeshRenderer bileþenini al
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (skinnedMeshRenderer != null)
        {
            // Mevcut renk ve kýrmýzý rengi tanýmla
            Material[] materials = skinnedMeshRenderer.materials;
            if (materials.Length > 0)
            {
                Color originalColor = materials[0].color;
                Color red = Color.red;

                // Rengi kýrmýzý yap ve sonra eski rengini geri getir
                foreach (Material material in materials)
                {
                    material.DOColor(red, duration / 2f) // Yarým sürede kýrmýzýya dönüþ
                        .OnComplete(() =>
                        {
                            // Eski rengin geri dönmesi
                            material.DOColor(originalColor, duration / 2f);
                        });
                }
            }
        }
        else
        {
            Debug.LogError("SkinnedMeshRenderer component not found.");
        }

    }

    // Handles the mouse click event to initiate the tongue movement
    private void OnMouseDown()
    {
        if (lineRenderer == null) return;

        SetDirection();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = new Color(1.0f, 0.5f, 0.5f);
        lineRenderer.endColor = new Color(1.0f, 0.3f, 0.3f);

        Vector3 startPoint = new Vector3(transform.position.x, 0.35f, transform.position.z);
        var nextCell = _defaultCell.GetNeigborCellAtDirection(entityDirection);

        if (nextCell == null)
        {
            Debug.Log("Next cell not found.");
            return;
        }

        Vector3 endPoint = nextCell.activeCell.entityOnCell.transform.position;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, startPoint);

        visitedCells.Add(entityCell);
        visitedCells.Add(nextCell.activeCell);
        Hit();

        MoveTongueToNextCellPosition(endPoint, nextCell, 1);
    }

    // Retrieves all positions of the LineRenderer, excluding the last one
    private Vector3[] GetLineRendererPositions()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount - 1];
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            positions[i] = lineRenderer.GetPosition(i);
        }
        return positions;
    }
}
