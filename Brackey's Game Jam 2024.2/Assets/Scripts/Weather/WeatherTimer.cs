using UnityEngine;

public class WeatherTimer : MonoBehaviour
{
	private float timer;
	private int currentStateIndex;
	public float[] timesForWeatherStates = new float[7];
	
	public delegate void TimerExpiredHandler();
	public event TimerExpiredHandler OnTimerExpired;
	
	void Start()
	{
		currentStateIndex = 0;
		timer = timesForWeatherStates[currentStateIndex];
	}

	void OnEnable() 
	{
		OnTimerExpired += CycleTimer;
	}
	
	void OnDisable()
	{
		OnTimerExpired -= CycleTimer;
	}
	
	// Update is called once per frame
	void Update()
	{
		timer -= Time.deltaTime;
		
		if (timer <= 0)
		{
			OnTimerExpired?.Invoke();
		}
	}
	
	public void CycleTimer()
	{
		SetTimer(currentStateIndex + 1);
	}
	
	// Set the timer for the next state
	public void SetTimer(int nextStateIndex)
	{
		currentStateIndex = nextStateIndex % timesForWeatherStates.Length;
		timer = timesForWeatherStates[currentStateIndex];
	}
	public float GetCurrentTime()
	{
		return timer;
	}
}
