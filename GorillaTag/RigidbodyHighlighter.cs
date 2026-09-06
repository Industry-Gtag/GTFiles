using System;
using System.Collections.Generic;
using GorillaExtensions;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001220 RID: 4640
	public class RigidbodyHighlighter : MonoBehaviour
	{
		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x0600759F RID: 30111 RVA: 0x00264A04 File Offset: 0x00262C04
		private string ButtonText
		{
			get
			{
				if (!this.Active)
				{
					return "Highlight Rigidbodies";
				}
				return "Unhighlight Rigidbodies";
			}
		}

		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x060075A0 RID: 30112 RVA: 0x00264A19 File Offset: 0x00262C19
		// (set) Token: 0x060075A1 RID: 30113 RVA: 0x00264A21 File Offset: 0x00262C21
		public bool Active { get; set; }

		// Token: 0x060075A2 RID: 30114 RVA: 0x00264A2C File Offset: 0x00262C2C
		private void Awake()
		{
			Object.Destroy(base.gameObject);
			if (RigidbodyHighlighter.Instance != null && RigidbodyHighlighter.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			RigidbodyHighlighter.Instance = this;
			this._lineRenderer.startWidth = this._lineWidth;
			this._lineRenderer.endWidth = this._lineWidth;
		}

		// Token: 0x060075A3 RID: 30115 RVA: 0x00264A94 File Offset: 0x00262C94
		private void Update()
		{
			if (!this.Active)
			{
				this._lineRenderer.positionCount = 0;
				return;
			}
			this._rigidbodies.Clear();
			this._rigidbodies.AddAll(RigidbodyHighlighter.GetAwakeRigidbodies());
			this.DrawTracers();
			foreach (Rigidbody rigidbody in this._rigidbodies)
			{
				RigidbodyHighlighter.DrawBox(rigidbody.transform, Color.red, 0.1f);
			}
		}

		// Token: 0x060075A4 RID: 30116 RVA: 0x00264B2C File Offset: 0x00262D2C
		private static List<Rigidbody> GetAwakeRigidbodies()
		{
			List<Rigidbody> list = new List<Rigidbody>();
			Object[] array = Object.FindObjectsByType(typeof(Rigidbody), FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody rigidbody = array[i] as Rigidbody;
				if (rigidbody == null)
				{
					throw new Exception("Non-rigidbody found by FindObjectsByType.");
				}
				if (!rigidbody.IsSleeping())
				{
					list.Add(rigidbody);
				}
			}
			return list;
		}

		// Token: 0x060075A5 RID: 30117 RVA: 0x00264B85 File Offset: 0x00262D85
		private void HighlightActiveRigidbodies()
		{
			this.Active = !this.Active;
		}

		// Token: 0x060075A6 RID: 30118 RVA: 0x00264B98 File Offset: 0x00262D98
		private void GetRigidbodyNames()
		{
			List<Rigidbody> list = ((this._rigidbodies.Count > 0) ? this._rigidbodies : RigidbodyHighlighter.GetAwakeRigidbodies());
			for (int i = 0; i < list.Count; i++)
			{
				Debug.Log(string.Format("Rigidbody {0} of {1}: {2}", i, list.Count, list[i].name));
			}
		}

		// Token: 0x060075A7 RID: 30119 RVA: 0x00264C00 File Offset: 0x00262E00
		private void OnDrawGizmos()
		{
			if (!this.Active)
			{
				return;
			}
			Gizmos.color = Color.red;
			foreach (Rigidbody rigidbody in this._rigidbodies)
			{
				Gizmos.DrawWireCube(rigidbody.transform.position, Vector3.one);
			}
		}

		// Token: 0x060075A8 RID: 30120 RVA: 0x00264C74 File Offset: 0x00262E74
		private static void DrawBox(Transform tx, Color color, float duration)
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			matrix4x.SetTRS(tx.position, tx.rotation, tx.lossyScale);
			Vector3 vector = matrix4x.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));
			Vector3 vector2 = matrix4x.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
			Vector3 vector3 = matrix4x.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));
			Vector3 vector4 = matrix4x.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
			Vector3 vector5 = matrix4x.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
			Vector3 vector6 = matrix4x.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
			Vector3 vector7 = matrix4x.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
			Vector3 vector8 = matrix4x.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
			Debug.DrawLine(vector, vector2, color, duration, false);
			Debug.DrawLine(vector2, vector4, color, duration, false);
			Debug.DrawLine(vector4, vector3, color, duration, false);
			Debug.DrawLine(vector3, vector, color, duration, false);
			Debug.DrawLine(vector8, vector7, color, duration, false);
			Debug.DrawLine(vector7, vector5, color, duration, false);
			Debug.DrawLine(vector5, vector6, color, duration, false);
			Debug.DrawLine(vector6, vector8, color, duration, false);
			Debug.DrawLine(vector, vector5, color, duration, false);
			Debug.DrawLine(vector2, vector6, color, duration, false);
			Debug.DrawLine(vector3, vector7, color, duration, false);
			Debug.DrawLine(vector4, vector8, color, duration, false);
		}

		// Token: 0x060075A9 RID: 30121 RVA: 0x00264E10 File Offset: 0x00263010
		private void DrawTracers()
		{
			Vector3[] array = new Vector3[this._rigidbodies.Count * 2 + 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((i % 2 == 0) ? (Camera.main.transform.position + this._tracerOffset) : this._rigidbodies[i / 2].transform.position);
			}
			this._lineRenderer.positionCount = array.Length;
			this._lineRenderer.SetPositions(array);
		}

		// Token: 0x04008588 RID: 34184
		[CanBeNull]
		public static RigidbodyHighlighter Instance;

		// Token: 0x04008589 RID: 34185
		[SerializeField]
		private float _inGameDuration = 10f;

		// Token: 0x0400858A RID: 34186
		[SerializeField]
		private LineRenderer _lineRenderer;

		// Token: 0x0400858B RID: 34187
		[SerializeField]
		private float _lineWidth = 0.01f;

		// Token: 0x0400858C RID: 34188
		[SerializeField]
		private Vector3 _tracerOffset = 0.5f * Vector3.down;

		// Token: 0x0400858E RID: 34190
		private readonly List<Rigidbody> _rigidbodies = new List<Rigidbody>();
	}
}
