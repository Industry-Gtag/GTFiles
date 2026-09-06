using System;

// Token: 0x020001DE RID: 478
public static class DeSerializationExtensions
{
	// Token: 0x06000CB0 RID: 3248 RVA: 0x00045908 File Offset: 0x00043B08
	public static bool TryDeserializeTo<T1>(this object[] eventData, out T1 v1)
	{
		v1 = default(T1);
		if (eventData == null || eventData.Length != 1)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			v1 = t;
			return true;
		}
		return false;
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x00045948 File Offset: 0x00043B48
	public static bool TryDeserializeTo<T1, T2>(this object[] eventData, out T1 v1, out T2 v2)
	{
		v1 = default(T1);
		v2 = default(T2);
		if (eventData == null || eventData.Length != 2)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				v1 = t;
				v2 = t2;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x000459A8 File Offset: 0x00043BA8
	public static bool TryDeserializeTo<T1, T2, T3>(this object[] eventData, out T1 v1, out T2 v2, out T3 v3)
	{
		v1 = default(T1);
		v2 = default(T2);
		v3 = default(T3);
		if (eventData == null || eventData.Length != 3)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					v1 = t;
					v2 = t2;
					v3 = t3;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x00045A28 File Offset: 0x00043C28
	public static bool TryDeserializeTo<T1, T2, T3, T4>(this object[] eventData, out T1 v1, out T2 v2, out T3 v3, out T4 v4)
	{
		v1 = default(T1);
		v2 = default(T2);
		v3 = default(T3);
		v4 = default(T4);
		if (eventData == null || eventData.Length != 4)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						v1 = t;
						v2 = t2;
						v3 = t3;
						v4 = t4;
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x00045AD8 File Offset: 0x00043CD8
	public static bool TryDeserializeTo<T1, T2, T3, T4, T5>(this object[] eventData, out T1 v1, out T2 v2, out T3 v3, out T4 v4, out T5 v5)
	{
		v1 = default(T1);
		v2 = default(T2);
		v3 = default(T3);
		v4 = default(T4);
		v5 = default(T5);
		if (eventData == null || eventData.Length != 5)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						obj = eventData[4];
						if (obj is T5)
						{
							T5 t5 = (T5)((object)obj);
							v1 = t;
							v2 = t2;
							v3 = t3;
							v4 = t4;
							v5 = t5;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x00045BB0 File Offset: 0x00043DB0
	public static bool TryDeserializeTo<T1, T2, T3, T4, T5, T6>(this object[] eventData, out T1 v1, out T2 v2, out T3 v3, out T4 v4, out T5 v5, out T6 v6)
	{
		v1 = default(T1);
		v2 = default(T2);
		v3 = default(T3);
		v4 = default(T4);
		v5 = default(T5);
		v6 = default(T6);
		if (eventData == null || eventData.Length != 6)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						obj = eventData[4];
						if (obj is T5)
						{
							T5 t5 = (T5)((object)obj);
							obj = eventData[5];
							if (obj is T6)
							{
								T6 t6 = (T6)((object)obj);
								v1 = t;
								v2 = t2;
								v3 = t3;
								v4 = t4;
								v5 = t5;
								v6 = t6;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x00045CB0 File Offset: 0x00043EB0
	public static bool TryDeserializeToRef<T1>(this object[] eventData, ref T1 v1)
	{
		if (eventData == null || eventData.Length != 1)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			v1 = t;
			return true;
		}
		return false;
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x00045CE8 File Offset: 0x00043EE8
	public static bool TryDeserializeToRef<T1, T2>(this object[] eventData, ref T1 v1, ref T2 v2)
	{
		if (eventData == null || eventData.Length != 2)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				v1 = t;
				v2 = t2;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x00045D3C File Offset: 0x00043F3C
	public static bool TryDeserializeToRef<T1, T2, T3>(this object[] eventData, ref T1 v1, ref T2 v2, ref T3 v3)
	{
		if (eventData == null || eventData.Length != 3)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					v1 = t;
					v2 = t2;
					v3 = t3;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x00045DA8 File Offset: 0x00043FA8
	public static bool TryDeserializeToRef<T1, T2, T3, T4>(this object[] eventData, ref T1 v1, ref T2 v2, ref T3 v3, ref T4 v4)
	{
		if (eventData == null || eventData.Length != 4)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						v1 = t;
						v2 = t2;
						v3 = t3;
						v4 = t4;
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x00045E3C File Offset: 0x0004403C
	public static bool TryDeserializeToRef<T1, T2, T3, T4, T5>(this object[] eventData, ref T1 v1, ref T2 v2, ref T3 v3, ref T4 v4, ref T5 v5)
	{
		if (eventData == null || eventData.Length != 5)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						obj = eventData[4];
						if (obj is T5)
						{
							T5 t5 = (T5)((object)obj);
							v1 = t;
							v2 = t2;
							v3 = t3;
							v4 = t4;
							v5 = t5;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x00045EF0 File Offset: 0x000440F0
	public static bool TryDeserializeToRef<T1, T2, T3, T4, T5, T6>(this object[] eventData, ref T1 v1, ref T2 v2, ref T3 v3, ref T4 v4, ref T5 v5, ref T6 v6)
	{
		if (eventData == null || eventData.Length != 6)
		{
			return false;
		}
		object obj = eventData[0];
		if (obj is T1)
		{
			T1 t = (T1)((object)obj);
			obj = eventData[1];
			if (obj is T2)
			{
				T2 t2 = (T2)((object)obj);
				obj = eventData[2];
				if (obj is T3)
				{
					T3 t3 = (T3)((object)obj);
					obj = eventData[3];
					if (obj is T4)
					{
						T4 t4 = (T4)((object)obj);
						obj = eventData[4];
						if (obj is T5)
						{
							T5 t5 = (T5)((object)obj);
							obj = eventData[5];
							if (obj is T6)
							{
								T6 t6 = (T6)((object)obj);
								v1 = t;
								v2 = t2;
								v3 = t3;
								v4 = t4;
								v5 = t5;
								v6 = t6;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}
}
