using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CsvImporter.Db
{
	public partial class DatabaseDataContext : DatabaseDataContextBase
	{
		public DatabaseDataContext()
			: base(InitialConnectionString)
		{
		}

		public DatabaseDataContext(string connectionString)
			: base(connectionString)
		{
		}

		private static string initialConnectionString = null;

		public static string InitialConnectionString
		{
			get
			{
				if (!string.IsNullOrEmpty(initialConnectionString))
				{
					return initialConnectionString;
				}
				else
				{
					var config = ConfigurationManager.ConnectionStrings["DbConnectionString"];
					if (config == null || config.ConnectionString == null)
					{
						throw new Exception("Missing connection string: DbConnectionString!", new ArgumentNullException("DbConnectionString"));
					}
					return config.ConnectionString;
				}
			}
			set
			{
				initialConnectionString = value;
			}
		}
	}
}
