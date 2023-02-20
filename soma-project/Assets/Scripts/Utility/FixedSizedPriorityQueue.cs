using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace SOMA
{
	// cf. https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections/src/System/Collections/Generic/PriorityQueue.cs
	// PriorityQueue with fixed max size. Set it from constructor.
	//
	// Dequeue/Peek will return max priority.
	// If you enqueue more than max size, deque will happen first, so queue will have max size least elements.
	public class FixedSizedPriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
	{
		private (TElement Element, TPriority Priority)[] _nodes;
		private readonly int _maxSize;
		private int _size;

		public int Count => _size;

		// THIS COLLECTION IS NOT SORTED!!!!!!!!!!
		public IEnumerable<TElement> Elements => (
			from node in _nodes[1..(_size + 1)]
			select node.Element
		);

		public FixedSizedPriorityQueue(int maxSize)
		{
			if (maxSize <= 0)
			{
				throw new ArgumentOutOfRangeException("Queue size cannot be smaller than 1");
			}

			_maxSize = maxSize;
			_size = 0;
			_nodes = new (TElement, TPriority)[maxSize + 1];
		}

		public void Enqueue(TElement element, TPriority priority)
		{
			if (priority is null)
			{
				throw new ArgumentNullException("priority is null");
			}

			if (_size != _maxSize)
			{
				MoveUp((element, priority), ++_size);
			}
			else
			{
				EnqueueDequeue(element, priority);
			}
		}

		public TElement EnqueueDequeue(TElement element, TPriority priority)
		{
			if (priority is null)
			{
				throw new ArgumentNullException("priority is null");
			}

			if (_size != 0)
			{
				var root = _nodes[1];

				// root has higher priority; dequeue root and insert element
				if (priority.CompareTo(root.Priority) < 0)
				{
					MoveDown((element, priority), 1);
					return root.Element;
				}
			}

			return element;
		}

		public TElement Peek()
		{
			if (_size == 0)
			{
				throw new InvalidOperationException("Queue is empty");
			}

			return _nodes[1].Element;
		}

		public TElement Dequeue()
		{
			// Peek throws InvalidOperationException when queue is empty
			var rootElement = Peek();

			int lastNodeIndex = _size--;
			if (lastNodeIndex > 1)
			{
				MoveDown(_nodes[lastNodeIndex], 1);
			}

			if (RuntimeHelpers.IsReferenceOrContainsReferences<(TElement, TPriority)>())
			{
				// Clear the elements so that the gc can reclaim the references
				_nodes[lastNodeIndex] = default;
			}

			return rootElement;
		}

		private int GetParentIndex(int nodeIndex) => nodeIndex >> 1;
		private int GetFirstChildIndex(int nodeIndex) => nodeIndex << 1;

		private void MoveUp((TElement Element, TPriority Priority) node, int nodeIndex)
		{
			while (nodeIndex > 1)
			{
				int parentIndex = GetParentIndex(nodeIndex);
				var parent = _nodes[parentIndex];

				// node has higher priority than parent; move up
				if (node.Priority.CompareTo(parent.Priority) > 0)
				{
					_nodes[nodeIndex] = parent;
					nodeIndex = parentIndex;
				}
				else
				{
					break;
				}
			}

			_nodes[nodeIndex] = node;
		}

		private void MoveDown((TElement Element, TPriority Priority) node, int nodeIndex)
		{
			int childIndex;
			while ((childIndex = GetFirstChildIndex(nodeIndex)) <= _size)
			{
				var child = _nodes[childIndex];

				if (childIndex + 1 <= _size)
				{
					var nextChild = _nodes[childIndex + 1];

					// next child has higher priority than previous child; use this
					if (nextChild.Priority.CompareTo(child.Priority) > 0)
					{
						++childIndex;
						child = nextChild;
					}
				}

				// child has higher priority than node; move down
				if (node.Priority.CompareTo(child.Priority) < 0)
				{
					_nodes[nodeIndex] = child;
					nodeIndex = childIndex;
				}
				else
				{
					break;
				}
			}

			_nodes[nodeIndex] = node;
		}

		public void Clear()
		{
			if (RuntimeHelpers.IsReferenceOrContainsReferences<(TElement, TPriority)>())
			{
				// Clear the elements so that the gc can reclaim the references
				Array.Clear(_nodes, 1, _size);
			}

			_size = 0;
		}
	}
}
