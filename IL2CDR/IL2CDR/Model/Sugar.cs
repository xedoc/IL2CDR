﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR
{
	///
	/// http://www.codeproject.com/Articles/109026/Chained-null-checks-and-the-Maybe-monad
	/// 
	public static class Sugar
	{
		public static TResult With<TInput, TResult>(this TInput o,
			Func<TInput, TResult> evaluator)
			where TResult : class
			where TInput : class
		{
			if (o == null) {
				return null;
			}

			return evaluator(o);
		}

		public static TResult Return<TInput, TResult>(this TInput o,
			Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
		{
			if (o == null) {
				return failureValue;
			}

			return evaluator(o);
		}

		public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator)
			where TInput : class
		{
			if (o == null) {
				return null;
			}

			return evaluator(o) ? o : null;
		}

		public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator)
			where TInput : class
		{
			if (o == null) {
				return null;
			}

			return evaluator(o) ? null : o;
		}

		public static TInput Do<TInput>(this TInput o, Action<TInput> action)
			where TInput : class
		{
			if (o == null) {
				return null;
			}

			action(o);
			return o;
		}
	}

	public class LambdaComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> _lambdaComparer;
		private readonly Func<T, int> _lambdaHash;

		public LambdaComparer(Func<T, T, bool> lambdaComparer) :
			this(lambdaComparer, o => 0)
		{
		}

		public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
		{
			if (lambdaComparer == null) {
				return;
			}

			if (lambdaHash == null) {
				return;
			}

			this._lambdaComparer = lambdaComparer;
			this._lambdaHash = lambdaHash;
		}

		public bool Equals(T x, T y)
		{
			return this._lambdaComparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			return this._lambdaHash(obj);
		}
	}
}