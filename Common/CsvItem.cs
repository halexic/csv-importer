using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace CsvImporter.Common
{
	public partial class CsvItem : NameValueCollection
	{
		#region Static methods
		public static string[] GetValues(string line, char separator)
		{
			//Create regular expression pattern for CSV line
			Regex regex = new Regex(separator + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
			string[] values = regex.Split(line);
			return values;
		}

		/// <summary>
		/// Parses single CSV line.
		/// </summary>
		/// <param name="line">CSV line as string.</param>
		/// <param name="separator">Field separator char.</param>
		public static CsvItem Parse(string[] keys, string line, char separator)
		{
			CsvItem item = null;

			string[] values = GetValues(line, separator);
			if (keys != null && keys.Length == values.Length)
			{
				item = new CsvItem();

				for (int i = 0; i < values.Length; i++)
				{
					//Remove leading and trailing double quotes for each field in the array
					if (values[i].Length >= 2)
					{
						if (values[i].StartsWith("\""))
						{
							values[i] = values[i].Remove(0, 1);
						}
						if (values[i].EndsWith("\""))
						{
							values[i] = values[i].Remove(values[i].Length - 1, 1);
						}
						values[i] = values[i].Replace("\"\"", "\"");
					}

					//Add key-value item
					item.Add(keys[i], values[i]);
				}
			}

			return item;
		}
		#endregion
	}
}
