using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class WindZone : MonoBehaviour
{
	public delegate void WindZoneTrigger(Collider2D other);
	public event WindZoneTrigger OnWindZoneTrigger;

	void OnEnable()
	{
		WindEffect.OnWindEffectToggled += ToggleWindZone;
	}
	void OnDisable()
	{
		WindEffect.OnWindEffectToggled -= ToggleWindZone;
	}
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Player")) OnWindZoneTrigger?.Invoke(other);
	}
	
	public void ToggleWindZone(bool active)
	{
		GetComponent<Collider2D>().enabled = active;
	}
}
