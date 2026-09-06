using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020004C7 RID: 1223
public class SlingshotProjectileTrail : MonoBehaviour
{
	// Token: 0x06001DEB RID: 7659 RVA: 0x000A158C File Offset: 0x0009F78C
	private void Awake()
	{
		this.initialWidthMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x000A15A0 File Offset: 0x0009F7A0
	public void AttachTrail(GameObject obj, bool blueTeam, bool redTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		this.followObject = obj;
		this.followXform = this.followObject.transform;
		Transform transform = base.transform;
		transform.position = this.followXform.position;
		this.initialScale = transform.localScale.x;
		transform.localScale = this.followXform.localScale;
		this.trailRenderer.widthMultiplier = this.initialWidthMultiplier * this.followXform.localScale.x;
		this.trailRenderer.Clear();
		if (shouldOverrideColor)
		{
			this.SetColor(overrideColor);
		}
		else if (blueTeam)
		{
			this.SetColor(this.blueColor);
		}
		else if (redTeam)
		{
			this.SetColor(this.orangeColor);
		}
		else
		{
			this.SetColor(this.defaultColor);
		}
		this.timeToDie = -1f;
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x000A1674 File Offset: 0x0009F874
	protected void LateUpdate()
	{
		if (this.followObject.IsNull())
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		base.gameObject.transform.position = this.followXform.position;
		if (!this.followObject.activeSelf && this.timeToDie < 0f)
		{
			this.timeToDie = Time.time + this.trailRenderer.time;
		}
		if (this.timeToDie > 0f && Time.time > this.timeToDie)
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001DEE RID: 7662 RVA: 0x000A1730 File Offset: 0x0009F930
	public void SetColor(Color color)
	{
		TrailRenderer trailRenderer = this.trailRenderer;
		this.trailRenderer.endColor = color;
		trailRenderer.startColor = color;
	}

	// Token: 0x0400283F RID: 10303
	public TrailRenderer trailRenderer;

	// Token: 0x04002840 RID: 10304
	public Color defaultColor = Color.white;

	// Token: 0x04002841 RID: 10305
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x04002842 RID: 10306
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x04002843 RID: 10307
	private GameObject followObject;

	// Token: 0x04002844 RID: 10308
	private Transform followXform;

	// Token: 0x04002845 RID: 10309
	private float timeToDie = -1f;

	// Token: 0x04002846 RID: 10310
	private float initialScale;

	// Token: 0x04002847 RID: 10311
	private float initialWidthMultiplier;
}
