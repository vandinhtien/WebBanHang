using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020742.DataLayers
{
	public interface ISimpleQueryDAL<T> where T : class
	{
		List<T> List();
	}
}
