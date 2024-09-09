using UnityEngine;
using TMPro;
public class WeatherUI : MonoBehaviour
{
	public TMP_Text weatherStateText;
	public WeatherTimer weatherTimer;
	public float timer;
	
	void Update()
	{
		weatherStateText.text = weatherTimer.GetCurrentTime().ToString("F1");
	}
	
}
