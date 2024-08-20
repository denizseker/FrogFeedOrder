using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Frog : Entity
{
    private LineRenderer lineRenderer;  // LineRenderer component
    private Cell_Default cellDefault;    // Reference to the default cell of the entity
    private float duration = 0.5f;         // Duration of the line animation
    private List<Vector3> pathPoints = new List<Vector3>(); // List to store the path points

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
        Cell nextCell = cellDefault.CheckNextCell(entityDirection);

        if (nextCell == null)
        {
            Debug.LogWarning("Next cell not found.");
            return;
        }

        Vector3 endPoint = nextCell.entityOnCell.transform.position;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, startPoint);

        // Initialize path points list with the start point
        pathPoints.Clear();
        pathPoints.Add(startPoint);
        pathPoints.Add(endPoint);

        // Start the line expansion animation
        AnimateLineRenderer(endPoint, nextCell,1);
    }

    private void AnimateLineRenderer(Vector3 endPoint, Cell currentCell,int newIndex)
    {


        DOTween.To(() => lineRenderer.GetPosition(1),
            x => lineRenderer.SetPosition(newIndex, x),
            endPoint,
            duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                // OnUpdate içinde animasyon her adýmda çalýþýrken yapýlacak iþlemler
            })
                .OnComplete(() =>
                {
                    Debug.Log(newIndex);
                    OnLineRendererAnimationComplete(currentCell);
                });
    }

    private void OnLineRendererAnimationComplete(Cell currentCell)
    {

        // Get the next cell from the current cell
        Cell nextCell = currentCell.ownerDefaultCell.CheckNextCell(entityDirection);

        if (nextCell != null)
        {
            // Add the new end point to the path
            Vector3 newEndPoint = nextCell.entityOnCell.transform.position;

            // Add the new end point to the pathPoints list
            pathPoints.Add(newEndPoint);

            // Add a new point to the line renderer
            lineRenderer.positionCount += 1;
            int newIndex = lineRenderer.positionCount - 1;
            lineRenderer.SetPosition(newIndex, lineRenderer.GetPosition(newIndex -1));  // Set the start point of the new segment

            // Start the animation for the new segment
            AnimateLineRenderer(newEndPoint, nextCell, newIndex);
        }
        else
        {
            // Once all cells are processed, start the return animation
            StartReturnAnimation();
        }
    }

    private void StartReturnAnimation()
    {
        //if (pathPoints.Count < 2)
        //{
        //    Debug.LogWarning("Not enough points to return.");
        //    return;
        //}

        //// Create a sequence for the return animation
        //Sequence returnSequence = DOTween.Sequence();

        //// Add animations for returning through all points in reverse order
        //for (int i = pathPoints.Count - 1; i > 0; i--)
        //{
        //    int currentIndex = i;
        //    int nextIndex = i - 1;

        //    // Add animation to the sequence
        //    returnSequence.Insert(0, DOTween.To(() => lineRenderer.GetPosition(currentIndex),
        //        x => lineRenderer.SetPosition(currentIndex, x),
        //        pathPoints[nextIndex], duration)
        //        .SetEase(Ease.Linear));
        //}

        //// Animate the last segment to match the first point
        //returnSequence.Insert(0, DOTween.To(() => lineRenderer.GetPosition(1),
        //    x => lineRenderer.SetPosition(1, x),
        //    pathPoints[0], duration)
        //    .SetEase(Ease.Linear));

        //// Play the sequence
        //returnSequence.Play();

        //// Log completion
        //returnSequence.OnComplete(() => Debug.Log("Return animation completed."));
    }

    private void OnMouseDown()
    {
        Interact();  // Trigger interaction on mouse click
    }
}
