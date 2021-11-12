using System.Collections.Generic;
using UnityEngine;

namespace CityCreator
{
    public class PlacementManager : MonoBehaviour
    {
        public GridScriptableObject PlacementGrid;

        public List<StructureData> _temporaryRoadobjects = new List<StructureData>();
        public List<StructureData> _structureDictionary = new List<StructureData>();

        public void Reset()
        {
            PlacementGrid.Reset();
            
            foreach (var data in _temporaryRoadobjects)
            {
                if (null != data.Model)
                {
                    DestroyImmediate(data.Model.gameObject);
                }
            }
            
            foreach (var data in _structureDictionary)
            {
                if (null != data.Model)
                {
                    DestroyImmediate(data.Model.gameObject);
                }
            }
            
            _temporaryRoadobjects.Clear();
            _structureDictionary.Clear();
        }

        internal CellType[] GetNeighbourTypesFor(Vector3Int position)
        {
            return PlacementGrid.GetAllAdjacentCellTypes(position.x, position.z);
        }

        internal bool CheckIfPositionInBound(Vector3Int position)
        {
            return position.x >= 0 && position.x < PlacementGrid.Width && position.z >= 0 &&
                   position.z < PlacementGrid.Height;
        }

        internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type, int width = 1,
            int height = 1)
        {
            StructureModel structure = CreateANewStructureModel(position, structurePrefab, type);
            var structureNeedingRoad = structure.GetComponent<INeedingRoad>();
            if (structureNeedingRoad != null)
            {
                structureNeedingRoad.RoadPosition = GetNearestRoad(position, width, height).Value;
                Debug.Log("My nearest road position is: " + structureNeedingRoad.RoadPosition);
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    var newPosition = position + new Vector3Int(x, 0, z);
                    PlacementGrid[newPosition.x, newPosition.z] = type;
                    _structureDictionary.Add(new StructureData(newPosition, structure));
                    DestroyNatureAt(newPosition);
                }
            }
        }

        public void RemoveObjectOnTheMap(Vector3Int position)
        {
            var structureData = _structureDictionary.Find(x => x.Pos == position);
            DestroyImmediate(structureData.Model.gameObject);
            _structureDictionary.Remove(structureData);
            PlacementGrid[position.x, position.z] = CellType.Empty;
        }

        private Vector3Int? GetNearestRoad(Vector3Int position, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var newPosition = position + new Vector3Int(x, 0, y);
                    var roads = GetNeighboursOfTypeFor(newPosition, CellType.Road);
                    if (roads.Count > 0)
                    {
                        return roads[0];
                    }
                }
            }

            return null;
        }

        private void DestroyNatureAt(Vector3Int position)
        {
            RaycastHit[] hits = Physics.BoxCastAll(position + new Vector3(0, 0.5f, 0), new Vector3(0.5f, 0.5f, 0.5f),
                transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Nature"));
            foreach (var item in hits)
            {
                Destroy(item.collider.gameObject);
            }
        }

        internal bool CheckIfPositionIsFree(Vector3Int position)
        {
            return CheckIfPositionIsOfType(position, CellType.Empty);
        }

        private bool CheckIfPositionIsOfType(Vector3Int position, CellType type)
        {
            return PlacementGrid[position.x, position.z] == type;
        }

        internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
        {
            PlacementGrid[position.x, position.z] = type;
            StructureModel structure = CreateANewStructureModel(position, structurePrefab, type);
            _temporaryRoadobjects.Add(new StructureData(position, structure));
        }

        internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
        {
            var neighbourVertices = PlacementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
            List<Vector3Int> neighbours = new List<Vector3Int>();
            foreach (var point in neighbourVertices)
            {
                neighbours.Add(new Vector3Int(point.X, 0, point.Y));
            }

            return neighbours;
        }

        private StructureModel CreateANewStructureModel(Vector3Int position, GameObject structurePrefab, CellType type)
        {
            GameObject structure = new GameObject(type.ToString());
            structure.transform.SetParent(transform);
            structure.transform.localPosition = position;

            StructureModel structureModel = structure.AddComponent<StructureModel>();
            structureModel.CreateModel(structurePrefab);
            return structureModel;
        }

        internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition, bool isAgent = false)
        {
            var resultPath = GridSearch.AStarSearch(PlacementGrid, new Point(startPosition.x, startPosition.z),
                new Point(endPosition.x, endPosition.z), isAgent);
            List<Vector3Int> path = new List<Vector3Int>();
            foreach (Point point in resultPath)
            {
                path.Add(new Vector3Int(point.X, 0, point.Y));
            }

            return path;
        }

        internal void RemoveAllTemporaryStructures()
        {
            foreach (var structureData in _temporaryRoadobjects)
            {
                var position = Vector3Int.RoundToInt(structureData.Model.transform.position);
                PlacementGrid[position.x, position.z] = CellType.Empty;
                DestroyImmediate(structureData.Model.gameObject);
            }

            _temporaryRoadobjects.Clear();
        }

        internal void AddtemporaryStructuresToStructureDictionary()
        {
            foreach (var structureData in _temporaryRoadobjects)
            {
                _structureDictionary.Add(new StructureData(structureData.Pos, structureData.Model));
                DestroyNatureAt(structureData.Pos);
            }

            _temporaryRoadobjects.Clear();
        }

        public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
        {
            var tempData = _temporaryRoadobjects.Find(x => x.Pos == position);
            if (null != tempData)
            {
                tempData.Model.SwapModel(newModel, rotation);
            }
            else
            {
                var objData = _structureDictionary.Find(x => x.Pos == position);
                objData?.Model.SwapModel(newModel, rotation);
            }
        }

        public StructureModel GetRandomRoad()
        {
            var point = PlacementGrid.GetRandomRoadPoint();
            return GetStructureAt(point);
        }

        public StructureModel GetRandomSpecialStrucutre()
        {
            var point = PlacementGrid.GetRandomSpecialStructurePoint();
            return GetStructureAt(point);
        }

        public StructureModel GetRandomHouseStructure()
        {
            var point = PlacementGrid.GetRandomHouseStructurePoint();
            return GetStructureAt(point);
        }

        public List<StructureModel> GetAllHouses()
        {
            List<StructureModel> returnList = new List<StructureModel>();
            var housePositions = PlacementGrid.GetAllHouses();
            foreach (var point in housePositions)
            {
                var pos = new Vector3Int(point.X, 0, point.Y);
                returnList.Add(_structureDictionary.Find(x => x.Pos == pos).Model);
            }

            return returnList;
        }

        internal List<StructureModel> GetAllSpecialStructures()
        {
            List<StructureModel> returnList = new List<StructureModel>();
            var housePositions = PlacementGrid.GetAllSpecialStructure();
            foreach (var point in housePositions)
            {
                var pos = new Vector3Int(point.X, 0, point.Y);
                returnList.Add(_structureDictionary.Find(x => x.Pos == pos).Model);
            }

            return returnList;
        }

        private StructureModel GetStructureAt(Point point)
        {
            if (point != null)
            {
                return _structureDictionary.Find(x => x.Pos == new Vector3Int(point.X, 0, point.Y)).Model;
            }

            return null;
        }

        public StructureModel GetStructureAt(Vector3Int position)
        {
            return _structureDictionary.Find(x => x.Pos == position)?.Model;
        }
    }
}