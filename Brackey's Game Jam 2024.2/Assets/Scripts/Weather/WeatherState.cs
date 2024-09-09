using UnityEngine;

public class WeatherState : MonoBehaviour
{
	private int currentStateIndex;
	public static State currentWeatherState;
	public enum State
	{
		Sunny,
		Cloudy,
		Windy,
		Rainy,
		Stormy,
	}
	
	public State[] WeatherStateOrder = new State[]
	{
		State.Sunny,
		State.Cloudy,
		State.Windy,
		State.Rainy,
		State.Stormy,
		State.Rainy,
		State.Cloudy,
	};
	
	void Start()
	{
		currentStateIndex = 0;
		currentWeatherState = WeatherStateOrder[currentStateIndex];
	}
	
	public State GetCurrentWeatherState() 
	{
		return WeatherStateOrder[currentStateIndex];
	}
	public void CycleWeatherState()
	{
		currentStateIndex = (currentStateIndex + 1) % WeatherStateOrder.Length;
		currentWeatherState = WeatherStateOrder[currentStateIndex];
		print("The weather is now: " + WeatherStateOrder[currentStateIndex]);
	}
	public int GetNextWeatherStateIndex()
	{
		return (currentStateIndex + 1) % WeatherStateOrder.Length;
	}
}
