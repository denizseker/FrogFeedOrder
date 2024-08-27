using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // Eventler
    public UnityEvent onHit;
    public UnityEvent onDestroy;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Ses çalma metodlarý
    public void PlayHitSound()
    {
        Debug.Log("Hit sound played!");
        // Ses çalma kodlarý buraya gelecek.
    }

    public void PlayDestroySound()
    {
        Debug.Log("Destroy sound played!");
        // Ses çalma kodlarý buraya gelecek.
    }

    private void OnEnable()
    {
        // Eventlere metotlarý ekliyoruz.
        onHit.AddListener(PlayHitSound);
        onDestroy.AddListener(PlayDestroySound);
    }

    private void OnDisable()
    {
        // Eventlerden metotlarý çýkarýyoruz.
        onHit.RemoveListener(PlayHitSound);
        onDestroy.RemoveListener(PlayDestroySound);
    }
}
