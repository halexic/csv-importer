using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CsvImporter.Common;
using CsvImporter.Db;

namespace CsvImporter
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					string csv = File.ReadAllText(ofd.FileName, Encoding.UTF8);
					var countries = CsvHelper.Deserialize<CountryItem>(csv);
					int totalRows = countries.Count();
					DatabaseDataContext db = new DatabaseDataContext();
					foreach (var country in countries)
					{
						var dbCountry = db.Countries.FirstOrDefault(x => x.CountryId == country.CountryId);
						if (dbCountry == null)
						{
							dbCountry = new Country();
							db.Countries.InsertOnSubmit(dbCountry);
						}
						dbCountry.LanguageId = country.LanguageId;
						dbCountry.CountryId = country.CountryId;
						dbCountry.Name = country.Name;
						dbCountry.Capital = country.Capital;
						dbCountry.Population = country.Population;
						dbCountry.SquareArea = country.SquareArea;
						dbCountry.DensityPerKm = country.DensityPerKm;
					}
					db.SubmitChanges();
					lblResult.Text = string.Format("Total imported rows: {0}", totalRows);
				}				
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
