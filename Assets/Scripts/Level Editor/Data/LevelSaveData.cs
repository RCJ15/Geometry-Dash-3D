using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Level;
using GD3D.Player;

using Object = UnityEngine.Object;

namespace GD3D.LevelEditor
{
    /// <summary>
    /// The class that contains all the data for a singular level.
    /// </summary>
    [Serializable]
    public class LevelSaveData
    {
        public string Name = "Unnamed";
        public string Description = "";

        public Color BackgroundColor = LevelColors.DEFAULT_BACKGROUND_COLOR;
        public Color GroundColor = LevelColors.DEFAULT_GROUND_COLOR;
        public Color FogColor = LevelColors.DEFAULT_FOG_COLOR;

        public Gamemode StartGamemode = Gamemode.cube;
        public GameSpeed StartSpeed = GameSpeed.normalSpeed;

        public DifficultyRating Difficulty = DifficultyRating.na;

        public string Song = null;

        public LevelObjectSaveData[] Objects;
    }

    /// <summary>
    /// Data about a single object in a Level.
    /// </summary>
    [Serializable]
    public class LevelObjectSaveData
    {
        public ulong ID; // Object ID (which type of object it is)
        public Vector3 P; // Position
        public Quaternion R; // Rotation
        public Vector3 S; // Scale
        public Component[] C; // Component data on the object

        /// <summary>
        /// Data about a single component on the object
        /// </summary>
        [Serializable]
        public class Component
        {
            public ulong ID; // ID
            public Field[] F; // Field Data for the component (EG. Which color a color trigger component will change)
        }

        /// <summary>
        /// Loads a <see cref="GameObject"/> using the data that is saved on this <see cref="LevelObjectSaveData"/>.
        /// </summary>
        /// <returns>The <see cref="GameObject"/> loaded.</returns>
        public GameObject Load(Transform parent = null)
        {
            if (!LevelObjectManager.Objects.TryGetValue(ID, out GameObject copy))
            {
                return null;
            }

            GameObject obj = Object.Instantiate(copy, P, R, parent);

            obj.transform.localScale = S;

            foreach (Component componentData in C)
            {
                // Continue if try get fails #1
                if (!LevelObjectManager.ObjectComponents.TryGetValue(ID, out var typeDictionary))
                {
                    continue;
                }

                // Continue if try get fails #2
                if (!typeDictionary.TryGetValue(componentData.ID, out Type type))
                {
                    continue;
                }

                // Continue if try get fails #3
                if (!obj.TryGetComponent(type, out var component))
                {
                    continue;
                }

                // Loop through every single saved field in the component
                foreach (Field field in componentData.F)
                {
                    // Continue if try get fails #4
                    if (!LevelObjectManager.ObjectFields.TryGetValue(type, out var fieldInfoDictionary))
                    {
                        continue;
                    }

                    // Continue if try get fails #5
                    if (!fieldInfoDictionary.TryGetValue(field.ID, out FieldInfo fieldInfo))
                    {
                        continue;
                    }

                    // Get the value from the field
                    object value = field.GetData(fieldInfo.FieldType);

                    // Set value! :)
                    fieldInfo.SetValue(component, value);
                }
            }

            // Return the loaded object
            return obj;
        }

        /// <summary>
        /// Data about a single Field for a component
        /// </summary>
        [Serializable]
        public class Field
        {
            private const string START = "{\"V\":";
            private const string END = "}";

            public ulong ID; // ID
            public string V; // Value

            public Field(ulong id, object value, Type objectType)
            {
                ID = id;

                MethodInfo method = _toJsonMethod.MakeGenericMethod(objectType);
                
                V = (string)method.Invoke(null, new object[] { value });
            }

            public T GetData<T>()
            {
                return (T)GetData(typeof(T));
            }

            public object GetData(Type objectType)
            {
                MethodInfo method = _fromJsonMethod.MakeGenericMethod(objectType);

                return method.Invoke(null, new object[] { V });
            }

            #region Method Info
            private static MethodInfo _cachedToJsonMethod = null;
            private static MethodInfo _toJsonMethod
            {
                get
                {
                    if (_cachedToJsonMethod == null)
                    {
                        foreach (var methodInfo in typeof(Field).GetRuntimeMethods())
                        {
                            if (methodInfo.Name != "ToJson")
                            {
                                continue;
                            }

                            _cachedToJsonMethod = methodInfo;
                            break;
                        }
                    }

                    return _cachedToJsonMethod;
                }
            }

            private static MethodInfo _cachedFromJsonMethod = null;
            private static MethodInfo _fromJsonMethod
            {
                get
                {
                    if (_cachedFromJsonMethod == null)
                    {
                        foreach (var methodInfo in typeof(Field).GetRuntimeMethods())
                        {
                            if (methodInfo.Name != "FromJson")
                            {
                                continue;
                            }

                            _cachedFromJsonMethod = methodInfo;
                            break;
                        }
                    }

                    return _cachedFromJsonMethod;
                }
            }
            #endregion

            #region Data Holder Shenanigans
            private static string ToJson<T>(T value)
            {
                string data = JsonUtility.ToJson(new DataHolder<T>(value));

                data = data.Substring(START.Length);
                data = data.Substring(0, data.Length - END.Length);

                return data.Trim();
            }

            private static T FromJson<T>(string json)
            {
                return JsonUtility.FromJson<DataHolder<T>>(START + json + END).V;
            }

            [Serializable]
            private class DataHolder<T>
            {
                public T V;

                public DataHolder(T value)
                {
                    V = value;
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// An enum which represents the different difficulties a level can have.
    /// </summary>
    [Serializable]
    public enum DifficultyRating
    {
        na,
        auto,
        easy,
        normal,
        hard,
        harder,
        insane,
        demon,
        easyDemon,
        mediumDemon,
        hardDemon,
        insaneDemon,
        extremeDemon,
    }
}
