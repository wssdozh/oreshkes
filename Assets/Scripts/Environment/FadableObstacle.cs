using UnityEngine;
using DG.Tweening;

public class FadableObstacle : MonoBehaviour, IFadable
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private FadableSettings _settings;

    private Material _material;
    private Tween _tween;
    private float _currentAlpha = 1f;
    private float _fadeDuration = 0.25f;
    private float _occludedAlpha = 0.35f;

    private void Awake()
    {
        _material = _renderer.material;

        if (_settings != null)
        {
            _fadeDuration = _settings.FadeDuration;
            _occludedAlpha = _settings.OccludedAlpha;
        }
    }

    public void OnOccluded()
    {
        FadeTo(_occludedAlpha);
    }

    public void OnVisible()
    {
        FadeTo(1f);
    }

    private void FadeTo(float targetAlpha)
    {
        if (_tween != null && _tween.IsActive() == true)
        {
            _tween.Kill();
        }

        _tween = _material.DOFade(targetAlpha, "_BaseColor", _fadeDuration)
            .OnUpdate(UpdateCachedAlpha);
    }

    private void UpdateCachedAlpha()
    {
        Color color = _material.GetColor("_BaseColor");
        _currentAlpha = color.a;
    }
}
