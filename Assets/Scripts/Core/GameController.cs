using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OperationBlackwell.Core {
	public class GameController : MonoBehaviour {
		private const bool DebugMovement = false;

		public static GameController Instance { get; private set; }

		[SerializeField] private Vector3 gridWorldSize_;
		[SerializeField] private float cellSize_;

		[SerializeField] private bool drawGridLines_;

		[SerializeField] private MovementTilemapVisual movementTilemapVisual_;
		private MovementTilemap movementTilemap_;

		public Grid<Tilemap.Node> grid { get; private set; }

		public GridPathfinding gridPathfinding { get; private set; }
		public Tilemap tilemap { get; private set; }

		[SerializeField] private TilemapVisual tilemapVisual_;
		private Tilemap.Node.NodeSprite nodeSprite_;

		private void Awake() {
			grid = new Grid<Tilemap.Node>((int)gridWorldSize_.x, (int)gridWorldSize_.y, cellSize_, new Vector3(0, 0, 0), 
				(Grid<Tilemap.Node> g, Vector3 worldPos, int x, int y) => new Tilemap.Node(worldPos, x, y, g, true, Tilemap.Node.floorHitChanceModifier, false), drawGridLines_);
			tilemap = new Tilemap(grid);
			Instance = this;
			Vector3 origin = new Vector3(0, 0);

			gridPathfinding = new GridPathfinding(origin + new Vector3(1, 1) * cellSize_ * .5f, new Vector3(gridWorldSize_.x, gridWorldSize_.y) * cellSize_, cellSize_);
			movementTilemap_ = new MovementTilemap((int)gridWorldSize_.x, (int)gridWorldSize_.y, cellSize_, new Vector3(0, 0, 0));
		}

		private void Start() {
			tilemap.SetTilemapVisual(tilemapVisual_);
			movementTilemap_.SetTilemapVisual(movementTilemapVisual_);
			if(SceneManager.GetActiveScene().name == "TutorialLevel") {
				tilemap.Load("tutoriallevel");
			} else {
				Debug.Log(SceneManager.GetActiveScene().name + " has no level to load!");
			}
		}

		private void Update() {
			HandleMisc();
			HandleWeaponSwitch();
		}

		public Grid<Tilemap.Node> GetGrid() {
			return grid;
		}

		public MovementTilemap GetMovementTilemap() {
			return movementTilemap_;
		}

		private void HandleMisc() {
			if(Input.GetKeyDown(KeyCode.Escape)) {
				SceneManager.LoadScene("MainMenu");
			}
			if(Input.GetKeyDown(KeyCode.Home)) {
				Application.OpenURL("https://github.com/Online-Pluisje-Art/Operation-Blackwell/tree/development");
			}
			if(Input.GetKeyDown(KeyCode.End)) {
				Application.OpenURL("https://docs.opa.rip/");
			}
		}

		private void HandleWeaponSwitch() {
			if(Input.GetKeyDown(KeyCode.Alpha1)) {
				GridCombatSystem.Instance.GetActiveUnit().SetActiveWeapon(0);
				GridCombatSystem.Instance.OnWeaponChanged?.Invoke(this, 0);
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)) {
				GridCombatSystem.Instance.GetActiveUnit().SetActiveWeapon(1);
				GridCombatSystem.Instance.OnWeaponChanged?.Invoke(this, 1);
			}
		}
	}
}
