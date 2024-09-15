using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public enum GameState // not being used atm
	{
		Playing,
		Paused,
		GameOver
	}
	
	public static GameManager instance;
	[SerializeField] GameState currentGameState;
	[SerializeField] GameObject player;
	[SerializeField] Transform playerSpawnPoint;
	
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
	void OnEnable()
	{
		Hive.OnEnteredHive += PauseGame;
	}
	
	void OnDisable()
	{
		Hive.OnEnteredHive -= PauseGame;
	}
	
	void Start()
	{
		player.transform.position = playerSpawnPoint.position;
		currentGameState = GameState.Playing;
	}
	
	public void PauseGame()
	{
		currentGameState = GameState.Paused;
		Time.timeScale = 0;
	}
	
	public void RestartGame()
	{
		ResumeGame();
		GameTimer.instance.ResetTimer();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	public void ResumeGame()
	{
		currentGameState = GameState.Playing;
		Time.timeScale = 1;
	}
	
	public float CalculateScore()
	{
		float time = GameTimer.instance.GetGameTime();
		float collectionCount = Collection.GetCollectionCount();
		float score = collectionCount / time * collectionCount * 100;
		
		float highscore = PlayerPrefs.GetFloat("Highscore", 0);
		if (score > highscore)
		{
			PlayerPrefs.SetFloat("Highscore", score);
			PlayerPrefs.Save();
		}
		return score;
	}
		
}
