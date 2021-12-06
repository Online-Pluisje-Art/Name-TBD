﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationBlackwell.Core;

namespace OperationBlackwell.Player {
	public class UnitGridCombat : CoreUnit {

		[SerializeField] private Team team_;
		[SerializeField] private int actionPoints_;
		[SerializeField] private int maxActionPoints_;
		[SerializeField] private List<Weapon> weapons_;
		
		private PlayerBase characterBase_;
		private GameObject selectedGameObject_;
		private MovePositionPathfinding movePosition_;
		private State state_;

		private HealthSystem healthSystem_;
		private WorldBar healthBar_;

		private Weapon currentWeapon_;

		private enum State {
			Normal,
			Moving,
			Attacking
		}

		private void Awake() {
			characterBase_ = GetComponent<PlayerBase>();
			selectedGameObject_ = transform.Find("Selected").gameObject;
			movePosition_ = GetComponent<MovePositionPathfinding>();
			//SetSelectedVisible(false);
			state_ = State.Normal;
			healthSystem_ = new HealthSystem(100);
			healthBar_ = new WorldBar(transform, new Vector3(0, 6.6f), new Vector3(1, .13f), Color.grey, Color.red, 1f, 10000, new WorldBar.Outline { color = Color.black, size = .05f });
			healthSystem_.OnHealthChanged += HealthSystem_OnHealthChanged;
			SetActiveWeapon(0);
		}

		private void Update() {
			switch(state_) {
				case State.Normal:
					break;
				case State.Moving:
					break;
				case State.Attacking:
					break;
				default: 
					break;
			}
		}

		private void OnDestroy() {
			healthSystem_.OnHealthChanged -= HealthSystem_OnHealthChanged;
		}

		private void HealthSystem_OnHealthChanged(object sender, EventArgs e) {
			healthBar_.SetSize(healthSystem_.GetHealthNormalized());
		}

		public override void SetActiveWeapon(int index) {
			if(index >= 0 && index < weapons_.Count) {
				currentWeapon_ = weapons_[index];
			}
		}

		public override string GetActiveWeapon() {
			return currentWeapon_.GetName();
		}

		public void SetSelectedVisible(bool visible) {
			selectedGameObject_.SetActive(visible);
		}

		public override bool CanAttackUnit(CoreUnit unitGridCombat) {
			/* 
			 * If the unit is on the same team, return false.
			 * Calculate the distance between the two units. 
			 * The CalculatePoints method is used to calculate the distance between two points, 
			 * when its the same node it returns 1 so we subtract one from the distance to get the actual distance.
			 * If the distance is less or equal than the weapon range, return true.
			 */
			if(unitGridCombat == null || unitGridCombat.GetTeam() == team_ || actionPoints_ < currentWeapon_.GetActionPointsCost()) {
				return false;
			}

			// Calculate the distance between the two units. But due to the -1 we can attack diagonal units, but also sometimes 1 node extra on the range.
			int nodesBetweenPlayers = GridCombatSystem.Instance.CalculatePoints(GetPosition(), unitGridCombat.GetPosition()).Count - 1;
			return nodesBetweenPlayers <= currentWeapon_.GetRange();
		}

		public override void MoveTo(Vector3 targetPosition, Action onReachedPosition) {
			state_ = State.Moving;
			movePosition_.SetMovePosition(targetPosition, () => {
				state_ = State.Normal;
				onReachedPosition();
			});
		}

		public override Vector3 GetPosition() {
			return transform.position;
		}

		public override Team GetTeam() {
			return team_;
		}

		public override bool IsEnemy(CoreUnit unitGridCombat) {
			return unitGridCombat.GetTeam() != team_;
		}

		public override void SetActionPoints(int actionPoints) {
			actionPoints_ = actionPoints;
		}

		public override int GetActionPoints() {
			return actionPoints_;
		}

		public override int GetMaxActionPoints() {
			return maxActionPoints_;
		}

		public override void ResetActionPoints() {
			actionPoints_ = maxActionPoints_;
		}

		public override void AttackUnit(CoreUnit unitGridCombat, Action onAttackComplete) {
			state_ = State.Attacking;

			ShootUnit(unitGridCombat, () => {
				state_ = State.Normal;
				onAttackComplete(); 
			});
		}

		private void ShootUnit(CoreUnit unitGridCombat, Action onShootComplete) {
			GetComponent<IMoveVelocity>().Disable();

			// The value of 50 is a placeholder for the damage of the units attack.
			unitGridCombat.Damage(this, currentWeapon_.GetDamage());

			GetComponent<IMoveVelocity>().Enable();
			onShootComplete();
		}

		public override void Damage(CoreUnit attacker, float damageAmount) {	
			healthSystem_.Damage((int)damageAmount);
			if(healthSystem_.IsDead()) {
				GridCombatSystem.Instance.OnUnitDeath?.Invoke(this, EventArgs.Empty);
				Destroy(gameObject);
			} else {
				// Knockback
				//transform.position += bloodDir * 5f;
			}
		}

		public override bool IsDead() {
			return healthSystem_.IsDead();
		}

		public override int GetAttackCost() {
			return currentWeapon_.GetActionPointsCost();
		}

		/*	
		 *	This method calculates and returns the hit chance between two Vector3's 
		 *	It is possible to add another float variable WeaponModifierHitChance to this method, then adjust float hitChance accordingly.
		 */
		private float RangedHitChance(Vector3 player, Vector3 target) {
			Grid<Tilemap.Node> grid = GameController.Instance.GetGrid();
			List<Vector3> points = GridCombatSystem.Instance.CalculatePoints(player, target);

			float hitChance = currentWeapon_.GetBaseHitchance();

			if(currentWeapon_.GetRange() >= points.Count - 1) {
				// Target is in range
				foreach(Vector3 v in points) {
					// Calculates hitchance
					hitChance -= grid.GetGridObject(v).hitChanceModifier;
				}
			}
			return hitChance;
		}

		public override void SaveAction(Actions action) {
			// OrderObject order = new OrderObject();
			// GridCombatSystem.Instance.AddOrderElement();
		}

		public override void ExecuteActions() {
			Debug.Log("Executing actions");
		}
	}
}
