using UnityEngine;

namespace Crowd.Game
{
	public partial class GameDirector
	{
		private readonly int MapSize = 500;
		private Ray ray;
		private RaycastHit hit;

		private Vector2 GetNewCharacterPosition()
		{
			Vector2 result = default;
			while (result == default)
			{
				int x = Random.Range(-MapSize / 2, MapSize / 2);
				int z = Random.Range(-MapSize / 2, MapSize / 2);

				ray = new Ray();
				ray.origin = new Vector3(x, 100, z);
				ray.direction = Vector3.down;

				Physics.Raycast(ray, out hit);
				if (!(hit.collider is null) && hit.collider.gameObject.CompareTag("Bottom"))
					result = new Vector2(x, z);
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
