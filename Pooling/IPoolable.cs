using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Pooling
{
	// Token: 0x02000F19 RID: 3865
	public interface IPoolable<T> where T : Component, IPoolable<T>
	{
		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06005ECE RID: 24270
		// (set) Token: 0x06005ECF RID: 24271
		IObjectPool<T> Pool { get; set; }

		// Token: 0x06005ED0 RID: 24272
		void OnCreate();

		// Token: 0x06005ED1 RID: 24273
		void OnPreGet();

		// Token: 0x06005ED2 RID: 24274
		void OnPostGet();

		// Token: 0x06005ED3 RID: 24275
		void OnRelease();
	}
}
