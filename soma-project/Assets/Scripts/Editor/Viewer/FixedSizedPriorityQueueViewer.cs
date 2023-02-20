using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

using Random = System.Random;

namespace SOMA.Editor.Viewer
{
	public class FixedSizedPriorityQueueViewer : EditorWindow
	{
		private const string NULL_STRING = "[NULL]";

		[MenuItem("Tools/Viewer/FixedSizedPriorityQueue")]
		private static void ShowWindow()
		{
			if (EditorWindow.HasOpenInstances<FixedSizedPriorityQueueViewer>())
			{
				FocusWindowIfItsOpen<FixedSizedPriorityQueueViewer>();
			}
			else
			{
				var window = CreateWindow<FixedSizedPriorityQueueViewer>("FixedSizedPriorityQueueViewer");
				window.InitNewQueue(window._type, window._maxSize);
			}
		}

		private enum FSPQType
		{
			IntInt,
			IntString,
			StringInt,
			StringString
		}

		private FSPQType _type = FSPQType.IntInt;
		private int _maxSize = 10;

		private bool _isElementInt => _type == FSPQType.IntInt || _type == FSPQType.IntString;
		private bool _isPriorityInt => _type == FSPQType.IntInt || _type == FSPQType.StringInt;

		private FixedSizedPriorityQueue<int, int> _fspqIntInt;
		private FixedSizedPriorityQueue<int, string> _fspqIntString;
		private FixedSizedPriorityQueue<string, int> _fspqStringInt;
		private FixedSizedPriorityQueue<string, string> _fspqStringString;

		private int _intElement;
		private string _stringElement;

		private int _intPriority;
		private string _stringPriority;

		private int _mod = 3;

		private Random _random = new Random();

		private int GetRandomInt()
		{
			return _random.Next();
		}

		private string GetRandomString()
		{
			int stringLength = _random.Next(5, 15);
			byte[] bytes = new byte[stringLength];
			_random.NextBytes(bytes);
			return Convert.ToBase64String(bytes);
		}

		private void OnGUI()
		{
			ShowQueueTypeControl();
			DrawUILine(Color.gray);
			ShowQueueItemControl();
			ShowQueueControl();
			DrawUILine(Color.gray);
			ShowQueue();
		}

		private void ShowQueueTypeControl()
		{
			FSPQType newType = _type;

			EditorGUILayout.LabelField("Element Type", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Int"))
			{
				switch (newType)
				{
					case FSPQType.IntInt:
					case FSPQType.StringInt:
						newType = FSPQType.IntInt;
						break;
					case FSPQType.IntString:
					case FSPQType.StringString:
						newType = FSPQType.IntString;
						break;
				}
			}
			if (GUILayout.Button("String"))
			{
				switch (newType)
				{
					case FSPQType.IntInt:
					case FSPQType.StringInt:
						newType = FSPQType.StringInt;
						break;
					case FSPQType.IntString:
					case FSPQType.StringString:
						newType = FSPQType.StringString;
						break;
				}
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.LabelField("Priority Type", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Int"))
			{
				switch (newType)
				{
					case FSPQType.IntInt:
					case FSPQType.IntString:
						newType = FSPQType.IntInt;
						break;
					case FSPQType.StringInt:
					case FSPQType.StringString:
						newType = FSPQType.StringInt;
						break;
				}
			}
			if (GUILayout.Button("String"))
			{
				switch (newType)
				{
					case FSPQType.IntInt:
					case FSPQType.IntString:
						newType = FSPQType.IntString;
						break;
					case FSPQType.StringInt:
					case FSPQType.StringString:
						newType = FSPQType.StringString;
						break;
				}
			}
			GUILayout.EndHorizontal();

			var originalStyle = EditorStyles.label.fontStyle;
			EditorStyles.label.fontStyle = FontStyle.Bold;
			int newMaxSize = EditorGUILayout.DelayedIntField("Max Size: ", _maxSize);
			if (newMaxSize <= 0)
			{
				newMaxSize = _maxSize;
			}
			EditorStyles.label.fontStyle = originalStyle;

			if (newType != _type || newMaxSize != _maxSize)
			{
				InitNewQueue(newType, newMaxSize);
			}
		}

		private void InitNewQueue(FSPQType newType, int newMaxSize)
		{
			switch (_type)
			{
				case FSPQType.IntInt: _fspqIntInt = null; break;
				case FSPQType.IntString: _fspqIntString = null; break;
				case FSPQType.StringInt: _fspqStringInt = null; break;
				case FSPQType.StringString: _fspqStringString = null; break;
			}

			switch (newType)
			{
				case FSPQType.IntInt:
					_fspqIntInt = new FixedSizedPriorityQueue<int, int>(newMaxSize);
					if (!_isElementInt) _stringElement = default;
					if (!_isPriorityInt) _stringPriority = default;
					break;
				case FSPQType.IntString:
					_fspqIntString = new FixedSizedPriorityQueue<int, string>(newMaxSize);
					if (!_isElementInt) _stringElement = default;
					if (_isPriorityInt) _intPriority = default;
					break;
				case FSPQType.StringInt:
					_fspqStringInt = new FixedSizedPriorityQueue<string, int>(newMaxSize);
					if (_isElementInt) _intElement = default;
					if (!_isPriorityInt) _stringPriority = default;
					break;
				case FSPQType.StringString:
					_fspqStringString = new FixedSizedPriorityQueue<string, string>(newMaxSize);
					if (_isElementInt) _intElement = default;
					if (_isPriorityInt) _intPriority = default;
					break;
			}

			_type = newType;
			_maxSize = newMaxSize;
		}

		private void ShowQueueItemControl()
		{
			// 이상하게도 버튼이 터져서 여기서는 GUILayout.Label 씀
			GUILayout.BeginHorizontal();
			GUILayout.Label("Element", EditorStyles.boldLabel);
			if (GUILayout.Button("Get Random Element"))
			{
				if (_isElementInt)
				{
					_intElement = GetRandomInt();
				}
				else
				{
					_stringElement = GetRandomString();
				}
			}
			GUILayout.EndHorizontal();

			if (!_isElementInt)
			{
				if (GUILayout.Button("Get Null"))
				{
					_stringElement = null;
				}
			}

			if (_isElementInt)
			{
				_intElement = EditorGUILayout.IntField(_intElement);
			}
			else
			{
				_stringElement = EditorGUILayout.TextField(_stringElement ?? NULL_STRING);
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Priority", EditorStyles.boldLabel);
			if (GUILayout.Button("Get Random Priority"))
			{
				if (_isPriorityInt)
				{
					_intPriority = GetRandomInt();
				}
				else
				{
					_stringPriority = GetRandomString();
				}
			}
			GUILayout.EndHorizontal();

			if (_isPriorityInt)
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Get Random Priority % "))
				{
					_intPriority = GetRandomInt() % _mod;
				}
				_mod = EditorGUILayout.IntField(_mod);
				if (_mod <= 1)
				{
					_mod = 3;
				}
				GUILayout.EndHorizontal();
			}

			if (_isPriorityInt)
			{
				_intPriority = EditorGUILayout.IntField(_intPriority);
			}
			else
			{
				_stringPriority = EditorGUILayout.TextField(_stringPriority);
			}
		}

		private void ShowQueueControl()
		{
			if (GUILayout.Button("Enqueue"))
			{
				switch (_type)
				{
					case FSPQType.IntInt:
						_fspqIntInt.Enqueue(_intElement, _intPriority);
						break;
					case FSPQType.IntString:
						_fspqIntString.Enqueue(_intElement, _stringPriority);
						break;
					case FSPQType.StringInt:
						_fspqStringInt.Enqueue(_stringElement, _intPriority);
						break;
					case FSPQType.StringString:
						_fspqStringString.Enqueue(_stringElement, _stringPriority);
						break;
				}
			}

			if (GUILayout.Button("EnqueueDequeue"))
			{
				switch (_type)
				{
					case FSPQType.IntInt:
						_intElement = _fspqIntInt.EnqueueDequeue(_intElement, _intPriority);
						_intPriority = default;
						break;
					case FSPQType.IntString:
						_intElement = _fspqIntString.EnqueueDequeue(_intElement, _stringPriority);
						_intPriority = default;
						break;
					case FSPQType.StringInt:
						_stringElement = _fspqStringInt.EnqueueDequeue(_stringElement, _intPriority);
						_stringPriority = default;
						break;
					case FSPQType.StringString:
						_stringElement = _fspqStringString.EnqueueDequeue(_stringElement, _stringPriority);
						_stringPriority = default;
						break;
				}
			}

			if (GUILayout.Button("Peek"))
			{
				switch (_type)
				{
					case FSPQType.IntInt:
						_intElement = _fspqIntInt.Peek();
						_intPriority = default;
						break;
					case FSPQType.IntString:
						_intElement = _fspqIntString.Peek();
						_intPriority = default;
						break;
					case FSPQType.StringInt:
						_stringElement = _fspqStringInt.Peek();
						_stringPriority = default;
						break;
					case FSPQType.StringString:
						_stringElement = _fspqStringString.Peek();
						_stringPriority = default;
						break;
				}
			}

			if (GUILayout.Button("Dequeue"))
			{
				switch (_type)
				{
					case FSPQType.IntInt:
						_intElement = _fspqIntInt.Dequeue();
						_intPriority = default;
						break;
					case FSPQType.IntString:
						_intElement = _fspqIntString.Dequeue();
						_intPriority = default;
						break;
					case FSPQType.StringInt:
						_stringElement = _fspqStringInt.Dequeue();
						_stringPriority = default;
						break;
					case FSPQType.StringString:
						_stringElement = _fspqStringString.Dequeue();
						_stringPriority = default;
						break;
				}
			}

			if (GUILayout.Button("Clear"))
			{
				switch (_type)
				{
					case FSPQType.IntInt:
						_fspqIntInt.Clear();
						break;
					case FSPQType.IntString:
						_fspqIntString.Clear();
						break;
					case FSPQType.StringInt:
						_fspqStringInt.Clear();
						break;
					case FSPQType.StringString:
						_fspqStringString.Clear();
						break;
				}
			}
		}

		private Vector2 _elementsScrollPos;

		private void ShowQueue()
		{
			switch (_type)
			{
				case FSPQType.IntInt: EditorGUILayout.LabelField($"Count: {_fspqIntInt.Count}"); break;
				case FSPQType.IntString: EditorGUILayout.LabelField($"Count: {_fspqIntString.Count}"); break;
				case FSPQType.StringInt: EditorGUILayout.LabelField($"Count: {_fspqStringInt.Count}"); break;
				case FSPQType.StringString: EditorGUILayout.LabelField($"Count: {_fspqStringString.Count}"); break;
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Elements:");

			string elementsString = "";
			switch (_type)
			{
				case FSPQType.IntInt: elementsString = string.Join("\n", _fspqIntInt.Elements); break;
				case FSPQType.IntString: elementsString = string.Join("\n", _fspqIntString.Elements); break;
				case FSPQType.StringInt: elementsString = string.Join("\n", (
					from element in _fspqStringInt.Elements
					select element ?? NULL_STRING
				)); break;
				case FSPQType.StringString: elementsString = string.Join("\n", (
					from element in _fspqStringString.Elements
					select element ?? NULL_STRING
				)); break;
			}
			float labelHeight = EditorStyles.label.CalcSize(new GUIContent(elementsString)).y;
			_elementsScrollPos = EditorGUILayout.BeginScrollView(_elementsScrollPos);
			EditorGUILayout.LabelField(elementsString, GUILayout.Height(labelHeight));
			EditorGUILayout.EndScrollView();
		}

		public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
			r.height = thickness;
			r.y += padding / 2;
			r.x -= 2 - EditorGUI.indentLevel * 16;
			r.width += 6;
			EditorGUI.DrawRect(r, color);
		}
	}
}
