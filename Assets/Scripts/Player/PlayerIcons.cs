using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Player
{
    /// <summary>
    /// Contains data about which icons the player has selected
    /// </summary>
    public class PlayerIcons : PlayerScript
    {
        [SerializeField] private GamemodeIconData[] gamemodeIconData;
        public GamemodeIconData[] GetGamemodeIconData => gamemodeIconData;

        public static Dictionary<Gamemode, GamemodeIconData.MeshData[]> MeshDataDictionary = null;

        public override void Awake()
        {
            base.Awake();

            TryCreateDictionary();
        }

        public void TryCreateDictionary()
        {
            if (MeshDataDictionary == null)
            {
                MeshDataDictionary = new Dictionary<Gamemode, GamemodeIconData.MeshData[]>();

                foreach (GamemodeIconData iconData in gamemodeIconData)
                {
                    MeshDataDictionary.Add(iconData.Gamemode, iconData.Meshes);
                }
            }
        }

        /// <summary>
        /// Returns the currently equipped icon on the given <paramref name="gamemode"/>.
        /// </summary>
        public static int GetIconIndex(Gamemode gamemode)
        {
            return SaveData.SaveFile.GetEquippedIconIndex(gamemode);
        }

        /// <summary>
        /// Returns the current mesh equipped on the given <paramref name="gamemode"/> icon.
        /// </summary>
        public static Mesh GetCurrentMesh(Gamemode gamemode)
        {
            return GetMeshData(gamemode).Mesh;
        }

        /// <summary>
        /// Returns the currently equipped <see cref="GamemodeIconData.MeshData"/> on the given <paramref name="gamemode"/>.
        /// </summary>
        public static GamemodeIconData.MeshData GetMeshData(Gamemode gamemode)
        {
            return MeshDataDictionary[gamemode][GetIconIndex(gamemode)];
        }

        /// <summary>
        /// Returns the index maximum length of a the given <paramref name="gamemode"/>.
        /// </summary>
        public static int GetIndexMaxLength(Gamemode gamemode)
        {
            return MeshDataDictionary[gamemode].Length;
        }

        /// <summary>
        /// Class that contains data about a single icon.
        /// </summary>
        [Serializable]
        public class GamemodeIconData
        {
            public Gamemode Gamemode;

            [Space]
            public MeshData[] Meshes;

            /// <summary>
            /// Contains data about a single mesh of a icon.
            /// </summary>
            [Serializable]
            public class MeshData
            {
                public Mesh Mesh;
            }
        }
    }
}
