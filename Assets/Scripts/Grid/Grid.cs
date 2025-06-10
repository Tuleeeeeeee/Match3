using System;
using UnityEngine;
using Tuleeeeee.Utilities;


namespace Tuleeeeee.GridSystem
{
    public class Grid<TGridObject>
    {
        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;

        public class OnGridObjectChangedEventArgs : EventArgs
        {
            public int x;
            public int y;
        }

        private int width;
        private int height;
        private float cellSize;
        private Vector3 originPosition;

        private TGridObject[,] gridArray;
        private TextMesh[,] debugTextArray;

        public Grid(int width, int height, float cellSize, Vector3 originPosition,
            Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;

            gridArray = new TGridObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }

            bool showDebug = false; // Set to false to disable debug text and lines
            if (showDebug)
            {
                debugTextArray = new TextMesh[width, height];

                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    for (int y = 0; y < gridArray.GetLength(1); y++)
                    {
                        debugTextArray[x, y] = HelplerUtilities.CreateWorldText(gridArray[x, y].ToString(), null,
                            GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 1,
                            Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
                        debugTextArray[x, y].characterSize = .1f;
                        debugTextArray[x, y].fontSize = 30;
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    }
                }

                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

                OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
                {
                    debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
                };
            }
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return originPosition + new Vector3(x, y) * cellSize;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            Vector3 adjustedPos = (worldPosition - originPosition) / cellSize;
            x = Mathf.FloorToInt(adjustedPos.x);
            y = Mathf.FloorToInt(adjustedPos.y);
        }

        public void SetGridObject(int x, int y, TGridObject value)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                gridArray[x, y] = value;
                if (OnGridObjectChanged != null)
                    OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
            }
        }

        public void TriggerGridObjectChanged(int x, int y)
        {
            if (OnGridObjectChanged != null)
                OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetGridObject(x, y, value);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return gridArray[x, y];
            }
            else
            {
                return default(TGridObject);
            }
        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetGridObject(x, y);
        }
    }
}