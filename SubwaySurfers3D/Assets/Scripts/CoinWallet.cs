using System;
using System.Collections;
using UnityEngine;

public class CoinWallet : MonoBehaviour
{
    public static CoinWallet Instance { get; private set; }

    public int Coins { get; private set; } = 0;
    public int CoinMultiplier { get; private set; } = 1; // 1 o 2

    public event Action<int> OnCoinsChanged;
    public event Action<int> OnMultiplierChanged;

    private Coroutine _doubleCoinsRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoins(int baseAmount)
    {
        int finalAmount = baseAmount * CoinMultiplier;
        Coins += finalAmount;
        OnCoinsChanged?.Invoke(Coins);
    }

    public void SetMultiplier(int multiplier)
    {
        CoinMultiplier = Mathf.Max(1, multiplier);
        OnMultiplierChanged?.Invoke(CoinMultiplier);
    }

    public void ActivateDoubleCoins(float duration)
    {
        if (_doubleCoinsRoutine != null) StopCoroutine(_doubleCoinsRoutine);
        _doubleCoinsRoutine = StartCoroutine(DoubleCoinsRoutine(duration));
    }

    private IEnumerator DoubleCoinsRoutine(float duration)
    {
        SetMultiplier(2);
        yield return new WaitForSeconds(duration);
        SetMultiplier(1);
        _doubleCoinsRoutine = null;
    }

    public void ResetPowerUps()
    {
        // Cortar cualquier timer activo (Double Coins, etc.)
        if (_doubleCoinsRoutine != null)
        {
            StopCoroutine(_doubleCoinsRoutine);
            _doubleCoinsRoutine = null;
        }

        // Volver a estado normal
        SetMultiplier(1);
    }
    public void ResetRun()
    {
        ResetPowerUps();
        Coins = 0;
        OnCoinsChanged?.Invoke(Coins);
    }

}