using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public enum Colour
    {
        Blue,
        Red,
        Green,
        Yellow,
        Purple,
        Default,
    }

    public static GameManager Instance { get; private set; }

    public int moveLimit = 20;
    private int frogCount = 0;
    public TextMeshProUGUI moveText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
#if UNITY_EDITOR
            Cursor.SetCursor(PlayerSettings.defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
#endif
    }

    public void IncreaseFrogCount()
    {
        frogCount++;
    }
    public void DecreaseFrogCount()
    {
        if (frogCount <= 0) return;

        frogCount--;
        if (frogCount == 0)
        {
            LevelManager.Instance.LoadNextLevel();
        }
    }


    public void DecreaseMoveCount()
    {
        if (moveLimit <= 0) return;

        moveLimit--;
        moveText.text = moveLimit.ToString() + " MOVES";
        if (moveLimit == 0)
        {
            Debug.Log("Lost");
        }
    }

}
