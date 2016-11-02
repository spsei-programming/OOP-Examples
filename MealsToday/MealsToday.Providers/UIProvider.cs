using System;
using System.Collections.Generic;
using MealsToday.Data;
using MealsToday.Data.Enums;

namespace MealsToday.Providers
{
	public class UIProvider
	{
		public void DisplayMainMenu()
		{
			Console.WriteLine("1. Show meals");
		}

		public void DisplayMeals(List<Meal> meals)
		{
			foreach (Meal meal in meals)
			{
				Console.WriteLine($"{meal.Id}\t\t{meal.Name}");
			}
		}

		public Actions ReadMainMenuInput()
		{
			Console.Write("Next action: ");
			string key = Console.ReadLine();

			switch (key.ToLower())
			{
				case "exit":
					return Actions.Exit;
				case "1":
					return Actions.ShowMeals;
			}

			return Actions.None;
		}
	}
}