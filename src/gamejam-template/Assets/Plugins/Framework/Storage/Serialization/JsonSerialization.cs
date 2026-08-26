using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

namespace Framework.Storage.Serialization
{
	public static class JsonSerialization
	{
		public static string ToJson(this object self)
		{
			return JsonConvert.SerializeObject(self, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				Formatting = Formatting.Indented,
				TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});
		}

		public static T FromJson<T>(this string json)
		{
			return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});
		}

		public static object FromJson(this string json, Type type)
		{
			return JsonConvert.DeserializeObject(json, type, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto,
				TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});
		}

		// Force the AOT compiler to include the necessary code for the ReferenceConverter.
		static JsonSerialization() => AotHelper.Ensure(() => { _ = new ReferenceConverter(typeof(Dummy)); });

		public class Dummy { }
	}
}
