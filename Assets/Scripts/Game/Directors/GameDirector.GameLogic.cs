using System.Collections.Generic;
using UnityEngine;

namespace Crowd.Game
{
	public partial class GameDirector
	{
		private readonly int MapSize = 500;
		private Dictionary<(int, int), bool> canConstruct = new Dictionary<(int, int), bool>();
		private Dictionary<int, Character> characterDictionary = new Dictionary<int, Character>();
		private Ray ray = new Ray();
		private RaycastHit hit;

		private Vector2 GetNewCharacterPosition()
		{
			Vector2 result = default;
			while (result == default)
			{
				int x = Random.Range(-MapSize / 2, MapSize / 2);
				int y = Random.Range(-MapSize / 2, MapSize / 2);

				if (canConstruct.ContainsKey((x, y)) && canConstruct[(x, y)])
					result = new Vector2(x, y);
				else
				{
					ray.origin = new Vector3(x, 100, y);
					ray.direction = Vector3.down;
					Physics.Raycast(ray, out hit);
					canConstruct.Add((x, y), hit.collider.gameObject.CompareTag("Bottom"));
				}
			}

			return result;
		}

		private void OnDrawGizmos()
		{
			if (hit.collider is null)
				Debug.DrawRay(ray.origin, ray.direction * 200, Color.red);
			else
				Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
		}
	}
}
