using UnityEngine;

/// <summary>
/// This class is responsible for controlling the rain effect.
/// </summary>
public class RainEffect : MonoBehaviour
{
	[SerializeField] ParticleSystem ps; 
	
	/// <summary>
	/// This is the value that the particle system will use for particles per second.
	/// </summary>
	private float rainIntensity;
	
	/// <summary>
	/// Set the rain intensity. This will set the rate of the particle system.
	/// </summary>
	/// <param name="intensity"></param>
	public void SetRainIntensity(float intensity)
	{
		rainIntensity = intensity;
		ParticleSystem.EmissionModule em = ps.emission;
		em.rateOverTime = rainIntensity;
		print("Rain intensity set to: " + rainIntensity);
	}
	
	/// <summary>
	/// Activate the rain effect with a given intensity. This will set the emission rate of the particle system and play it.
	/// </summary>
	/// <param name="intensity"></param>
	public void ActivateRain(int intensity)
	{
		SetRainIntensity(intensity);
		ps.Play();
	}
	
	/// <summary>
	/// Deactivate the rain effect. This will stop the particle system.
	/// </summary>
	public void DeactivateRain()
	{
		ps.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
	}
	
}
