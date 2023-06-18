/// https://gist.github.com/wappenull/2391b3c23dd20ede74483d0da4cab3f1
/// https://forum.unity.com/threads/solved-custompropertydrawer-not-being-using-in-editorgui-propertyfield.534968/

// REVISION HISTORY
//
// Rev 1 - 16/MAY/2021
//  + initial
//
// Rev 2 - 23/AUG/2021
//  + add support for array property path
//
// Rev 3 - 23/AUG/2021
//  + cache using type+path (s_PathHashVsType)
//
// Rev 5 - 09/SEP/2022
//  + apply code styles
//  + only run `FindDrawerForType` method over the assembly which contains `PropertyDrawerFinder`
//

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace ZBase.UnityScreenNavigator.Editor.Foundation
{
    /// <summary>
    /// Finds custom property drawer for a given type.
    /// </summary>
    internal static class PropertyDrawerFinder
    {
        struct TypeAndFieldInfo
        {
            internal Type type;
            internal FieldInfo fi;
        }

        private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags BINDING_FLAGS_PUBLIC = BINDING_FLAGS | BindingFlags.Public;

        private static readonly Type s_thisFinderType = typeof(PropertyDrawerFinder);
        private static readonly Type s_customPropertyDrawerType = typeof(CustomPropertyDrawer);
        private static readonly Type s_propertyDrawerType = typeof(PropertyDrawer);

        // Rev 3, be more evil with more cache!
        private static readonly Dictionary<int, TypeAndFieldInfo> s_PathHashVsType = new();
        private static readonly Dictionary<Type, PropertyDrawer> s_TypeVsDrawerCache = new();

        /// <summary>
        /// Searches for custom property drawer for given property,
        /// or returns null if no custom property drawer was found.
        /// </summary>
        public static bool TryFind(SerializedProperty property, out PropertyDrawer drawer)
        {
            var pathHash = GetUniquePropertyPathHash( property );

            if (!s_PathHashVsType.TryGetValue(pathHash, out TypeAndFieldInfo tfi))
            {
                tfi.type = GetPropertyType(property, out tfi.fi);
                s_PathHashVsType[pathHash] = tfi;
            }

            drawer = default;

            if (tfi.type != null)
            {
                if (s_TypeVsDrawerCache.TryGetValue(tfi.type, out drawer) == false)
                {
                    if (TryFind(tfi.type, out drawer))
                    {
                        s_TypeVsDrawerCache.Add(tfi.type, drawer);
                    }
                }

                if (drawer != null)
                {
                    // Drawer created by custom way like this will not have "fieldInfo" field installed
                    // It is an optional, but some user code in advanced drawer might use it.
                    //
                    // To install it, we must use reflection again,
                    // the backing field name is "internal FieldInfo m_FieldInfo".
                    //
                    // See ref file in UnityCsReference (2019) project.
                    //
                    // Note that name could changed in future update.
                    // unitycsreference\Editor\Mono\ScriptAttributeGUI\PropertyDrawer.cs

                    var fieldInfoBacking = s_propertyDrawerType.GetField(
                          "m_FieldInfo"
                        , BindingFlags.NonPublic | BindingFlags.Instance)
                    ;

                    if (fieldInfoBacking != null)
                    {
                        fieldInfoBacking.SetValue(drawer, tfi.fi);
                    }
                }
            }

            return drawer != null;
        }

        /// <summary>
        /// Returns custom property drawer for type if one could be found, or null if
        /// no custom property drawer could be found. Does not use cached values, so it's resource intensive.
        /// </summary>
        public static bool TryFind(Type propertyType, out PropertyDrawer drawer)
        {
            var cpdType = s_customPropertyDrawerType;
            FieldInfo typeField = cpdType.GetField("m_Type", BINDING_FLAGS);
            FieldInfo childField = cpdType.GetField("m_UseForChildren", BINDING_FLAGS);

            // Optimization note:
            // For benchmark (on DungeonLooter 0.8.4)
            // - Original, search all assemblies and classes: 250 msec
            // - Wappen optimized, search only specific name assembly and classes: 5 msec

            var assem = s_thisFinderType.Assembly;
            var types = assem.GetTypes();

            foreach (Type candidate in types)
            {
                // Wappen optimization: filter only "*Drawer" class name, like "SomeTypeDrawer"
                if (!candidate.Name.EndsWith("Drawer"))
                    continue;

                // See if this is a class that has [CustomPropertyDrawer(typeof(T))]
                foreach (Attribute a in candidate.GetCustomAttributes(s_customPropertyDrawerType))
                {
                    var aType = a.GetType();

                    if (aType.IsSubclassOf(s_customPropertyDrawerType) || aType == s_customPropertyDrawerType)
                    {
                        var drawerAtt = (CustomPropertyDrawer)a;
                        var drawerType =  (Type) typeField.GetValue(drawerAtt);

                        if (drawerType == propertyType
                            || ((bool)childField.GetValue(drawerAtt) && propertyType.IsSubclassOf(drawerType))
                            || ((bool)childField.GetValue(drawerAtt) && IsGenericSubclass(drawerType, propertyType)))
                        {
                            if (candidate.IsSubclassOf(s_propertyDrawerType))
                            {
                                // Technical note: PropertyDrawer.fieldInfo will not available via this drawer
                                // It has to be manually setup by caller.
                                drawer = (PropertyDrawer)Activator.CreateInstance(candidate);
                                return true;
                            }
                        }
                    }
                }
            }

            drawer = default;
            return false;
        }

        /// <summary>
        /// Gets type of a serialized property.
        /// </summary>
        private static Type GetPropertyType(SerializedProperty property, out FieldInfo fi)
        {
            // To see real property type, must dig into object that hosts it.
            GetPropertyFieldInfo(property, out Type resolvedType, out fi);
            return resolvedType;
        }

        /// <summary>
        /// For caching.
        /// </summary>
        private static int GetUniquePropertyPathHash(SerializedProperty property)
        {
            var hash = property.serializedObject.targetObject.GetType( ).GetHashCode( );
            hash += property.propertyPath.GetHashCode();
            return hash;
        }

        private static void GetPropertyFieldInfo(SerializedProperty property, out Type resolvedType, out FieldInfo fi)
        {
            var fullPath = property.propertyPath.Split('.');

            // fi is FieldInfo in perspective of parentType (property.serializedObject.targetObject)
            // NonPublic to support [SerializeField] vars
            Type parentType = property.serializedObject.targetObject.GetType();
            fi = parentType.GetField(fullPath[0], BINDING_FLAGS_PUBLIC);
            resolvedType = fi.FieldType;

            for (var i = 1; i < fullPath.Length; i++)
            {
                // To properly handle array and list
                // This has deeper rabbit hole, see `GetFieldInfoFromPropertyPath` in
                // unitycsreference\Editor\Mono\ScriptAttributeGUI\ScriptAttributeUtility.cs
                //
                // Here we will simplify it for now (could break)

                // If we are at 'Array' section like in `tiles.Array.data[0].tilemodId`
                if (IsArrayPropertyPath(fullPath, i))
                {
                    if (fi.FieldType.IsArray)
                        resolvedType = fi.FieldType.GetElementType();
                    else if (IsListType(fi.FieldType, out Type underlying))
                        resolvedType = underlying;

                    i++; // skip also the 'data[x]' part
                    // In this case, fi is not updated, FieldInfo stay the same pointing to 'tiles' part
                }
                else
                {
                    fi = resolvedType.GetField(fullPath[i]);
                    resolvedType = fi.FieldType;
                }
            }
        }

        private static bool IsArrayPropertyPath(string[] fullPath, int i)
        {
            // Also search for array pattern, thanks user https://gist.github.com/kkolyan
            // like `tiles.Array.data[0].tilemodId`
            // This is just a quick check, actual check in Unity uses RegEx
            return fullPath[i] == "Array" && i + 1 < fullPath.Length && fullPath[i + 1].StartsWith("data");
        }

        /// <summary>
        /// Stolen from unitycsreference\Editor\Mono\ScriptAttributeGUI\ScriptAttributeUtility.cs
        /// </summary>
        private static bool IsListType(Type t, out Type containedType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                containedType = t.GetGenericArguments()[0];
                return true;
            }

            containedType = null;
            return false;
        }

        /// <summary>
        /// Returns true if the parent type is generic and the child type implements it.
        /// </summary>
        private static bool IsGenericSubclass(Type parent, Type child)
        {
            if (!parent.IsGenericType)
            {
                return false;
            }

            Type currentType = child;
            var isAccessor = false;

            while (!isAccessor && currentType != null)
            {
                if (currentType.IsGenericType
                    && currentType.GetGenericTypeDefinition() == parent.GetGenericTypeDefinition()
                )
                {
                    isAccessor = true;
                    break;
                }

                currentType = currentType.BaseType;
            }

            return isAccessor;
        }

    }
}
