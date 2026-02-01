using TMPro;
using UnityEngine;

public class UIHUD : MonoBehaviour
{
    public TMP_Text coinsText;
    public GameObject doubleCoinsIndicator; // icono o texto "x2"
    public GameObject shieldIndicator;      // icono de escudo (opcional)
    public PlayerController player;         // arrastra el Player aquí
    public TMP_Text distanceText;

    private void Start()
    {
        if (CoinWallet.Instance != null)
        {
            CoinWallet.Instance.OnCoinsChanged += _ => Refresh();
            CoinWallet.Instance.OnMultiplierChanged += _ => Refresh();
        }
        Refresh();
    }

    private void Update()
    {
        // Indicador de escudo en tiempo real (opcional)
        if (shieldIndicator && player)
            shieldIndicator.SetActive(IsShieldActive(player));
        if (!player) return;
        if (distanceText) distanceText.text = $"{Mathf.FloorToInt(player.DistanceMeters)} m";
    }

    private void Refresh()
    {
        if (coinsText)
            coinsText.text = $"Coins: {CoinWallet.Instance.Coins}";

        if (doubleCoinsIndicator)
            doubleCoinsIndicator.SetActive(CoinWallet.Instance.CoinMultiplier == 2);
    }

    // Como _shieldActive es private, puedes:
    // 1) hacer una propiedad pública en PlayerController (recomendado) o
    // 2) quitar este indicador si no lo necesitas.
    private bool IsShieldActive(PlayerController p)
    {
        return p.ShieldActive;
    }

}