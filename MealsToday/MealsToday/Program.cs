using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealsToday.Data.Enums;
using MealsToday.Providers;

namespace MealsToday
{
	class Program
	{
		static void Main(string[] args)
		{
			MealsProvider mealsProvider = new MealsProvider();
			UIProvider uiProvider = new UIProvider();

			var defaultMeals = mealsProvider.GetDefaultMeals();

			uiProvider.DisplayMainMenu();
			var nextAction = uiProvider.ReadMainMenuInput();

			switch (nextAction)
			{
				case Actions.ShowMeals:
					uiProvider.DisplayMeals(defaultMeals);
					break;
				case Actions.PlaceOrderForToday:
					break;
				case Actions.PlaceOrderForTomorrow:
					break;
				case Actions.ShowOrdersForToday:
					break;
				case Actions.Exit:
					break;
				case Actions.None:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			
		}
	}
}
