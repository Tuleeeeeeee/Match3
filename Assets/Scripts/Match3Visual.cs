using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tuleeeeee.Utilities;
using Tuleeeeee.Enum;
using Tuleeeeee.GridSystem;
using System;

public class Match3Visual : MonoBehaviour
{

    public event EventHandler OnStateChanged;
    [SerializeField] private Transform pfGemGridVisual;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Match3 match3;

    private Grid<GemGridPosition> grid;
    private Dictionary<GemGrid, GemGridVisual> gemGridDictionary;

    private bool isSetup;
    private GameState state;
    private float busyTimer;
    private Action onBusyTimerElapsedAction;
    private int startDragX;
    private int startDragY;
    private Vector3 startDragMouseWorldPosition;

    void Awake()
    {
        state = GameState.Busy;
        isSetup = false;

        match3.OnLevelSet += Match3_OnLevelSet;

    }

    private void Match3_OnLevelSet(object sender, OnLevelSetEventArgs e)
    {
        FunctionTimer.Create(() =>
        {
            Setup(sender as Match3, e.grid);
        }, .1f);
    }

    public void Setup(Match3 match3, Grid<GemGridPosition> grid)
    {
        this.match3 = match3;
        this.grid = grid;

        float cameraYOffset = 1f;
        Vector3 gridCenter = grid.GetWorldPosition(grid.GetWidth() / 2, grid.GetHeight() / 2);
        cameraTransform.position = new Vector3(gridCenter.x, gridCenter.y + cameraYOffset, cameraTransform.position.z);
        // cameraTransform.position = new Vector3(grid.GetWidth() * .5f, grid.GetHeight() * .5f + cameraYOffset, cameraTransform.position.z);

        match3.OnGemGridPositionDestroyed += Match3_OnGemGridPositionDestroyed;
        match3.OnNewGemGridSpawned += Match3_OnNewGemGridSpawned;

        // Initialize Visual
        gemGridDictionary = new Dictionary<GemGrid, GemGridVisual>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                GemGridPosition gemGridPosition = grid.GetGridObject(x, y);

                GemGrid gemGrid = gemGridPosition.GetGemGrid();

                Vector3 position = grid.GetWorldPosition(x, y);
                position = new Vector3(position.x, 12);

                // Visual Transform
                Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, position, Quaternion.identity);
                gemGridVisualTransform.GetComponentInChildren<SpriteRenderer>().sprite = gemGrid.GetGem().gemSprite;

                GemGridVisual gemGridVisual = new GemGridVisual(gemGridVisualTransform, gemGrid);

                gemGridDictionary[gemGrid] = gemGridVisual;

            }
        }

        SetBusyState(.5f, () => SetState(GameState.TryFindMatches));

        isSetup = true;
    }


    private void Update()
    {
        if (!isSetup) return;

        UpdateVisual();

        switch (state)
        {
            case GameState.Busy:
                busyTimer -= Time.deltaTime;
                if (busyTimer <= 0f)
                {
                    onBusyTimerElapsedAction();
                }
                break;
            case GameState.WaitingForUser:
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mouseWorldPosition = HelplerUtilities.GetMouseWorldPosition();
                    grid.GetXY(mouseWorldPosition, out startDragX, out startDragY);
                    startDragMouseWorldPosition = mouseWorldPosition;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 mouseWorldPosition = HelplerUtilities.GetMouseWorldPosition();
                    grid.GetXY(mouseWorldPosition, out int x, out int y);
                    if (x != startDragX)
                    {
                        // Different X
                        y = startDragY;

                        if (x < startDragX)
                        {
                            x = startDragX - 1;
                        }
                        else
                        {
                            x = startDragX + 1;
                        }
                    }
                    else
                    {
                        // Different Y
                        x = startDragX;

                        if (y < startDragY)
                        {
                            y = startDragY - 1;
                        }
                        else
                        {
                            y = startDragY + 1;
                        }
                    }

                    if (match3.CanSwapGridPositions(startDragX, startDragY, x, y))
                    {
                        SwapGridPositions(startDragX, startDragY, x, y);
                    }
                }


                break;
            case GameState.TryFindMatches:
                if (match3.TryFindMatchesAndDestroyThem())
                {
                    SetBusyState(.3f, () =>
                    {
                        match3.FallGemsIntoEmptyPositions();

                        SetBusyState(.3f, () =>
                        {
                            match3.SpawnNewMissingGridPositions();

                            SetBusyState(.5f, () => SetState(GameState.TryFindMatches));
                        });
                    });
                }
                else
                {
                    TrySetStateWaitingForUser();
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void UpdateVisual()
    {
        foreach (GemGrid gemGrid in gemGridDictionary.Keys)
        {
            gemGridDictionary[gemGrid].Update();
        }
    }

    public void SwapGridPositions(int startX, int startY, int endX, int endY)
    {
        match3.SwapGridPositions(startX, startY, endX, endY);
        match3.UseMove();

        SetBusyState(.5f, () => SetState(GameState.TryFindMatches));
    }

    private void Match3_OnNewGemGridSpawned(object sender, OnNewGemGridSpawnedEventArgs e)
    {
        Vector3 position = e.gemGridPosition.GetWorldPosition();
        position = new Vector3(position.x, 12);

        Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, position, Quaternion.identity);
        gemGridVisualTransform.GetComponentInChildren<SpriteRenderer>().sprite = e.gemGrid.GetGem().gemSprite;

        GemGridVisual gemGridVisual = new GemGridVisual(gemGridVisualTransform, e.gemGrid);

        gemGridDictionary[e.gemGrid] = gemGridVisual;
    }

    private void Match3_OnGemGridPositionDestroyed(object sender, EventArgs e)
    {
        GemGridPosition gemGridPosition = sender as GemGridPosition;
        if (gemGridPosition != null && gemGridPosition.GetGemGrid() != null)
        {
            gemGridDictionary.Remove(gemGridPosition.GetGemGrid());
        }
    }
    private void TrySetStateWaitingForUser()
    {
        if (match3.TryIsGameOver())
        {
            // Game Over!
            Debug.Log("Game Over!");
            SetState(GameState.GameOver);
        }
        else
        {
            // Keep Playing
            SetState(GameState.WaitingForUser);
        }
    }
    private void SetBusyState(float busyTimer, Action onBusyTimerElapsedAction)
    {
        SetState(GameState.Busy);
        this.busyTimer = busyTimer;
        this.onBusyTimerElapsedAction = onBusyTimerElapsedAction;
    }
    private void SetState(GameState state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }
    public GameState GetState()
    {
        return state;
    }

    public class GemGridVisual
    {
        private Transform gemTransform;
        private GemGrid gemGrid;

        public GemGridVisual(Transform gemTransform, GemGrid gemGrid)
        {
            this.gemTransform = gemTransform;
            this.gemGrid = gemGrid;

            gemGrid.OnDestroyed += GemGrid_OnDestroyed;
        }

        private void GemGrid_OnDestroyed(object sender, System.EventArgs e)
        {
            gemTransform.GetComponent<Animation>().Play();
            Destroy(gemTransform.gameObject, 1f);
        }

        public void Update()
        {
            Vector3 targetPosition = gemGrid.GetWorldPosition();
            Vector3 moveDir = (targetPosition - gemTransform.position);
            float moveSpeed = 10f;
            gemTransform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }
}
