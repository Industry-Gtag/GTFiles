using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x02001099 RID: 4249
	public static class Reflection
	{
		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x060069EE RID: 27118 RVA: 0x00220E81 File Offset: 0x0021F081
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x060069EF RID: 27119 RVA: 0x00220E88 File Offset: 0x0021F088
		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		// Token: 0x060069F0 RID: 27120 RVA: 0x00220E8F File Offset: 0x0021F08F
		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		// Token: 0x060069F1 RID: 27121 RVA: 0x00220EA0 File Offset: 0x0021F0A0
		private static Assembly[] PreFetchAllAssemblies()
		{
			if (Reflection.gAssemblyCache != null)
			{
				return Reflection.gAssemblyCache;
			}
			return Reflection.gAssemblyCache = (from a in AppDomain.CurrentDomain.GetAssemblies()
				where a != null
				select a).ToArray<Assembly>();
		}

		// Token: 0x060069F2 RID: 27122 RVA: 0x00220EF4 File Offset: 0x0021F0F4
		private static Type[] PreFetchAllTypes()
		{
			if (Reflection.gTypeCache != null)
			{
				return Reflection.gTypeCache;
			}
			return Reflection.gTypeCache = (from t in Reflection.PreFetchAllAssemblies().SelectMany((Assembly a) => a.GetTypes())
				where t != null
				select t).ToArray<Type>();
		}

		// Token: 0x060069F3 RID: 27123 RVA: 0x00220F68 File Offset: 0x0021F168
		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return (from m in Reflection.AllTypes.SelectMany((Type t) => t.GetRuntimeMethods())
				where m.GetCustomAttributes(typeof(T), false).Length != 0
				select m).ToArray<MethodInfo>();
		}

		// Token: 0x04007992 RID: 31122
		private static Assembly[] gAssemblyCache;

		// Token: 0x04007993 RID: 31123
		private static Type[] gTypeCache;
	}
}
