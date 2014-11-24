using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CsvImporter.Common
{
	/// <summary>
	/// http://en.wikipedia.org/wiki/Comma-separated_values
	/// </summary>
	public partial class CsvBuilder
	{
		#region Constructor
		public CsvBuilder()
		{
			this.Separator = ";";
			this.AlwaysEscapeValue = false;
			this.Columns = new List<string>();
			this.Rows = new List<List<object>>();
		}
		#endregion

		#region Protected properties
		protected List<string> Columns
		{
			get;
			set;
		}

		protected List<List<object>> Rows
		{
			get;
			set;
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
		public void DefineColumns(IEnumerable<string> columns)
		{
			this.Columns = columns.ToList();
		}

		public void DefineColumns(params string[] columns)
		{
			this.Columns = new List<string>();
			foreach (string column in columns)
			{
				this.Columns.Add(column);
			}
		}

		public void AppendRow(IEnumerable<string> values)
		{
			var list = new List<object>();
			foreach (string value in values)
			{
				list.Add(value);
			}
			this.Rows.Add(list);
		}

		public void AppendRow(params object[] values)
		{
			var list = new List<object>();
			foreach (object value in values)
			{
				list.Add(value);
			}
			this.Rows.Add(list);
		}

		public void FromModel(IEnumerable<object> rows)
		{
			if (rows == null)
			{
				return;
			}

			Type type = null;
			foreach (object row in rows)
			{
				type = row.GetType();
				break;
			}

			if (type == null)
			{
				return;
			}

			this.Columns = new List<string>();
			this.Rows = new List<List<object>>();

			//Define columns
			var properties = type.GetProperties();
			foreach (var property in properties)
			{
				var attribute = Attribute.GetCustomAttribute(property, typeof(CsvColumnAttribute)) as CsvColumnAttribute;
				if (attribute != null)
				{
					this.Columns.Add(attribute.Name ?? "");
				}
			}

			//Collect rows
			foreach (var row in rows)
			{
				this.Rows.Add(new List<object>());
				int index = this.Rows.Count - 1;

				foreach (var property in properties)
				{
					var attribute = Attribute.GetCustomAttribute(property, typeof(CsvColumnAttribute)) as CsvColumnAttribute;
					if (attribute != null)
					{
						object value = property.GetValue(row, null);
						this.Rows[index].Add(value);
					}
				}
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			string separator = this.Separator ?? ",";

			if (this.Columns == null || this.Columns.Count == 0)
			{
				throw new Exception("No CSV columns defined!");
			}

			//Build columns
			foreach (string column in this.Columns)
			{
				builder.Append(Escape(column)).Append(separator);
			}

			//Remove last ',' and add new line
			builder.Remove(builder.Length - separator.Length, separator.Length).AppendLine();

			foreach (var row in this.Rows)
			{
				foreach (var item in row)
				{
					string value = Convert.ToString(item, CultureInfo.InvariantCulture);
					builder.Append(Escape(value)).Append(separator);
				}

				//Remove last ',' and add new line
				builder.Remove(builder.Length - separator.Length, separator.Length).AppendLine();
			}

			//Remove last new line character(s)
			builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);

			return builder.ToString();
		}
		#endregion

		#region Protected methods
		protected string Escape(string input)
		{
			string result = input ?? "";
			if (this.AlwaysEscapeValue || result.Contains("\"") || result.Contains(this.Separator))
			{
				result = "\"" + result.Replace("\"", "\"\"") + "\"";
			}
			return result;
		}
		#endregion
	}
}
