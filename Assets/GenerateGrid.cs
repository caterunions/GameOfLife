using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

public class GenerateGrid : MonoBehaviour
{
    // width of the cell grid
    [SerializeField]
    private int _gridWidth;

    // height of the cell grid
    [SerializeField]
    private int _gridHeight;

    // time between steps
    private float _stepTime = 0.5f;

    [SerializeField]
    private Cell _cell;

    private Cell[,] _grid;

    private bool _looping = false;

    // cache coroutine
    private Coroutine _playRoutine = null;

    private void OnEnable()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        // initialize empty grid of size
        _grid = new Cell[_gridWidth, _gridHeight];

        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for (int j = 0; j < _grid.GetLength(1); j++) 
            {
                // instantiate where 0,0 is at the top left (i've been using p5js coordinates a lot so this is familiar to me)
                Vector2 gridPos = new Vector2(i,-j);
                _grid[i,j] = Instantiate(_cell, gridPos, Quaternion.identity);
            }
        }

        // center camera on grid
        Camera.main.transform.position = new Vector3(_gridWidth / 2 - 0.5f, -_gridHeight / 2 - 0.5f, Camera.main.transform.position.z);
        // resize camera to fit grid inside
        Camera.main.orthographicSize = (_gridHeight > _gridWidth ? _gridHeight / 2 : _gridWidth / 2) + 1;
    }

    public void GenerateNextStep()
    {
        for(int i = 0; i < _grid.GetLength(0); i++)
        {
            for(int j = 0; j < _grid.GetLength(1); j++)
            {
                Cell curCell = _grid[i,j];

                // get top 3 neighboring cells
                int trNeighbor = _grid[Wrap(i - 1, 0), Wrap(j - 1, 1)].Alive;
                int tNeighbor = _grid[Wrap(i - 1, 0), j].Alive;
                int tlNeighbor = _grid[Wrap(i - 1, 0), Wrap(j + 1, 1)].Alive;

                // get left and right neighbors
                int rNeighbor = _grid[i, Wrap(j - 1, 1)].Alive;
                int lNeighbor = _grid[i, Wrap(j + 1, 1)].Alive;

                // get bottom 3 neighboring cells
                int brNeighbor = _grid[Wrap(i + 1, 0), Wrap(j - 1, 1)].Alive;
                int bNeighbor = _grid[Wrap(i + 1, 0), j].Alive;
                int blNeighbor = _grid[Wrap(i + 1, 0), Wrap(j + 1, 1)].Alive;

                // add all neighbors
                int aliveNeighbors = trNeighbor + tNeighbor + tlNeighbor + rNeighbor + lNeighbor + brNeighbor + bNeighbor + blNeighbor;

                if(curCell.Alive == 1)
                {
                    // alive rules
                    if(aliveNeighbors > 1 && aliveNeighbors < 4)
                    {
                        _grid[i, j].NextAlive = 1;
                    }
                    else
                    {
                        _grid[i, j].NextAlive = 0;
                    }
                }
                else
                {
                    // dead rules
                    if(aliveNeighbors == 3)
                    {
                        _grid[i, j].NextAlive = 1;
                    }
                    else
                    {
                        _grid[i, j].NextAlive = 0;
                    }
                }
            }
        }

        foreach(Cell cell in _grid)
        {
            // update cells in grid
            cell.LastAlive = cell.Alive;
            cell.Alive = cell.NextAlive;
        }
    }

    private int Wrap(int index, int axis)
    {
        // returns the correct index for wrapping should it be out of bounds
        return (index + _grid.GetLength(axis)) % _grid.GetLength(axis);
    }

    public void Play()
    {
        // do not play if already playing
        if (_playRoutine == null)
        {
            // play
            _looping = true;
            _playRoutine = StartCoroutine(PlayRoutine());
        }
    }

    public void Stop()
    {
        // stop current coroutine
        _playRoutine = null;
        _looping = false;
    }

    public void ResetGrid()
    {
        foreach (Cell cell in _grid)
        {
            // reset values of all cells
            cell.LastAlive = 0;
            cell.Alive = 0;
            cell.NextAlive = 0;
        }
    }

    public void Randomize()
    {
        foreach (Cell cell in _grid)
        {
            // randomize cell state
            int rng = UnityEngine.Random.Range(0, 2);
            cell.LastAlive = 0;
            cell.NextAlive = 0;
            cell.Alive = rng;
        }
    }

    public void Fish()
    {
        ResetGrid();
        // do we have enough space to build a fish?
        if (_gridWidth < 7 || _gridHeight < 6)
        {
            Debug.LogWarning("Not enough room to build a fish!!");
            return;
        }

        // find an area around the middle of the canvas using the dimensions of a fish
        int startingX = Mathf.FloorToInt((_gridWidth - 6) / 2);
        int startingY = Mathf.FloorToInt((_gridHeight - 5) / 2);

        for(int x = 0; x < 5; x++)
        {
            for(int y = 0; y < 4; y++)
            {
                if(
                    // check fish blueprint
                    (x == 1 && y == 0) ||
                    (x == 4 && y == 0) || 
                    (x == 0 && y > 0) ||
                    (x < 4 && y == 3) ||
                    (x == 4 && y == 2)
                ) 
                {
                    // fill in that cell
                    _grid[startingX + x, startingY + y].Alive = 1;
                }
            }
        }
    }

    public void Eel()
    {
        ResetGrid();
        // do we have enough space to build an eel?
        if (_gridWidth < 17 || _gridHeight < 7)
        {
            Debug.LogWarning("Not enough room to build an eel!!");
            return;
        }

        // find an area around the middle of the canvas using the dimensions of an eel
        int startingX = Mathf.FloorToInt((_gridWidth - 16) / 2);
        int startingY = Mathf.FloorToInt((_gridHeight - 6) / 2);

        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 6; y++)
            {
                if (
                    // check eel blueprint
                    (x > 6 && x != 9 && x < 11 && y == 0) ||
                    (x > 3 && x != 6 && x != 8 && x != 11 && x < 15 && y == 1) ||
                    (x > 0 && x < 5 && y == 2) ||
                    ((x == 7 || x == 8 || x == 15) && y == 2) ||
                    ((x == 0 || x == 5 || x == 9 || x == 13 || x == 14) && y == 3) || 
                    ((x == 1 || x == 2) && y == 4)
                )
                {
                    // fill in that cell
                    _grid[startingX + x, startingY + y].Alive = 1;
                }
            }
        }
    }

    public void ChangeSpeed(Single value)
    {
        // set steps per second to the slider value
        _stepTime = 1 / value;
    }

    private IEnumerator PlayRoutine()
    {
        while(_looping)
        {
            yield return new WaitForSeconds(_stepTime);

            if(_looping) GenerateNextStep();
        }
        // failsafe to clear cached coroutine
        _playRoutine = null;
    }
}
