using System;
using System.Collections.Generic;
using BoingKit;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class BushFieldReactorMain : MonoBehaviour
{
	// Token: 0x0600008D RID: 141 RVA: 0x00004C28 File Offset: 0x00002E28
	public void Start()
	{
		Random.InitState(0);
		for (int i = 0; i < this.NumBushes; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.Bush);
			float num = Random.Range(this.BushScaleRange.x, this.BushScaleRange.y);
			gameObject.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.x), 0.2f * num, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject.transform.localScale = num * Vector3.one;
			BoingBehavior component = gameObject.GetComponent<BoingBehavior>();
			if (component != null)
			{
				component.Reboot();
			}
		}
		for (int j = 0; j < this.NumBlossoms; j++)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.Blossom);
			float num2 = Random.Range(this.BlossomScaleRange.x, this.BlossomScaleRange.y);
			gameObject2.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.y), 0.2f * num2, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject2.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject2.transform.localScale = num2 * Vector3.one;
			BoingBehavior component2 = gameObject2.GetComponent<BoingBehavior>();
			if (component2 != null)
			{
				component2.Reboot();
			}
		}
		this.m_aSphere = new List<GameObject>(this.NumSpheresPerCircle * this.NumCircles);
		for (int k = 0; k < this.NumCircles; k++)
		{
			for (int l = 0; l < this.NumSpheresPerCircle; l++)
			{
				this.m_aSphere.Add(Object.Instantiate<GameObject>(this.Sphere));
			}
		}
		this.m_basePhase = 0f;
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00004E94 File Offset: 0x00003094
	public void Update()
	{
		int num = 0;
		for (int i = 0; i < this.NumCircles; i++)
		{
			float num2 = this.MaxCircleRadius / (float)(i + 1);
			for (int j = 0; j < this.NumSpheresPerCircle; j++)
			{
				float num3 = this.m_basePhase + (float)j / (float)this.NumSpheresPerCircle * 2f * 3.1415927f;
				num3 *= ((i % 2 == 0) ? 1f : (-1f));
				this.m_aSphere[num].transform.position = new Vector3(num2 * Mathf.Cos(num3), 0.2f, num2 * Mathf.Sin(num3));
				num++;
			}
		}
		this.m_basePhase -= this.CircleSpeed / this.MaxCircleRadius * Time.deltaTime;
	}

	// Token: 0x0400009A RID: 154
	public GameObject Bush;

	// Token: 0x0400009B RID: 155
	public GameObject Blossom;

	// Token: 0x0400009C RID: 156
	public GameObject Sphere;

	// Token: 0x0400009D RID: 157
	public int NumBushes;

	// Token: 0x0400009E RID: 158
	public Vector2 BushScaleRange;

	// Token: 0x0400009F RID: 159
	public int NumBlossoms;

	// Token: 0x040000A0 RID: 160
	public Vector2 BlossomScaleRange;

	// Token: 0x040000A1 RID: 161
	public Vector2 FieldBounds;

	// Token: 0x040000A2 RID: 162
	public int NumSpheresPerCircle;

	// Token: 0x040000A3 RID: 163
	public int NumCircles;

	// Token: 0x040000A4 RID: 164
	public float MaxCircleRadius;

	// Token: 0x040000A5 RID: 165
	public float CircleSpeed;

	// Token: 0x040000A6 RID: 166
	private List<GameObject> m_aSphere;

	// Token: 0x040000A7 RID: 167
	private float m_basePhase;
}
