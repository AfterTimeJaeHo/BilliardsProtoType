#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace Aftertime.MyTinyStreamer.Editor
{
    [CustomEditor(typeof(Tilemap))]
    public class TilemapToGameObjectEditor : UnityEditor.Editor
    {
        private const string DefaultRootName = "TilemapGameObjects";

        private bool _createUnderTilemap = true;
        private Transform _customParent;
        private bool _clearPreviousRoot = true;
        private string _rootName = DefaultRootName;
        private int _baseSortingOrder;
        private bool _selectCreatedRoot;

        /// <summary>
        /// 타일맵 기본 인스펙터를 그리고 변환 옵션 UI를 추가한다.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GameObject 변환", EditorStyles.boldLabel);

            _rootName = EditorGUILayout.TextField("루트 이름", _rootName);
            _createUnderTilemap = EditorGUILayout.Toggle("타일맵 자식으로 생성", _createUnderTilemap);
            if (_createUnderTilemap == false)
            {
                _customParent = (Transform)EditorGUILayout.ObjectField("생성 부모", _customParent, typeof(Transform), true);
            }

            _clearPreviousRoot = EditorGUILayout.Toggle("동일 이름 루트 삭제", _clearPreviousRoot);
            _baseSortingOrder = EditorGUILayout.IntField("기준 Sorting Order", _baseSortingOrder);
            _selectCreatedRoot = EditorGUILayout.Toggle("생성 후 선택", _selectCreatedRoot);

            EditorGUILayout.HelpBox("2D Isometric Z as Y 타일맵을 SpriteRenderer GameObject로 변환합니다.", MessageType.Info);

            if (GUILayout.Button("타일을 GameObject로 변환"))
            {
                ConvertSelectedTilemap();
            }
        }

        /// <summary>
        /// 선택된 타일맵의 타일을 GameObject로 변환한다.
        /// </summary>
        private void ConvertSelectedTilemap()
        {
            Tilemap tilemap = (Tilemap)target;
            if (tilemap == null)
            {
                return;
            }

            Transform parentTransform = ResolveParentTransform(tilemap.transform);
            if (parentTransform == null)
            {
                EditorUtility.DisplayDialog("변환 실패", "생성 부모가 필요합니다.", "확인");
                return;
            }

            List<TileSpriteData> collectedTiles = CollectTileSprites(tilemap);
            if (collectedTiles.Count == 0)
            {
                EditorUtility.DisplayDialog("변환 결과", "타일맵에 Sprite가 존재하지 않습니다.", "확인");
                return;
            }

            GameObject rootObject = PrepareRootObject(parentTransform);
            TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
            CreateSpriteObjects(rootObject.transform, collectedTiles, tilemapRenderer);

            if (_selectCreatedRoot)
            {
                Selection.activeGameObject = rootObject;
            }

            EditorUtility.DisplayDialog("변환 완료", string.Format("{0}개의 타일을 GameObject로 변환했습니다.", collectedTiles.Count), "확인");
        }

        /// <summary>
        /// 타일맵 데이터를 순회하며 Sprite 정보 리스트를 만든다.
        /// </summary>
        private List<TileSpriteData> CollectTileSprites(Tilemap tilemap)
        {
            List<TileSpriteData> tileSprites = new List<TileSpriteData>();
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    for (int z = bounds.zMin; z < bounds.zMax; z++)
                    {
                        Vector3Int cellPosition = new Vector3Int(x, y, z);
                        Sprite sprite = tilemap.GetSprite(cellPosition);
                        if (sprite == null)
                        {
                            continue;
                        }

                        Color tileColor = tilemap.GetColor(cellPosition);
                        Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);

                        TileSpriteData spriteData = new TileSpriteData(cellPosition, worldPosition, sprite, tileColor);
                        tileSprites.Add(spriteData);
                    }
                }
            }

            return tileSprites;
        }

        /// <summary>
        /// 변환 부모 Transform을 결정한다.
        /// </summary>
        private Transform ResolveParentTransform(Transform tilemapTransform)
        {
            if (_createUnderTilemap)
            {
                return tilemapTransform;
            }

            if (_customParent != null)
            {
                return _customParent;
            }

            return tilemapTransform;
        }

        /// <summary>
        /// 생성 루트 GameObject를 반환하거나 새로 만든다.
        /// </summary>
        private GameObject PrepareRootObject(Transform parentTransform)
        {
            string resolvedName = string.IsNullOrWhiteSpace(_rootName) ? DefaultRootName : _rootName;
            Transform existing = parentTransform.Find(resolvedName);

            if (existing != null)
            {
                if (_clearPreviousRoot)
                {
                    Undo.DestroyObjectImmediate(existing.gameObject);
                }
                else
                {
                    return existing.gameObject;
                }
            }

            GameObject newRoot = new GameObject(resolvedName);
            Undo.RegisterCreatedObjectUndo(newRoot, "타일 GameObject 루트 생성");
            newRoot.transform.SetParent(parentTransform, false);
            newRoot.transform.localPosition = Vector3.zero;
            newRoot.transform.localRotation = Quaternion.identity;
            newRoot.transform.localScale = Vector3.one;
            return newRoot;
        }

        /// <summary>
        /// SpriteRenderer GameObject를 생성하고 정렬 정보를 적용한다.
        /// </summary>
        private void CreateSpriteObjects(Transform rootTransform, List<TileSpriteData> tileSprites, TilemapRenderer tilemapRenderer)
        {
            int sortingLayerId = SortingLayer.NameToID("Default");
            SpriteMaskInteraction maskInteraction = SpriteMaskInteraction.None;
            Material sharedMaterial = null;

            if (tilemapRenderer != null)
            {
                sortingLayerId = tilemapRenderer.sortingLayerID;
                maskInteraction = tilemapRenderer.maskInteraction;
                sharedMaterial = tilemapRenderer.sharedMaterial;
            }

            List<TileSpriteData> orderedTiles = tileSprites
                .OrderByDescending(data => data.WorldPosition.y)
                .ThenBy(data => data.WorldPosition.x)
                .ToList();

            for (int index = 0; index < orderedTiles.Count; index++)
            {
                TileSpriteData tileData = orderedTiles[index];
                int sortingOrder = _baseSortingOrder + index;

                GameObject spriteObject = new GameObject(string.Format("Tile_{0}_{1}_{2}", tileData.Cell.x, tileData.Cell.y, tileData.Cell.z));
                Undo.RegisterCreatedObjectUndo(spriteObject, "타일 GameObject 생성");
                spriteObject.transform.SetParent(rootTransform, false);
                spriteObject.transform.position = tileData.WorldPosition;
                spriteObject.transform.localScale = Vector3.one;

                SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = tileData.Sprite;
                spriteRenderer.color = tileData.Color;
                spriteRenderer.sortingLayerID = sortingLayerId;
                spriteRenderer.sortingOrder = sortingOrder;
                spriteRenderer.maskInteraction = maskInteraction;
                if (sharedMaterial != null)
                {
                    spriteRenderer.sharedMaterial = sharedMaterial;
                }
            }
        }

        /// <summary>
        /// 타일 Sprite 정보 묶음 구조체.
        /// </summary>
        private struct TileSpriteData
        {
            public Vector3Int Cell;
            public Vector3 WorldPosition;
            public Sprite Sprite;
            public Color Color;

            public TileSpriteData(Vector3Int cell, Vector3 worldPosition, Sprite sprite, Color color)
            {
                Cell = cell;
                WorldPosition = worldPosition;
                Sprite = sprite;
                Color = color;
            }
        }
    }
}
#endif
