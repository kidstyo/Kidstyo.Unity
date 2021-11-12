using UnityEditor;
using UnityEngine;

namespace CityCreator
{
    public class CityCreatorEditorWindow : EditorWindow
    {
        [MenuItem("Tools/City Creator")]
        public static void ShowWindow()
        {
            var window = GetWindow<CityCreatorEditorWindow>();
            window.titleContent = new GUIContent("City Creator");
            window.Show();
        }

        void OnEnable()
        {
            Tools.current = Tool.None;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Reset"))
            {
                FindObjectOfType<RoadManager>().ResetCity();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            // fix MouseUp not work.
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                OnMouseUp();
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                OnMouseClick();
            }

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X)
            {
                OnMockKeyDelete();
            }

            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                OnMouseHold();
            }
        }

        private void OnMouseClick()
        {
            // convert GUI coordinates to screen coordinates
            Vector3 screenPosition = Event.current.mousePosition;
            screenPosition.y = Camera.current.pixelHeight - screenPosition.y;
            var ray = Camera.current.ScreenPointToRay(screenPosition);
            var hitPos = ObjectDetector.RaycastGround(ray);
            if (hitPos.HasValue)
            {
                var roadManager = FindObjectOfType<RoadManager>();
                if (roadManager.PlaceRoad(hitPos.Value))
                {
                    EditorUtility.SetDirty(roadManager);
                }
            }
        }

        private void OnMockKeyDelete()
        {
            // convert GUI coordinates to screen coordinates
            Vector3 screenPosition = Event.current.mousePosition;
            screenPosition.y = Camera.current.pixelHeight - screenPosition.y;
            var ray = Camera.current.ScreenPointToRay(screenPosition);
            var hitPos = ObjectDetector.RaycastGround(ray);
            if (hitPos.HasValue)
            {
                var roadManager = FindObjectOfType<RoadManager>();
                if (roadManager.DeleteRoad(hitPos.Value))
                {
                    EditorUtility.SetDirty(roadManager);
                }
            }
        }

        private void OnMouseHold()
        {
            // convert GUI coordinates to screen coordinates
            Vector3 screenPosition = Event.current.mousePosition;
            screenPosition.y = Camera.current.pixelHeight - screenPosition.y;
            var ray = Camera.current.ScreenPointToRay(screenPosition);
            var hitPos = ObjectDetector.RaycastGround(ray);
            if (hitPos.HasValue)
            {
                var roadManager = FindObjectOfType<RoadManager>();
                if (roadManager.PlaceRoad(hitPos.Value))
                {
                    EditorUtility.SetDirty(roadManager);
                }
            }
        }

        private void OnMouseUp()
        {
            FindObjectOfType<RoadManager>().FinishPlacingRoad();
        }
    }
}