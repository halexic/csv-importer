using System;

namespace CsvImporter.Common
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class CsvColumnAttribute : Attribute
	{
		public CsvColumnAttribute(string name)
		{
			this.Name = name;
		}

		public string Name
		{
			get;
			set;
		}
	}
}
