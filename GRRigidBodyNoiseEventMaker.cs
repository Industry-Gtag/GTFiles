using System;
using UnityEngine;

// Token: 0x020007E3 RID: 2019
public class GRRigidBodyNoiseEventMaker : MonoBehaviour
{
	// Token: 0x0600338D RID: 13197 RVA: 0x00119DF4 File Offset: 0x00117FF4
	public void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > this.velocityThreshold && base.GetComponent<GameEntity>() != null)
		{
			GRNoiseEventManager.instance.AddNoiseEvent(collision.GetContact(0).point, 1f, 1f);
		}
	}

	// Token: 0x040042DF RID: 17119
	public float velocityThreshold = 5f;
}
