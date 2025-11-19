#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype.Editor
{
    [InitializeOnLoad]
    internal static class ArrowPrefabAutoBuilder
    {
        private const string PrefabPath = "Assets/Resource/Resources/Prefabs/ArrowProjectile.prefab";
        private const string SpritePath = "Assets/Resource/Resources/Textures/Arrow.png";

        static ArrowPrefabAutoBuilder()
        {
            EditorApplication.delayCall += EnsurePrefab;
        }

        private static void EnsurePrefab()
        {
            EditorApplication.delayCall -= EnsurePrefab;

            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (existingPrefab != null)
                return;

            if (Directory.Exists(Path.GetDirectoryName(PrefabPath)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(PrefabPath));

            Sprite spriteAsset = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
            if (spriteAsset == null)
            {
                Debug.LogWarning($"[ArrowPrefabAutoBuilder] Sprite not found at {SpritePath}. Prefab creation skipped.");
                return;
            }

            GameObject temp = new GameObject("ArrowProjectile");
            try
            {
                SpriteRenderer renderer = temp.AddComponent<SpriteRenderer>();
                renderer.sprite = spriteAsset;
                renderer.sortingOrder = 1;

                Rigidbody2D body = temp.AddComponent<Rigidbody2D>();
                body.gravityScale = 0f;
                body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                body.constraints = RigidbodyConstraints2D.FreezeRotation;

                BoxCollider2D collider = temp.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(0.6f, 0.15f);

                ArrowProjectile projectile = temp.AddComponent<ArrowProjectile>();

                SerializedObject serializedProjectile = new SerializedObject(projectile);
                serializedProjectile.FindProperty("_rigidbody").objectReferenceValue = body;
                serializedProjectile.FindProperty("_collider").objectReferenceValue = collider;
                serializedProjectile.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(temp, PrefabPath);
                AssetDatabase.SaveAssets();
            }
            finally
            {
                Object.DestroyImmediate(temp);
            }
        }
    }
}
#endif
