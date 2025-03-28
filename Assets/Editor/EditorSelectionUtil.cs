using Utils;
using UnityEditor;

namespace Editor.Tools
{
    public static class EditorSelectionUtil
    {
        [MenuItem("GameObject/UI Tools/CopyGameObjectsRelativePath")]
        public static void CopyGameObjectsRelativePath()
        {
            var selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length != 2)
            {
                return;
            }

            string path = GameObjectUtil.GetTransRelativePath(selectedGameObjects[0].transform, selectedGameObjects[1].transform);

            System.Windows.Forms.Clipboard.SetText(path);
        }
    }
}