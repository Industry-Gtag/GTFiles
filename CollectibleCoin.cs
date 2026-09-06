using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class CollectibleCoin : MonoBehaviour
{
	// Token: 0x0600006F RID: 111 RVA: 0x00003B08 File Offset: 0x00001D08
	public void Update()
	{
		BoingBehavior component = base.GetComponent<BoingBehavior>();
		if (this.m_taken)
		{
			if (Time.time - this.m_respawnTimerStartTime < this.RespawnTime)
			{
				return;
			}
			base.transform.position = this.m_respawnPosition + 0.4f * Vector3.down;
			if (component != null)
			{
				component.Reboot();
			}
			base.transform.position = this.m_respawnPosition;
			this.m_taken = false;
		}
		GameObject gameObject = GameObject.Find("Character");
		GameObject gameObject2 = GameObject.Find("Coin Icon");
		GameObject gameObject3 = GameObject.Find("Coin Counter");
		if ((gameObject.transform.position - base.transform.position).sqrMagnitude > 0.4f)
		{
			return;
		}
		this.m_respawnPosition = base.transform.position;
		if (component != null)
		{
			Vector3Spring positionSpring = component.PositionSpring;
			positionSpring.Reset(base.transform.position, new Vector3(100f, 0f, 0f));
			component.PositionSpring = positionSpring;
		}
		base.transform.position = gameObject2.transform.position + new Vector3(-2f, 0.5f, 0f);
		TextMesh component2 = gameObject3.GetComponent<TextMesh>();
		component2.text = (Convert.ToInt32(component2.text) + 1).ToString();
		this.m_respawnTimerStartTime = Time.time;
		this.m_taken = true;
	}

	// Token: 0x04000061 RID: 97
	public float RespawnTime;

	// Token: 0x04000062 RID: 98
	private bool m_taken;

	// Token: 0x04000063 RID: 99
	private Vector3 m_respawnPosition;

	// Token: 0x04000064 RID: 100
	private float m_respawnTimerStartTime;
}
