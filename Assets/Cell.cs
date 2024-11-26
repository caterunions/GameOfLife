using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public int LastAlive { get; set; } = 0;

    public int Alive { get; set; } = 0;

    public int NextAlive { get; set; } = 0;

    [SerializeField]
    private Color _newlyCreatedColor;
    [SerializeField]
    private Color _newlyDeadColor;
    [SerializeField]
    private Color _aliveColor;
    [SerializeField]
    private Color _deadColor;

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0)) Alive = 1;
        else if (Input.GetMouseButton(1)) Alive = 0;
    }

    private void Update()
    {
        if(LastAlive == 0 && Alive == 1)
        {
            _spriteRenderer.color = _newlyCreatedColor;
        }
        else if(LastAlive == 1 && Alive == 0)
        {
            _spriteRenderer.color = _newlyDeadColor;
        }
        else if(Alive == 1)
        {
            _spriteRenderer.color = _aliveColor;
        }
        else
        {
            _spriteRenderer.color = _deadColor;
        }
    }
}
