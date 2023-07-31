#if UNITY_EDITOR

using UnityEditor;

namespace Runtime.UI
{
    [CustomEditor(typeof(CustomButton))]
    public class CustomButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CustomButton targetMenuButton = (CustomButton)target;
            // Show default inspector property editor
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif