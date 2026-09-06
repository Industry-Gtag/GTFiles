using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class PropSelector : MonoBehaviour
{
	// Token: 0x0600116C RID: 4460 RVA: 0x0005DC10 File Offset: 0x0005BE10
	private void Start()
	{
		foreach (GameObject gameObject in new List<GameObject>(this._props.OrderBy((GameObject x) => PropSelector._gRandom.Next()).Take(this._desiredActivePropsNum)))
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x040014C6 RID: 5318
	[SerializeField]
	private List<GameObject> _props = new List<GameObject>();

	// Token: 0x040014C7 RID: 5319
	[SerializeField]
	private int _desiredActivePropsNum = 1;

	// Token: 0x040014C8 RID: 5320
	private static readonly Random _gRandom = new Random();
}
