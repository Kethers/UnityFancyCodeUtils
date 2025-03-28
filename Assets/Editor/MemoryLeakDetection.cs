using Unity.Collections;
using UnityEditor;

namespace Editor.Tools
{
	/// <summary>
	/// 内存泄露检测模式
	/// </summary>
	public class MemoryLeakDetection
	{
		[MenuItem("Jobs/内存泄漏检测/显示当前模式")]
		static void ShowLeakDetection()
		{
			string message = string.Format("当前模式： {0}", NativeLeakDetection.Mode.ToString());
			EditorUtility.DisplayDialog("内存泄漏检测模式", message, "OK");
		}


		[MenuItem("Jobs/内存泄漏检测/禁用")]
		static void LeakDetectionDisable()
		{
			NativeLeakDetection.Mode = NativeLeakDetectionMode.Disabled;
		}

		// 验证方法会在正式方法前执行，通不过就会置灰
		[MenuItem("Jobs/内存泄漏检测/禁用", true)]
		static bool ValidateLeakDetectionDisable()
		{
			return NativeLeakDetection.Mode != NativeLeakDetectionMode.Disabled;
		}


		[MenuItem("Jobs/内存泄漏检测/启用")]
		static void LeakDetectionEnabled()
		{
			NativeLeakDetection.Mode = NativeLeakDetectionMode.Enabled;
		}

		[MenuItem("Jobs/内存泄漏检测/启用", true)]
		static bool ValidateLeakDetectionEnabled()
		{
			return NativeLeakDetection.Mode != NativeLeakDetectionMode.Enabled;
		}


		[MenuItem("Jobs/内存泄漏检测/启用堆栈跟踪")]
		static void LeakDetectionEnabledWithStackTrace()
		{
			NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
		}

		[MenuItem("Jobs/内存泄漏检测/启用堆栈跟踪", true)]
		static bool ValidateLeakDetectionEnabledWithStackTrace()
		{
			return NativeLeakDetection.Mode != NativeLeakDetectionMode.EnabledWithStackTrace;
		}
	}
}
