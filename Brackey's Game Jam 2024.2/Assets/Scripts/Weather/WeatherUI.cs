using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class WeatherUI : MonoBehaviour
{
	[SerializeField] TMP_Text weatherTimerText;
	[SerializeField] WeatherTimer weatherTimer;
	[SerializeField] WeatherEffects weatherEffects;
	[SerializeField] float timer;
	[SerializeField] Image windDirectionIcon;
	[SerializeField] Sprite windDirectionLeft;
	[SerializeField] Sprite windDirectionRight;
	[SerializeField] TMP_Text windSpeedText;
	
	void Update()
	{
		WindEffect windEffect = weatherEffects.GetWindEffect();
		
		weatherTimerText.text = weatherTimer.GetCurrentTime().ToString("F1");
		windSpeedText.text = (windEffect.GetWindSpeed() * 40).ToString("F0") + " mph";
		
		if (windEffect.GetWindDirection() < 0)
		{
			windDirectionIcon.sprite = windDirectionLeft;
		}
		else
		{
			windDirectionIcon.sprite = windDirectionRight;
		}
	}
	
}
