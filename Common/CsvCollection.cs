using System.Collections.Generic;
using System;

namespace CsvImporter.Common
{
	public partial class CsvCollection : List<CsvItem>
	{
		#region Constructor
		public CsvCollection(string[] keys)
		{
			this.Keys = keys;
		}
		#endregion

		#region Public properties
		public string[] Keys
		{
			get;
			protected set;
		}
		#endregion

		#region Static methods
		public static CsvCollection Parse(string content, char separator)
		{
			if (string.IsNullOrEmpty(content))
			{
				throw new Exception("No input CSV data found!");
			}

			CsvCollection result = null;

			string[] lines = content.Split('\n', '\r');
			int count = lines.Length;
			if (count > 0)
			{
				string[] keys = CsvItem.GetValues(lines[0], separator);
				result = new CsvCollection(keys);
				for (int i = 1; i < count; i++)
				{
					string line = lines[i];
					if (!string.IsNullOrEmpty(line))
					{
						var item = CsvItem.Parse(keys, line, separator);
						if (item != null)
						{
							result.Add(item);
						}
					}
				}
			}

			return result;
		}
		#endregion
	}
}
