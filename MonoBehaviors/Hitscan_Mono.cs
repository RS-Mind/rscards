using UnityEngine;
using System;
using UnboundLib;
using UnboundLib.GameModes;
using System.Collections;

namespace RSCards.MonoBehaviors
{
    public class Hitscan_Mono : MonoBehaviour
    {
		private void OnDestroy()
		{
			this.trail.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            RSCards.instance.ExecuteAfterSeconds(2, () => UnityEngine.GameObject.Destroy(this.trail));
		}

		private void Start()
        {
            player = this.GetComponentInParent<Player>();
            this.trail = new GameObject("Hitscan_Trail", new Type[] { typeof(TrailRenderer) });
			this.trail.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
			this.trail.GetComponent<TrailRenderer>().startWidth = 0.1f;
			this.trail.GetComponent<TrailRenderer>().endWidth = 0.1f;
			this.trail.GetComponent<TrailRenderer>().time = 1f;
			this.trail.GetComponent<TrailRenderer>().sharedMaterial = RSCards.assets.LoadAsset<Material>("Material");
            GameModeManager.AddHook(GameModeHooks.HookRoundStart, RoundStart);
        }

		IEnumerator RoundStart(IGameModeHandler gm)
		{
			player.data.weaponHandler.gun.reflects = -100;
			yield break;
		}

		private void Update()
		{
			this.trail.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
        }

		Player player;
		GameObject trail;
		GameObject bullet;
	}
}

