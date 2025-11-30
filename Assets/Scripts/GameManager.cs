using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // moedas acumuladas (meta currency)
    public int totalCoins = 0;

    // moedas só dessa run (se quiser usar depois)
    public int coinsThisRun = 0;

    // níveis dos upgrades (persistentes)
    public int hydraLevel = 0;
    public int armoredBodyLevel = 0;
    public int plasmaEngineLevel = 0;

    private void Awake()
    {
        // singleton básico
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -----------------------------------
    // MOEDAS
    // -----------------------------------
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        coinsThisRun += amount;
    }

    public void ResetRunCoins()
    {
        coinsThisRun = 0;
    }

    // -----------------------------------
    // CUSTOS DOS UPGRADES
    // -----------------------------------
    public int GetUpgradeCost(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.HydraShot:
                // custo depende do nível atual
                return 20 * (hydraLevel + 1);

            case UpgradeType.ArmoredBody:
                return 30 * (armoredBodyLevel + 1);

            case UpgradeType.PlasmaEngine:
                return 15 * (plasmaEngineLevel + 1);

            default:
                return 999999;
        }
    }

    public bool CanBuyUpgrade(UpgradeType type)
    {
        int cost = GetUpgradeCost(type);
        return totalCoins >= cost;
    }

    // compra efetiva do upgrade (desconta moedas e aumenta nível)
    public bool BuyUpgrade(UpgradeType type)
    {
        int cost = GetUpgradeCost(type);

        if (totalCoins < cost)
            return false;

        totalCoins -= cost;

        switch (type)
        {
            case UpgradeType.HydraShot:
                hydraLevel++;
                break;

            case UpgradeType.ArmoredBody:
                armoredBodyLevel++;
                break;

            case UpgradeType.PlasmaEngine:
                plasmaEngineLevel++;
                break;
        }

        return true;
    }

    // -----------------------------------
    // APLICAÇÃO DOS UPGRADES NO PLAYER
    // 
    // -----------------------------------
    public void ApplyUpgradesToPlayer(mov player)
    {
        if (player == null) return;


        // PlasmaEngine: aumenta velocidade base
        if (plasmaEngineLevel > 0)
        {
            float multiplier = 1f + 0.15f * plasmaEngineLevel;
            player.velocidade *= multiplier;
        }

        // ArmoredBody e HydraShot vamos implementar com lógica específica depois
        // (escudo nos segmentos e tiros múltiplos).
    }
}
