using UnityEngine;

/// <summary>
/// This class is responsible for controlling the wind effect. It will apply a force to the player in the wind direction while the wind zone is active and the player is inside it.
/// <para>
/// The wind effect is simulated with a particle system.
/// </para>
/// <para>
/// The effect is customizable with a wind direction and speed. The wind direction will set the rotation of the particle system, and the wind speed will set the simulation speed of the particle system.
/// </para>
/// </summary>
public class WindEffect : MonoBehaviour
{
	[SerializeField] ParticleSystem[] windParticleSystems;
	private int windDirection;
	private float windSpeed;
	[SerializeField] private Collider2D windZoneCollider;
	private WindZone windZone;
	private bool effectEnabled;
	void Awake()
	{
		if (!windZoneCollider) Debug.LogError("WindEffect: WindZoneCollider not set.");
		windZoneCollider.isTrigger = true;
		windZoneCollider.enabled = false;
		
		windZone = windZoneCollider.gameObject.GetComponent<WindZone>();
		
		SetWindDirection(-1);
		SetWindSpeed(1);
		DeactivateWind();
	}
	
	void OnEnable()
	{
		if (!windZone) Debug.LogError("WindEffect: WindZone script not found on WindZone object.");
		else windZone.OnWindZoneTrigger += ApplyWindForceToPlayer;
	}
	
	void OnDisable()
	{
		if (windZone) windZone.OnWindZoneTrigger -= ApplyWindForceToPlayer;
	}
	
	/// <summary>
	/// While the wind zone is active and the player is inside it, apply a force to the player in the wind direction with the set wind speed.
	/// </summary>
	/// <param name="other"></param>
	private void ApplyWindForceToPlayer(Collider2D other)
	{
		if (effectEnabled && other.TryGetComponent<Rigidbody2D>(out var rb)) 
		{
			rb.AddForce(new Vector2(windDirection, 0) * windSpeed);
			print("Applying wind force to player.");
		}
	}
	
	/// <summary>
	/// Activate the wind effect with a given direction and speed. This will play the particle system after setting the rotation and simulation speed of it.
	///<para>
	/// If a wind direction is not given, a random direction (either -1 or 1) will be chosen.
	/// </para>
	/// </summary>
	/// <param name="windSpeed"></param>
	/// <param name="windDirection"></param>
	public void ActivateWind(float windSpeed, int windDirection = 0)
	{
		SetWindDirection(windDirection == 0 ? GetRandomWindDirection() : windDirection);
		SetWindSpeed(windSpeed);
		foreach (ParticleSystem ps in windParticleSystems) ps.Play();
		windZoneCollider.enabled = true;
		effectEnabled = true;
	}
	
	/// <summary>
	/// Deactivate the wind effect. This will stop the particle system and disable the collider.
	/// </summary>
	public void DeactivateWind()
	{
		foreach (ParticleSystem ps in windParticleSystems) ps.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
		windZoneCollider.enabled = false;
		effectEnabled = false;
	}
	
	/// <summary>
	/// Set the wind direction. This will set the rotation of the particle system.
	/// </summary>
	/// <param name="direction"></param>
	public void SetWindDirection(int direction)
	{
		windDirection = direction;
		foreach (ParticleSystem ps in windParticleSystems)
		{
			ps.transform.rotation = Quaternion.Euler(0, 0, windDirection * 180);
		}
	}
	
	/// <summary>
	/// Set the wind speed. This will set the simulation speed of the particle system.
	/// </summary>
	/// <param name="speed"></param>
	public void SetWindSpeed(float speed)
	{
		windSpeed = speed;
		foreach (ParticleSystem ps in windParticleSystems)
		{
			ParticleSystem.MainModule main = ps.main;
			main.simulationSpeed = speed;
		}
	}
	
	private int GetRandomWindDirection()
	{
		return Random.Range(-1, 2);
	}
}
