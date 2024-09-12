using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class WindZone : MonoBehaviour
{
	public delegate void WindZoneTrigger(Collider2D other);
	public event WindZoneTrigger OnWindZoneTrigger;

	private void OnTriggerStay2D(Collider2D other)
	{
		print("Wind zone is active!");
		if (other.CompareTag("Player")) OnWindZoneTrigger?.Invoke(other);
	}
}
