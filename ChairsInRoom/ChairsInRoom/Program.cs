using System;
using System.Collections.Generic;

namespace ChairsInRoom
{
	class Program
	{
		static void Main(string[] args)
		{
			Chair firstChair = new Chair();
			firstChair.Color = "oranzova";
			firstChair.LegCount = 4;
			firstChair.Back = Chair.BackType.Rounded;

			firstChair.PullChair();

			Console.WriteLine($"First chair is {firstChair.State}");

			Chair secondChair = new Chair();
			secondChair.Color = "Light biege";
			secondChair.Back = Chair.BackType.Rectangle;
			secondChair.LegCount = 4;

			// note 50 does not create an array of 50 chairs, it just prepares the list for 50 items
			List<Chair> allChairsInRoom = new List<Chair>(50);

			allChairsInRoom.Add(firstChair);
			allChairsInRoom.Add(secondChair);

			Table firstTable = new Table();
			firstTable.LegCount = 6;
			firstTable.HasPrivacyShield = true;
			firstTable.Color = "Dark Brown";

			foreach (Chair chair in allChairsInRoom)
			{
				Console.WriteLine($"Chair has color {chair.Color}, back is of type {chair.Back} and leg count is {chair.LegCount}");
			}

		}

		public class Furniture
		{
			public string Color;
			public int LegCount;
		}

		public class Chair : Furniture
		{
			public enum BackType { Rounded, Circle, Rectangle}
			public enum ChairState { PushedUnderTable, PulledOut, RestingOnTable}

			public BackType Back;
			public ChairState State = ChairState.PushedUnderTable;


			public void PushChair()
			{
				// chair can be pushed under table only if it's pulled out, 
				// if it's pushed already or resting on table there is no sense to continue 
				// this method
				if (State != ChairState.PulledOut)
					return;

				State = ChairState.PushedUnderTable;
			}

			public void PullChair()
			{
				// chair can be pulled out from under the table only if it's pushed under it, 
				// if it's pulled out already or resting on table there is no sense to continue 
				// this method
				if (State != ChairState.PushedUnderTable)
					return;
				State = ChairState.PulledOut;
			}
		}

		public class Table : Furniture
		{
			public bool HasPrivacyShield;
		}
	}
}
