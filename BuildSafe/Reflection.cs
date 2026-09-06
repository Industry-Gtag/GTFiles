using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02001098 RID: 4248
	public static class Reflection<T>
	{
		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x060069E4 RID: 27108 RVA: 0x00220DBD File Offset: 0x0021EFBD
		public static Type Type { get; } = typeof(T);

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x060069E5 RID: 27109 RVA: 0x00220DC4 File Offset: 0x0021EFC4
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x060069E6 RID: 27110 RVA: 0x00220DCB File Offset: 0x0021EFCB
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x060069E7 RID: 27111 RVA: 0x00220DD2 File Offset: 0x0021EFD2
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x060069E8 RID: 27112 RVA: 0x00220DD9 File Offset: 0x0021EFD9
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x060069E9 RID: 27113 RVA: 0x00220DE0 File Offset: 0x0021EFE0
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Reflection<T>.Type.GetRuntimeEvents().ToArray<EventInfo>();
		}

		// Token: 0x060069EA RID: 27114 RVA: 0x00220E04 File Offset: 0x0021F004
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Reflection<T>.Type.GetRuntimeProperties().ToArray<PropertyInfo>();
		}

		// Token: 0x060069EB RID: 27115 RVA: 0x00220E28 File Offset: 0x0021F028
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Reflection<T>.Type.GetRuntimeMethods().ToArray<MethodInfo>();
		}

		// Token: 0x060069EC RID: 27116 RVA: 0x00220E4C File Offset: 0x0021F04C
		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Reflection<T>.Type.GetRuntimeFields().ToArray<FieldInfo>();
		}

		// Token: 0x0400798C RID: 31116
		private static Type gCachedType;

		// Token: 0x0400798D RID: 31117
		private static MethodInfo[] gMethodsCache;

		// Token: 0x0400798E RID: 31118
		private static FieldInfo[] gFieldsCache;

		// Token: 0x0400798F RID: 31119
		private static PropertyInfo[] gPropertiesCache;

		// Token: 0x04007990 RID: 31120
		private static EventInfo[] gEventsCache;
	}
}
