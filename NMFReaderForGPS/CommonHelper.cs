using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMFReaderForGPS
{
	public class CommonHelper
	{
		private static string Connection { get { return ConfigurationManager.ConnectionStrings["MySQLConnection"].ToString(); } }


	}
}
