using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001225 RID: 4645
	public class SimpleAABB : MonoBehaviour
	{
		// Token: 0x060075BE RID: 30142 RVA: 0x0026505E File Offset: 0x0026325E
		private void Awake()
		{
			this.m_bounds = new Bounds(this.m_center, this.m_size);
		}

		// Token: 0x060075BF RID: 30143 RVA: 0x00265078 File Offset: 0x00263278
		public bool IsInBounds(Vector3 point)
		{
			Vector3 vector = base.transform.InverseTransformPoint(point);
			return this.m_bounds.Contains(vector);
		}

		// Token: 0x04008593 RID: 34195
		[SerializeField]
		private Vector3 m_center;

		// Token: 0x04008594 RID: 34196
		[SerializeField]
		private Vector3 m_size;

		// Token: 0x04008595 RID: 34197
		private Bounds m_bounds;
	}
}
