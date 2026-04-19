using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startingGold = 50;
    private int currentGold;

    public int CurrentGold => currentGold;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        currentGold = startingGold;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"Altın: {currentGold}");
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount) return false;
        currentGold -= amount;
        return true;
    }
}