using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Media3D;

namespace IL2CDR.Model
{
	public static class Collections
	{
		public static void RemoveAll<T>(this ObservableCollection<T> list, Func<T, bool> match) where T : class
		{
			var removeItem = list.With(x => list)
				.With(x => list.FirstOrDefault(match));

			while (removeItem != null) {
				list.Remove(removeItem);
				removeItem = list.FirstOrDefault(match);
			}
		}

		public static Guid GetGuid(this Dictionary<string, string> dict, string name)
		{
			Guid.TryParse(dict.GetString(name), out var guid);
			return guid;
		}

		public static Vector3D GetVector3D(this Dictionary<string, string> dict, string posName)
		{
			return Util.POSToVector3D(dict.GetString(posName));
		}

		public static Vector3D[] GetVectorArray(this Dictionary<string, string> dict, string name)
		{
			return Util.BoundaryPointsToVectorCollection(dict.GetString(name));
		}

		public static int GetInt(this Dictionary<string, string> dict, string name)
		{
			var i = -1;
			int.TryParse(dict.GetString(name), out i);
			return i;
		}

		public static double GetDouble(this Dictionary<string, string> dict, string name)
		{
			double d = -1;
			double.TryParse(dict.GetString(name), NumberStyles.Any, CultureInfo.InvariantCulture, out d);
			return d;
		}

		public static string GetString(this Dictionary<string, string> dict, string name)
		{
			string value = null;
			dict.TryGetValue(name, out value);
			return value;
		}

		public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source,
			Func<T, TIdentity> identitySelector)
		{
			return source.Distinct(By(identitySelector));
		}

		public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
		{
			return new DelegateComparer<TSource, TIdentity>(identitySelector);
		}

		private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
		{
			private readonly Func<T, TIdentity> identitySelector;

			public DelegateComparer(Func<T, TIdentity> identitySelector)
			{
				this.identitySelector = identitySelector;
			}

			public bool Equals(T x, T y)
			{
				return Equals(this.identitySelector(x), this.identitySelector(y));
			}

			public int GetHashCode(T obj)
			{
				return this.identitySelector(obj).GetHashCode();
			}
		}
	}


	public class SmartCollection<T> : ObservableCollection<T>
	{
		public SmartCollection()
			: base()
		{
		}

		public SmartCollection(IEnumerable<T> collection)
			: base(collection)
		{
		}

		public SmartCollection(List<T> list)
			: base(list)
		{
		}

		public void AddRange(IEnumerable<T> range)
		{
			foreach (var item in range) {
				this.Items.Add(item);
			}

			UI.Dispatch(() => this.OnPropertyChanged(new PropertyChangedEventArgs("Count")));
			UI.Dispatch(() => this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]")));
			UI.Dispatch(() =>
				this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
		}

		public void Reset(IEnumerable<T> range)
		{
			this.Items.Clear();

			this.AddRange(range);
		}
	}
}