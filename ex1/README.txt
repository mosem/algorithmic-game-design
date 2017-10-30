Moshemandel 301117800

BasherSpaceship - This spaceship tries to be as aggressive as possible by shooting down or colliding with the nearest spaceship, even at the cost of its own life. It tries to preserve as much of its shield as possible, by using it only if it predicts that a shot will hit it. To do so, it calculates the future positions of all the shots in the game (except its own) and if any of them collide with the spaceship’s own position - it turns on its shield. Also, it turns on its shield if it close to another spaceship.

RunnerSpaceship - This spaceship tries to run away from any spaceship, unless that spaceship is right in front and its energy is weak. Like BasherSpaceship - it predicts the positions of all the shots in the game.

Note: PredictionUtils is needed for both brains. This file contains the code that predicts the future positions of shots in the game.