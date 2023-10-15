using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicTables
{
	public class Map
	{
		public bool val;
		public char name;
		public Map(char name, bool value)
		{
			this.val = value;
			this.name = name;
		}

		public void SetValue(bool value)
		{
			val = value;
		}
	}

	public class Token
	{
		public bool is_val;
		public Operator op;
		public bool value;
		public char name;
		public Token(Operator opr, char name)
		{
			is_val = opr == Operator.NONE ? true : false;
			op = opr;
			value = false;
			this.name = name;
		}

		public Token(Operator opr)
		{
			is_val = opr == Operator.NONE ? true : false;
			op = opr;
			value = false;
			this.name = '\0';
		}

		public override string ToString()
		{
			if (op == Operator.NONE) return " " + name + " ";
			else return " " + op.ToString() + " ";
		}

		public bool is_operator() => (op == Operator.AND || op == Operator.OR || op == Operator.IMPL || op == Operator.BICOND);

		public bool is_num() => is_val;

		public bool left_assoc() => op != Operator.IMPL;

		public void check_map(ref List<Map> maps)
		{
			foreach (Map map in maps)
			{
				if(map.name == name)
				{
					value = map.val;
				}
			}
		}

		public void SetValue(bool val)
		{
			value = val;
		}
		public static bool operator >(in Token tk1, in Token tk2) => tk1.op > tk2.op;
		public static bool operator >=(in Token tk1, in Token tk2) => tk1.op >= tk2.op;
		public static bool operator <(in Token tk1, in Token tk2) => tk1.op < tk2.op;
		public static bool operator <=(in Token tk1, in Token tk2) => tk1.op <= tk2.op;

		public static bool operator ==(in Token tk1, in Token tk2) => tk1.op == tk2.op;
		public static bool operator !=(in Token tk1, in Token tk2) => tk1.op != tk2.op;

	}

	public enum Operator
	{
		NONE = 0,
		NOT = 20,
		AND = 10,
		OR = 5,
		IMPL = 3,
		BICOND = 1,
		LP = 100,
		RP = 200
	}

	public enum Precedence { 
		Same = 0,
		High,
		Low
	}

	public static class ShuntingYard
	{
		public static void Parse(string str, out List<Token> RPN, out List<Map> operands, out List<List<Token>> sub_RPNs)
		{
			List<Token> tokens;
			List<Map> maps;
			tokenizer(str, out tokens, out maps);

			var RPNStack = shunt(tokens);
			RPN = RPNStack.ToList();
			operands = maps;

			sub_RPNs = new List<List<Token>>();
			RPN.Reverse();
			int num_operators = 0;
			RPN.ForEach(delegate(Token t) { if (t.is_operator() || t.op == Operator.NOT) num_operators++; });
			int found_op = 0;

			for (int o = 0; o < num_operators - 1; o++)
			{
				sub_RPNs.Add(new List<Token>());
				for (int i = 0; i < RPN.Count; i++)
				{
					sub_RPNs[sub_RPNs.Count - 1].Add(RPN[i]);
					if (RPN[i].is_operator() || RPN[i].op == Operator.NOT)
					{
						if (found_op >= o)
							break;
						found_op++;
					}
				}
			}

			Console.Write("\n\nRPN:");
			RPN.ForEach(delegate (Token t) { Console.Write(t); });
		}

		private static Stack<Token> shunt(List<Token> tokens)
		{
			Stack<Token> output = new Stack<Token>();
			Stack<Token> operators = new Stack<Token>();

			foreach (Token token in tokens)
			{
				if (token.is_val)
				{
					output.Push(token);
				}
				else if (token.op == Operator.NOT)
					operators.Push(token);
				else if (token.op == Operator.LP)
					operators.Push(token);
				else if(token.op == Operator.RP)
				{
					while (operators.Count > 0)
					{
						var x = operators.Pop();
						if (x.op != Operator.LP)
						{
							output.Push(x);
						}
						else if (x.op == Operator.LP)
						{
							break;
						}
						else if (operators.Count == 0) throw new Exception("Incomplete parentheses found");
					}
				}
				else if (token.is_operator())
				{
					if (operators.Count > 0)
					{
						while (token.left_assoc() && (token <= operators.Peek()) && operators.Peek().op != Operator.LP)
						{
							if (operators.Count == 0) break;
							var o2_tkn = operators.Pop();
							output.Push(o2_tkn);
							if (operators.Count == 0) break;
							if (!(token.left_assoc() && (token <= operators.Peek())))
							{
								break;
							}
						}
					}
					operators.Push(token);
				}
			}

			while (operators.Count > 0)
			{
				Token tkn = operators.Pop();
				if(tkn.op != Operator.LP)
				{
					output.Push(tkn);
				}
			}

			return output;
		}

		public static void tokenizer(string str, out List<Token> tokens, out List<Map> maps)
		{
			tokens = new List<Token>();
			maps = new List<Map>();

			for (int i = 0; i < str.Length; i++)
			{
				if (!char.IsWhiteSpace(str[i]))
				{
					if (str[i] == '(')
						tokens.Add(new Token(Operator.LP));

					if (str[i] == ')')
						tokens.Add(new Token(Operator.RP));

					if (char.IsLower(str[i]))
					{
						tokens.Add(new Token(Operator.NONE, str[i]));
						bool contains = false;
						for (int t = 0; t < maps.Count; t++)
						{
							if (maps[t].name == str[i])
								contains |= true;
						}
						if(!contains)
							maps.Add(new Map(str[i], false));
					}

					if (char.IsUpper(str[i]))
					{
						if (str[i] == 'N')
						{
							tokens.Add(new Token(Operator.NOT));
							i += 3;
						}

						else if (str[i] == 'A')
						{
							tokens.Add(new Token(Operator.AND));
							i += 3;
						}

						else if (str[i] == 'O')
						{
							tokens.Add(new Token(Operator.OR));
							i += 2;
						}

						else if (str[i] == 'I' && str[i + 1] == 'M')
						{
							tokens.Add(new Token(Operator.IMPL));
							i += 4;
						}

						else if (str[i] == 'B' | (str[i] == 'I' && str[i + 1] == 'F'))
						{
							tokens.Add(new Token(Operator.BICOND));
							i += str[i] == 'B' ? 6 : 3;
						}
					}
				}
			}
		}
	}
}
