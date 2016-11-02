using System.Collections.Generic;
using MealsToday.Data;

namespace MealsToday.Providers
{
	public class MealsProvider
	{
		public List<Meal> GetDefaultMeals()
		{
			List<Meal> meals = new List<Meal>(5);
			var meal1 = new Meal() {
				Id = 1,
				Name = "Buchticky se sodo",
				Kalories = 3000
			};
			meal1.Alergens.Add(1);
			meal1.Alergens.Add(3);
			meal1.Alergens.Add(5);


			meals.Add(meal1);

			return meals;
		}
	}
}