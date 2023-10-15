using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicTables
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("\nInput Expression: ");
			string input_str = Console.ReadLine();
			if (input_str == string.Empty) return;

			List<Token> RPN;
			List<Map> operands;
			List<List<Token>> sub_RPNs;

			ShuntingYard.Parse(input_str,out RPN, out operands, out sub_RPNs);
			PostFixEvalution.Evaluate(ref RPN, ref operands, ref sub_RPNs);

			Console.ReadKey();
		}
	}
}
