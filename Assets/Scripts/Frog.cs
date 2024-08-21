using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class Frog : Entity
{
    private LineRenderer lineRenderer;  // LineRenderer component
    private Cell_Default cellDefault;    // Reference to the default cell of the entity
    private float duration = 0.3f;         // Duration of the line animation
    private List<Vector3> pathPoints = new List<Vector3>(); // List to store the path points
    private List<Cell> cellQue = new List<Cell>(); // List to store the path points
    Sequence returnSequenceGrapes;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();  // Get the LineRenderer component
        cellDefault = entityCell.ownerDefaultCell;    // Get the default cell of the entity
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Interact();  // Trigger interaction when the 'K' key is pressed
        }
    }

    private void SetDirection()
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

    public override void Interact()
    {
        if (lineRenderer == null) return;  // Exit if LineRenderer component is not found

        SetDirection();  // Set the direction of the entity

        // Configure the LineRenderer properties
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;

        // Set the LineRenderer colors
        lineRenderer.startColor = new Color(1.0f, 0.5f, 0.5f);  // Start color
        lineRenderer.endColor = new Color(1.0f, 0.3f, 0.3f);    // End color

        // Set start and end points
        Vector3 startPoint = new Vector3(transform.position.x, 0.35f, transform.position.z);
        Cell nextCell = cellDefault.GetNextCell(entityDirection);

        if (nextCell == null)
        {
            Debug.Log("Next cell not found.");
            return;
        }

        Vector3 endPoint = nextCell.entityOnCell.transform.position;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, startPoint);

        // Initialize path points list with the start point
        pathPoints.Clear();
        pathPoints.Add(startPoint);
        pathPoints.Add(endPoint);
        cellQue.Add(cellDefault);
        cellQue.Add(nextCell);
        Hit();

        returnSequenceGrapes = DOTween.Sequence();
        returnSequenceGrapes.Pause();

        // Start the line expansion animation
        AnimateLineRenderer(endPoint, nextCell,1);

    }

    private void AnimateLineRenderer(Vector3 endPoint, Cell currentCell,int newIndex)
    {
        DOTween.To(() => lineRenderer.GetPosition(newIndex - 1),
            x => lineRenderer.SetPosition(newIndex, x),
            endPoint,
            duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                // OnUpdate i�inde animasyon her ad�mda �al���rken yap�lacak i�lemler
            })
                .OnComplete(() =>
                {
                    currentCell.entityOnCell.GetComponent<Grape>().Hit();             
                    //TODO: �leri giderken her cellin orta noktas�na gelince cell �st�ndeki entititylerin ve cellin davran���n� triggerla.
                    OnLineRendererAnimationComplete(currentCell);
                });
    }

    private void OnLineRendererAnimationComplete(Cell currentCell)
    {

        // Get the next cell from the current cell
        Cell nextCell = currentCell.ownerDefaultCell.GetNextCell(entityDirection);

        if (nextCell != null)
        {
            // Add the new end point to the path
            Vector3 newEndPoint = nextCell.entityOnCell.transform.position;

            // Add the new end point to the pathPoints list
            pathPoints.Add(newEndPoint);
            cellQue.Add(nextCell);
            // Add a new point to the line renderer
            lineRenderer.positionCount += 1;
            int newIndex = lineRenderer.positionCount - 1;
            lineRenderer.SetPosition(newIndex, lineRenderer.GetPosition(newIndex -1));  // Set the start point of the new segment

            // Start the animation for the new segment
            AnimateLineRenderer(newEndPoint, nextCell, newIndex);
        }
        else
        {
            DOVirtual.DelayedCall(0.6f, StartReturnAnimation);
        }
    }


    public void Hit()
    {
        // Animasyon s�ras� olu�tur
        Sequence pickUpSequence = DOTween.Sequence();
        // Scale'i 0.7'den 1.7'ye h�zl�ca art�r
        pickUpSequence.Append(transform.DOScale(1.4f, 0.1f).SetEase(Ease.InOutQuad));
        // Scale'i 1'den 0.7'ye yava��a azalt
        pickUpSequence.Append(transform.DOScale(1f, 0.4f).SetEase(Ease.OutQuad));
        // Animasyonu ba�lat
        pickUpSequence.Play();
    }

    private void StartReturnAnimation()
    {
        if (pathPoints.Count < 2)
        {
            Debug.LogWarning("Not enough points to return.");
            return;
        }

        // Create a sequence for the return animation
        Sequence returnSequenceTongue = DOTween.Sequence();
        // Add animations for returning through all points in reverse order
        for (int i = pathPoints.Count - 1; i > 0; i--)
        {
            int currentIndex = i;
            int nextIndex = i - 1;

            //Debug.Log("Next target: " + pathPoints[nextIndex]);

            returnSequenceTongue.Append(DOTween.To(() => lineRenderer.GetPosition(currentIndex),
                     x => lineRenderer.SetPosition(currentIndex, x),
                     pathPoints[nextIndex], duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        //TODO: Geri d�nerken her cell �zerinden d�n�� tamamland���nda cell �st�ndeki entititylerin ve cellin davran���n� triggerla.
                        lineRenderer.positionCount -= 1;
                    }));
        }


        for (int i = cellQue.Count-1; i >= 1; i--)
        {
            int timesToRun = cellQue.Count - i;
            if (i == cellQue.Count-1)
            {
                for (int j = 0; j < timesToRun; j++)
                {
                    returnSequenceGrapes.Append(cellQue[i].entityOnCell.transform.DOMove(cellQue[i - 1].transform.position, 0.4f));
                }
            }
        }


        // Play the sequence
        //returnSequenceGrapes.Play();
        returnSequenceTongue.Play();

        // Log completion
        returnSequenceTongue.OnComplete(() => Debug.Log("Return animation completed."));
    }

    private void OnMouseDown()
    {
        Interact();  // Trigger interaction on mouse click
    }
}
