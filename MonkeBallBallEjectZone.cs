using System;
using UnityEngine;

// Token: 0x02000608 RID: 1544
public class MonkeBallBallEjectZone : MonoBehaviour
{
	// Token: 0x06002682 RID: 9858 RVA: 0x000CBEC0 File Offset: 0x000CA0C0
	private void OnCollisionEnter(Collision collision)
	{
		GameBall component = collision.gameObject.GetComponent<GameBall>();
		if (component != null && collision.contacts.Length != 0)
		{
			component.SetVelocity(collision.contacts[0].impulse.normalized * this.ejectVelocity);
		}
	}

	// Token: 0x04003201 RID: 12801
	public Transform target;

	// Token: 0x04003202 RID: 12802
	public float ejectVelocity = 15f;
}
