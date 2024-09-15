using UnityEngine;
using TMPro;

public class Collection : MonoBehaviour
{
	private static int collectionCount;
	private int totalCollected;
	[SerializeField] TMP_Text _collectionText;
	
	void OnEnable(){
		Hive.OnEnteredHive += UpdateTotalCollected;
		Upgrades.OnUpgrade += SubtractFromTotalCollected;
	}
	
	void OnDisable(){
		Hive.OnEnteredHive -= UpdateTotalCollected;
		Upgrades.OnUpgrade -= SubtractFromTotalCollected;
	}
	
	void Awake() 
	{
		totalCollected = PlayerPrefs.GetInt("TotalCollected", 0);
		collectionCount = 0;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Collect")){
			collectionCount++;
			UpdateUI();
			Destroy(other.gameObject);
		}
	}

	private void UpdateUI(){
		_collectionText.text = collectionCount.ToString();
	}
	
	void UpdateTotalCollected(){
		totalCollected += collectionCount;
		PlayerPrefs.SetInt("TotalCollected", totalCollected);
		PlayerPrefs.Save();
		Upgrades.instance.UpdateUI();
	}
	
	public void SubtractFromTotalCollected(int amount){
		totalCollected -= amount;
		PlayerPrefs.SetInt("TotalCollected", totalCollected);
		PlayerPrefs.Save();
	}
	public static int GetCollectionCount(){
		return collectionCount;
	}
}
