using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityCreator
{
    [Serializable]
    public class StructureData
    {
        public Vector3Int Pos;
        public StructureModel Model;

        public StructureData(Vector3Int pos, StructureModel model)
        {
            Pos = pos;
            Model = model;
        }
    }

    [CreateAssetMenu(fileName = "CityGrid", menuName = "City Creator/CityGrid", order = 1)]
    public class GridScriptableObject : ScriptableObject
    {
        public int Width = 15;
        public int Height = 15;
        public List<CellType> CityGrid;

        public List<Point> RoadList = new List<Point>();
        public List<Point> SpecialStructure = new List<Point>();
        public List<Point> HouseStructure = new List<Point>();

        public void Reset()
        {
            CityGrid = new List<CellType>();
            for (int i = 0; i < Width * Height; i++)
            {
                CityGrid.Add(CellType.Empty);
            }
            
            RoadList.Clear();
            SpecialStructure.Clear();
            HouseStructure.Clear();
        }

        // Adding index operator to our Grid class so that we can use grid[][] to access specific cell from our grid. 
        public CellType this[int i, int j]
        {
            get => CityGrid[j * Height + i];
            set
            {
                switch (value)
                {
                    case CellType.Road:
                        RoadList.Add(new Point(i, j));
                        break;
                    case CellType.SpecialStructure:
                        SpecialStructure.Add(new Point(i, j));
                        break;
                    case CellType.Structure:
                        HouseStructure.Add(new Point(i, j));
                        break;
                }

                CityGrid[j * Height + i] = value;
            }
        }

        public static bool IsCellWakable(CellType cellType, bool aiAgent = false)
        {
            if (aiAgent)
            {
                return cellType == CellType.Road;
            }

            return cellType == CellType.Empty || cellType == CellType.Road;
        }

        public Point GetRandomRoadPoint()
        {
            if (RoadList.Count == 0)
            {
                return null;
            }

            return RoadList[UnityEngine.Random.Range(0, RoadList.Count)];
        }

        public Point GetRandomSpecialStructurePoint()
        {
            if (SpecialStructure.Count == 0)
            {
                return null;
            }

            return SpecialStructure[UnityEngine.Random.Range(0, SpecialStructure.Count)];
        }

        public Point GetRandomHouseStructurePoint()
        {
            if (HouseStructure.Count == 0)
            {
                return null;
            }

            return HouseStructure[UnityEngine.Random.Range(0, HouseStructure.Count)];
        }

        public List<Point> GetAllHouses()
        {
            return HouseStructure;
        }

        internal List<Point> GetAllSpecialStructure()
        {
            return SpecialStructure;
        }

        public List<Point> GetAdjacentCells(Point cell, bool isAgent)
        {
            return GetWakableAdjacentCells((int) cell.X, (int) cell.Y, isAgent);
        }

        public float GetCostOfEnteringCell(Point cell)
        {
            return 1;
        }

        public List<Point> GetAllAdjacentCells(int x, int y)
        {
            List<Point> adjacentCells = new List<Point>();
            if (x > 0)
            {
                adjacentCells.Add(new Point(x - 1, y));
            }

            if (x < Width - 1)
            {
                adjacentCells.Add(new Point(x + 1, y));
            }

            if (y > 0)
            {
                adjacentCells.Add(new Point(x, y - 1));
            }

            if (y < Height - 1)
            {
                adjacentCells.Add(new Point(x, y + 1));
            }

            return adjacentCells;
        }

        public List<Point> GetWakableAdjacentCells(int x, int y, bool isAgent)
        {
            List<Point> adjacentCells = GetAllAdjacentCells(x, y);
            for (int i = adjacentCells.Count - 1; i >= 0; i--)
            {
                var index = adjacentCells[i].X + Width * adjacentCells[i].Y;
                if (IsCellWakable(CityGrid[index], isAgent) == false)
                {
                    adjacentCells.RemoveAt(i);
                }
            }

            return adjacentCells;
        }

        public List<Point> GetAdjacentCellsOfType(int x, int y, CellType type)
        {
            List<Point> adjacentCells = GetAllAdjacentCells(x, y);
            for (int i = adjacentCells.Count - 1; i >= 0; i--)
            {
                var index = adjacentCells[i].X + Width * adjacentCells[i].Y;
                if (CityGrid[index] != type)
                {
                    adjacentCells.RemoveAt(i);
                }
            }

            return adjacentCells;
        }

        /// <summary>
        /// Returns array [Left neighbour, Top neighbour, Right neighbour, Down neighbour]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public CellType[] GetAllAdjacentCellTypes(int x, int y)
        {
            CellType[] neighbours = {CellType.None, CellType.None, CellType.None, CellType.None};
            if (x > 0)
            {
                neighbours[0] = CityGrid[x - 1 + y * Width];
            }

            if (x < Width - 1)
            {
                neighbours[2] = CityGrid[x + 1 + y * Width];
            }

            if (y > 0)
            {
                neighbours[3] = CityGrid[x + (y - 1) * Width];
            }

            if (y < Height - 1)
            {
                neighbours[1] = CityGrid[x + (y + 1) * Width];
            }

            return neighbours;
        }
    }
}