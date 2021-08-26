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

        private bool  _lastTopApply;
        private float _lastTopApplicableOffsetPercentage;
        private float _lastTopApplicableOffsetPixels;
        private float _lastTopUnapplicableOffsetPercentage;
        private float _lastTopUnapplicableOffsetPixels;
        
        private bool  _lastBottomApply;
        private float _lastBottomApplicableOffsetPercentage;
        private float _lastBottomApplicableOffsetPixels;
        private float _lastBottomUnapplicableOffsetPercentage;
        private float _lastBottomUnapplicableOffsetPixels;
        
        private bool  _lastLeftApply;
        private float _lastLeftApplicableOffsetPercentage;
        private float _lastLeftApplicableOffsetPixels;
        private float _lastLeftUnapplicableOffsetPercentage;
        private float _lastLeftUnapplicableOffsetPixels;
        
        private bool  _lastRightApply;
        private float _lastRightApplicableOffsetPercentage;
        private float _lastRightApplicableOffsetPixels;
        private float _lastRightUnapplicableOffsetPercentage;
        private float _lastRightUnapplicableOffsetPixels;

        private void Update()
        {
            UpdateReferenceResolution();

            if (_lastSafeArea == Screen.safeArea &&
                _lastResolutionSource == source &&
                _lastResolution == _referenceResolution &&
                
                _lastTopApply == topApply &&
                _lastTopApplicableOffsetPercentage == topApplicableOffsetPercentage &&
                _lastTopApplicableOffsetPixels == topApplicableOffsetPixels &&
                _lastTopUnapplicableOffsetPercentage == topUnapplicableOffsetPercentage &&
                _lastTopUnapplicableOffsetPixels == topUnapplicableOffsetPixels &&
                
                _lastBottomApply == bottomApply &&
                _lastBottomApplicableOffsetPercentage == bottomApplicableOffsetPercentage &&
                _lastBottomApplicableOffsetPixels == bottomApplicableOffsetPixels &&
                _lastBottomUnapplicableOffsetPercentage == bottomUnapplicableOffsetPercentage &&
                _lastBottomUnapplicableOffsetPixels == bottomUnapplicableOffsetPixels &&
                
                _lastLeftApply == leftApply &&
                _lastLeftApplicableOffsetPercentage == leftApplicableOffsetPercentage &&
                _lastLeftApplicableOffsetPixels == leftApplicableOffsetPixels &&
                _lastLeftUnapplicableOffsetPercentage == leftUnapplicableOffsetPercentage &&
                _lastLeftUnapplicableOffsetPixels == leftUnapplicableOffsetPixels &&
                
                _lastRightApply == rightApply &&
                _lastRightApplicableOffsetPercentage == rightApplicableOffsetPercentage &&
                _lastRightApplicableOffsetPixels == rightApplicableOffsetPixels &&
                _lastRightUnapplicableOffsetPercentage == rightUnapplicableOffsetPercentage &&
                _lastRightUnapplicableOffsetPixels == rightUnapplicableOffsetPixels)
                return;

            ApplySafeArea();

            _lastSafeArea = Screen.safeArea;
            _lastResolutionSource = source;
            _lastResolution = _referenceResolution;

            _lastTopApply = topApply;
            _lastTopApplicableOffsetPercentage = topApplicableOffsetPercentage;
            _lastTopApplicableOffsetPixels = topApplicableOffsetPixels;
            _lastTopUnapplicableOffsetPercentage = topUnapplicableOffsetPercentage;
            _lastTopUnapplicableOffsetPixels = topUnapplicableOffsetPixels;

            _lastBottomApply = bottomApply;
            _lastBottomApplicableOffsetPercentage = bottomApplicableOffsetPercentage;
            _lastBottomApplicableOffsetPixels = bottomApplicableOffsetPixels;
            _lastBottomUnapplicableOffsetPercentage = bottomUnapplicableOffsetPercentage;
            _lastBottomUnapplicableOffsetPixels = bottomUnapplicableOffsetPixels;
            
            _lastLeftApply = leftApply;
            _lastLeftApplicableOffsetPercentage = leftApplicableOffsetPercentage;
            _lastLeftApplicableOffsetPixels = leftApplicableOffsetPixels;
            _lastLeftUnapplicableOffsetPercentage = leftUnapplicableOffsetPercentage;
            _lastLeftUnapplicableOffsetPixels = leftUnapplicableOffsetPixels;
            
            _lastRightApply = rightApply;
            _lastRightApplicableOffsetPercentage = rightApplicableOffsetPercentage;
            _lastRightApplicableOffsetPixels = rightApplicableOffsetPixels;
            _lastRightUnapplicableOffsetPercentage = rightUnapplicableOffsetPercentage;
            _lastRightUnapplicableOffsetPixels = rightUnapplicableOffsetPixels;
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

            var topUnits = 0F;
            var bottomUnits = 0F;
            var leftUnits = 0F;
            var rightUnits = 0F;

            if (topApply)
            {
                topUnits = topPixel * scaleHeight * 1.2f;
                if (topPixel != 0)
                    topUnits += topApplicableOffsetPixels + screenHeight * topApplicableOffsetPercentage;
                else
                    topUnits += topUnapplicableOffsetPixels + screenHeight * topUnapplicableOffsetPercentage;
            }

            if (bottomApply)
            {
                bottomUnits = bottomPixels * scaleHeight * 1.2F;
                if (bottomPixels != 0)
                    bottomUnits += bottomApplicableOffsetPixels + screenHeight * bottomApplicableOffsetPercentage;
                else
                    bottomUnits += bottomUnapplicableOffsetPixels + screenHeight * bottomUnapplicableOffsetPercentage;
            }

            if (leftApply)
            {
                leftUnits = leftPixels * scaleWidth;
                if (leftPixels != 0)
                    leftUnits += leftApplicableOffsetPixels + screenWidth * leftApplicableOffsetPercentage;
                else
                    leftUnits += leftUnapplicableOffsetPixels + screenWidth * leftUnapplicableOffsetPercentage;
            }

            if (leftApply)
            {
                rightUnits = rightPixels * scaleWidth;
                if (rightPixels != 0)
                    rightUnits -= rightApplicableOffsetPixels + screenWidth * rightApplicableOffsetPercentage;
                else
                    rightUnits -= rightUnapplicableOffsetPixels + screenWidth * rightUnapplicableOffsetPercentage;
            }

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(leftUnits, bottomUnits);
            rectTransform.offsetMax = new Vector2(rightUnits, -topUnits);
        }
    }
}