using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust;

public class JoustPlayer : MonoBehaviour
{
	private Vector2 velocity;

	private RaycastHit2D[] raycastHitResults = new RaycastHit2D[8];

	private float HSpeed;

	private bool flap;

	public float HorizontalSpeed
	{
		get
		{
			return HSpeed;
		}
		set
		{
			HSpeed = value;
		}
	}

	private void LateUpdate()
	{
		velocity.x = HSpeed * 0.001f;
		if (flap)
		{
			velocity.y = Mathf.Min(velocity.y + 0.0005f, 0.0005f);
			flap = false;
		}
		else
		{
			velocity.y = Mathf.Max(velocity.y - Time.deltaTime * 0.0001f, -0.001f);
			ContactFilter2D contactFilter = default(ContactFilter2D);
			contactFilter.SetLayerMask(-5);
			contactFilter.useTriggers = Physics2D.queriesHitTriggers;
			int num = Physics2D.Raycast(base.transform.position, velocity.normalized, contactFilter, raycastHitResults, velocity.magnitude);
			for (int i = 0; i < num; i++)
			{
				if (raycastHitResults[i].collider.TryGetComponent<JoustTerrain>(out var component))
				{
					velocity.y = 0f;
					if (component.transform.localPosition.y < base.transform.localPosition.y)
					{
						base.transform.localPosition = new Vector2(base.transform.localPosition.x, component.transform.localPosition.y + raycastHitResults[i].collider.bounds.size.y);
					}
					break;
				}
			}
		}
		base.transform.Translate(velocity);
		if ((double)Mathf.Abs(base.transform.localPosition.x) > 4.5)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x * -0.95f, base.transform.localPosition.y);
		}
	}

	public void Flap()
	{
		flap = true;
	}
}
