using System;
using UnityEngine;
using UnityEngine.UI;

namespace Build1.UnitySafeArea
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        [Header("Reference Resolution"), SerializeField] public ResolutionSource source;
        [SerializeField]                                 public Resolution       resolution;
        [SerializeField]                                 public Vector2          resolutionWidthAndHeight;

        [Header("Applicable Offsets"), SerializeField] private Rect    applicableOffsetPercentage;
        [SerializeField]                               private RectInt applicableOffsetPixels;

        [Header("Unapplicable Offsets"), SerializeField] private Rect    unapplicableOffsetPercentage;
        [SerializeField]                                 private RectInt unapplicableOffsetPixels;

        public bool CanvasScalerFound      => _canvasScaler != null;
        public bool ReferenceResolutionSet => resolution != null;

        private CanvasScaler _canvasScaler;
        private Vector2      _referenceResolution;

        private void Start()
        {
            if (source == ResolutionSource.CanvasScaler)
                UpdateCanvasScaler();
            UpdateReferenceResolution();
            ApplySafeArea();
        }

        public void UpdateCanvasScaler()
        {
            _canvasScaler = GetComponentInParent<CanvasScaler>();
        }

        public void UpdateReferenceResolution()
        {
            _referenceResolution = GetCurrentReferenceResolution();
        }

        public Vector2 GetCurrentReferenceResolution()
        {
            switch (source)
            {
                case ResolutionSource.CanvasScaler:
                    if (!CanvasScalerFound)
                        throw new Exception("SafeArea CanvasScaler not found.");
                    return _canvasScaler.referenceResolution;

                case ResolutionSource.Resolution:
                    if (!ReferenceResolutionSet)
                        throw new Exception("SafeArea Resolution not set.");
                    return resolution.ToVector2();

                case ResolutionSource.WidthAndHeight:
                    if (resolutionWidthAndHeight == Vector2.zero)
                        throw new Exception("SafeArea resolution values not set.");
                    return resolutionWidthAndHeight;

                default:
                    throw new Exception($"Not implemented for source: {source}");
            }
        }

        #if UNITY_EDITOR

        private Rect             _lastSafeArea;
        private ResolutionSource _lastResolutionSource;
        private Vector2          _lastResolution;

        private Rect    _applicableOffsetPercentage;
        private RectInt _applicableOffsetPixels;

        private Rect    _unapplicableOffsetPercentage;
        private RectInt _unapplicableOffsetPixels;

        private void Update()
        {
            UpdateReferenceResolution();
            
            if (_lastSafeArea == Screen.safeArea &&
                _lastResolutionSource == source &&
                _lastResolution == _referenceResolution &&
                _applicableOffsetPercentage == applicableOffsetPercentage &&
                _applicableOffsetPixels.Equals(applicableOffsetPixels) &&
                _unapplicableOffsetPercentage == unapplicableOffsetPercentage &&
                _unapplicableOffsetPixels.Equals(unapplicableOffsetPixels))
                return;

            ApplySafeArea();

            _lastSafeArea = Screen.safeArea;
            _lastResolutionSource = source;
            _lastResolution = _referenceResolution;

            _applicableOffsetPercentage = applicableOffsetPercentage;
            _applicableOffsetPixels = applicableOffsetPixels;

            _unapplicableOffsetPercentage = unapplicableOffsetPercentage;
            _unapplicableOffsetPixels = unapplicableOffsetPixels;
        }
        
        private void Reset()
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
        }
        
        #endif

        private void ApplySafeArea()
        {
            var safeArea = Screen.safeArea;
            
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            
            var scaleHeight = _referenceResolution.y / screenHeight;
            var scaleWidth = _referenceResolution.x / screenWidth;

            var topPixel = screenHeight - (safeArea.y + safeArea.height);
            var bottomPixels = safeArea.y;
            var leftPixels = safeArea.xMin;
            var rightPixels = -(screenWidth - safeArea.xMax);

            var topUnits = topPixel * scaleHeight * 1.2f;
            if (topPixel != 0)
                topUnits += applicableOffsetPixels.y + screenHeight * applicableOffsetPercentage.y;
            else
                topUnits += unapplicableOffsetPixels.y + screenHeight * unapplicableOffsetPercentage.y;

            var bottomUnits = bottomPixels * scaleHeight * 1.2F;
            if (bottomPixels != 0)
                bottomUnits += applicableOffsetPixels.height + screenHeight * applicableOffsetPercentage.height;
            else
                bottomUnits += unapplicableOffsetPixels.height + screenHeight * unapplicableOffsetPercentage.height;

            var leftUnits = leftPixels * scaleWidth;
            if (leftPixels != 0)
                leftUnits += applicableOffsetPixels.x + screenWidth * applicableOffsetPercentage.x;
            else
                leftUnits += unapplicableOffsetPixels.x + screenWidth * unapplicableOffsetPercentage.x;

            var rightUnits = rightPixels * scaleWidth;
            if (rightPixels != 0)
                rightUnits -= applicableOffsetPixels.width + screenWidth * applicableOffsetPercentage.width;
            else
                rightUnits -= unapplicableOffsetPixels.width + screenWidth * unapplicableOffsetPercentage.width;

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(leftUnits, bottomUnits);
            rectTransform.offsetMax = new Vector2(rightUnits, -topUnits);
        }
    }
}