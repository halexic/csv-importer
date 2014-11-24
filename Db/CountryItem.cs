using System;
using System.Runtime.Serialization;
using CsvImporter.Common;
using System.Data.Linq;

namespace CsvImporter.Db
{
	[DataContract(Name = "Country", Namespace = "")]
	public partial class CountryItem
	{
		[CsvColumn("LanguageId")]
		public int? LanguageId { get; set; }

		[CsvColumn("CountryId")]
		public int? CountryId { get; set; }

		[CsvColumn("Name")]
		public string Name { get; set; }

		[CsvColumn("Capital")]
		public string Capital { get; set; }

		[CsvColumn("Population")]
		public long? Population { get; set; }

		[CsvColumn("SquareArea")]
		public int? SquareArea { get; set; }

		[CsvColumn("DensityPerKm")]
		public decimal? DensityPerKm { get; set; }
	}
}
