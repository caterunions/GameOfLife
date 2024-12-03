using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class to store a cell and the various states associated with it.
 * Could be split into a model/view in a larger project.
 */

public class Cell : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    // previous state
    public int LastAlive { get; set; } = 0;

    // current state
    public int Alive { get; set; } = 0;

    // future state (used for calculating the next step without affecting current cells)
    public int NextAlive { get; set; } = 0;

    // color for when cell wasn't alive but now is
    [SerializeField]
    private Color _newlyCreatedColor;

    // color for when cell was alive and now isn't
    [SerializeField]
    private Color _newlyDeadColor;

    // color for when cell was alive and still is
    [SerializeField]
    private Color _aliveColor;

    // color for when cell was dead and still is
    [SerializeField]
    private Color _deadColor;

    private void OnMouseOver()
    {
        // paint cells with LMB
        if (Input.GetMouseButton(0)) Alive = 1;
        // erase cells with RMB
        else if (Input.GetMouseButton(1)) Alive = 0;
    }

    private void Update()
    {
        // display cell state with color
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
