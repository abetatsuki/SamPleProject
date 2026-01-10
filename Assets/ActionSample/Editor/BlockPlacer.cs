using UnityEngine;
using UnityEditor;

namespace ActionSample.Editor
{
    /// <summary>
    /// Sceneビュー上でマウスクリックした位置にPrefabを配置するエディタ拡張
    /// </summary>
    public class BlockPlacer : EditorWindow
    {
        /// <summary>
        /// ツールメニューからウィンドウを開くための静的メソッド
        /// </summary>
        [MenuItem("Tools/Block Placer")]
        public static void ShowWindow()
        {
            // 既存のウィンドウを取得するか、新規に作成して表示を行う
            GetWindow<BlockPlacer>("Block Placer");
        }

        /// <summary>
        /// 配置する対象となるPrefab
        /// </summary>
        private GameObject _prefabToPlace;

        /// <summary>
        /// ウィンドウが有効化した際の初期化処理
        /// </summary>
        private void OnEnable()
        {
            // SceneビューのGUIイベントにメソッドを登録し、入力を監視できるようにする
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary>
        /// ウィンドウが無効化した際の終了処理
        /// </summary>
        private void OnDisable()
        {
            // 登録したイベントを解除し、メモリリークや意図しない動作を防ぐ
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        /// <summary>
        /// エディタウィンドウ自体のGUI描画
        /// </summary>
        private void OnGUI()
        {
            // 配置用Prefabの設定フィールドを表示
            // Project内のアセットのみを受け付けるよう、allowSceneObjectsはfalseに設定
            _prefabToPlace = (GameObject)EditorGUILayout.ObjectField(
                "Prefab to Place",
                _prefabToPlace,
                typeof(GameObject),
                false
            );

            // 操作ガイドを表示してユーザーを補助する
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Select a Prefab above.\nClick in the Scene View to place the Prefab on colliders.",
                MessageType.Info
            );
        }

        /// <summary>
        /// Sceneビュー上での入力イベントを処理する
        /// </summary>
        /// <param name="sceneView">対象のSceneView</param>
        private void OnSceneGUI(SceneView sceneView)
        {
            // 現在発生しているイベントを取得
            Event currentEvent = Event.current;

            // マウスの左クリック（button 0）が押され、かつ装飾キー（Alt等）が押されていない場合のみ処理
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && currentEvent.modifiers == EventModifiers.None)
            {
                // クリック位置に基づいて配置処理を試行
                TryPlaceBlock(currentEvent.mousePosition);
            }
        }

        /// <summary>
        /// 指定された画面座標に対してRayを飛ばし、Prefabを配置する
        /// </summary>
        /// <param name="mousePosition">Sceneビュー上のマウス座標</param>
        private void TryPlaceBlock(Vector2 mousePosition)
        {
            // Prefabが未設定の場合は警告を出して中断
            if (_prefabToPlace == null)
            {
                Debug.LogWarning("BlockPlacer: Please assign a Prefab in the 'Tools > Block Placer' window.");
                return;
            }

            // マウス位置からカメラの視線を考慮したRayを生成
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

            // 物理的な衝突判定を行い、衝突した座標を取得
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Prefabのリンクを維持した状態でインスタンスを生成
                GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefabToPlace);

                // 生成したオブジェクトを衝突位置に移動
                newObject.transform.position = hit.point;

                // Undo操作に対応させるため、生成したオブジェクトを登録
                Undo.RegisterCreatedObjectUndo(newObject, "Place Block");

                // 他のツールがこのクリックに反応しないようイベントを消費
                Event.current.Use();
            }
        }
    }
}