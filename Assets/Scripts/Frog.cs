using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

public class Frog : Entity
{
    private LineRenderer lineRenderer;  // LineRenderer component
    private Default_Cell _defaultCell;    // Reference to the default cell of the entity
    private float duration = 0.3f;         // Duration of the line animation
    private List<Entity_Cell> visitedCells = new List<Entity_Cell>(); // List to store the path points
    private Sequence collectSequence;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();  // Get the LineRenderer component
        _defaultCell = entityCell.ownerDefaultCell;    // Get the default cell of the entity
        collectSequence = DOTween.Sequence();
        collectSequence.Pause();
    }
    private void Update()
    {

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


    private void TongueReachedToThisCell(Default_Cell cell)
    {
        
        var entity = cell.activeCell.entityOnCell;
        cell.activeCell.entityOnCell.HitByTongue(this);

        switch (entity)
        {
            case Frog frog:
                break;
            case Arrow arrow:
                if (isColourSame(cell))
                {
                    cell.activeCell.entityOnCell.GetComponent<Arrow>().SetDirection();
                    entityDirection = cell.activeCell.entityOnCell.entityDirection;
                }
                else
                {
                    Debug.Log("Wrong colour arrow");
                    return;
                }
                
                break;
            case Grape grape:

                if (isColourSame(cell))
                {
                    Vector3[] tempPoints = new Vector3[lineRenderer.positionCount - 1]; // Array'i baþlat
                    for (int i = 0; i < lineRenderer.positionCount - 1; i++) // Tüm pozisyonlarý kopyala
                    {
                        tempPoints[i] = lineRenderer.GetPosition(i);
                    }
                    cell.activeCell.entityOnCell.GetComponent<Grape>().SetForCollect(tempPoints);
                }
                else
                {
                    Debug.Log("Wrong colour grape");
                    return;
                }
                break;
            default:
                break;
        }


        if (isNextCellValid(cell))
        {
            SetTongueForNextCell(cell);
        }
        else
        {
            DOVirtual.DelayedCall(0.6f, StartReturnAnimation);
            Debug.Log("Not valid or dont have active cell on it");
            return;
        }

        

    }

    private bool isNextCellValid(Default_Cell currentCell)
    {
        Default_Cell nextCell = currentCell.GetNeigborCellAtDirection(entityDirection);

        //There is defaultcell but dont have entitycell on it.
        if (nextCell == null) return false;
        if (nextCell != null && nextCell.activeCell == null) return false;
        if (nextCell != null && nextCell.activeCell != null) return true;
        else return false;
    }

    private void SetTongueForNextCell(Default_Cell currentCell)
    {
        Default_Cell nextCell = currentCell.GetNeigborCellAtDirection(entityDirection);

        // Add the new end point to the path
        Vector3 newEndPoint = nextCell.activeCell.entityOnCell.transform.position;

        visitedCells.Add(nextCell.activeCell);
        // Add a new point to the line renderer
        lineRenderer.positionCount += 1;
        int newIndex = lineRenderer.positionCount - 1;
        lineRenderer.SetPosition(newIndex, lineRenderer.GetPosition(newIndex - 1));  // Set the start point of the new segment

        // Start the animation for the new segment
        MoveTongueToNextCellPosition(newEndPoint, nextCell, newIndex);
    }

    public override void HitByTongue(Frog frog)
    {
        Debug.Log(gameObject + " Hit by tongue" + frog);
    }

    private void MoveTongueToNextCellPosition(Vector3 endPoint, Default_Cell currentCell,int newIndex)
    {
        DOTween.To(() => lineRenderer.GetPosition(newIndex - 1),
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
                    //TODO: Ýleri giderken her cellin orta noktasýna gelince cell üstündeki entititylerin ve cellin davranýþýný triggerla.
                    TongueReachedToThisCell(currentCell);
                });
    }


    private bool isColourSame(Default_Cell currentCell)
    {
        if (currentCell.activeCell.cellColour == entityCell.cellColour)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void Hit()
    {
        // Animasyon sýrasý oluþtur
        Sequence pickUpSequence = DOTween.Sequence();
        // Scale'i 0.7'den 1.7'ye hýzlýca artýr
        pickUpSequence.Append(transform.DOScale(1.4f, 0.1f).SetEase(Ease.InOutQuad));
        // Scale'i 1'den 0.7'ye yavaþça azalt
        pickUpSequence.Append(transform.DOScale(1f, 0.4f).SetEase(Ease.OutQuad));
        // Animasyonu baþlat
        pickUpSequence.Play();
    }

    private void StartReturnAnimation()
    {
        if (lineRenderer.positionCount < 2)
        {

            Debug.LogWarning("Not enough points to return.");
            return;
        }


        // Create a sequence for the return animation
        Sequence returnSequenceTongue = DOTween.Sequence();
        // Add animations for returning through all points in reverse order
        for (int i = lineRenderer.positionCount - 1; i > 0; i--)
        {
            int currentIndex = i;
            int nextIndex = i - 1;

            //Debug.Log("Next target: " + pathPoints[nextIndex]);

            returnSequenceTongue.Append(DOTween.To(() => lineRenderer.GetPosition(currentIndex),
                     x => lineRenderer.SetPosition(currentIndex, x),
                     lineRenderer.GetPosition(nextIndex), duration)
                    .SetEase(Ease.Linear)
                    .OnStart(() => 
                    {
                        if(visitedCells[currentIndex].entityOnCell.GetType() == typeof(Grape)) visitedCells[currentIndex].entityOnCell.GetComponent<Grape>().StartCollect();
                        visitedCells[currentIndex].entityOnCell.gameObject.transform.parent = null;
                        visitedCells[currentIndex].ownerDefaultCell.DeleteCell();
                    })
                    .OnComplete(() =>
                    {
                        //visitedCells[currentIndex].entityOnCell.GetComponent<Grape>().StartCollect();
                        //TODO: Geri dönerken her cell üzerinden dönüþ tamamlandýðýnda cell üstündeki entititylerin ve cellin davranýþýný triggerla.
                        
                        lineRenderer.positionCount -= 1;
                    }));
        }

        //collectSequence.Play();

        returnSequenceTongue.Play();
        //Debug.Log("Sequence started");
        
        // Log completion
        returnSequenceTongue
            .OnComplete(() => 
            {

                _defaultCell.DeleteCell();
            
            });
    }

    private void OnMouseDown()
    {
        if (lineRenderer == null) return;  // Exit if LineRenderer component is not found

        SetDirection();  // Set the direction of the tongue

        // Configure the LineRenderer properties
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;

        // Set the LineRenderer colors
        lineRenderer.startColor = new Color(1.0f, 0.5f, 0.5f);  // Start color
        lineRenderer.endColor = new Color(1.0f, 0.3f, 0.3f);    // End color

        // Set start and end points
        Vector3 startPoint = new Vector3(transform.position.x, 0.35f, transform.position.z);

        Default_Cell nextCell = _defaultCell.GetNeigborCellAtDirection(entityDirection);

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

        // Start the line expansion animation
        MoveTongueToNextCellPosition(endPoint, nextCell, 1);
    }
}
