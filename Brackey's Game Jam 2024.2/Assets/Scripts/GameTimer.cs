using UnityEngine;

public class GameTimer : MonoBehaviour
{
	public static GameTimer instance;
	private float gameTimer = 0;
	
	void Awake() 
	{
		gameTimer = 0;
		
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	void Update()
	{
		gameTimer += Time.deltaTime;
	}
	
	public void ResetTimer()
	{
		gameTimer = 0;
	}
	
	public float GetGameTime()
	{
		return gameTimer;
	}
}
