using UnityEngine;
using System;
using UnboundLib;

namespace RSCards.MonoBehaviors
{
    public class Hitscan_Bullet_Mono : MonoBehaviour
    {
		private void OnDestroy()
		{
			trail.transform.SetPositionAndRotation(transform.position, transform.rotation);
            RSCards.instance.ExecuteAfterSeconds(2, () => UnityEngine.GameObject.Destroy(trail));
		}

		private void Start()
        {
            trail = new GameObject("Hitscan_Trail", new Type[] { typeof(TrailRenderer) });
			trail.transform.SetPositionAndRotation(transform.position, transform.rotation);
			trail.GetComponent<TrailRenderer>().startWidth = 0.1f;
			trail.GetComponent<TrailRenderer>().endWidth = 0.1f;
			trail.GetComponent<TrailRenderer>().time = 1f;
			trail.GetComponent<TrailRenderer>().sharedMaterial = RSCards.assets.LoadAsset<Material>("Material");

			if (GetComponentInParent<RayHitReflect>() is RayHitReflect hit) // Stop the bullet from bouncing
                hit.reflects = 0;
        }
		private void Update()
		{
			trail.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

		GameObject trail;
	}
}

