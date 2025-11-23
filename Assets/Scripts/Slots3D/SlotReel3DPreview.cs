using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Waving.SlotGame.Slots3D
{
    [ExecuteAlways]
    public class SlotReel3DPreview : MonoBehaviour
    {
        [SerializeField] private Material _segmentMaterial;
        [SerializeField] private List<Texture2D> _symbolTextures = new List<Texture2D>();
        [SerializeField] private float _radius = 0.35f;
        [SerializeField] private float _segmentHeight = 0.65f;
        [SerializeField] private float _segmentWidth = 0.55f;
        [SerializeField] private int _segmentCount = 6;
        [SerializeField] private float _spinDuration = 1.8f;
        [SerializeField] private AnimationCurve _spinCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private bool _autoBuildSegments = true;
        [SerializeField] private bool _autoPlayOnEnable = true;
        [SerializeField] private int _previewTargetIndex;
        [SerializeField] private int _previewExtraRounds = 2;

        private readonly List<ReelSegment> _segments = new List<ReelSegment>();
        private Transform _segmentRoot;
        private float _currentAngle;
        private CancellationTokenSource _spinCancellation;
        private static Mesh _sharedQuadMesh;
        private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");
        private const string SegmentRootName = "ReelSegmentsRoot";

        private void OnEnable()
        {
            ClampSettings();
            EnsureSegmentRoot();
            if (_autoBuildSegments && _segments.Count != _segmentCount)
            {
                RebuildSegments();
            }
            else
            {
                ApplySegmentLayout();
                RefreshSymbols();
            }

            ApplyAngle(_currentAngle);

            if (Application.isPlaying && _autoPlayOnEnable)
            {
                PlayPreview();
            }
        }

        private void OnDisable()
        {
            CancelSpin();
        }

        private void Reset()
        {
            _autoBuildSegments = true;
            _autoPlayOnEnable = true;
            _segmentCount = 6;
            _previewExtraRounds = 2;
        }

        private void OnValidate()
        {
            ClampSettings();
            if (_autoBuildSegments)
            {
                RebuildSegments();
            }
            else
            {
                ApplySegmentLayout();
                RefreshSymbols();
            }
        }

        [ContextMenu("Rebuild Segments")]
        public void RebuildSegments()
        {
            ClampSettings();
            EnsureSegmentRoot();
            ClearSegmentsInternal();

            if (_segmentCount <= 0)
            {
                return;
            }

            float perStep = 360f / _segmentCount;
            for (int i = 0; i < _segmentCount; i++)
            {
                ReelSegment segment = CreateSegment(i, perStep);
                _segments.Add(segment);
            }

            RefreshSymbols();
            ApplyAngle(_currentAngle);
        }

        public async UniTask SpinToIndexAsync(int targetIndex, int extraRounds, CancellationToken token)
        {
            if (_segments.Count == 0)
            {
                return;
            }

            extraRounds = Mathf.Max(0, extraRounds);
            CancelSpin();
            CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            _spinCancellation = linkedSource;
            CancellationToken linkedToken = linkedSource.Token;

            float startAngle = _currentAngle;
            float endAngle = CalculateTargetAngle(targetIndex, extraRounds);
            float duration = Mathf.Max(0.1f, _spinDuration);
            float elapsed = 0f;

            try
            {
                while (elapsed < duration)
                {
                    linkedToken.ThrowIfCancellationRequested();
                    float deltaTime = Application.isPlaying ? Time.deltaTime : 0.016f;
                    elapsed += deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    float eased = _spinCurve != null ? _spinCurve.Evaluate(t) : t;
                    float angle = Mathf.Lerp(startAngle, endAngle, eased);
                    ApplyAngle(angle);
                    await UniTask.Yield(PlayerLoopTiming.Update, linkedToken);
                }

                ApplyAngle(endAngle);
            }
            finally
            {
                linkedSource.Dispose();
                _spinCancellation = null;
            }
        }

        public void SnapToIndex(int index)
        {
            if (_segments.Count == 0)
            {
                return;
            }

            CancelSpin();
            int normalizedIndex = NormalizeIndex(index);
            float perStep = 360f / _segments.Count;
            float angle = -normalizedIndex * perStep;
            ApplyAngle(angle);
        }

        public void PlayPreview()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            CancellationToken token = this.GetCancellationTokenOnDestroy();
            _ = SpinToIndexAsync(_previewTargetIndex, _previewExtraRounds, token);
        }

        private void EnsureSegmentRoot()
        {
            if (_segmentRoot != null)
            {
                return;
            }

            Transform found = transform.Find(SegmentRootName);
            if (found != null)
            {
                _segmentRoot = found;
                return;
            }

            GameObject rootObject = new GameObject(SegmentRootName);
            _segmentRoot = rootObject.transform;
            _segmentRoot.SetParent(transform, false);
            _segmentRoot.localPosition = Vector3.zero;
            _segmentRoot.localRotation = Quaternion.identity;
        }

        private void ClearSegmentsInternal()
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                DestroySegment(_segments[i]);
            }

            _segments.Clear();
        }

        private void DestroySegment(ReelSegment segment)
        {
            if (segment == null || segment.Pivot == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(segment.Pivot.gameObject);
            }
            else
            {
                DestroyImmediate(segment.Pivot.gameObject);
            }
        }

        private ReelSegment CreateSegment(int index, float perStep)
        {
            string pivotName = string.Format("SegmentPivot_{0:00}", index);
            GameObject pivotObject = new GameObject(pivotName);
            pivotObject.transform.SetParent(_segmentRoot, false);
            pivotObject.transform.localPosition = Vector3.zero;
            pivotObject.transform.localRotation = Quaternion.Euler(perStep * index, 0f, 0f);

            string visualName = string.Format("SegmentVisual_{0:00}", index);
            GameObject visualObject = new GameObject(visualName);
            visualObject.transform.SetParent(pivotObject.transform, false);
            visualObject.transform.localPosition = new Vector3(0f, 0f, _radius);
            visualObject.transform.localRotation = Quaternion.identity;
            visualObject.transform.localScale = new Vector3(_segmentWidth, _segmentHeight, 1f);

            MeshFilter filter = visualObject.AddComponent<MeshFilter>();
            filter.sharedMesh = GetSharedQuadMesh();

            MeshRenderer renderer = visualObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = _segmentMaterial;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            ReelSegment segment = new ReelSegment(pivotObject.transform, visualObject.transform, renderer);
            return segment;
        }

        private void ApplySegmentLayout()
        {
            if (_segments.Count == 0)
            {
                return;
            }

            float perStep = 360f / _segments.Count;
            for (int i = 0; i < _segments.Count; i++)
            {
                ReelSegment segment = _segments[i];
                if (segment == null)
                {
                    continue;
                }

                if (segment.Pivot != null)
                {
                    segment.Pivot.localPosition = Vector3.zero;
                    segment.Pivot.localRotation = Quaternion.Euler(perStep * i, 0f, 0f);
                }

                if (segment.Visual != null)
                {
                    segment.Visual.localPosition = new Vector3(0f, 0f, _radius);
                    segment.Visual.localRotation = Quaternion.identity;
                    segment.Visual.localScale = new Vector3(_segmentWidth, _segmentHeight, 1f);
                }
            }
        }

        private void RefreshSymbols()
        {
            if (_segments.Count == 0)
            {
                return;
            }

            if (_symbolTextures == null || _symbolTextures.Count == 0)
            {
                for (int i = 0; i < _segments.Count; i++)
                {
                    ApplyTexture(_segments[i], null);
                }

                return;
            }

            for (int i = 0; i < _segments.Count; i++)
            {
                Texture2D texture = _symbolTextures[i % _symbolTextures.Count];
                ApplyTexture(_segments[i], texture);
            }
        }

        private void ApplyTexture(ReelSegment segment, Texture2D texture)
        {
            if (segment == null || segment.Renderer == null)
            {
                return;
            }

            if (segment.PropertyBlock == null)
            {
                segment.PropertyBlock = new MaterialPropertyBlock();
            }

            segment.PropertyBlock.Clear();
            segment.PropertyBlock.SetTexture(MainTexId, texture);
            segment.PropertyBlock.SetTexture(BaseMapId, texture);
            segment.Renderer.SetPropertyBlock(segment.PropertyBlock);
        }

        private void ApplyAngle(float angle)
        {
            _currentAngle = angle;
            if (_segmentRoot != null)
            {
                _segmentRoot.localRotation = Quaternion.Euler(angle, 0f, 0f);
            }
        }

        private float CalculateTargetAngle(int targetIndex, int extraRounds)
        {
            if (_segments.Count == 0)
            {
                return _currentAngle;
            }

            int count = _segments.Count;
            int normalizedTarget = NormalizeIndex(targetIndex);
            int currentIndex = GetFrontIndex();
            int stepsToTarget = normalizedTarget - currentIndex;
            if (stepsToTarget < 0)
            {
                stepsToTarget += count;
            }

            int totalSteps = stepsToTarget + extraRounds * count;
            float perStep = 360f / count;
            float deltaAngle = perStep * totalSteps;
            return _currentAngle - deltaAngle;
        }

        private int GetFrontIndex()
        {
            if (_segments.Count == 0)
            {
                return 0;
            }

            float perStep = 360f / _segments.Count;
            float normalized = Mathf.Repeat(-_currentAngle, 360f);
            int index = Mathf.RoundToInt(normalized / perStep) % _segments.Count;
            if (index < 0)
            {
                index += _segments.Count;
            }

            return index;
        }

        private int NormalizeIndex(int index)
        {
            if (_segments.Count == 0)
            {
                return 0;
            }

            int count = _segments.Count;
            int normalized = index % count;
            if (normalized < 0)
            {
                normalized += count;
            }

            return normalized;
        }

        private void CancelSpin()
        {
            if (_spinCancellation == null)
            {
                return;
            }

            _spinCancellation.Cancel();
            _spinCancellation.Dispose();
            _spinCancellation = null;
        }

        private static Mesh GetSharedQuadMesh()
        {
            if (_sharedQuadMesh != null)
            {
                return _sharedQuadMesh;
            }

            Mesh mesh = new Mesh();
            mesh.name = "SlotReelQuad";
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(-0.5f, -0.5f, 0f);
            vertices[1] = new Vector3(0.5f, -0.5f, 0f);
            vertices[2] = new Vector3(-0.5f, 0.5f, 0f);
            vertices[3] = new Vector3(0.5f, 0.5f, 0f);
            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0f, 0f);
            uv[1] = new Vector2(1f, 0f);
            uv[2] = new Vector2(0f, 1f);
            uv[3] = new Vector2(1f, 1f);
            int[] triangles = new int[6];
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 1;
            triangles[3] = 2;
            triangles[4] = 3;
            triangles[5] = 1;
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            _sharedQuadMesh = mesh;
            return _sharedQuadMesh;
        }

        private void ClampSettings()
        {
            _segmentCount = Mathf.Max(3, _segmentCount);
            _radius = Mathf.Max(0.05f, _radius);
            _segmentHeight = Mathf.Max(0.05f, _segmentHeight);
            _segmentWidth = Mathf.Max(0.05f, _segmentWidth);
            _spinDuration = Mathf.Max(0.1f, _spinDuration);
            _previewExtraRounds = Mathf.Max(0, _previewExtraRounds);
        }

        private class ReelSegment
        {
            public Transform Pivot;
            public Transform Visual;
            public MeshRenderer Renderer;
            public MaterialPropertyBlock PropertyBlock;

            public ReelSegment(Transform pivot, Transform visual, MeshRenderer renderer)
            {
                Pivot = pivot;
                Visual = visual;
                Renderer = renderer;
            }
        }
    }
}
