using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.Audio
{
	/// <summary>
	/// Class that has many methods for using some internal methods from the "UnityEditor.AudioUtil" class
	/// </summary>
	public static class AudioUtility
	{
		private static Assembly unityEditorAssembly => typeof(AudioImporter).Assembly;
		private static Type audioUtilClass => unityEditorAssembly.GetType("UnityEditor.AudioUtil");

		private static MethodInfo GetMethod(string name)
        {
			return audioUtilClass.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
		}

		private static MethodInfo GetMethod(string name, params Type[] types)
        {
			return audioUtilClass.GetMethod(
				name,
				BindingFlags.Static | BindingFlags.Public,
				null,
				types,
				null
			);
		}

		private static object InvokeMethod(MethodInfo method)
        {
			return method.Invoke(null, new object[] { });
        }
		private static object InvokeMethod(MethodInfo method, params object[] parameters)
		{
			return method.Invoke(null, parameters);
		}

		public static void PlayPreviewClip(AudioClip clip , int startSample = 0, bool loop = false)
		{
			MethodInfo method = GetMethod("PlayPreviewClip",
				typeof(AudioClip),
				typeof(int),
				typeof(bool)
			);

			InvokeMethod(method,
				clip,
				startSample,
				loop
			);
		}

		public static void PausePreviewClip()
		{
			MethodInfo method = GetMethod("PausePreviewClip");

			InvokeMethod(method);
		}

		public static void ResumePreviewClip()
		{
			MethodInfo method = GetMethod("ResumePreviewClip");

			method.Invoke(
				null,
				new object[] { }
			);
		}

		public static void LoopPreviewClip(bool on)
		{
			MethodInfo method = GetMethod("LoopPreviewClip",
				typeof(bool)
			);

			InvokeMethod(method, on);
		}

		public static bool IsPreviewClipPlaying()
		{
			MethodInfo method = GetMethod("IsPreviewClipPlaying");

			bool value = (bool)InvokeMethod(method);

			return value;
		}

		public static void StopAllPreviewClips()
		{
			MethodInfo method = GetMethod("StopAllPreviewClips");

			InvokeMethod(method);
		}

		public static float GetPreviewClipPosition()
		{
			MethodInfo method = audioUtilClass.GetMethod("GetPreviewClipPosition");

			float value = (float)InvokeMethod(method);

			return value;
		}

		public static int GetPreviewClipSamplePosition()
		{
			MethodInfo method = audioUtilClass.GetMethod("GetPreviewClipSamplePosition");

			int value = (int)InvokeMethod(method);

			return value;
		}
	}
}