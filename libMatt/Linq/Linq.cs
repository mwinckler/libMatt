using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libMatt.Linq {
	public static class LinqExtensions {

		/// <summary>
		/// Creates a SortedList(TKey, TValue) from an IEnumerable(T) according to the specified 
		/// key selector and element selector functions.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="source"></param>
		/// <param name="keySelector"></param>
		/// <param name="elementSelector"></param>
		/// <returns></returns>
		public static SortedList<TKey, TValue> ToSortedList<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector) {
			if (null == keySelector) {
				throw new ArgumentNullException("keySelector", "You must supply a keySelector function.");
			} else if (null == elementSelector) {
				throw new ArgumentNullException("elementSelector", "You must supply an elementSelector function.");
			} else {
				SortedList<TKey, TValue> ret = new SortedList<TKey, TValue>();
				foreach (var item in source) {
					ret.Add(keySelector(item), elementSelector(item));
				}
				return ret;
			}
		}


		/// <summary>
		/// Creates a HashSet(T) from the source IEnumerable(T).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) {
			HashSet<T> ret = new HashSet<T>();
			foreach (T item in source) {
				ret.Add(item);
			}
			return ret;
		}

		/// <summary>
		/// Returns true if <c>source</c> contains all values in <c>values</c>, else returns false.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values) {
			if (source == null || values == null)
				return false;

			bool ret = true;
			foreach (T item in values) {
				ret = ret && source.Contains(item);
			}
			return ret;
		}

		
		/// <summary>
		/// Removes duplicate entries from the IEnumerable based on the delegate you provide.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer) {
			return source.Distinct(new DelegateComparer<T>(comparer));
		}

		/// <summary>
		/// Removes duplicate entries from the IEnumerable based on the delegate you provide, using the hashMethod you provide.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer, Func<T, int> hashMethod) {
			return source.Distinct(new DelegateComparer<T>(comparer, hashMethod));
		}



		/// <summary>
		/// Y fixed-point combinator for anonymous recursion (e.g. from linq).
		/// </summary>
		/// <remarks>
		/// <p>From http://blogs.msdn.com/wesdyer/archive/2007/02/02/anonymous-recursion-in-c.aspx;
		/// this is the fixed version found in the comments, http://blogs.msdn.com/wesdyer/archive/2007/02/02/anonymous-recursion-in-c.aspx#1689605</p>
		/// 
		/// <p>For some decent examples of usage, see:
		/// <ul>
		///		<li>http://bugsquash.blogspot.com/2008/07/y-combinator-and-linq.html</li>
		///		<li>http://mikehadlow.blogspot.com/2009/03/recursive-linq-with-y-combinator.html</li>
		/// </ul>
		/// </p>
		/// 
		/// <p>For more info on the Y combinator:
		/// <ul>
		///		<li>http://www.dreamsongs.com/NewFiles/WhyOfY.pdf</li>
		///		<li>http://en.wikipedia.org/wiki/Fixed_point_combinator</li>
		/// </ul>
		/// </p>
		/// </remarks>
		/// <typeparam name="A">The argument type.</typeparam>
		/// <typeparam name="R">The return type.</typeparam>
		/// <param name="f">The function to call when recursing</param>
		/// <returns></returns>
		public static Func<A, R> Y<A, R>(Func<Func<A, R>, Func<A, R>> f) {
			Func<A, R> g = null;
			g = f(a => g(a));
			return g;
		}

	}


	public class DelegateComparer<T> : IEqualityComparer<T> {
		private Func<T, T, bool> _equals;
		private Func<T, int> _getHashCode;

		public DelegateComparer(Func<T, T, bool> equals) {
			this._equals = equals;
		}

		public DelegateComparer(Func<T, T, bool> equals, Func<T, int> getHashCode) {
			this._equals = equals;
			this._getHashCode = getHashCode;
		}

		public bool Equals(T a, T b) {
			return _equals(a, b);
		}

		public int GetHashCode(T a) {
			if (_getHashCode != null)
				return _getHashCode(a);
			else
				return a.GetHashCode();
		}
	}

}
