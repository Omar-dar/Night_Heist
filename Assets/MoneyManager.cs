using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    public int Money { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // Optional: DontDestroyOnLoad(gameObject);
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        Debug.Log($"Money: {Money}");
    }
}
