#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

namespace Puzzle
{
    [CustomPropertyDrawer(typeof(SeatConstraint), true)]
    public class SeatConstraintDrawer : PropertyDrawer
    {
        private static Type[] _constraintTypes;

        private static Type[] GetConstraintTypes()
        {
            if (_constraintTypes == null)
            {
                _constraintTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => typeof(SeatConstraint).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToArray();
            }
            return _constraintTypes;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true) + 20f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var currentTypeName = property.managedReferenceFullTypename;
            var types = GetConstraintTypes();
            var typeNames = types.Select(t => t.Name).ToArray();

            int currentIndex = Array.FindIndex(types, t => currentTypeName.EndsWith(t.Name));
            if (currentIndex < 0) currentIndex = 0;

            var dropdownRect = new Rect(position.x, position.y, position.width, 18f);
            int newIndex = EditorGUI.Popup(dropdownRect, "Constraint Type", currentIndex, typeNames);

            if (newIndex != currentIndex || property.managedReferenceValue == null)
            {
                property.managedReferenceValue = Activator.CreateInstance(types[newIndex]);
            }

            var fieldsRect = new Rect(position.x, position.y + 20f, position.width, position.height - 20f);
            EditorGUI.PropertyField(fieldsRect, property, GUIContent.none, true);
        }
    }
}
#endif