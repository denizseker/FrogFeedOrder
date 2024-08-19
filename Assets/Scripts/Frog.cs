using UnityEngine;
using DG.Tweening;

public class Frog : Entity
{

    private LineRenderer lineRenderer;
    public Vector3 endPoint;
    private float duration = 1.2f;  // �izginin uzama s�resi

    private void Start()
    {
        // Mevcut LineRenderer bile�enini al
        lineRenderer = GetComponent<LineRenderer>();

        // E�er LineRenderer mevcut de�ilse bir hata mesaj� yazd�r
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer bile�eni bu objede mevcut de�il.");
        }
        else
        {
            // LineRenderer �zelliklerini ayarla
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.05f;  // Ba�lang�� geni�li�i b�y�k, biti� geni�li�i k���k (daralan bir etkisi var)

            // LineRenderer rengini insan diline benzet
            Color tongueColorStart = new Color(1.0f, 0.5f, 0.5f); // A��k k�rm�z�-pembe
            Color tongueColorEnd = new Color(1.0f, 0.3f, 0.3f);   // Koyu k�rm�z�

            lineRenderer.startColor = tongueColorStart;
            lineRenderer.endColor = tongueColorEnd;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            
        }
    }

    void SetDirection()
    {
        Vector3 forward = gameObject.transform.forward;

        // Basit y�n kontrol� ve debug yazd�rma
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
        if (lineRenderer == null) return;

        SetDirection();

        CellManager.Instance.CheckNextCell(entityCell.GetComponent<Cell>().ownerDefaultCell.GetComponent<Cell_Default>(),entityDirection);

        //Debug.Log(transform.rotation.y);

        //// Ba�lang�� ve biti� noktalar�n� ayarla
        //Vector3 startPoint = new Vector3(transform.position.x, 0.35f, transform.position.z);
        //endPoint = new Vector3(0, 0.35f, 4);  // Y ekseninde y�ksekli�i 5 olan hedef nokta

        //// �izginin ba�lang�� noktas�n� ve biti� noktas�n� ayarla
        //lineRenderer.SetPosition(0, startPoint);
        //lineRenderer.SetPosition(1, startPoint);  // �izgi ba�lang��ta uzamam�� olacak

        //// DOTween ile �izginin ikinci noktas�n� endPoint'e do�ru hareket ettir
        //DOTween.To(() => lineRenderer.GetPosition(1), x => lineRenderer.SetPosition(1, x), endPoint, duration)
        //       .SetEase(Ease.Linear)
        //       .OnComplete(OnLineRendererAnimationComplete); // Animasyon tamamland���nda �a�r�lacak metod
    }

    private void OnLineRendererAnimationComplete()
    {
        Debug.Log("LineRenderer animasyonu tamamland�.");
        // Buraya animasyon tamamland���nda �al��mas�n� istedi�iniz kodu yazabilirsiniz
    }

    private void OnMouseDown()
    {
        Interact();
    }
}
