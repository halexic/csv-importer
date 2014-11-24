using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace CsvImporter.Common
{
	public partial class CsvSerializer
	{
		#region Constructor
		public CsvSerializer()
		{
			this.Separator = ";";
			this.AlwaysEscapeValue = false;
		}
		#endregion

		#region Public properties
		public string Separator
		{
			get;
			set;
		}

		public bool AlwaysEscapeValue
		{
			get;
			set;
		}
		#endregion

		#region Public methods
		public string Serialize(IEnumerable<object> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			//Get type with attributes
			var first = items.FirstOrDefault();
			var type = (first != null ? first.GetType() : items.GetType().GetGenericArguments()[0]);

			var columns = new List<string>();
			var rows = new List<List<object>>();

			//Define columns
			var properties = type.GetProperties();
			foreach (var property in properties)
			{
				var attribute = Attribute.GetCustomAttribute(property, typeof(CsvColumnAttribute)) as CsvColumnAttribute;
				if (attribute != null)
				{
					columns.Add(attribute.Name ?? "");
				}
			}

			var builder = new CsvBuilder();
			builder.DefineColumns(columns);

			//Collect rows
			foreach (var row in items)
			{
				rows.Add(new List<object>());
				int index = rows.Count - 1;

				var list = new List<string>();

				foreach (var property in properties)
				{
					var attribute = Attribute.GetCustomAttribute(property, typeof(CsvColumnAttribute)) as CsvColumnAttribute;
					if (attribute != null)
					{
						object value = property.GetValue(row, null);
						rows[index].Add(value);
						list.Add("" + value);
					}
				}

				builder.AppendRow(list);
			}

			//Build result
			string csv = builder.ToString();
			return csv;
		}

		public IEnumerable<TModel> Deserialize<TModel>(string csv)
		{
			if (csv == null)
			{
				throw new ArgumentNullException("csv");
			}

			if (string.IsNullOrEmpty(this.Separator))
			{
				this.Separator = ",";
			}

			var type = typeof(TModel);
			var properties = type.GetProperties();

			var collection = CsvCollection.Parse(csv, this.Separator[0]);
			foreach (var pair in collection)
			{
				var item = Activator.CreateInstance<TModel>();
				foreach (var property in properties)
				{
					var attribute = Attribute.GetCustomAttribute(property, typeof(CsvColumnAttribute)) as CsvColumnAttribute;
					if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
					{
						string value = pair[attribute.Name];
						//TODO: Cast if int, date time, etc.
						if (property.PropertyType == typeof(string))
						{
							property.SetValue(item, value, null);
						}
						else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(System.Nullable<System.Int32>))
						{
							int intValue = 0;
							if (!string.IsNullOrEmpty(value))
							{
								if (Int32.TryParse(value, out intValue))
								{
									property.SetValue(item, intValue, null);
								}
							}
						}
						else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(System.Nullable<Decimal>))
						{
							decimal decimalValue = 0;
							if (!string.IsNullOrEmpty(value))
							{
								if (decimal.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowTrailingSign, CultureInfo.InstalledUICulture, out decimalValue))
								{ 
									property.SetValue(item, decimalValue, null);
								}
							}
						}
						else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(System.Nullable<System.Boolean>))
						{
							bool boolValue = false;
							if (!string.IsNullOrEmpty(value))
							{
								if (bool.TryParse(value, out boolValue))
								{
									property.SetValue(item, boolValue, null);
								}
							}
						}
						else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(System.Nullable<System.DateTime>))
						{
							DateTime datetimeValue = DateTime.MinValue;
							if (!string.IsNullOrEmpty(value))
							{
								if (DateTime.TryParse(value, out datetimeValue))
								{
									property.SetValue(item, datetimeValue, null);
								}
							}
						}
					}
				}
				yield return item;
			}
		}
		#endregion
	}
}
