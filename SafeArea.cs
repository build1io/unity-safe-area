using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Build1.UnitySafeArea
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        [Header("Parts"), SerializeField]                public  RectTransform    rectTransform;
        [Header("Reference Resolution"), SerializeField] public  ResolutionSource source                = ResolutionSource.CanvasScaler;
        [Header("Update"), SerializeField]               private bool             monitorSafeAreaChange = true;

        [Header("Top"), SerializeField] private bool  topApply;
        [SerializeField]                private float topApplicableOffsetPercentage;
        [SerializeField]                private float topApplicableOffsetPixels;
        [SerializeField]                private float topUnapplicableOffsetPercentage;
        [SerializeField]                private float topUnapplicableOffsetPixels;

        [Header("Bottom"), SerializeField] private bool  bottomApply;
        [SerializeField]                   private float bottomApplicableOffsetPercentage;
        [SerializeField]                   private float bottomApplicableOffsetPixels;
        [SerializeField]                   private float bottomUnapplicableOffsetPercentage;
        [SerializeField]                   private float bottomUnapplicableOffsetPixels;

        [Header("Left"), SerializeField] private bool  leftApply;
        [SerializeField]                 private float leftApplicableOffsetPercentage;
        [SerializeField]                 private float leftApplicableOffsetPixels;
        [SerializeField]                 private float leftUnapplicableOffsetPercentage;
        [SerializeField]                 private float leftUnapplicableOffsetPixels;

        [Header("Right"), SerializeField] private bool  rightApply;
        [SerializeField]                  private float rightApplicableOffsetPercentage;
        [SerializeField]                  private float rightApplicableOffsetPixels;
        [SerializeField]                  private float rightUnapplicableOffsetPercentage;
        [SerializeField]                  private float rightUnapplicableOffsetPixels;

        private bool                       _initialized;
        private CanvasScaler               _canvasScaler;
        private RectTransform              _canvasScalerRectTransform;
        private Vector2                    _lastOffsetMin;
        private Vector2                    _lastOffsetMax;
        private DrivenRectTransformTracker _tracker;

        private void Start()
        {
            Initialize();
            OnEnableImpl();
        }

        private void OnEnable()
        {
            if (_initialized)
                OnEnableImpl();
        }

        private void OnDisable()
        {
            _tracker.Clear();
            
            StopAllCoroutines();
        }

        #if UNITY_EDITOR

        private void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5F, 0.5F);
        }

        private void Update()
        {
            if (Application.isPlaying)
                return;
            
            var offsetMin = CalculateOffsetMin();
            var offsetMax = CalculateOffsetMax();

            if (_lastOffsetMin != offsetMin || _lastOffsetMax != offsetMax)
                ApplySafeAreaImpl(offsetMin, offsetMax);
        }

        private void OnValidate()
        {
            _lastOffsetMin = default;
            _lastOffsetMax = default;
        }

        /*
         * Public.
         */

        public void UpdateCanvasScaler()
        {
            _canvasScaler = GetComponentInParent<CanvasScaler>();
        }

        public bool CheckCanvasScalerFound()
        {
            return _canvasScaler != null;
        }

        public Vector2 GetReferenceResolution()
        {
            return _canvasScaler.referenceResolution;
        }

        #endif

        /*
         * Private.
         */

        private void Initialize()
        {
            if (_initialized)
                return;

            if (!rectTransform)
                rectTransform = GetComponent<RectTransform>();
            
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5F, 0.5F);
            
            _canvasScaler = GetComponentInParent<CanvasScaler>();
            _canvasScalerRectTransform = _canvasScaler.GetComponent<RectTransform>();
            
            _initialized = true;
        }

        private void OnEnableImpl()
        {
            _tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta | 
                                              DrivenTransformProperties.Anchors | 
                                              DrivenTransformProperties.AnchoredPosition3D | 
                                              DrivenTransformProperties.Pivot);
            
            var offsetMin = CalculateOffsetMin();
            var offsetMax = CalculateOffsetMax();
            
            ApplySafeAreaImpl(offsetMin, offsetMax);

            if (Application.isPlaying && monitorSafeAreaChange)
                StartCoroutine(SafeAreaCheckCoroutine());
        }

        private IEnumerator SafeAreaCheckCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.05f);

                var offsetMin = CalculateOffsetMin();
                var offsetMax = CalculateOffsetMax();
                
                if (_lastOffsetMin != offsetMin || _lastOffsetMax != offsetMax)
                    ApplySafeAreaImpl(offsetMin, offsetMax);
            }
        }

        private void ApplySafeAreaImpl(Vector2 offsetMin, Vector2 offsetMax)
        {
            rectTransform.offsetMin = new Vector2(offsetMin.x, offsetMin.y);
            rectTransform.offsetMax = new Vector2(offsetMax.x, offsetMax.y);

            _lastOffsetMin = offsetMin;
            _lastOffsetMax = offsetMax;
        }

        private Vector2 CalculateOffsetMin()
        {
            var offset = new Vector2();

            if (bottomApply)
            {
                var bottomPixels = Screen.safeArea.y;

                offset.y = bottomPixels / Screen.height * _canvasScalerRectTransform.sizeDelta.y;

                if (bottomPixels != 0)
                    offset.y += bottomApplicableOffsetPixels + Screen.height * bottomApplicableOffsetPercentage;
                else
                    offset.y += bottomUnapplicableOffsetPixels + Screen.height * bottomUnapplicableOffsetPercentage;
            }

            if (leftApply)
            {
                var leftPixels = Screen.safeArea.xMin;

                offset.x = leftPixels / Screen.width * _canvasScalerRectTransform.sizeDelta.x;

                if (leftPixels != 0)
                    offset.x += leftApplicableOffsetPixels + Screen.width * leftApplicableOffsetPercentage;
                else
                    offset.x += leftUnapplicableOffsetPixels + Screen.width * leftUnapplicableOffsetPercentage;
            }

            return offset;
        }

        private Vector2 CalculateOffsetMax()
        {
            var offset = new Vector2();

            if (topApply)
            {
                var topPixel = Math.Abs(Screen.safeArea.y + Screen.safeArea.height - Screen.height);

                offset.y = -(topPixel / Screen.height * _canvasScalerRectTransform.sizeDelta.y);

                if (topPixel != 0)
                    offset.y -= topApplicableOffsetPixels + Screen.height * topApplicableOffsetPercentage;
                else
                    offset.y -= topUnapplicableOffsetPixels + Screen.height * topUnapplicableOffsetPercentage;
            }

            if (rightApply)
            {
                var rightPixels = -(Screen.width - Screen.safeArea.xMax);
                offset.x = rightPixels / Screen.width * _canvasScalerRectTransform.sizeDelta.x;

                if (rightPixels != 0)
                    offset.x -= rightApplicableOffsetPixels + Screen.width * rightApplicableOffsetPercentage;
                else
                    offset.x -= rightUnapplicableOffsetPixels + Screen.width * rightUnapplicableOffsetPercentage;
            }

            return offset;
        }
    }
}