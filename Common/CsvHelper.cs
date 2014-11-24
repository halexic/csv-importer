using System.Collections.Generic;

namespace CsvImporter.Common
{
	public class CsvHelper
	{
		public static string Serialize(IEnumerable<object> rows)
		{
			var serializer = new CsvSerializer();
			serializer.AlwaysEscapeValue = false;
			serializer.Separator = ";";
			string result = serializer.Serialize(rows);
			return result;
		}

		public static IEnumerable<TModel> Deserialize<TModel>(string csv)
		{
			var serializer = new CsvSerializer();
			serializer.AlwaysEscapeValue = false;
			serializer.Separator = ";";
			var result = serializer.Deserialize<TModel>(csv);
			return result;
		}

		public static string TrySerialize(IEnumerable<object> model)
		{
			try
			{
				return Serialize(model);
			}
			catch
			{
				return null;
			}
		}

		public static IEnumerable<TModel> TryDeserialize<TModel>(string csv)
		{
			try
			{
				return Deserialize<TModel>(csv);
			}
			catch
			{
				return default(IEnumerable<TModel>);
			}
		}
	}
}
