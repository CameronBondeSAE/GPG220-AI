using System;
using System.Collections.Generic;
using System.Reflection;

namespace JMiles42 {
	public static class CSharpExtensions
	{
		public static bool IsEmpty<T>(this List<T> list) { return list.Count == 0; }
		public static bool NotEmpty<T>(this List<T> list) { return list.Count > 0; }

		public static void Trigger(this Action action) {
			if (action != null) action();
		}

		public static void Trigger<TA>(this Action<TA> action, TA valueA) {
			if (action != null) action(valueA);
		}

		public static void Trigger<TA, TB>(this Action<TA, TB> action, TA valueA, TB valueB) {
			if (action != null) action(valueA, valueB);
		}

		public static void Trigger<TA, TB, TC>(this Action<TA, TB, TC> action, TA valueA, TB valueB, TC valueC) {
			if (action != null) action(valueA, valueB, valueC);
		}

		public static void Trigger<TA, TB, TC, TD>(this Action<TA, TB, TC, TD> action, TA valueA, TB valueB, TC valueC, TD valueD) {
			if (action != null) action(valueA, valueB, valueC, valueD);
		}

		public static T GetNextItemAndReAddItToTheEnd<T>(this Queue<T> queue) {
			//Get first
			var item = queue.Dequeue();
			//Re add
			queue.Enqueue(item);
			return item;
		}

		public static bool DoesStringHaveInvalidCharsOrWhiteSpace(this string str) {
			return string.IsNullOrEmpty(str) ||
				   str.Contains("-") ||
				   str.Contains(" ") ||
				   str.Contains("\n") ||
				   str.Contains("\t") ||
				   str.Contains("=") ||
				   str.Contains("+") ||
				   str.Contains("{") ||
				   str.Contains("}") ||
				   str.Contains("[") ||
				   str.Contains("]") ||
				   str.Contains("\"") ||
				   str.Contains("'") ||
				   str.Contains("?") ||
				   str.Contains(".") ||
				   str.Contains(">") ||
				   str.Contains("<") ||
				   str.Contains(",") ||
				   str.Contains("/") ||
				   str.Contains("\\") ||
				   str.Contains("|") ||
				   str.Contains(")") ||
				   str.Contains("(") ||
				   str.Contains("*") ||
				   str.Contains("&") ||
				   str.Contains("^") ||
				   str.Contains("%") ||
				   str.Contains("$") ||
				   str.Contains("#") ||
				   str.Contains("@") ||
				   str.Contains("!") ||
				   str.Contains(":");
		}

		public static bool DoesStringHaveInvalidChars(this string str) {
			return string.IsNullOrEmpty(str) ||
				   str.Contains("-") ||
				   str.Contains("\n") ||
				   str.Contains("\t") ||
				   str.Contains("=") ||
				   str.Contains("+") ||
				   str.Contains("{") ||
				   str.Contains("}") ||
				   str.Contains("[") ||
				   str.Contains("]") ||
				   str.Contains("\"") ||
				   str.Contains("'") ||
				   str.Contains("?") ||
				   str.Contains(".") ||
				   str.Contains(">") ||
				   str.Contains("<") ||
				   str.Contains(",") ||
				   str.Contains("/") ||
				   str.Contains("\\") ||
				   str.Contains("|") ||
				   str.Contains(")") ||
				   str.Contains("(") ||
				   str.Contains("*") ||
				   str.Contains("&") ||
				   str.Contains("^") ||
				   str.Contains("%") ||
				   str.Contains("$") ||
				   str.Contains("#") ||
				   str.Contains("@") ||
				   str.Contains("!") ||
				   str.Contains(":");
		}

		public static string ReplaceStringHaveInvalidCharsOrWhiteSpace(this string str,string replaceChar = "") {
			str = str.Replace("\n", replaceChar);
			str = str.Replace("\t", replaceChar);
			str = str.Replace("\"", replaceChar);
			str = str.Replace("\\", replaceChar);
			str = str.Replace("=", replaceChar);
			str = str.Replace("-", replaceChar);
			str = str.Replace("+", replaceChar);
			str = str.Replace("{", replaceChar);
			str = str.Replace("}", replaceChar);
			str = str.Replace("[", replaceChar);
			str = str.Replace("]", replaceChar);
			str = str.Replace("'", replaceChar);
			str = str.Replace("?", replaceChar);
			str = str.Replace(".", replaceChar);
			str = str.Replace(">", replaceChar);
			str = str.Replace("<", replaceChar);
			str = str.Replace(",", replaceChar);
			str = str.Replace("/", replaceChar);
			str = str.Replace("|", replaceChar);
			str = str.Replace(")", replaceChar);
			str = str.Replace("(", replaceChar);
			str = str.Replace("*", replaceChar);
			str = str.Replace("&", replaceChar);
			str = str.Replace("^", replaceChar);
			str = str.Replace("%", replaceChar);
			str = str.Replace("$", replaceChar);
			str = str.Replace("#", replaceChar);
			str = str.Replace("@", replaceChar);
			str = str.Replace("!", replaceChar);
			str = str.Replace(":", replaceChar);
			str = str.Replace(" ", replaceChar);
			return str;
		}

		public static string ReplaceStringHaveInvalidChars(this string str, string replaceChar = "") {
			str = str.Replace("\n", replaceChar);
			str = str.Replace("\t", replaceChar);
			str = str.Replace("\"", replaceChar);
			str = str.Replace("\\", replaceChar);
			str = str.Replace("=", replaceChar);
			str = str.Replace("-", replaceChar);
			str = str.Replace("+", replaceChar);
			str = str.Replace("{", replaceChar);
			str = str.Replace("}", replaceChar);
			str = str.Replace("[", replaceChar);
			str = str.Replace("]", replaceChar);
			str = str.Replace("'", replaceChar);
			str = str.Replace("?", replaceChar);
			str = str.Replace(".", replaceChar);
			str = str.Replace(">", replaceChar);
			str = str.Replace("<", replaceChar);
			str = str.Replace(",", replaceChar);
			str = str.Replace("/", replaceChar);
			str = str.Replace("|", replaceChar);
			str = str.Replace(")", replaceChar);
			str = str.Replace("(", replaceChar);
			str = str.Replace("*", replaceChar);
			str = str.Replace("&", replaceChar);
			str = str.Replace("^", replaceChar);
			str = str.Replace("%", replaceChar);
			str = str.Replace("$", replaceChar);
			str = str.Replace("#", replaceChar);
			str = str.Replace("@", replaceChar);
			str = str.Replace("!", replaceChar);
			str = str.Replace(":", replaceChar);
			return str;
		}
	}

	public static class MethodInfoUtil {
		public static bool IsOverride(this MethodInfo methodInfo) {
			return (methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType);
		}
		public static bool IsMethodFromType<T>(this MethodInfo methodInfo) {
			return (methodInfo.GetBaseDefinition().DeclaringType == typeof(T));
		}
	}
}