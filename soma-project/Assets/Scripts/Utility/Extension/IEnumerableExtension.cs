using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace SOMA
{
	public static class IEnumerableExtension
	{
		public static bool IsEmpty<TSource>(this IEnumerable<TSource> source) => !source.Any();

		// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Chunk.cs
		// Split the elements of a sequence into chunks of size at most size.
		public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
		{
			if (source is null)
			{
				throw new ArgumentNullException("source is null");
			}

			if (size < 1)
			{
				throw new ArgumentOutOfRangeException("size is below 1");
			}

			// using declaration, auto dispose
			using IEnumerator<TSource> e = source.GetEnumerator();

			// Before allocating anything, make sure there's at least one element.
			if (e.MoveNext())
			{
				// Now that we know we have at least one item, allocate an initial storage array. This is not
				// the array we'll yield.  It starts out small in order to avoid significantly overallocating
				// when the source has many fewer elements than the chunk size.
				var array = new TSource[Math.Min(size, 4)];
				int i;
				do
				{
					// Store the first item.
					array[0] = e.Current;
					i = 1;

					if (size != array.Length)
					{
						// This is the first chunk. As we fill the array, grow it as needed.
						for (; i < size && e.MoveNext(); i++)
						{
							if (i >= array.Length)
							{
								Array.Resize(ref array, Math.Min(size, 2 * array.Length));
							}

							array[i] = e.Current;
						}
					}
					else
					{
						// For all but the first chunk, the array will already be correctly sized.
						// We can just store into it until either it's full or MoveNext returns false.
						TSource[] local = array; // avoid bounds checks by using cached local (`array` is lifted to iterator object as a field)
						for (; i < local.Length && e.MoveNext(); i++)
						{
							local[i] = e.Current;
						}
					}

					// Extract the chunk array to yield, then clear out any references in our storage so that we don't keep
					// objects alive longer than needed (the caller might clear out elements from the yielded array.)
					var chunk = new TSource[i];
					Array.Copy(array, 0, chunk, 0, i);
					if (RuntimeHelpers.IsReferenceOrContainsReferences<TSource>())
					{
						Array.Clear(array, 0, i);
					}
					yield return chunk;
				}
				while (i >= size && e.MoveNext());
			}
		}

		// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Max.cs
		// Returns the maximum value in a generic sequence according to a specified key selector function.
		//
		// Returns null if source is empty and TSource is reference type
		// Ignores null keys, however returns first element if all keys are null
		public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
		{
			if (source is null)
			{
				throw new ArgumentNullException("source is null");
			}

			if (keySelector is null)
			{
				throw new ArgumentNullException("keySelector is null");
			}

			using IEnumerator<TSource> e = source.GetEnumerator();

			if (!e.MoveNext())
			{
				if (default(TSource) is null)
				{
					return default;
				}
				else
				{
					throw new InvalidOperationException("source is empty");
				}
			}

			TSource value = e.Current;
			TKey key = keySelector(value);

			if (default(TKey) is null)
			{
				if (key is null)
				{
					TSource firstValue = value;

					do
					{
						if (!e.MoveNext())
						{
							// All keys are null, surface the first element.
							return firstValue;
						}

						value = e.Current;
						key = keySelector(value);
					}
					while (key is null);
				}

				while (e.MoveNext())
				{
					TSource nextValue = e.Current;
					TKey nextKey = keySelector(nextValue);
					if (nextKey?.CompareTo(key) > 0)
					{
						key = nextKey;
						value = nextValue;
					}
				}
			}
			else
			{
				while (e.MoveNext())
				{
					TSource nextValue = e.Current;
					TKey nextKey = keySelector(nextValue);
					if (nextKey.CompareTo(key) > 0)
					{
						key = nextKey;
						value = nextValue;
					}
				}
			}

			return value;
		}
	}
}
