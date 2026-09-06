using System;
using UnityEngine;

// Token: 0x02000A0E RID: 2574
public class TransformFollowXScene : MonoBehaviour
{
	// Token: 0x06004206 RID: 16902 RVA: 0x0015F9DF File Offset: 0x0015DBDF
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06004207 RID: 16903 RVA: 0x0015F9F2 File Offset: 0x0015DBF2
	private void Start()
	{
		this.refToFollow.TryResolve<Transform>(out this.transformToFollow);
	}

	// Token: 0x06004208 RID: 16904 RVA: 0x0015FA08 File Offset: 0x0015DC08
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x040052C8 RID: 21192
	public XSceneRef refToFollow;

	// Token: 0x040052C9 RID: 21193
	private Transform transformToFollow;

	// Token: 0x040052CA RID: 21194
	public Vector3 offset;

	// Token: 0x040052CB RID: 21195
	public Vector3 prevPos;
}
