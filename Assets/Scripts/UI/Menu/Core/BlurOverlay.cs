using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public sealed class BlurOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _image;
    [SerializeField] private float _showDurationSeconds = 0.45f;
    [SerializeField] private float _hideDurationSeconds = 0.30f;
    [SerializeField] private AnimationCurve _showEaseCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private AnimationCurve _hideEaseCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private bool _overrideMaterialProperties;
    [SerializeField] private float _iterations = 36.0f;
    [SerializeField] private float _escapeRadius = 4.0f;
    [SerializeField] private float _glow = 1.0f;
    [SerializeField] private float _alpha = 1.0f;
    [SerializeField] private float _scale = 2.7f;
    [SerializeField] private float _pixelResolution = 200.0f;

    private Tween _tween;
    private Material _runtimeMaterial;

    private void Awake()
    {
        InitializeMaterial();
        SetState(0.0f);
    }

    private void OnDestroy()
    {
        if (_runtimeMaterial == null)
        {
            return;
        }

        Destroy(_runtimeMaterial);
        _runtimeMaterial = null;
    }

    public void Show()
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;

        Animate(1.0f, _showDurationSeconds, _showEaseCurve);
    }

    public void ShowImmediate()
    {
        KillTween();

        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        SetState(1.0f);
    }

    public void Hide()
    {
        Animate(0.0f, _hideDurationSeconds, _hideEaseCurve);
    }

    public void HideImmediate()
    {
        KillTween();
        SetState(0.0f);

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private void Animate(float targetAlpha, float durationSeconds, AnimationCurve easeCurve)
    {
        KillTween();

        _tween = DOTween
            .To(() => _canvasGroup.alpha, value => _canvasGroup.alpha = value, targetAlpha, durationSeconds)
            .SetEase(easeCurve)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                SetState(targetAlpha);

                if (targetAlpha <= 0.0f)
                {
                    gameObject.SetActive(false);
                }
            });
    }

    private void KillTween()
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill(false);
        }
    }

    private void SetState(float alpha)
    {
        _canvasGroup.alpha = alpha;

        if (alpha <= 0.0f)
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            return;
        }

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    private void InitializeMaterial()
    {
        if (_overrideMaterialProperties == false)
        {
            return;
        }

        if (_image == null)
        {
            return;
        }

        if (_image.material == null)
        {
            return;
        }

        _runtimeMaterial = new Material(_image.material);
        ApplyMaterialOverrides(_runtimeMaterial);
        _image.material = _runtimeMaterial;
    }

    private void ApplyMaterialOverrides(Material material)
    {
        if (material.HasProperty("_Iterations"))
        {
            material.SetFloat("_Iterations", _iterations);
        }

        if (material.HasProperty("_EscapeRadius"))
        {
            material.SetFloat("_EscapeRadius", _escapeRadius);
        }

        if (material.HasProperty("_Glow"))
        {
            material.SetFloat("_Glow", _glow);
        }

        if (material.HasProperty("_Alpha"))
        {
            material.SetFloat("_Alpha", _alpha);
        }

        if (material.HasProperty("_Scale"))
        {
            material.SetFloat("_Scale", _scale);
        }

        if (material.HasProperty("_PixelResolution"))
        {
            material.SetFloat("_PixelResolution", _pixelResolution);
        }
    }
}
