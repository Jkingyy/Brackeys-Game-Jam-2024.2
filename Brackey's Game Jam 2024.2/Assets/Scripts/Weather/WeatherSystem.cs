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
		weatherState.OnWeatherChanged += HandleWeatherChanged;
	}

	void OnDisable()
	{
		weatherTimer.OnTimerExpired -= HandleTimerExpired;
	}

	private void HandleTimerExpired()
	{
		weatherState.CycleWeatherState();
	}
	
	private void HandleWeatherChanged(WeatherState.State state)
	{
		weatherEffects.ActivateWeatherEffect(state);
	}
}
