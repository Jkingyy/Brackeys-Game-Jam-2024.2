using UnityEngine;

[RequireComponent(typeof(WeatherTimer))]
[RequireComponent(typeof(WeatherState))]
[RequireComponent(typeof(WeatherEffects))]
public class WeatherSystem : MonoBehaviour
{
	public WeatherTimer weatherTimer;
	public WeatherState weatherState;
	public WeatherEffects weatherEffects;

	void OnEnable()
	{
		weatherTimer.OnTimerExpired += HandleTimerExpired;
	}

	void OnDisable()
	{
		weatherTimer.OnTimerExpired -= HandleTimerExpired;
	}

	private void HandleTimerExpired()
	{
		// Change to the next weather state
		weatherState.CycleWeatherState();
		
		// Reset the timer for the next weather state duration
		int nextStateIndex = weatherState.GetNextWeatherStateIndex();
		weatherTimer.SetTimer(nextStateIndex);
		
		// Activate the particle effect for the new weather state
		weatherEffects.ActivateWeatherEffect(weatherState.GetCurrentWeatherState());
	}
}
