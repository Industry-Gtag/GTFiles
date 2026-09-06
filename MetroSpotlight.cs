using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001C0 RID: 448
public class MetroSpotlight : MonoBehaviour
{
	// Token: 0x06000BFB RID: 3067 RVA: 0x00041874 File Offset: 0x0003FA74
	public void Tick()
	{
		if (!this._light)
		{
			return;
		}
		if (!this._target)
		{
			return;
		}
		this._time += this.speed * Time.deltaTime * Time.deltaTime;
		Vector3 position = this._target.position;
		Vector3 normalized = (position - this._light.position).normalized;
		Vector3 vector = Vector3.Cross(normalized, this._blimp.forward);
		Vector3 vector2 = Vector3.Cross(normalized, vector);
		Vector3 vector3 = MetroSpotlight.Figure8(position, vector, vector2, this._radius, this._time, this._offset, this._theta);
		this._light.LookAt(vector3);
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00041928 File Offset: 0x0003FB28
	private static Vector3 Figure8(Vector3 origin, Vector3 xDir, Vector3 yDir, float scale, float t, float offset, float theta)
	{
		float num = 2f / (3f - Mathf.Cos(2f * (t + offset)));
		float num2 = scale * num * Mathf.Cos(t + offset);
		float num3 = scale * num * Mathf.Sin(2f * (t + offset)) / 2f;
		Vector3 vector = Vector3.Cross(xDir, yDir);
		Quaternion quaternion = Quaternion.AngleAxis(theta, vector);
		xDir = quaternion * xDir;
		yDir = quaternion * yDir;
		Vector3 vector2 = xDir * num2 + yDir * num3;
		return origin + vector2;
	}

	// Token: 0x04000E97 RID: 3735
	[SerializeField]
	private Transform _blimp;

	// Token: 0x04000E98 RID: 3736
	[SerializeField]
	private Transform _light;

	// Token: 0x04000E99 RID: 3737
	[SerializeField]
	private Transform _target;

	// Token: 0x04000E9A RID: 3738
	[FormerlySerializedAs("_scale")]
	[SerializeField]
	private float _radius = 1f;

	// Token: 0x04000E9B RID: 3739
	[SerializeField]
	private float _offset;

	// Token: 0x04000E9C RID: 3740
	[SerializeField]
	private float _theta;

	// Token: 0x04000E9D RID: 3741
	public float speed = 16f;

	// Token: 0x04000E9E RID: 3742
	[Space]
	private float _time;
}
