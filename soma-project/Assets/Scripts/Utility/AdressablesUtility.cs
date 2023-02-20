using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SOMA
{
	public static class AdressablesUtility
	{
		public static IEnumerator LoadAssetAsync<TObject>(string key, Action<TObject> callback)
		{
			TObject result;

			var opHandle = Addressables.LoadAssetAsync<TObject>(key);
			if (!opHandle.IsDone)
			{
				yield return opHandle;
			}

			if (opHandle.Status == AsyncOperationStatus.Succeeded)
			{
				result = opHandle.Result;
			}
			else
			{
				throw new InvalidOperationException("Failed to get the text config");
			}

			callback?.Invoke(result);
			yield return null; // callback 완전히 끝난 다음 LoadAssetAsync 리턴되도록 만듦
			Addressables.Release(opHandle);
		}

		public static void LoadAssetSync<TObject>(string key, Action<TObject> callback)
		{
			var opHandle = Addressables.LoadAssetAsync<TObject>(key);
			TObject result = opHandle.WaitForCompletion();
			callback?.Invoke(result);
			Addressables.Release(opHandle);
		}
	}
}
