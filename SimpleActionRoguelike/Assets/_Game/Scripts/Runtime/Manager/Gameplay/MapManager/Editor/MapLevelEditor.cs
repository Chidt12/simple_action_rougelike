using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Manager.Gameplay;
using UnityEditor;

namespace Runtime.Manager.Gameplay
{
    [CustomEditor(typeof(MapLevel))]
    public class MapLevelEditor : Editor
    {
        private Editor cachedEditor;

        public void OnEnable()
        {
            /* Resetting cachedEditor. This will ensure it is written to
               The next time OnInspectorGUI is called
            */
            cachedEditor = null;
        }


        public override void OnInspectorGUI()
        {
            // Grabbing the object this inspector is editing.
            MapLevel editedMonobehaviour = (MapLevel)target;

            /* Checking if we need to get our Editor. Calling Editor.CreateEditor() 
               if needed */
            if (cachedEditor == null && editedMonobehaviour != null)
            {
                cachedEditor =
                    Editor.CreateEditor(editedMonobehaviour.scriptableObject);
            }

            /* We want to show the other variables in our Monobehaviour as well, 
               so we'll call the superclasses' OnInspectorGUI(). Note this could 
               also be accomplished by a call to DrawDefaultInspector() */
            base.OnInspectorGUI();

            //Drawing our ScriptableObjects inspector
            if(cachedEditor)
                cachedEditor.DrawDefaultInspector();
        }
    }
}