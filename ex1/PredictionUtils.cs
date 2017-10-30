using UnityEngine;
using StarWars.Actions;
using System.Collections.Generic;
using Infra.Utils;

namespace StarWars.Brains {
	public class FutureBody {

		Vector2 position;
		Vector2 forward;

		public FutureBody(SpaceObject spaceObject) {
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

