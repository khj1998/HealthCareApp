using UnityEngine;

namespace SOMA.StartTasks
{
	public class StartTask : MonoBehaviour
	{
		public bool IsFinished { get; private set; } = false;

		protected void FinishTask()
		{
			IsFinished = true;
		}
	}
}
