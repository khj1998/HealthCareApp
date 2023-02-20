using System.Collections;
using UnityEngine;
using SOMA.MediaPipe;

namespace SOMA.StartTasks
{
	public class MediaPipeTask : StartTask
	{
		private IEnumerator Start()
		{
			yield return new WaitUntil(() => PoseGraph.Instance.IsReady);
			Debug.Log("미디어 파이프 준비됨");
			FinishTask();
		}
	}
}
