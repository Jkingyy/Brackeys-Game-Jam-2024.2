using UnityEngine;

public class WeatherEffects : MonoBehaviour
{
	[SerializeField] ParticleSystem currentWeatherEffect;
	[SerializeField] ParticleSystem rain;
	[SerializeField] ParticleSystem wind;
	[SerializeField] ParticleSystem lightning;
	[SerializeField] ParticleSystem sunny;
	[SerializeField] ParticleSystem cloudy;
	
	void Start()
	{
		currentWeatherEffect = sunny;
	}
	
	public void ActivateWeatherEffect(WeatherState.State weatherState)
	{
		StopCurrentWeatherEffect();
		switch (weatherState)
		{
			case WeatherState.State.Sunny:
				ActivateSunny();
				break;
			case WeatherState.State.Cloudy:
				ActivateCloudy();
				break;
			case WeatherState.State.Windy:
				ActivateWindy();
				break;
			case WeatherState.State.Rainy:
				ActivateRain();
				break;
			case WeatherState.State.Stormy:
				ActivateLightning();
				break;
		}
		print("The weather particle effect is now: " + weatherState);
	}
	
	void ActivateRain()
	{
		rain.Play();
		currentWeatherEffect = rain;
	}
	
	void ActivateWindy()
	{
		wind.Play();
		currentWeatherEffect = wind;
	}
	
	void ActivateLightning()
	{
		lightning.Play();
		currentWeatherEffect = lightning;
	}
	
	void ActivateSunny()
	{
		sunny.Play();
		currentWeatherEffect = sunny;
	}
	
	void ActivateCloudy()
	{
		cloudy.Play();
		currentWeatherEffect = cloudy;
	}
	
	public void StopCurrentWeatherEffect()
	{
		if (currentWeatherEffect != null) currentWeatherEffect.Stop();
	}
	
}
