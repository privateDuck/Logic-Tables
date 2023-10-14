using System;
using System.Collections.Generic;

namespace LogicTables
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("\nInput Expression: ");
			string input_str = Console.ReadLine();
			var tokens = ShuntingYard.Parse(input_str);

			PostFixEvalution.Evaluate(ref tokens.Item1,ref tokens.Item2, ref tokens.Item3);

			Console.ReadKey();
		}
	}
}
