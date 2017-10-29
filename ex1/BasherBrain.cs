using UnityEngine;
using StarWars.Actions;
using System.Collections.Generic;
using Infra.Utils;

namespace StarWars.Brains {
public class BasherBrain : SpaceshipBrain {
    public override string DefaultName {
        get {
            return "Basher";
        }
    }
    public override Color PrimaryColor {
        get {
            return new Color((float)0xF9 / 0xFF, (float)0x6C / 0xFF, (float)0xC6 / 0xFF, 1f);
        }
    }
    public override SpaceshipBody.Type BodyType {
        get {
				return SpaceshipBody.Type.XWing;
        }
    }

	/// <summary>
	/// Tries to bash or shoot down nearest ship.
	/// Tries to predict if any of the shots will hit next turn, if so turns on shield.
	/// If any ship is too close, turns on shield.
	/// </summary>
    public override Action NextAction() {
		var isBeingShotAt = IsBeingShotAt();
		
		var nearestShip = FindNearsetShip();
		if (nearestShip == null) {
			return spaceship.IsShieldUp ? ShieldDown.action : DoNothing.action;
		}
		var pos = spaceship.ClosestRelativePosition(nearestShip);
		var angle = pos.GetAngle(spaceship.Forward);
		if ((pos.magnitude < 2f && spaceship.CanRaiseShield) || isBeingShotAt){
			return ShieldUp.action;
		} else if (spaceship.IsShieldUp && pos.magnitude >= 2f && !isBeingShotAt) {
			return ShieldDown.action;
		} else if (angle >= 10f) {
			return TurnLeft.action;
		} else if (angle <= -10f) {
			return TurnRight.action;
		} else if (spaceship.CanShoot && pos.magnitude < 10f) {
			return Shoot.action;
		}
		return DoNothing.action;
    }

	private Spaceship FindNearsetShip() {
		return FindNearest(Space.Spaceships, IsNotMe);
	}

	private bool IsBeingShotAt() {
		foreach (Shot shot in Space.Shots) {
			if (IsNotMyShot(shot) && CheckFutureCollision (shot)) {
				return true;
			}
		}
		return false;
	}

	private bool IsNotMe(Spaceship ship) {
		return ship != spaceship;
	}

	private bool IsNotMyShot(Shot shot) {
		var angleToShot = shot.Forward.GetAngle(spaceship.Forward);
		return angleToShot < -30f || angleToShot > 30f;
	}

	private bool CheckFutureCollision(Shot nearestShot) {
		var futureShot = new FutureBody(nearestShot);
		futureShot.SimulateMoveForward(Shot.SPEED_PER_TURN);
		var futureSpaceship = new FutureBody(spaceship);
		futureSpaceship.SimulateMoveForward(Spaceship.SPEED_PER_TURN);
		var posDiff = futureShot.Position - futureSpaceship.Position;
		var radius = spaceship.Radius + nearestShot.Radius;
		// Check for collisions that wrap over the edge of the world.
		var diffSize =  Game.Size;
		diffSize.x -= radius;
		diffSize.y -= radius;
		bool wrapX = Mathf.Abs(posDiff.x) >= diffSize.x;
		bool wrapY = Mathf.Abs(posDiff.y) >= diffSize.y;
		if (wrapX || wrapY) {
			// There might be a collision over the edge.
			var pos = futureSpaceship.Position;
			var otherPos = futureShot.Position;
			if (wrapX) {
				if (posDiff.x < 0) {
					// This object is to the right of the other.
					pos.x -= Game.Size.x;
				} else {
					otherPos.x -= Game.Size.x;
				}
			}
			if (wrapY) {
				if (posDiff.y < 0) {
					// This object is above the other.
					pos.y -= Game.Size.y;
				} else {
					otherPos.y -= Game.Size.y;
				}
			}
			posDiff = otherPos - pos;
		}
		var sqrDistance = posDiff.sqrMagnitude;
		return sqrDistance <= radius * radius;
	}


	private T FindNearest<T>(IList<T> list, System.Func<T ,bool> predicate = null) where T : SpaceObject {
		T nearest = null;
		var minDistance = float.MaxValue;
		foreach (var obj in list) {
			if (!obj.IsAlive) continue;
			var distance = spaceship.ClosestRelativePosition(obj).magnitude;
			if ((nearest == null
				|| distance < minDistance)
				&& (predicate == null || predicate(obj))) {
				nearest = obj;
				minDistance = distance;
			}
		}
		return nearest;
	}

	
}
	public class FutureBody {

		float rotation;
		Vector2 position;
		Vector2 forward;

		public FutureBody(SpaceObject spaceObject) {
			this.rotation = spaceObject.Rotation;
			this.position = spaceObject.Position;
			this.forward = spaceObject.Forward;
		}
	
		public Vector2 Position {
			get {
				return position;
			}
		}

		public void FixPosition() {
			var pos = position;
			var bounds = Game.Size / 2;
			if (pos.x < -bounds.x) pos.x += bounds.x * 2;
			if (pos.x >= bounds.x) pos.x -= bounds.x * 2;
			if (pos.y < -bounds.y) pos.y += bounds.y * 2;
			if (pos.y >= bounds.y) pos.y -= bounds.y * 2;
			position = pos;
		}

		public void SimulateMoveForward(float speed) {
			var direction = forward.GetWithMagnitude(speed);
			position += direction;
			FixPosition();
		}
			
	}
}