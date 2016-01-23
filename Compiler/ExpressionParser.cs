﻿using System;
using System.Collections.Generic;
using Crayon.ParseTree;

namespace Crayon
{
	internal static class ExpressionParser
	{
		public static Expression Parse(TokenStream tokens)
		{
			Dictionary<string, Annotation> annotations = null;
			if (tokens.IsNext("@"))
			{
				annotations = new Dictionary<string, Annotation>();
				while (tokens.IsNext("@"))
				{
					Annotation annotation = AnnotationParser.ParseAnnotation(tokens);
					annotations[annotation.Type] = annotation;
				}
			}
			Expression output = ParseTernary(tokens);
			output.Annotations = annotations;
			return output;
		}

		private static Expression ParseTernary(TokenStream tokens)
		{
			Expression root = ParseNullCoalescing(tokens);
			if (tokens.PopIfPresent("?"))
			{
				Expression trueExpr = ParseTernary(tokens);
				tokens.PopExpected(":");
				Expression falseExpr = ParseTernary(tokens);

				return new Ternary(root, trueExpr, falseExpr);
			}
			return root;
		}

		private static Expression ParseNullCoalescing(TokenStream tokens)
		{
			Expression root = ParseBooleanCombination(tokens);
			if (tokens.PopIfPresent("??"))
			{
				Expression secondaryExpression = ParseNullCoalescing(tokens);
				return new NullCoalescer(root, secondaryExpression);
			}
			return root;
		}

		private static Expression ParseBooleanCombination(TokenStream tokens)
		{
			Expression expr = ParseBitwiseOp(tokens);
			string next = tokens.PeekValue();
			if (next == "||" || next == "&&")
			{
				List<Expression> expressions = new List<Expression>() { expr };
				List<Token> ops = new List<Token>();
				while (next == "||" || next == "&&")
				{
					ops.Add(tokens.Pop());
					expressions.Add(ParseBitwiseOp(tokens));
					next = tokens.PeekValue();
				}
				return new BooleanCombination(expressions, ops);
			}
			return expr;
		}

		private static Expression ParseBitwiseOp(TokenStream tokens)
		{
			Expression expr = ParseEqualityComparison(tokens);
			string next = tokens.PeekValue();
			if (next == "|" || next == "&" || next == "^")
			{
				Token bitwiseToken = tokens.Pop();
				Expression rightExpr = ParseBitwiseOp(tokens);
				return new BinaryOpChain(expr, bitwiseToken, rightExpr);
			}
			return expr;
		}

		private static Expression ParseEqualityComparison(TokenStream tokens)
		{
			Expression expr = ParseInequalityComparison(tokens);
			string next = tokens.PeekValue();
			if (next == "==" || next == "!=")
			{
				Token equalityToken = tokens.Pop();
				Expression rightExpr = ParseEqualityComparison(tokens);
				return new BinaryOpChain(expr, equalityToken, rightExpr);
			}
			return expr;
		}

		private static Expression ParseInequalityComparison(TokenStream tokens)
		{
			Expression expr = ParseBitShift(tokens);
			string next = tokens.PeekValue();
			if (next == "<" || next == ">" || next == "<=" || next == ">=")
			{
				// Don't allow chaining of inqeualities
				Token opToken = tokens.Pop();
				Expression rightExpr = ParseBitShift(tokens);
				return new BinaryOpChain(expr, opToken, rightExpr);
			}
			return expr;
		}

		private static Expression ParseBitShift(TokenStream tokens)
		{
			Expression expr = ParseAddition(tokens);
			string next = tokens.PeekValue();
			if (next == "<<" || next == ">>")
			{
				Token opToken = tokens.Pop();
				Expression rightExpr = ParseBitShift(tokens);
				return new BinaryOpChain(expr, opToken, rightExpr);
			}
			return expr;
		}

		private static readonly HashSet<string> ADDITION_OPS = new HashSet<string>("+ -".Split(' '));
		private static readonly HashSet<string> MULTIPLICATION_OPS = new HashSet<string>("* / %".Split(' '));
		private static readonly HashSet<string> NEGATE_OPS = new HashSet<string>("! -".Split(' '));

		private static Expression ParseAddition(TokenStream tokens)
		{
			Expression expr = ParseMultiplication(tokens);
			string next = tokens.PeekValue();
			while (ADDITION_OPS.Contains(next))
			{
				Token op = tokens.Pop();
				Expression right = ParseMultiplication(tokens);
				expr = new BinaryOpChain(expr, op, right);
				next = tokens.PeekValue();
			}
			return expr;
		}

		private static Expression ParseMultiplication(TokenStream tokens)
		{
			Expression expr = ParseNegate(tokens);
			string next = tokens.PeekValue();
			while (MULTIPLICATION_OPS.Contains(next))
			{
				Token op = tokens.Pop();
				Expression right = ParseNegate(tokens);
				expr = new BinaryOpChain(expr, op, right);
				next = tokens.PeekValue();
			}
			return expr;
		}

		private static Expression ParseNegate(TokenStream tokens)
		{
			string next = tokens.PeekValue();
			if (NEGATE_OPS.Contains(next))
			{
				Token negateOp = tokens.Pop();
				Expression root = ParseNegate(tokens);
				// TODO: just make a negation parse tree node
				if (negateOp.Value == "!") return new BooleanNot(negateOp, root);
				if (negateOp.Value == "-") return new NegativeSign(negateOp, root);
				throw new Exception("This shouldn't happen.");
			}

			return ParseExponents(tokens);
		}

		private static Expression ParseExponents(TokenStream tokens)
		{
			Expression expr = ParseIncrement(tokens);
			string next = tokens.PeekValue();
			if (next == "**")
			{
				Token op = tokens.Pop();
				Expression right = ParseNegate(tokens);
				expr = new BinaryOpChain(expr, op, right);
			}
			return expr;
		}

		private static Expression ParseIncrement(TokenStream tokens)
		{
			Expression root;
			if (tokens.IsNext("++") || tokens.IsNext("--"))
			{
				Token incrementToken = tokens.Pop();
				root = ParseEntity(tokens);
				return new Increment(incrementToken, incrementToken, incrementToken.Value == "++", true, root);
			}

			root = ParseEntity(tokens);
			if (tokens.IsNext("++") || tokens.IsNext("--"))
			{
				Token incrementToken = tokens.Pop();
				return new Increment(root.FirstToken, incrementToken, incrementToken.Value == "++", false, root);
			}

			return root;
		}

		private static readonly HashSet<char> VARIABLE_STARTER = new HashSet<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$".ToCharArray());

		private static Expression ParseInstantiate(TokenStream tokens)
		{
			Token newToken = tokens.PopExpected("new");
			Token classNameToken = tokens.Pop();
			Parser.VerifyIdentifier(classNameToken);

			List<Expression> args = new List<Expression>();
			tokens.PopExpected("(");
			while (!tokens.PopIfPresent(")"))
			{
				if (args.Count > 0)
				{
					tokens.PopExpected(",");
				}
				args.Add(Parse(tokens));
			}

			return new Instantiate(newToken, classNameToken, args);
		}

		private static Expression ParseEntity(TokenStream tokens)
		{
			Expression root;
			if (tokens.PopIfPresent("("))
			{
				root = Parse(tokens);
				tokens.PopExpected(")");
			}
			else
			{
				root = ParseEntityWithoutSuffixChain(tokens);
			}
			bool anySuffixes = true;
			while (anySuffixes)
			{
				if (tokens.IsNext("."))
				{
					Token dotToken = tokens.Pop();
					Token stepToken = tokens.Pop();
					Parser.VerifyIdentifier(stepToken);
					root = new DotStep(root, dotToken, stepToken);
				}
				else if (tokens.IsNext("["))
				{
					Token openBracket = tokens.Pop();
					List<Expression> sliceComponents = new List<Expression>();
					if (tokens.IsNext(":"))
					{
						sliceComponents.Add(null);
					}
					else
					{
						sliceComponents.Add(Parse(tokens));
					}

					for (int i = 0; i < 2; ++i)
					{
						if (tokens.PopIfPresent(":"))
						{
							if (tokens.IsNext(":") || tokens.IsNext("]"))
							{
								sliceComponents.Add(null);
							}
							else
							{
								sliceComponents.Add(Parse(tokens));
							}
						}
					}

					tokens.PopExpected("]");

					if (sliceComponents.Count == 1)
					{
						Expression index = sliceComponents[0];
						root = new BracketIndex(root, openBracket, index);
					}
					else
					{
						root = new ListSlice(root, sliceComponents, openBracket);
					}
				}
				else if (tokens.IsNext("("))
				{
					Token openParen = tokens.Pop();
					List<Expression> args = new List<Expression>();
					while (!tokens.PopIfPresent(")"))
					{
						if (args.Count > 0)
						{
							tokens.PopExpected(",");
						}

						args.Add(Parse(tokens));
					}
					root = new FunctionCall(root, openParen, args);
				}
				else
				{
					anySuffixes = false;
				}
			}
			return root;
		}

		private static Expression ParseEntityWithoutSuffixChain(TokenStream tokens)
		{
			string next = tokens.PeekValue();

			if (next == "null") return new NullConstant(tokens.Pop());
			if (next == "true") return new BooleanConstant(tokens.Pop(), true);
			if (next == "false") return new BooleanConstant(tokens.Pop(), false);

			Token peekToken = tokens.Peek();
			if (next.StartsWith("'")) return new StringConstant(tokens.Pop(), StringConstant.ParseOutRawValue(peekToken));
			if (next.StartsWith("\"")) return new StringConstant(tokens.Pop(), StringConstant.ParseOutRawValue(peekToken));
			if (next == "new") return ParseInstantiate(tokens);

			char firstChar = next[0];
			if (VARIABLE_STARTER.Contains(firstChar))
			{
				Token varToken = tokens.Pop();
				return new Variable(varToken, varToken.Value);
			}

			if (firstChar == '[')
			{
				Token bracketToken = tokens.PopExpected("[");
				List<Expression> elements = new List<Expression>();
				bool previousHasCommaOrFirst = true;
				while (!tokens.PopIfPresent("]"))
				{
					if (!previousHasCommaOrFirst) tokens.PopExpected("]"); // throws appropriate error
					elements.Add(Parse(tokens));
					previousHasCommaOrFirst = tokens.PopIfPresent(",");
				}
				return new ListDefinition(bracketToken, elements);
			}

			if (firstChar == '{')
			{
				Token braceToken = tokens.PopExpected("{");
				List<Expression> keys = new List<Expression>();
				List<Expression> values = new List<Expression>();
				bool previousHasCommaOrFirst = true;
				while (!tokens.PopIfPresent("}"))
				{
					if (!previousHasCommaOrFirst) tokens.PopExpected("}"); // throws appropriate error
					keys.Add(Parse(tokens));
					tokens.PopExpected(":");
					values.Add(Parse(tokens));
					previousHasCommaOrFirst = tokens.PopIfPresent(",");
				}
				return new DictionaryDefinition(braceToken, keys, values);
			}

			if (next.Length > 2 && next.Substring(0, 2) == "0x")
			{
				Token intToken = tokens.Pop();
				int intValue = IntegerConstant.ParseIntConstant(intToken, intToken.Value);
				return new IntegerConstant(intToken, intValue);
			}

			if (Parser.IsInteger(next))
			{
				Token numberToken = tokens.Pop();
				string numberValue = numberToken.Value;

				if (tokens.IsNext("."))
				{
					Token decimalToken = tokens.Pop();
					if (decimalToken.HasWhitespacePrefix)
					{
						throw new ParserException(decimalToken, "Decimals cannot have whitespace before them.");
					}

					Token afterDecimal = tokens.Pop();
					if (afterDecimal.HasWhitespacePrefix) throw new ParserException(afterDecimal, "Cannot have whitespace after the decimal.");
					if (!Parser.IsInteger(afterDecimal.Value)) throw new ParserException(afterDecimal, "Decimal must be followed by an integer.");

					numberValue += "." + afterDecimal.Value;

					double floatValue = FloatConstant.ParseValue(numberToken, numberValue);
					return new FloatConstant(numberToken, floatValue);
				}

				int intValue = IntegerConstant.ParseIntConstant(numberToken, numberToken.Value);
				return new IntegerConstant(numberToken, intValue);
			}

			if (tokens.IsNext("."))
			{
				Token dotToken = tokens.PopExpected(".");
				string numberValue = "0.";
				Token postDecimal = tokens.Pop();
				if (postDecimal.HasWhitespacePrefix || !Parser.IsInteger(postDecimal.Value))
				{
					throw new ParserException(dotToken, "Unexpected dot.");
				}

				numberValue += postDecimal.Value;

				double floatValue;
				if (double.TryParse(numberValue, out floatValue))
				{
					return new FloatConstant(dotToken, floatValue);
				}

				throw new ParserException(dotToken, "Invalid float literal.");
			}

			throw new ParserException(tokens.Peek(), "Encountered unexpected token: '" + tokens.PeekValue() + "'");
		}
	}
}