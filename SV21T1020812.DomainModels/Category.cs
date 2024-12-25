using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020742.DomainModels
{
	public class Category
	{
		public int CategoryID { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public string Description {  get; set; } = string.Empty;
	}
}
