using System;
using System.Collections.Generic;

namespace LogicTables
{
	public static class PostFixEvalution
	{
		public static void Evaluate(ref List<Token> tokens, ref List<Map> maps, ref List<List<Token>> sub_expr)
		{
			int count_t = (int)Math.Pow(2, maps.Count);
			string header = "";
			string[] values = new string[count_t];
			string[] expressions = new string[maps.Count + sub_expr.Count + 1];

			for (int j = 0; j < maps.Count; j++)
			{
				expressions[j] = maps[j].name.ToString();
			}

			for (int sub = 0; sub < sub_expr.Count; sub++)
			{
				eval(sub_expr[sub], ref expressions[maps.Count + sub]);
			}
			eval(tokens, ref expressions[expressions.Length - 1]);

			for (uint t = 0; t < Math.Pow(2, maps.Count); t++)
			{
				values[t] = string.Empty;
				for (int j = 0; j < maps.Count; j++)
				{
					bool value = (t & (1u << j)) != 0 ? true : false;
					maps[j].SetValue(value);
				}
				for (int j = 0; j < maps.Count; j++)
				{
					if(maps[j].val)
						values[t] += "T".PadLeft(4) + " |";
					else
						values[t] += "F".PadLeft(4) + " |";
				}

				for (int i = 0; i < tokens.Count; i++)
				{
					tokens[i].check_map(ref maps);
				}
				
				string waste = "";
				for (int sub = 0; sub < sub_expr.Count; sub++)
				{
					bool sub_r = eval(sub_expr[sub], ref waste);
					if (sub_r)
						values[t] += "T".PadLeft(4) + " |";
					else
						values[t] += "F".PadLeft(4) + " |";
				}
				bool result = eval(tokens, ref waste);

				if (result)
					values[t] += "T".PadLeft(4);
				else
					values[t] += "F".PadLeft(4);
			}

			for (int i = 0; i < maps.Count; i++)
			{
				header += maps[i].name.ToString().PadLeft(4) + " |";
			}
			for (int i = maps.Count; i < expressions.Length; i++)
			{
				header += ((char)(65 + i - maps.Count)).ToString().PadLeft(4);
				if (i < expressions.Length - 1) header += " |";
			}
			Console.Write("\n\n");
			Console.WriteLine(header);
			for (int i = 0; i < expressions.Length; i++)
			{
				Console.Write("------");
			}
			Console.Write("\n");
			for (int i = 0; i < count_t; i++)
			{
				Console.WriteLine(values[i]);
			}
			Console.Write("\n\n");
			for (int i = maps.Count; i < expressions.Length; i++)
			{
				Console.WriteLine(((char)(65 + i - maps.Count)).ToString() + $": {expressions[i]}");
			}
		}

		private static bool eval(List<Token> tokens, ref string expr)
		{
			var stack = new Stack<bool>();
			var name_stack = new Stack<string>();

			name_stack.Push(tokens[0].name.ToString());
			stack.Push(tokens[0].value);
			bool argument;

			for (int i = 0; i < tokens.Count; i++)
			{
				if (tokens[i].is_val)
				{
					stack.Push(tokens[i].value);
					name_stack.Push(tokens[i].ToString().Trim());
				}
				else if (tokens[i].is_operator())
				{
					bool right = stack.Pop();
					bool left = stack.Pop();
					string tmp_name = "";
					string right_name = name_stack.Pop();
					string left_name = name_stack.Pop();

					switch (tokens[i].op)
					{
						case Operator.AND:
							argument = left && right;
							tmp_name = left_name + " and " + right_name;
							break;
						case Operator.OR:
							argument = left || right;
							tmp_name = left_name + " or " + right_name;
							break;
						case Operator.IMPL:
							if (left == true && right == false) argument = false;
							else argument = true;
							tmp_name = left_name + " implies " + right_name;
							break;
						case Operator.BICOND:
							argument = left == right;
							tmp_name = left_name + " iff " + right_name;
							break;
						default:
							argument = false;
							break;
					}
					stack.Push(argument);
					name_stack.Push(tmp_name);
				}
				else if (tokens[i].op == Operator.NOT)
				{
					argument = !stack.Pop();
					stack.Push(argument);
					string arg_name = name_stack.Pop();
					name_stack.Push("~(" + arg_name.Trim() + ")");
				}
			}
			expr = name_stack.Pop().PadLeft(2).PadRight(2);
			return stack.Pop();
		}
	}
}
