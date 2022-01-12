using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace OperationBlackwell.Core {
	public class GlobalController : PersistentSingleton<GlobalController> {
		[Header("Levels")]
		[SerializeField] private List<LevelData> levels_;
		[System.Serializable]
		public struct LevelData {
			public string id;
			public bool completed;
		}

		public System.EventHandler<LevelDataArgs> LevelCompleted;
		public class LevelDataArgs : System.EventArgs {
			public LevelData levelData;
			public string newLevelId;
		}

		private void Start() {
			LevelCompleted += OnLevelCompleted;
		}

		private void OnDestroy() {
			LevelCompleted -= OnLevelCompleted;
		}

		private void OnLevelCompleted(object sender, LevelDataArgs e) {
			int index = levels_.FindIndex(x => x.id == e.levelData.id);
			levels_[index] = e.levelData;
			LoadNewLevel(e.newLevelId);
		}

		private void LoadNewLevel(string level) {
			SceneManager.LoadScene(level);
		}
	}
}
