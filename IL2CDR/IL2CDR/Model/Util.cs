﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Windows.Forms;

namespace IL2CDR.Model
{
	public class Util
	{
		public static DialogResult Message(string text)
		{
			return MessageBox.Show(text);
		}

		public static DialogResult Message(string text, string caption)
		{
			return MessageBox.Show(text, caption);
		}

		public static DialogResult Message(string text, string caption, MessageBoxButtons buttons)
		{
			return MessageBox.Show(text, caption, buttons);
		}

		public static Exception Try(Action action, bool logException = true)
		{
			if (action == null) {
				return null;
			}

			try {
				action();
			} catch (Exception e) {
				if (logException) {
					Log.WriteError("Exception: {0}\n{1}", e.Message, e.StackTrace);
				}

				return e;
			}

			return null;
		}

		public static string SourceFileName => new System.Diagnostics.StackTrace(true).GetFrame(4).GetFileName();

		public static string SourceLineNumber =>
			new System.Diagnostics.StackTrace(true).GetFrame(4).GetFileLineNumber().ToString();

		public static Vector3D[] BoundaryPointsToVectorCollection(string bp)
		{
			if (string.IsNullOrWhiteSpace(bp)) {
				return new Vector3D[] { };
			}

			var vectors = new List<Vector3D>();
			var matches = Regex.Matches(bp, @"\([\d|\.|,]+\)");
			foreach (Match match in matches) {
				var vector = POSToVector3D(match.Value);
				vectors.Add(vector);
			}

			return vectors.ToArray();
		}

		public static int[] SequenceToIntArray(string seq)
		{
			var result = new int[] { };
			if (string.IsNullOrWhiteSpace(seq)) {
				return result;
			}

			var values = Re.GetSubString(seq, @"([\d|\.|,]+)");
			if (string.IsNullOrWhiteSpace(values)) {
				return result;
			}

			return values.Split(',')
				.Select(x => {
					int.TryParse(x, out var i);
					return i;
				})
				.ToArray();
		}

		public static Vector3D POSToVector3D(string pos)
		{
			var result = new Vector3D();
			double x = 0.0, y = 0.0, z = 0.0;

			if (string.IsNullOrWhiteSpace(pos)) {
				return result;
			}

			var values = Re.GetSubString(pos, @"([\d|\.|,]+)");

			if (string.IsNullOrWhiteSpace(values)) {
				return result;
			}

			var xyz = values.Split(',');
			if (xyz.Length == 3) {
				x = double.Parse(xyz[0], CultureInfo.InvariantCulture);
				y = double.Parse(xyz[1], CultureInfo.InvariantCulture);
				z = double.Parse(xyz[2], CultureInfo.InvariantCulture);
			}

			return new Vector3D(x, y, z);
		}


		private static DateTime ConvertStringChunksToDatetime(string y, string m, string d, string h, string min, string sec)
		{
			return new DateTime(int.Parse(y), int.Parse(m), int.Parse(d), int.Parse(h), int.Parse(min), int.Parse(sec));
		}


		public static DateTime ParseDate(string text)
		{
			if (string.IsNullOrEmpty(text)) {
				return default(DateTime);
			}

			

			//2015-02-25_11-43-53
			var match = Regex.Match(text, @"(\d{4})-(\d{2})-(\d{2})_(\d{2})-(\d{2})-(\d{2})");
			if (match.Success && match.Groups.Count == 7) {
				return ConvertStringChunksToDatetime(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value,
													match.Groups[4].Value, match.Groups[5].Value, match.Groups[6].Value);
			}

			return default(DateTime);
		}

		private static readonly DateTimeOffset gregorianCalendarStart =
			new DateTimeOffset(1582, 10, 15, 0, 0, 0, TimeSpan.Zero);

		public static string GetNewestFilePath(string folder, string mask)
		{
			if (Directory.Exists(folder)) {
				dynamic newestFile = null;
				Try(() => {
					newestFile = Directory.GetFiles(folder, mask)
						.Select(path => new {Path = path, Time = File.GetLastWriteTime(path)})
						.OrderBy(file => file.Time).LastOrDefault();
				});
				if (newestFile != null) {
					return newestFile.Path;
				}
			}

			return null;
		}

		public static string[] GetFilesSortedByTime(string folder, string mask, bool asc)
		{
			var result = new string[] { };

			Try(() => {
				result = Directory.GetFiles(folder, mask)
					.Select(path => new {Path = path, Time = File.GetLastWriteTime(path)})
					.OrderBy(file => file.Time).Select(file => file.Path).ToArray();

				if (!asc) {
					result = result.Reverse().ToArray();
				}
			});
			return result;
		}
	}
}