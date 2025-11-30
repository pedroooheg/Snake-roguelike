using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    [Header("Cena Principal")]
    public string mainSceneName = "SampleScene";

    [Header("UI")]
    public TMP_Text coinsText;
    public TMP_Text hydraText;
    public TMP_Text armorText;
    public TMP_Text plasmaText;

    private void Start()
    {
        AtualizarUI();
    }

    public void Restart()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResetRunCoins();

        SceneManager.LoadScene(mainSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    // ----------------------------
    // BOTÕES DE UPGRADE
    // ----------------------------

    public void BuyHydra()
    {
        if (GameManager.Instance.BuyUpgrade(UpgradeType.HydraShot))
            AtualizarUI();
    }

    public void BuyArmor()
    {
        if (GameManager.Instance.BuyUpgrade(UpgradeType.ArmoredBody))
            AtualizarUI();
    }

    public void BuyPlasma()
    {
        if (GameManager.Instance.BuyUpgrade(UpgradeType.PlasmaEngine))
            AtualizarUI();
    }

    // ----------------------------
    // ATUALIZAÇÃO DE UI
    // ----------------------------

    private void AtualizarUI()
    {
        if (GameManager.Instance == null) return;

        var gm = GameManager.Instance;

        coinsText.text = $"Moedas: {gm.totalCoins}";
        hydraText.text = $"Hydra Lv {gm.hydraLevel} (Custo {gm.GetUpgradeCost(UpgradeType.HydraShot)})";
        armorText.text = $"Blindado Lv {gm.armoredBodyLevel} (Custo {gm.GetUpgradeCost(UpgradeType.ArmoredBody)})";
        plasmaText.text = $"Plasma Lv {gm.plasmaEngineLevel} (Custo {gm.GetUpgradeCost(UpgradeType.PlasmaEngine)})";
    }
}
