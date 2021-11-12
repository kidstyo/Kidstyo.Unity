using System.Collections.Generic;
using UnityEngine;

namespace CityCreator
{
    [ExecuteInEditMode]
    public class RoadManager : MonoBehaviour
    {
        public PlacementManager PlacementManager;

        public List<Vector3Int> TemporaryPlacementPositions = new List<Vector3Int>();
        public List<Vector3Int> RoadPositionsToRecheck = new List<Vector3Int>();

        private Vector3Int _startPosition;
        public bool PlacementMode;
        public RoadFixerScriptableObject roadFixer;

        public void ResetCity()
        {
            PlacementManager.Reset();

            RoadPositionsToRecheck.Clear();
            TemporaryPlacementPositions.Clear();
            PlacementMode = false;
            _startPosition = Vector3Int.zero;
        }

        public bool DeleteRoad(Vector3Int position)
        {
            if (PlacementManager.CheckIfPositionInBound(position) == false)
            {
                return false;
            }

            if (PlacementManager.CheckIfPositionIsFree(position))
            {
                return false;
            }

            PlacementManager.RemoveObjectOnTheMap(position);

            var point = PlacementManager.PlacementGrid.RoadList.Find(x => x.X == position.x && x.Y == position.z);
            PlacementManager.PlacementGrid.RoadList.Remove(point);

            roadFixer.FixRoadAtPosition(PlacementManager, position);
            
            RoadPositionsToRecheck.Clear();
            var neighbours = PlacementManager.GetNeighboursOfTypeFor(position, CellType.Road);
            foreach (var roadposition in neighbours)
            {
                if (RoadPositionsToRecheck.Contains(roadposition) == false)
                {
                    RoadPositionsToRecheck.Add(roadposition);
                }
            }

            foreach (var positionToFix in RoadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(PlacementManager, positionToFix);
            }

            RoadPositionsToRecheck.Clear();
            return true;
        }

        public bool PlaceRoad(Vector3Int position)
        {
            if (PlacementManager.CheckIfPositionInBound(position) == false)
            {
                return false;
            }

            if (PlacementManager.CheckIfPositionIsFree(position) == false)
            {
                return false;
            }

            Debug.Log($"PlaceRoad: {position}");

            if (PlacementMode == false)
            {
                TemporaryPlacementPositions.Clear();
                RoadPositionsToRecheck.Clear();

                PlacementMode = true;
                _startPosition = position;

                TemporaryPlacementPositions.Add(position);
                PlacementManager.PlaceTemporaryStructure(position, roadFixer.deadEnd, CellType.Road);
            }
            else
            {
                PlacementManager.RemoveAllTemporaryStructures();
                TemporaryPlacementPositions.Clear();

                foreach (var positionsToFix in RoadPositionsToRecheck)
                {
                    roadFixer.FixRoadAtPosition(PlacementManager, positionsToFix);
                }

                RoadPositionsToRecheck.Clear();

                TemporaryPlacementPositions = PlacementManager.GetPathBetween(_startPosition, position);

                foreach (var temporaryPosition in TemporaryPlacementPositions)
                {
                    if (PlacementManager.CheckIfPositionIsFree(temporaryPosition) == false)
                    {
                        RoadPositionsToRecheck.Add(temporaryPosition);
                        continue;
                    }

                    PlacementManager.PlaceTemporaryStructure(temporaryPosition, roadFixer.deadEnd, CellType.Road);
                }
            }

            FixRoadPrefabs();
            return true;
        }

        private void FixRoadPrefabs()
        {
            foreach (var temporaryPosition in TemporaryPlacementPositions)
            {
                roadFixer.FixRoadAtPosition(PlacementManager, temporaryPosition);
                var neighbours = PlacementManager.GetNeighboursOfTypeFor(temporaryPosition, CellType.Road);
                foreach (var roadposition in neighbours)
                {
                    if (RoadPositionsToRecheck.Contains(roadposition) == false)
                    {
                        RoadPositionsToRecheck.Add(roadposition);
                    }
                }
            }

            foreach (var positionToFix in RoadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(PlacementManager, positionToFix);
            }
        }

        public bool FinishPlacingRoad()
        {
            PlacementMode = false;
            PlacementManager.AddtemporaryStructuresToStructureDictionary();
            TemporaryPlacementPositions.Clear();
            _startPosition = Vector3Int.zero;
            return true;
        }
    }
}