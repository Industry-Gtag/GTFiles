using System;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class AngryBeeAnimator : MonoBehaviour
{
	// Token: 0x06000E29 RID: 3625 RVA: 0x0004D6F0 File Offset: 0x0004B8F0
	private void Awake()
	{
		this.bees = new GameObject[this.numBees];
		this.beeOrbits = new GameObject[this.numBees];
		this.beeOrbitalRadii = new float[this.numBees];
		this.beeOrbitalAxes = new Vector3[this.numBees];
		for (int i = 0; i < this.numBees; i++)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = base.transform;
			Vector2 vector = Random.insideUnitCircle * this.orbitMaxCenterDisplacement;
			gameObject.transform.localPosition = new Vector3(vector.x, Random.Range(-this.orbitMaxHeightDisplacement, this.orbitMaxHeightDisplacement), vector.y);
			gameObject.transform.localRotation = Quaternion.Euler(Random.Range(-this.orbitMaxTilt, this.orbitMaxTilt), (float)Random.Range(0, 360), 0f);
			this.beeOrbitalAxes[i] = gameObject.transform.up;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.beePrefab, gameObject.transform);
			float num = Random.Range(this.orbitMinRadius, this.orbitMaxRadius);
			this.beeOrbitalRadii[i] = num;
			gameObject2.transform.localPosition = Vector3.forward * num;
			gameObject2.transform.localRotation = Quaternion.Euler(-90f, 90f, 0f);
			gameObject2.transform.localScale = Vector3.one * this.beeScale;
			this.bees[i] = gameObject2;
			this.beeOrbits[i] = gameObject;
		}
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x0004D88C File Offset: 0x0004BA8C
	private void Update()
	{
		float num = this.orbitSpeed * Time.deltaTime;
		for (int i = 0; i < this.numBees; i++)
		{
			this.beeOrbits[i].transform.Rotate(this.beeOrbitalAxes[i], num);
		}
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x0004D8D8 File Offset: 0x0004BAD8
	public void SetEmergeFraction(float fraction)
	{
		for (int i = 0; i < this.numBees; i++)
		{
			this.bees[i].transform.localPosition = Vector3.forward * fraction * this.beeOrbitalRadii[i];
		}
	}

	// Token: 0x040010FC RID: 4348
	[SerializeField]
	private GameObject beePrefab;

	// Token: 0x040010FD RID: 4349
	[SerializeField]
	private int numBees;

	// Token: 0x040010FE RID: 4350
	[SerializeField]
	private float orbitMinRadius;

	// Token: 0x040010FF RID: 4351
	[SerializeField]
	private float orbitMaxRadius;

	// Token: 0x04001100 RID: 4352
	[SerializeField]
	private float orbitMaxHeightDisplacement;

	// Token: 0x04001101 RID: 4353
	[SerializeField]
	private float orbitMaxCenterDisplacement;

	// Token: 0x04001102 RID: 4354
	[SerializeField]
	private float orbitMaxTilt;

	// Token: 0x04001103 RID: 4355
	[SerializeField]
	private float orbitSpeed;

	// Token: 0x04001104 RID: 4356
	[SerializeField]
	private float beeScale;

	// Token: 0x04001105 RID: 4357
	private GameObject[] beeOrbits;

	// Token: 0x04001106 RID: 4358
	private GameObject[] bees;

	// Token: 0x04001107 RID: 4359
	private Vector3[] beeOrbitalAxes;

	// Token: 0x04001108 RID: 4360
	private float[] beeOrbitalRadii;
}
