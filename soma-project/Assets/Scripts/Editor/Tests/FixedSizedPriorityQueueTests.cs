using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using Random = System.Random;

namespace SOMA.Editor.Tests
{
	public class FixedSizedPriorityQueueTests
	{
		private const int MAGIC_VALUE = 34;
		private static Random _seedRandom;

		[MenuItem("Tools/Tests/FixedSizedPriorityQueue")]
		private static void TestFixedSizedPriorityQueue()
		{
			_seedRandom = new Random(MAGIC_VALUE);

			FSPQ_Constructor_Empty();
			FSPQ_Constructor_Exception();

			FSPQ_Enqueue();
			FSPQ_Enqueue_SamePriority();
			FSPQ_Enqueue_Overflow();
			FSPQ_Enqueue_Overflow_SamePriority();

			FSPQ_Peek();
			FSPQ_Peek_SamePriority();
			FSPQ_Peek_Overflow();
			FSPQ_Peek_Empty();

			FSPQ_Dequeue();
			FSPQ_Dequeue_Empty();

			FSPQ_EnqueueDequeue();
			FSPQ_EnqueueDequeue_Empty();

			FSPQ_Clear();
			FSPQ_Clear_Empty();

			FSPQ_ConsistentElements();
			FSPQ_CustomTest();
		}

		// Helper functions
		private static int GetRandomInt(Random random)
		{
			return random.Next();
		}

		private static string GetRandomString(Random random)
		{
			int stringLength = random.Next(5, 15);
			byte[] bytes = new byte[stringLength];
			random.NextBytes(bytes);
			return Convert.ToBase64String(bytes);
		}

		private static IEnumerable<(int Element, int Priority)> CreateIntItems(int count)
		{
			int seed = _seedRandom.Next() + MAGIC_VALUE;
			for (int i = 0; i < count; i++)
			{
				var random = new Random(seed++);
				yield return (GetRandomInt(random), GetRandomInt(random));
			}
		}

		private static IEnumerable<(string Element, int Priority)> CreateStringItems(int count)
		{
			int seed = _seedRandom.Next() + MAGIC_VALUE;
			for (int i = 0; i < count; i++)
			{
				var random = new Random(seed++);
				yield return (GetRandomString(random), GetRandomInt(random));
			}
		}

		private static bool CollectionEquals<T>(IEnumerable<T> a, IEnumerable<T> b)
		{
			var sortedA = a.OrderBy(x => x);
			var sortedB = b.OrderBy(x => x);
			return sortedA.SequenceEqual(sortedB);
		}

		private static void GetIntQueueAndData(out FixedSizedPriorityQueue<int, int> fspq, out List<(int Element, int Priority)> items, int maxSize, int itemSize)
		{
			fspq = new FixedSizedPriorityQueue<int, int>(maxSize);
			items = CreateIntItems(itemSize).ToList();
		}

		private static void GetIntQueueAndDataWithSamePriority(out FixedSizedPriorityQueue<int, int> fspq, out List<(int Element, int Priority)> items, int maxSize, int itemSize, int mod)
		{
			fspq = new FixedSizedPriorityQueue<int, int>(maxSize);
			items = (
				from item in CreateIntItems(itemSize)
				select (item.Element, item.Priority % mod)
			).ToList();
		}

		// Empty Test
		private static void FSPQ_CheckEmpty<TElement, TPriority>(FixedSizedPriorityQueue<TElement, TPriority> fspq) where TPriority : IComparable<TPriority>
		{
			Debug.Assert(fspq.Count == 0, "Empty queue's size should be zero");
			Debug.Assert(fspq.Elements.IsEmpty(), "Empty queue's Elements should be empty");
		}

		// Constructor Test
		private static void FSPQ_Constructor_Empty()
		{
			Debug.Log("Checking constructed queue is empty...");
			var fspq = new FixedSizedPriorityQueue<string, int>(5);
			FSPQ_CheckEmpty(fspq);
		}

		private static void FSPQ_Constructor_Exception()
		{
			Debug.Log("Checking constructor throws when argument out of range...");

			try
			{
				var fspq = new FixedSizedPriorityQueue<string, int>(-1);
				Debug.LogError("Constructor didn't throw exception");
			}
			catch (ArgumentOutOfRangeException) {}

			try
			{
				var fspq = new FixedSizedPriorityQueue<string, int>(0);
				Debug.LogError("Constructor didn't throw exception");
			}
			catch (ArgumentOutOfRangeException) {}

			try
			{
				var fspq = new FixedSizedPriorityQueue<string, int>(int.MinValue);
				Debug.LogError("Constructor didn't throw exception");
			}
			catch (ArgumentOutOfRangeException) {}
		}

		// Enqueue Test
		private static void FSPQ_Enqueue()
		{
			Debug.Log("Checking enqueue contains inserted elements...");

			GetIntQueueAndData(out var fspq, out var items, 20, 20);

			int size = 0;

			foreach (var (element, priority) in items.Take(10))
			{
				fspq.Enqueue(element, priority);
				Debug.Assert(fspq.Count == ++size, "Enqueue didn't increment size");
			}

			Debug.Assert(CollectionEquals(fspq.Elements, items.Take(10).Select(x => x.Element)), "Queue has different elements");

			foreach (var (element, priority) in items.Skip(10))
			{
				fspq.Enqueue(element, priority);
				Debug.Assert(fspq.Count == ++size, "Enqueue didn't increment size");
			}

			Debug.Assert(CollectionEquals(fspq.Elements, items.Select(x => x.Element)), "Queue has different elements");
		}

		private static void FSPQ_Enqueue_SamePriority()
		{
			Debug.Log("Checking enqueue contains inserted elements with same priority...");

			GetIntQueueAndDataWithSamePriority(out var fspq, out var items, 20, 20, 3);

			int size = 0;

			foreach (var (element, priority) in items)
			{
				fspq.Enqueue(element, priority);
				Debug.Assert(fspq.Count == ++size, "Enqueue didn't increment size");
			}

			Debug.Assert(CollectionEquals(fspq.Elements, items.Select(x => x.Element)), "Queue has different elements");
		}

		private static void FSPQ_Enqueue_Overflow()
		{
			Debug.Log("Checking enqueue contains least elements when overflowed...");

			GetIntQueueAndData(out var fspq, out var items, 10, 20);

			int size = 0;

			foreach (var (element, priority) in items)
			{
				fspq.Enqueue(element, priority);
				Debug.Assert(fspq.Count == Math.Min(++size, 10), "Enqueue didn't increment size");
			}

			Debug.Assert(CollectionEquals(fspq.Elements, items.OrderBy(x => x.Priority).Take(10).Select(x => x.Element)), "Queue has different elements");
		}

		private static void FSPQ_Enqueue_Overflow_SamePriority()
		{
			Debug.Log("Checking enqueue contains least elements with same priority when overflowed...");

			GetIntQueueAndDataWithSamePriority(out var fspq, out var items, 10, 20, 4);

			int size = 0;

			foreach (var (element, priority) in items)
			{
				fspq.Enqueue(element, priority);
				Debug.Assert(fspq.Count == Math.Min(++size, 10), "Enqueue didn't increment size");
			}

			Debug.Assert(CollectionEquals(fspq.Elements, items.OrderBy(x => x.Priority).Take(10).Select(x => x.Element)), "Queue has different elements");
		}

		// Peek Test
		private static void FSPQ_Peek()
		{
			Debug.Log("Checking peek returns maximal element...");

			GetIntQueueAndData(out var fspq, out var items, 10, 10);

			var maxItem = items[0];

			foreach (var (element, priority) in items)
			{
				if (priority > maxItem.Priority)
				{
					maxItem = (element, priority);
				}

				fspq.Enqueue(element, priority);

				var peekElement = fspq.Peek();
				Debug.Assert(maxItem.Element == peekElement, "Peek returns non maximal element");
			}
		}

		private static void FSPQ_Peek_SamePriority()
		{
			Debug.Log("Checking peek returns maximal element when same priority...");

			GetIntQueueAndDataWithSamePriority(out var fspq, out var items, 20, 20, 4);

			int maxPriority = int.MinValue;
			List<int> maxElements = new List<int>();

			foreach (var (element, priority) in items)
			{
				if (priority > maxPriority)
				{
					maxPriority = priority;
					maxElements.Clear();
				}
				maxElements.Add(element);

				fspq.Enqueue(element, priority);

				var peekElement = fspq.Peek();
				Debug.Assert(maxElements.Contains(peekElement), "Peek returns non maximal element");
			}
		}

		private static void FSPQ_Peek_Overflow()
		{
			Debug.Log("Checking peek returns maximal element when overflowed...");

			GetIntQueueAndData(out var fspq, out var items, 10, 20);

			for (int i = 0; i < 20;)
			{
				var (element, priority) = items[i++];

				var maxItem = items.Take(i).OrderBy(x => x.Priority).ElementAt(Math.Min(i, 10) - 1);

				fspq.Enqueue(element, priority);

				var peekElement = fspq.Peek();
				Debug.Assert(maxItem.Element == peekElement, "Peek returns non maximal element");
			}
		}

		private static void FSPQ_Peek_Empty()
		{
			Debug.Log("Checking peek throws when queue is empty...");

			try
			{
				var fspq = new FixedSizedPriorityQueue<string, int>(5);
				var element = fspq.Peek();
				Debug.LogError("Peek didn't throw exception");
			}
			catch (InvalidOperationException) {}
		}

		// Dequeue Test
		private static void FSPQ_Dequeue()
		{
			Debug.Log("Checking dequeue returns maximal element...");

			GetIntQueueAndData(out var fspq, out var items, 10, 10);

			foreach (var (element, priority) in items)
			{
				fspq.Enqueue(element, priority);
			}

			List<int> expectedElements = (
				from item in items
				orderby item.Priority descending
				select item.Element
			).ToList();

			int size = 10;

			foreach (int expectedElement in expectedElements)
			{
				int peekElement = fspq.Peek();
				int dequeueElement = fspq.Dequeue();
				Debug.Assert(fspq.Count == --size, "Dequeue didn't decrement size");

				Debug.Assert(expectedElement == peekElement, "Peek returns non maximal element");
				Debug.Assert(expectedElement == dequeueElement, "Dequeue returns non maximal element");
			}

			FSPQ_CheckEmpty(fspq);
		}

		private static void FSPQ_Dequeue_Empty()
		{
			Debug.Log("Checking dequeue throws when queue is empty...");

			try
			{
				var fspq = new FixedSizedPriorityQueue<string, int>(5);
				var element = fspq.Dequeue();
				Debug.LogError("Dequeue didn't throw exception");
			}
			catch (InvalidOperationException) {}
		}

		// EnqueueDequeue Test
		private static void FSPQ_EnqueueDequeue()
		{
			Debug.Log("Checking enqueuedequeue returns maximal element...");

			GetIntQueueAndData(out var fspq, out var items, 20, 20);

			foreach (var (element, priority) in items.Take(10))
			{
				fspq.Enqueue(element, priority);
			}

			for (int i = 10; i < 20;)
			{
				var (element, priority) = items[i++];

				var enqueueDequeueElement = fspq.EnqueueDequeue(element, priority);
				Debug.Assert(fspq.Count == 10, "EnqueueDequeue changed size");
				Debug.Assert(CollectionEquals(fspq.Elements, items.Take(i).OrderBy(x => x.Priority).Take(10).Select(x => x.Element)), "Queue has different elements");
			}

		}

		private static void FSPQ_EnqueueDequeue_Empty()
		{
			Debug.Log("Checking enqueuedequeue returns element when empty...");

			GetIntQueueAndData(out var fspq, out var items, 10, 10);

			foreach (var (element, priority) in items)
			{
				var enqueueDequeueElement = fspq.EnqueueDequeue(element, priority);

				FSPQ_CheckEmpty(fspq);
				Debug.Assert(element == enqueueDequeueElement, "EnqueueDequeue returns different element");
			}
		}

		// Clear Test
		private static void FSPQ_Clear()
		{
			Debug.Log("Checking clear empties queue...");

			GetIntQueueAndData(out var fspq, out var items, 10, 10);

			foreach (var (element, priority) in items)
			{
				fspq.Enqueue(element, priority);
			}

			fspq.Clear();

			FSPQ_CheckEmpty(fspq);
		}

		private static void FSPQ_Clear_Empty()
		{
			Debug.Log("Checking clear empties empty queue...");

			var fspq = new FixedSizedPriorityQueue<string, int>(5);

			fspq.Clear();

			FSPQ_CheckEmpty(fspq);
		}

		// Other Test
		private static void FSPQ_ConsistentElements()
		{
			Debug.Log("Checking elements is consistent...");

			GetIntQueueAndData(out var fspq, out var items, 10, 10);

			foreach (var (element, priority) in items)
			{
				fspq.Enqueue(element, priority);
			}

			var element1 = fspq.Elements.ToList();
			var element2 = fspq.Elements.ToList();

			Debug.Assert(element1.Count() == 10, "Elements returns different elements");
			Debug.Assert(CollectionEquals(element1, element2), "Elements returns different elements");
		}

		private static void FSPQ_CustomTest()
		{
			Debug.Log("Checking custom test dataset...");

			var fspq = new FixedSizedPriorityQueue<string, int>(10);

			fspq.Enqueue("one", 1);
			fspq.Enqueue("two", 2);
			fspq.Enqueue("three", 3);

			// insert maximum element...
			Debug.Assert(fspq.EnqueueDequeue("four", 4) == "four", "Wrong custom test result");

			// insert other element...
			Debug.Assert(fspq.EnqueueDequeue("zero", 0) == "three", "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() == "two", "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() == "one", "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() == "zero", "Wrong custom test result");

			FSPQ_CheckEmpty(fspq);

			// insert same priority element...
			fspq.Enqueue("one", 1);
			fspq.Enqueue("two", 2);
			fspq.Enqueue("three", 3);

			Debug.Assert(fspq.EnqueueDequeue("I'm not three", 3) == "I'm not three", "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() == "three", "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() == "two", "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() == "one", "Wrong custom test result");

			FSPQ_CheckEmpty(fspq);

			// insert null...
			fspq.Enqueue(null, 2);
			fspq.Enqueue("what?", 3);
			fspq.Enqueue("one", 1);

			Debug.Assert(fspq.Dequeue() == "what?", "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() is null, "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() == "one", "Wrong custom test result");

			FSPQ_CheckEmpty(fspq);

			// insert more null...
			fspq.Enqueue(null, 2);
			fspq.Enqueue(null, 0);
			fspq.Enqueue(null, 2);
			fspq.Enqueue("not null", 1);
			fspq.Enqueue(null, 0);
			fspq.Enqueue(null, 2);
			fspq.Enqueue(null, 0);

			Debug.Assert(fspq.Dequeue() is null, "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() is null, "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() is null, "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() == "not null", "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() is null, "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() is null, "Wrong custom test result");
			Debug.Assert(fspq.Dequeue() is null, "Wrong custom test result");

			FSPQ_CheckEmpty(fspq);
		}
	}
}
