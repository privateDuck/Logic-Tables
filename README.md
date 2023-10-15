# Logic-Tables
A CLI application that generates truth tables for logical expressions

## How does it work?
* It uses a simple Shunting Yard algorithm to parse the given expression and turns it into a set of RPN expressions. (Reversed Polish Notation)
* Then it evaluates the given expression to generate the truth table.

## How do I use it?
* It supports all the basic logical operators. namely,
  * NOT (Negation ¬ / ~)
  * AND (Conjunction ∧)
  * OR (Disjunction ∨)
  * IMPL (Implication ⇒)
  * BICOND/IFF (Biconditional ⇔)
* To input an expression, type it as if you're writing it, using the above keywords as the operators.
  * e.g:
   * (a AND b) IMPL c ≡ a ∧ b ⇒ c
   * NOT a IFF b ≡ (¬a) ⇔ b
* Please note that the operands must always be lowercase and operators must always be uppercase.

![alt text](https://github.com/privateDuck/Logic-Tables/blob/main/lgtb_prv.jpg "Logo Title Text 1")
