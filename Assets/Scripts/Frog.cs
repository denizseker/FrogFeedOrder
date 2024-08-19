using UnityEngine;
using DG.Tweening;

public class Frog : Entity
{

    private LineRenderer lineRenderer;
    public Vector3 endPoint;
    private float duration = 1.2f;  // Çizginin uzama süresi

    private void Start()
    {
        // Mevcut LineRenderer bileþenini al
        lineRenderer = GetComponent<LineRenderer>();

        // Eðer LineRenderer mevcut deðilse bir hata mesajý yazdýr
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer bileþeni bu objede mevcut deðil.");
        }
        else
        {
            // LineRenderer özelliklerini ayarla
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.05f;  // Baþlangýç geniþliði büyük, bitiþ geniþliði küçük (daralan bir etkisi var)

            // LineRenderer rengini insan diline benzet
            Color tongueColorStart = new Color(1.0f, 0.5f, 0.5f); // Açýk kýrmýzý-pembe
            Color tongueColorEnd = new Color(1.0f, 0.3f, 0.3f);   // Koyu kýrmýzý

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

        // Basit yön kontrolü ve debug yazdýrma
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

        //// Baþlangýç ve bitiþ noktalarýný ayarla
        //Vector3 startPoint = new Vector3(transform.position.x, 0.35f, transform.position.z);
        //endPoint = new Vector3(0, 0.35f, 4);  // Y ekseninde yüksekliði 5 olan hedef nokta

        //// Çizginin baþlangýç noktasýný ve bitiþ noktasýný ayarla
        //lineRenderer.SetPosition(0, startPoint);
        //lineRenderer.SetPosition(1, startPoint);  // Çizgi baþlangýçta uzamamýþ olacak

        //// DOTween ile çizginin ikinci noktasýný endPoint'e doðru hareket ettir
        //DOTween.To(() => lineRenderer.GetPosition(1), x => lineRenderer.SetPosition(1, x), endPoint, duration)
        //       .SetEase(Ease.Linear)
        //       .OnComplete(OnLineRendererAnimationComplete); // Animasyon tamamlandýðýnda çaðrýlacak metod
    }

    private void OnLineRendererAnimationComplete()
    {
        Debug.Log("LineRenderer animasyonu tamamlandý.");
        // Buraya animasyon tamamlandýðýnda çalýþmasýný istediðiniz kodu yazabilirsiniz
    }

    private void OnMouseDown()
    {
        Interact();
    }
}
