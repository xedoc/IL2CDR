﻿using System.Text.RegularExpressions;

namespace IL2CDR.Model
{
	public static class Re
	{
		public static string GetSubString(string input, string re)
		{
			if (input == null) {
				return null;
			}

			var match = Regex.Match(input, re, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if (!match.Success) {
				return null;
			}

			if (match.Groups.Count <= 1) {
				return null;
			}

			var result = match.Groups[1].Value;

			return string.IsNullOrEmpty(result) ? null : result;
		}

		public static bool IsMatch(string input, string pattern)
		{
			if (input != null) {
				return Regex.IsMatch(input, pattern);
			} else if (pattern == null) {
				return true;
			} else {
				return false;
			}
		}
	}
}