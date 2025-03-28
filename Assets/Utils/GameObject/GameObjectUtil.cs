using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
	public static partial class GameObjectUtil
	{
		/// <summary>
		/// Finds all GameObjects with the specified tag, including inactive ones.
		/// </summary>
		public static List<GameObject> FindAllGameObjectsWithTagIncludingInactive(string tag)
		{
			List<GameObject> taggedObjects = new List<GameObject>();
			var sceneCount = SceneManager.sceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				Scene currentScene = SceneManager.GetSceneAt(i);
				GameObject[] rootObjects = currentScene.GetRootGameObjects();

				foreach (GameObject rootObject in rootObjects)
				{
					FindInChildren(rootObject, tag, taggedObjects);
				}
			}

			return taggedObjects;
		}

		private static void FindInChildren(GameObject parent, string tag, List<GameObject> taggedObjects)
		{
			if (parent.CompareTag(tag))
			{
				taggedObjects.Add(parent);
			}

			foreach (Transform child in parent.transform)
			{
				FindInChildren(child.gameObject, tag, taggedObjects);
			}
		}
		
		
		public static GameObject  GetGameobjectFromDictionary(string key, System.Collections.Generic.Dictionary<string, GameObject> dictionary)
		{
			if (dictionary.ContainsKey(key))
			{
				return dictionary[key];
			}
			return null;
		}
		
		public static void DeleteAllChildrenObjects(GameObject parent)
		{
			int childCount = parent.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform childTransform = parent.transform.GetChild(i);
				GameObject childObject = childTransform.gameObject;
				// 先递归删除子物体的子物体
				DeleteAllChildrenObjects(childObject);
				// 再删除当前子物体
				UnityEngine.Object.Destroy(childObject);
			}
		}

		public static string GetGameObjectHierarchyPath(this GameObject go, bool showLog = true)
		{
			if (go == null)
			{
				if (showLog)
				{
					Debug.LogError("GameObject NULL ERROR from GetGameObjectHierarchyPath");
				}
				return string.Empty;
			}
			return go.transform.GetTransHierarchyPath();
		}

		public static string GetTransHierarchyPath(this Transform trans, bool showLog = true)
		{
			if (trans == null)
			{
				if (showLog)
				{
					Debug.LogError("Transform NULL ERROR from GetTransHierarchyPath");
				}
				return string.Empty;
			}

			StringBuilder sb = new();
			sb.Append(trans.name);
			while (trans.parent != null)
			{
				trans = trans.parent;
				sb.Insert(0, $"{trans.name}/");
			}
			sb.Insert(0, $"{trans.gameObject.scene.name}/");

			return sb.ToString();
		}

		public static string GetTransRelativePath(Transform transA, Transform transB)
		{
			var pathA = transA.GetTransHierarchyPath();
			var pathB = transB.GetTransHierarchyPath();
			
			if (pathA == pathB)
			{
				return ".";
			}

			string relativePath = string.Empty;
			if (pathA.Contains(pathB))	// pathA is longer thus is son
			{	
				relativePath = Path.GetRelativePath(pathB, pathA);
			}
			else if (pathB.Contains(pathA))
			{				
				relativePath = Path.GetRelativePath(pathA, pathB);
			}

			return relativePath.Replace('\\', '/');
		}

		/// <summary>
		/// Will instantiate an object disabled preventing it from calling Awake/OnEnable.
		/// </summary>
		public static T InstantiateDisabled<T>(T original, Transform parent = null, bool worldPositionStays = false) where T : Object
		{
			if (!GetActiveState(original))
			{
				return Object.Instantiate(original, parent, worldPositionStays);
			}

			(GameObject coreObject, Transform coreObjectTransform) = CreateDisabledCoreObject(parent);
			T instance = Object.Instantiate(original, coreObjectTransform, worldPositionStays);
			SetActiveState(instance, false);
			SetParent(instance, parent, worldPositionStays);
			Object.Destroy(coreObject);
			return instance;
		}

		/// <summary>
		/// Will instantiate an object disabled preventing it from calling Awake/OnEnable.
		/// </summary>
		public static T InstantiateDisabled<T>(T original, Vector3 position, Quaternion rotation, Transform parent = null) where T : Object
		{
			if (!GetActiveState(original))
			{
				return Object.Instantiate(original, position, rotation, parent);
			}

			(GameObject coreObject, Transform coreObjectTransform) = CreateDisabledCoreObject(parent);
			T instance = Object.Instantiate(original, position, rotation, coreObjectTransform);
			SetActiveState(instance, false);
			SetParent(instance, parent, false);
			Object.Destroy(coreObject);
			return instance;
		}

		private static (GameObject coreObject, Transform coreObjectTransform) CreateDisabledCoreObject(Transform parent = null)
		{
			GameObject coreObject = new GameObject(string.Empty);
			coreObject.SetActive(false);
			Transform coreObjectTransform = coreObject.transform;
			coreObjectTransform.SetParent(parent);

			return (coreObject, coreObjectTransform);
		}

		private static bool GetActiveState<T>(T obj) where T : Object
		{
			switch (obj)
			{
				case GameObject gameObject:
				{
					return gameObject.activeSelf;
				}
				case Component component:
				{
					return component.gameObject.activeSelf;
				}
				default:
				{
					return false;
				}
			}
		}

		private static void SetActiveState<T>(T obj, bool state) where T : Object
		{
			switch (obj)
			{
				case GameObject gameObject:
				{
					gameObject.SetActive(state);
					break;
				}
				case Component component:
				{
					component.gameObject.SetActive(state);
					break;
				}
			}
		}

		private static void SetParent<T>(T obj, Transform parent, bool worldPositionStays) where T : Object
		{
			switch (obj)
			{
				case GameObject gameObject:
				{
					gameObject.transform.SetParent(parent, worldPositionStays);
					break;
				}
				case Component component:
				{
					component.transform.SetParent(parent, worldPositionStays);
					break;
				}
			}
		}

		public static void SafeSetActive(this GameObject gameObject, bool enable, bool showLog = true)
		{
			if (gameObject == null)
			{
				if (showLog)
				{
					Debug.LogError($"GameObject NULL ERROR from SafeSetActive");
				}
				return;
			}

			if (gameObject.activeSelf != enable)
			{
				gameObject.SetActive(enable);
			}
		}

		public static void SafeSetActive(this Behaviour behaviour, bool enable, bool showLog = true)
		{
			if (behaviour == null)
			{
				if (showLog)
				{
					Debug.LogError($"Behaviour NULL ERROR from SafeSetActive");
				}
				return;
			}

			if (behaviour.enabled != enable)
			{
				behaviour.enabled = enable;
			}
		}

		public static void SafeSetActive(this Component component, bool enable, bool showLog = true)
		{
			if (component == null)
			{
				if (showLog)
				{
					Debug.LogError($"Component NULL ERROR from SafeSetActive");
				}
				return;
			}
			
			SafeSetActive(component.gameObject, enable, showLog);
		}
		
		public static GameObject SafeGetGameObject(this Component cmpt, string path = "", bool showLog = true)
		{
			if (cmpt == null)
			{
				if (showLog)
				{
					Debug.LogError($"Component NULL ERROR from SafeGetGameObject");
				}

				return null;
			}

			return cmpt.SafeFindTrans(path)?.gameObject;
		}

		public static T SafeGetCmpt<T>(this Component cmpt, string path, bool showLog = true) where T : Component
		{
			if (cmpt == null)
			{
				if (showLog)
				{
					Debug.LogError($"Component NULL ERROR from SafeGetCmpt");
				}

				return null;
			}

			return cmpt.SafeFindTrans(path).SafeGetCmpt<T>();
		}

		public static T SafeGetCmpt<T>(this Component cmpt, bool showLog = true) where T : Component
		{
			if (cmpt == null)
			{
				if (showLog)
				{
					Debug.LogError($"Component NULL ERROR from SafeGetCmpt");
				}

				return null;
			}

			return cmpt.GetComponent<T>();
		}
		
		public static T SafeAttachCmpt<T>(this Component cmpt, string path, bool showLog = true) where T : Component
		{
			if (cmpt == null)
			{
				if (showLog)
				{
					Debug.LogError($"Component NULL ERROR from SafeGetCmpt");
				}

				return null;
			}

			var oldCmpt = cmpt.SafeGetCmpt<T>(path);

			return oldCmpt ?? cmpt.SafeGetGameObject(path)?.AddComponent<T>();
		}
		
		public static T SafeAttachCmpt<T>(this Component cmpt, bool showLog = true) where T : Component
		{
			if (cmpt == null)
			{
				if (showLog)
				{
					Debug.LogError($"Component NULL ERROR from SafeGetCmpt");
				}

				return null;
			}

			var oldCmpt = cmpt.SafeGetCmpt<T>();

			return oldCmpt ?? cmpt.gameObject.AddComponent<T>();
		}

		public static Transform SafeFindTrans(this Transform trans, string path, bool showLog = true)
		{
			if (trans == null)
			{
				if (showLog)
				{
					Debug.LogError($"Transform NULL or string is null/empty ERROR from SafeFindTrans");
				}

				return null;
			}

			var returnTrans =  trans.Find(path);
			return returnTrans;
		}

		public static Transform SafeFindTrans(this Component cmpt, string path, bool showLog = true)
		{
			if (cmpt == null)
			{
				if (showLog)
				{
					Debug.LogError($"Component NULL or string is null/empty ERROR from SafeFindTrans");
				}

				return null;
			}

			return cmpt.transform.SafeFindTrans(path);
		}

		public static Transform SafeGetTrans(this Component cmpt, bool showLog = true)
		{
			if (cmpt == null)
			{
				if (showLog)
				{
					Debug.LogError($"Component NULL ERROR from SafeGetTrans");
				}

				return null;
			}

			return cmpt.transform;
		}
	}
}