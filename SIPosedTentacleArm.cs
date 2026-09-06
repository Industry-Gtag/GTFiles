using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200011D RID: 285
[ExecuteAlways]
public class SIPosedTentacleArm : MonoBehaviour
{
	// Token: 0x06000722 RID: 1826 RVA: 0x00028CCE File Offset: 0x00026ECE
	public void ConfigureFrom(SIGadgetTentacleArm source, MeshRenderer rend1, MeshRenderer rend2, Transform anchor1, Transform anchor2)
	{
		this.LengthFactor = source.LengthFactor;
		this.tentacleRenderer = rend1;
		this.tentacleRenderer2 = rend2;
		this.tentacleAnchor = anchor1;
		this.tentacleAnchor2 = anchor2;
		this.tentacleSharedMaterial = rend1.sharedMaterial;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00028D06 File Offset: 0x00026F06
	private void Start()
	{
		this.UpdateTentaclePose();
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00023F0C File Offset: 0x0002210C
	private bool CanUpdateTentaclePose()
	{
		return true;
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00028D10 File Offset: 0x00026F10
	private void EnsureMaterialsInitialized()
	{
		if (this._initialized)
		{
			return;
		}
		this._tentacleMat = new Material(this.tentacleSharedMaterial);
		this.tentacleRenderer.material = this._tentacleMat;
		this._hasTentacle2 = this.tentacleRenderer2;
		if (this._hasTentacle2)
		{
			this._tentacleMat2 = new Material(this.tentacleSharedMaterial);
			this.tentacleRenderer2.material = this._tentacleMat2;
		}
		this._initialized = true;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00028D8C File Offset: 0x00026F8C
	private void UpdateTentaclePose()
	{
		if (!this.CanUpdateTentaclePose())
		{
			return;
		}
		this.EnsureMaterialsInitialized();
		this.UpdateTentacle(this._tentacleMat, this.tentacleRenderer.transform, this.tentacleAnchor);
		if (this._hasTentacle2)
		{
			this.UpdateTentacle(this._tentacleMat2, this.tentacleRenderer2.transform, this.tentacleAnchor2);
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00028DEC File Offset: 0x00026FEC
	private void UpdateTentacle(Material material, Transform tentacle, Transform anchor)
	{
		Vector3 vector = Vector3.forward * this.LengthFactor;
		material.SetVector(this.tentacleStartDir_HASH, vector);
		Vector3 vector2 = tentacle.InverseTransformPoint(anchor.position);
		material.SetVector(this.tentacleEnd_HASH, vector2);
		Vector3 vector3 = -tentacle.InverseTransformDirection(anchor.forward) * this.LengthFactor;
		material.SetVector(this.tentacleEndDir_HASH, vector3);
		Vector3 vector4 = SIGadgetTentacleArm.SplineSample(0.25f, vector, vector2, vector3);
		Vector3 vector5 = SIGadgetTentacleArm.SplineSample(0.26f, vector, vector2, vector3);
		Vector3 vector6 = SIGadgetTentacleArm.SplineSample(0.75f, vector, vector2, vector3);
		Vector3 vector7 = SIGadgetTentacleArm.SplineSample(0.76f, vector, vector2, vector3);
		Vector3 planeIntersection = SIGadgetTentacleArm.GetPlaneIntersection(vector4, (vector5 - vector4).normalized, vector6, (vector7 - vector6).normalized, Quaternion.AngleAxis(90f, Vector3.forward) * vector2.WithZ(0f).normalized);
		material.SetVector(this.tentacleRingOrigin_HASH, planeIntersection);
	}

	// Token: 0x04000902 RID: 2306
	public float LengthFactor = 1.5f;

	// Token: 0x04000903 RID: 2307
	public MeshRenderer tentacleRenderer;

	// Token: 0x04000904 RID: 2308
	public MeshRenderer tentacleRenderer2;

	// Token: 0x04000905 RID: 2309
	public Transform tentacleAnchor;

	// Token: 0x04000906 RID: 2310
	public Transform tentacleAnchor2;

	// Token: 0x04000907 RID: 2311
	public Material tentacleSharedMaterial;

	// Token: 0x04000908 RID: 2312
	private bool _initialized;

	// Token: 0x04000909 RID: 2313
	private bool _hasTentacle2;

	// Token: 0x0400090A RID: 2314
	private Material _tentacleMat;

	// Token: 0x0400090B RID: 2315
	private Material _tentacleMat2;

	// Token: 0x0400090C RID: 2316
	private Vector3 _lastPos;

	// Token: 0x0400090D RID: 2317
	private Vector3 _lastAnchorPos;

	// Token: 0x0400090E RID: 2318
	private ShaderHashId tentacleStartDir_HASH = "_TentacleStartDir";

	// Token: 0x0400090F RID: 2319
	private ShaderHashId tentacleEnd_HASH = "_TentacleEndPos";

	// Token: 0x04000910 RID: 2320
	private ShaderHashId tentacleEndDir_HASH = "_TentacleEndDir";

	// Token: 0x04000911 RID: 2321
	private ShaderHashId tentacleRingOrigin_HASH = "_TentacleRingOrigin";
}
