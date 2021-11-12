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
        [Header("Orientation"), SerializeField]          private bool             monitorSafeAreaChange = true;

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

        private CanvasScaler  _canvasScaler;
        private RectTransform _canvasScalerRectTransform;
        private Vector2       _canvasScalerResolution;
        private Rect          _safeArea;

        private void Start()
        {
            // Canvas scaler will be the only option for now.
            // Other sources generate too many cases impossible to handle.

            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            _safeArea = Screen.safeArea;
            _canvasScaler = GetComponentInParent<CanvasScaler>();
            _canvasScalerRectTransform = _canvasScaler.GetComponent<RectTransform>();
            _canvasScalerResolution = _canvasScalerRectTransform.sizeDelta;
        }

        private void OnEnable()
        {
            ApplySafeArea();

            if (Application.isPlaying && monitorSafeAreaChange)
                StartCoroutine(SafeAreaCheckCoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator SafeAreaCheckCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);

                if (_safeArea == Screen.safeArea && _canvasScalerResolution == _canvasScalerRectTransform.sizeDelta)
                    continue;

                _safeArea = Screen.safeArea;
                _canvasScalerResolution = _canvasScalerRectTransform.sizeDelta;
                
                ApplySafeArea();
            }
        }

        #if UNITY_EDITOR

        private Vector2 _lastOffsetMin;
        private Vector2 _lastOffsetMax;

        private void Update()
        {
            if (Application.isPlaying)
                return;

            var offsetMin = CalculateOffsetMin();
            var offsetMax = CalculateOffsetMax();

            if (_lastOffsetMin == offsetMin &&
                _lastOffsetMax == offsetMax)
                return;

            _lastOffsetMin = offsetMin;
            _lastOffsetMax = offsetMax;

            ApplySafeArea();
        }

        private void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
        }

        public void UpdateCanvasScaler()
        {
            _canvasScaler = GetComponentInParent<CanvasScaler>();
            _canvasScalerResolution = _canvasScaler.GetComponent<RectTransform>().sizeDelta;
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

        private void ApplySafeArea()
        {
            var offsetMin = CalculateOffsetMin();
            var offsetMax = CalculateOffsetMax();

            rectTransform.offsetMin = new Vector2(offsetMin.x, offsetMin.y);
            rectTransform.offsetMax = new Vector2(offsetMax.x, offsetMax.y);
        }

        private Vector2 CalculateOffsetMin()
        {
            var offset = new Vector2();

            if (bottomApply)
            {
                var bottomPixels = Screen.safeArea.y;

                offset.y = bottomPixels / Screen.height * _canvasScalerResolution.y;

                if (bottomPixels != 0)
                    offset.y += bottomApplicableOffsetPixels + Screen.height * bottomApplicableOffsetPercentage;
                else
                    offset.y += bottomUnapplicableOffsetPixels + Screen.height * bottomUnapplicableOffsetPercentage;
            }

            if (leftApply)
            {
                var leftPixels = Screen.safeArea.xMin;

                offset.x = leftPixels / Screen.width * _canvasScalerResolution.x;

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

                offset.y = -(topPixel / Screen.height * _canvasScalerResolution.y);

                if (topPixel != 0)
                    offset.y -= topApplicableOffsetPixels + Screen.height * topApplicableOffsetPercentage;
                else
                    offset.y -= topUnapplicableOffsetPixels + Screen.height * topUnapplicableOffsetPercentage;
            }

            if (rightApply)
            {
                var rightPixels = -(Screen.width - Screen.safeArea.xMax);
                offset.x = rightPixels / Screen.width * _canvasScalerResolution.x;

                if (rightPixels != 0)
                    offset.x -= rightApplicableOffsetPixels + Screen.width * rightApplicableOffsetPercentage;
                else
                    offset.x -= rightUnapplicableOffsetPixels + Screen.width * rightUnapplicableOffsetPercentage;
            }

            return offset;
        }
    }
}