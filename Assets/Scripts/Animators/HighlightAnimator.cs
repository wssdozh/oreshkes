using UnityEngine;

public class HighlightAnimator : MonoBehaviour
{
    [Header("Настройки подсветки")]
    [SerializeField] private Color _highlightColor = Color.yellow;
    [SerializeField] private float _blinkTime = 0.7f;

    private Renderer[] _renderers;
    private Color[] _originalColors;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
        _originalColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            Material material = _renderers[i].sharedMaterial;
            if (material.HasProperty("_BaseColor") == true)
            {
                _originalColors[i] = material.GetColor("_BaseColor");
            }
            else
            {
                _originalColors[i] = Color.white;
            }
        }
    }

    public virtual void Highlight(bool state)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (state == true)
            {
                Colorer.LerpToColor(_renderers[i], _highlightColor, _blinkTime);
            }
            else
            {
                Colorer.Stop(_renderers[i], _originalColors[i], _blinkTime);
            }
        }
    }
}
