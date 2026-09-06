using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F8A RID: 3978
	public class GameObjectManagerWithId : MonoBehaviour
	{
		// Token: 0x060062E4 RID: 25316 RVA: 0x001FD878 File Offset: 0x001FBA78
		private void Awake()
		{
			Transform[] componentsInChildren = this.objectsContainer.GetComponentsInChildren<Transform>(false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				GameObjectManagerWithId.gameObjectData gameObjectData = new GameObjectManagerWithId.gameObjectData();
				gameObjectData.transform = componentsInChildren[i];
				gameObjectData.id = this.zone.ToString() + i.ToString();
				this.objectData.Add(gameObjectData);
			}
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x001FD8DE File Offset: 0x001FBADE
		private void OnDestroy()
		{
			this.objectData.Clear();
		}

		// Token: 0x060062E6 RID: 25318 RVA: 0x001FD8EC File Offset: 0x001FBAEC
		public void ReceiveEvent(string id, Transform _transform)
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.id == id)
				{
					gameObjectData.isMatched = true;
					gameObjectData.followTransform = _transform;
				}
			}
		}

		// Token: 0x060062E7 RID: 25319 RVA: 0x001FD954 File Offset: 0x001FBB54
		private void Update()
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.isMatched)
				{
					gameObjectData.transform.transform.position = gameObjectData.followTransform.position;
					gameObjectData.transform.transform.rotation = gameObjectData.followTransform.rotation;
				}
			}
		}

		// Token: 0x04007197 RID: 29079
		public GameObject objectsContainer;

		// Token: 0x04007198 RID: 29080
		public GTZone zone;

		// Token: 0x04007199 RID: 29081
		private readonly List<GameObjectManagerWithId.gameObjectData> objectData = new List<GameObjectManagerWithId.gameObjectData>();

		// Token: 0x02000F8B RID: 3979
		private class gameObjectData
		{
			// Token: 0x0400719A RID: 29082
			public Transform transform;

			// Token: 0x0400719B RID: 29083
			public Transform followTransform;

			// Token: 0x0400719C RID: 29084
			public string id;

			// Token: 0x0400719D RID: 29085
			public bool isMatched;
		}
	}
}
