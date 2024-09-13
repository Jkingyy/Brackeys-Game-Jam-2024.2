using UnityEngine;

/// <summary>
/// This class is responsible for controlling the rain effect.
/// </summary>
public class RainEffect : MonoBehaviour
{
	[SerializeField] ParticleSystem ps;
	private ParticleSystem.EmissionModule emissionModule;
	
	/// <summary>
	/// This is the value that the particle system will use for particles per second.
	/// </summary>
	private float rainIntensity;
	private float flyRainIntensityLimit = 35f;
	
	void Awake()
	{
		emissionModule = ps.emission;
	}
	
	/// <summary>
	/// Set the rain intensity. This will set the rate of the particle system.
	/// <para>
	/// The particle system will stop and start automatically based on the rate.
	/// </para>
	/// </summary>
	/// <param name="intensity"></param>
	public void SetRainIntensity(float intensity, PlayerMovement playerMovement)
	{
		if (intensity <= 0) DeactivateRainEffect();
		else if (!ps.isPlaying) ActivateRainEffect();
		
		rainIntensity = intensity;
		emissionModule.rateOverTime = rainIntensity;
		
		if (rainIntensity > flyRainIntensityLimit) playerMovement.SetIsRainingBlockingFlight(true);
		else playerMovement.SetIsRainingBlockingFlight(false);
	}
	
	public void ActivateRainEffect()
	{
		if (!ps.isPlaying) ps.Play();
	}
	
	public void DeactivateRainEffect()
	{
		ps.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
	}
}
