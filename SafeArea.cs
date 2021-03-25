using UnityEngine;
using UnityEngine.UI;
using Resolution = Build1.PostMVC.Extensions.Unity.ScriptableObjects.Resolution;

namespace Build1.PostMVC.Extensions.Unity.Components
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeArea : MonoBehaviour
    {
        [Header("Reference Resolution"), SerializeField] public  SafeAreaReferenceResolutionSource source;
        [SerializeField]                                 private Resolution                        resolution;
        [SerializeField]                                 private Vector2                           widthAndHeight;


        [Header("Applicable Offsets"), SerializeField] private Rect    applicableOffsetPercentage;
        [SerializeField]                               private RectInt applicableOffsetPixels;

        [Header("Unapplicable Offsets"), SerializeField] private Rect    unapplicableOffsetPercentage;
        [SerializeField]                                 private RectInt unapplicableOffsetPixels;

        private void Start()
        {
            if (resolution == null)
            {
                Debug.LogWarning("SafeArea: Reference resolution not set. Trying to find CanvasScaler...");

                var scaler = GetComponentInParent<CanvasScaler>();
                if (scaler == null)
                {
                    Debug.LogError("SafeArea: CanvasScaler not found. No reference resolution to use.");
                    return;
                }

                resolution = Resolution.FromVector2(scaler.referenceResolution);
                Debug.LogWarning($"SafeArea: CanvasScaler found. Reference resolution: {resolution}");
            }

            ApplySafeArea();
        }

        #if UNITY_EDITOR

        private SafeAreaReferenceResolutionSource _referenceResolutionSource;
        private Rect                              _lastSafeArea;
        private Vector2                           _referenceResolution;

        private Rect    _applicableOffsetPercentage;
        private RectInt _applicableOffsetPixels;

        private Rect    _unapplicableOffsetPercentage;
        private RectInt _unapplicableOffsetPixels;

        private void Update()
        {
            if (resolution == null)
                return;

            if (_lastSafeArea == Screen.safeArea &&
                _referenceResolutionSource == source &&
                _referenceResolution.Equals(resolution.ToVector2()) &&
                _applicableOffsetPercentage == applicableOffsetPercentage &&
                _applicableOffsetPixels.Equals(applicableOffsetPixels) &&
                _unapplicableOffsetPercentage == unapplicableOffsetPercentage &&
                _unapplicableOffsetPixels.Equals(unapplicableOffsetPixels))
                return;

            ApplySafeArea();

            _lastSafeArea = Screen.safeArea;

            _referenceResolutionSource = source;
            _referenceResolution = resolution.ToVector2();

            _applicableOffsetPercentage = applicableOffsetPercentage;
            _applicableOffsetPixels = applicableOffsetPixels;

            _unapplicableOffsetPercentage = unapplicableOffsetPercentage;
            _unapplicableOffsetPixels = unapplicableOffsetPixels;
        }

        #endif

        private void ApplySafeArea()
        {
            Debug.Log("ApplySafeArea");

            var safeArea = Screen.safeArea;
            var screenResolution = Screen.currentResolution;

            var scaleHeight = resolution.Height / screenResolution.height;
            var scaleWidth = resolution.Width / screenResolution.width;

            var topPixel = screenResolution.height - (safeArea.y + safeArea.height);
            var bottomPixels = safeArea.y;
            var leftPixels = safeArea.xMin;
            var rightPixels = -(screenResolution.width - safeArea.xMax);

            var topUnits = topPixel * scaleHeight * 1.2f;
            if (topPixel != 0)
                topUnits += applicableOffsetPixels.y + screenResolution.height * applicableOffsetPercentage.y;
            else
                topUnits += unapplicableOffsetPixels.y + screenResolution.height * unapplicableOffsetPercentage.y;

            var bottomUnits = bottomPixels * scaleHeight * 1.2F;
            if (bottomPixels != 0)
                bottomUnits += applicableOffsetPixels.height + screenResolution.height * applicableOffsetPercentage.height;
            else
                bottomUnits += unapplicableOffsetPixels.height + screenResolution.height * unapplicableOffsetPercentage.height;

            var leftUnits = leftPixels * scaleWidth;
            if (leftPixels != 0)
                leftUnits += applicableOffsetPixels.x + screenResolution.width * applicableOffsetPercentage.x;
            else
                leftUnits += unapplicableOffsetPixels.x + screenResolution.width * unapplicableOffsetPercentage.x;

            var rightUnits = rightPixels * scaleWidth;
            if (rightPixels != 0)
                rightUnits -= applicableOffsetPixels.width + screenResolution.width * applicableOffsetPercentage.width;
            else
                rightUnits -= unapplicableOffsetPixels.width + screenResolution.width * unapplicableOffsetPercentage.width;

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(leftUnits, bottomUnits);
            rectTransform.offsetMax = new Vector2(rightUnits, -topUnits);
        }
    }
}