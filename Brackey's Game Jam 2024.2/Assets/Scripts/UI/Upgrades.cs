using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
	public static Upgrades instance;
	[SerializeField] GameObject upgradeMenu;
	[SerializeField] Button UpgradeButton;
	[SerializeField] UpgradeType upgradeChoice;
	[SerializeField] TMP_Text upgradeCostText;
	[SerializeField] TMP_Text totalCollectedText;
	[SerializeField] TMP_Text speedLevelText;
	[SerializeField] TMP_Text jumpLevelText;
	[SerializeField] TMP_Text healthLevelText;
	[SerializeField] TMP_Text windResistanceLevelText;
	[SerializeField] TMP_Text highScoreText;
	public delegate void Upgrade(int upgradeCost = 0);
	public static event Upgrade OnUpgrade;
	
	public enum UpgradeType
	{
		None,
		Speed,
		Jump,
		Health,
		WindResistance
	}

	private int speedLevel = 0;
	private int jumpLevel = 0;
	private int healthLevel = 0;
	private int windResistanceLevel = 0;
	
	void Awake()
	{	
		// PlayerPrefs.DeleteAll();
		// PlayerPrefs.Save();
		// return;
		speedLevel = PlayerPrefs.GetInt("playerSpeed", 0);
		jumpLevel = PlayerPrefs.GetInt("playerJumpSpeed", 0);
		healthLevel = PlayerPrefs.GetInt("playerMaxHealth", 0);
		windResistanceLevel = PlayerPrefs.GetInt("playerWindResistance", 0);
		
		print("Speed: " + speedLevel + " Jump: " + jumpLevel + " Health: " + healthLevel + " Wind Resistance: " + windResistanceLevel);
		
		UpgradeButton.onClick.AddListener(UpgradeChosenAbility);
		upgradeMenu.SetActive(false);
		
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
	public void UpgradeChosenAbility()
	{
		int upgradeCost = GetUpgradeCost();
		if (upgradeCost > PlayerPrefs.GetInt("TotalCollected", 0)) return;
		switch (upgradeChoice)
		{
			case UpgradeType.None:
				return;
			case UpgradeType.Speed:
				speedLevel++;
				PlayerPrefs.SetInt("playerSpeed", speedLevel);
				break;
			case UpgradeType.Jump:
				jumpLevel++;
				PlayerPrefs.SetInt("playerJumpSpeed", jumpLevel);
				break;
			case UpgradeType.Health:
				healthLevel++;
				PlayerPrefs.SetInt("playerMaxHealth", healthLevel);
				break;
			case UpgradeType.WindResistance:
				windResistanceLevel++;
				PlayerPrefs.SetInt("playerWindResistance", windResistanceLevel);
				break;
		}
		OnUpgrade?.Invoke(upgradeCost);
		PlayerPrefs.Save();
		UpdateUI();
		CloseUpgradeMenu();
	}
	public void SetUpgradeChoice(int upgradeType)
	{
		upgradeChoice = (UpgradeType)upgradeType;
		UpdateUI();
	}
	public UpgradeType GetUpgradeChoice()
	{
		return upgradeChoice;
	}
	public int GetUpgradeCost()
	{
		switch (upgradeChoice)
		{
			case UpgradeType.None:
				return 0;
			case UpgradeType.Speed:
				return speedLevel + 3;
			case UpgradeType.Jump:
				return jumpLevel + 3;
			case UpgradeType.Health:
				return healthLevel + 3;
			case UpgradeType.WindResistance:
				return windResistanceLevel + 3;
		}
		return 0;
	}
	
	public void UpdateUI()
	{
		highScoreText.text = "Score: " + GameManager.instance.CalculateScore().ToString("F0") + "\nHigh Score: " + PlayerPrefs.GetFloat("Highscore", 0).ToString("F0");
		totalCollectedText.text = "Savings: $" + PlayerPrefs.GetInt("TotalCollected", 0).ToString();
		speedLevelText.text = "LVL " + speedLevel.ToString();
		jumpLevelText.text = "LVL " + jumpLevel.ToString();
		healthLevelText.text = "LVL " + healthLevel.ToString();
		windResistanceLevelText.text = "LVL " + windResistanceLevel.ToString();
		upgradeCostText.text = "Upgrade Selected: " + GetUpgradeChoice().ToString() + "\nUpgrade Costs: $" + GetUpgradeCost().ToString();
	}
	
	public void CloseUpgradeMenu()
	{
		upgradeMenu.SetActive(false);
		GameManager.instance.RestartGame();
	}
	
}
