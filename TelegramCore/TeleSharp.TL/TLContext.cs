using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using TeleSharp.TL;
namespace TeleSharp.TL
{
	public static class TLContext
	{
		private static Dictionary<int, Type> Types;

		static TLContext()
		{
			Types = new Dictionary<int, Type>();

			Types = GetTeleSharpTypes().ToDictionary(x => ((TLObjectAttribute)x.GetCustomAttribute(typeof(TLObjectAttribute))).Constructor, x => x);
			Types[481674261] = typeof(TLVector<>);
		}

		private static IEnumerable<Type> GetTeleSharpTypes()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			List<Type> result = new List<Type>();
			try
			{
				foreach (var t in assembly.GetTypes())
				{
					if (t.IsClass && string.IsNullOrWhiteSpace(t.Namespace))
					{

					}
					if (t.IsClass && !string.IsNullOrWhiteSpace(t.Namespace) && t.Namespace.StartsWith("TeleSharp.TL"))
					{
						if (t.IsSubclassOf(typeof(TLObject)))
						{
							if (t.GetCustomAttribute(typeof(TLObjectAttribute)) != null)
							{
								result.Add(t);
							}
						}
					}
				}
				return result;
			}
			catch (TypeInitializationException ex)
			{
				return new List<Type>();
			}
		}

		public static Type getType(int Constructor)
		{
			if (!Types.ContainsKey(Constructor))
			{
			}
			return Types[Constructor];
		}
	}
}
