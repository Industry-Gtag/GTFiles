using System;
using UnityEngine;

// Token: 0x02000E17 RID: 3607
[Serializable]
public class SceneObject : IEquatable<SceneObject>
{
	// Token: 0x06005862 RID: 22626 RVA: 0x001CB4C5 File Offset: 0x001C96C5
	public Type GetObjectType()
	{
		if (string.IsNullOrWhiteSpace(this.typeString))
		{
			return null;
		}
		if (this.typeString.Contains("ProxyType"))
		{
			return ProxyType.Parse(this.typeString);
		}
		return Type.GetType(this.typeString);
	}

	// Token: 0x06005863 RID: 22627 RVA: 0x001CB4FF File Offset: 0x001C96FF
	public SceneObject(int classID, ulong fileID)
	{
		this.classID = classID;
		this.fileID = fileID;
		this.typeString = UnityYaml.ClassIDToType[classID].AssemblyQualifiedName;
	}

	// Token: 0x06005864 RID: 22628 RVA: 0x001CB52B File Offset: 0x001C972B
	public bool Equals(SceneObject other)
	{
		return this.fileID == other.fileID && this.classID == other.classID;
	}

	// Token: 0x06005865 RID: 22629 RVA: 0x001CB54C File Offset: 0x001C974C
	public override bool Equals(object obj)
	{
		SceneObject sceneObject = obj as SceneObject;
		return sceneObject != null && this.Equals(sceneObject);
	}

	// Token: 0x06005866 RID: 22630 RVA: 0x001CB56C File Offset: 0x001C976C
	public override int GetHashCode()
	{
		int num = this.classID;
		int num2 = StaticHash.Compute((long)this.fileID);
		return StaticHash.Compute(num, num2);
	}

	// Token: 0x06005867 RID: 22631 RVA: 0x001CB591 File Offset: 0x001C9791
	public static bool operator ==(SceneObject x, SceneObject y)
	{
		return x.Equals(y);
	}

	// Token: 0x06005868 RID: 22632 RVA: 0x001CB59A File Offset: 0x001C979A
	public static bool operator !=(SceneObject x, SceneObject y)
	{
		return !x.Equals(y);
	}

	// Token: 0x04006883 RID: 26755
	public int classID;

	// Token: 0x04006884 RID: 26756
	public ulong fileID;

	// Token: 0x04006885 RID: 26757
	[SerializeField]
	public string typeString;

	// Token: 0x04006886 RID: 26758
	public string json;
}
