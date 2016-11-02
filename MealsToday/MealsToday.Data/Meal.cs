using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealsToday.Data
{
	public class Meal
	{
		public int Id { get; set; }
		public string Name { get; set; }
		
		public int Kalories { get; set; }
		public List<byte> Alergens { get; private set; }

		public Meal()
		{
			Alergens = new List<byte>(10);
		}
	}
}
