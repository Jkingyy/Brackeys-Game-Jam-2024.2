using UnityEngine;

public class WeatherEffects : MonoBehaviour
{
	[SerializeField] WeatherState.State currentWeatherEffect;
	[SerializeField] ParticleSystem lightning;
	[SerializeField] ParticleSystem sunny;
	[SerializeField] ParticleSystem clouds;
	
	[SerializeField] GameObject player;
	private PlayerMovement playerMovement;
	
	[SerializeField] WindEffect windEffect;
	[SerializeField] RainEffect rainEffect;
	
	void Awake()
	{
		if (player == null) {
			player = GameObject.FindGameObjectWithTag("Player");
		}
		playerMovement = player.GetComponent<PlayerMovement>();
		lightning.Stop();
	}
	
	void Start()
	{
		currentWeatherEffect = WeatherState.State.Sunny;
	}
	
	public void ActivateWeatherEffect(WeatherState.State weatherState)
	{
		currentWeatherEffect = weatherState;
		DeactivateCurrentWeatherEffect();
		switch (weatherState)
		{
			case WeatherState.State.Sunny:
				ActivateSunnyWeather();
				break;
			case WeatherState.State.Cloudy:
				ActivateCloudyWeather();
				break;
			case WeatherState.State.Windy:
				ActivateWindyWeather();
				break;
			case WeatherState.State.Rainy:
				ActivateRainyWeather();
				break;
			case WeatherState.State.Stormy:
				ActivateStormyWeather();
				break;
		}
		print("The weather effect is now: " + weatherState);
	}
	
	void ActivateSunnyWeather()
	{
		windEffect.DeactivateWind();
		sunny.Play();
	}
	
	void ActivateWindyWeather()
	{
		windEffect.ActivateWind(1);
	}
	
	void ActivateCloudyWeather()
	{
		clouds.Play();
	}
	
	void ActivateRainyWeather()
	{
		clouds.Play();
		windEffect.SetWindSpeed(1.15f);
		rainEffect.ActivateRain(75); 
		playerMovement.SetRaining(true);
	}
	
	void ActivateStormyWeather()
	{
		clouds.Play();
		windEffect.SetWindSpeed(1.5f);
		ActivateRainyWeather();
		lightning.Play();
	}
	
	public void DeactivateCurrentWeatherEffect()
	{
		switch (currentWeatherEffect)
		{
			case WeatherState.State.Sunny:
				DeactivateSunnyWeather();
				break;
			case WeatherState.State.Cloudy:
				DeactivateCloudyWeather();
				break;
			case WeatherState.State.Windy:
				DeactivateWindyWeather();
				break;
			case WeatherState.State.Rainy:
				DeactivateRainyWeather();
				break;
			case WeatherState.State.Stormy:
				DeactivateStormyWeather();
				break;
		}
	}
	
	void DeactivateSunnyWeather()
	{
		sunny.Stop();
	}
	
	void DeactivateWindyWeather()
	{
		windEffect.DeactivateWind();
	}
	
	void DeactivateCloudyWeather()
	{
		clouds.Stop();
		windEffect.DeactivateWind();
	}
	
	void DeactivateRainyWeather()
	{
		clouds.Stop();
		windEffect.DeactivateWind();
		rainEffect.DeactivateRain();
		playerMovement.SetRaining(false);
	}
	
	void DeactivateStormyWeather()
	{
		clouds.Stop();
		windEffect.DeactivateWind();
		rainEffect.DeactivateRain();
		lightning.Stop();
	}
}
