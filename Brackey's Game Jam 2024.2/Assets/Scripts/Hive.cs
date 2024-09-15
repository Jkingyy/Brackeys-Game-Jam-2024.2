using UnityEngine;

public class Hive : MonoBehaviour
{
	[SerializeField] GameObject upgradeMenu;
	public delegate void HiveActions();
	public static event HiveActions OnEnteredHive;
	public static event HiveActions OnExitedHive;
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			upgradeMenu.SetActive(true);
			OnEnteredHive?.Invoke();
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			OnExitedHive?.Invoke();
		}
	}
}
