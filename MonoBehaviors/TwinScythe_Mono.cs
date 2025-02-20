using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Sonigon;
using System.Collections;

namespace RSCards.MonoBehaviors
{
	class Scythe : MonoBehaviour
	{
        private void OnDestroy()
        {
            Destroy(scythe);
        }

		private void Start()
		{
			Player = GetComponentInParent<Player>();

            scythe = Instantiate(RSCards.assets.LoadAsset<GameObject>("Scythe"), Player.transform);
            scythe.SetActive(true);
            scythe.GetComponent<SpriteRenderer>().color = Player.GetTeamColors().color;
        }

		public void DoHit()
        {
            if (Player.data.view.IsMine)
            {
                var radius = transform.localScale.y;
                var hits = Physics2D.OverlapCircleAll(scythe.transform.position, radius);
                var Keys = recent.Keys.ToArray();
                foreach (int Key in Keys)
                {
                    recent[Key] -= TimeHandler.deltaTime;
                }

                foreach (var hit in hits)
                {
                    var bullet = hit.gameObject.GetComponentInParent<ProjectileHit>();
                    var damageable = hit.gameObject.GetComponent<Damagable>();
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                    if (healthHandler)
                    {
                        int id = ((Player)healthHandler.GetFieldValue("player")).playerID;
                        if (id == Player.playerID || (recent.ContainsKey(id) && recent[id] > 0)) { continue; }
                        SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, transform, damageable.transform);
                        healthHandler.CallTakeForce(((Vector2)damageable.transform.position - (Vector2)transform.position).normalized * 2500, ForceMode2D.Impulse, true);
                        recent[id] = 0.5f;
                    }
                    if (damageable)
                    {
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)transform.position).normalized * damage,
                            (Vector2)transform.position, gameObject, Player);
                    }
                    if (bullet)
                    {
                        bullet.deathEvent.Invoke();
                        Destroy(bullet.gameObject);
                    }
                }
            }
        }

		public void UpdatePos(double angle, float rotation, float radius)
        {
			double angle_radians = (angle * Math.PI) / 180;
			Vector3 position = new Vector3((float)(radius * Math.Sin(angle_radians)),
				(float)((radius * Math.Cos(angle_radians))), 0);
			Quaternion currentRotation = new Quaternion();
			currentRotation.eulerAngles = new Vector3(0, 0, rotation);
			scythe.transform.localPosition = position;
            scythe.transform.rotation = currentRotation;
		}

		public bool active = true;
		public float damage = 35;
		private Player Player;
        private Dictionary<int, float> recent = new Dictionary<int, float>();
		GameObject scythe;
	}

    public class TwinScythe_Mono : MonoBehaviour
    {
		private void Start()
        {
            player = GetComponentInParent<Player>();
            var componentsInChildren = player.GetComponentsInChildren<TwinScythe_Mono>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (!(componentsInChildren[i] == this))
                {
                    componentsInChildren[i].count += count;
                    Destroy(gameObject);
                }
            }

            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, BattleStart);
        }

        private void Update()
		{
            radius = 2f;
            angle = (angle + (200 * TimeHandler.deltaTime)) % 360;
            rotation = (rotation - (1200 * TimeHandler.deltaTime)) % 360;

            int index = 0;
            foreach (Scythe scythe in scythes)
            {
                double thisAngle = angle + ((360 / scythes.Count()) * index);
                scythe.UpdatePos(thisAngle, rotation, radius);
                index++;
            }
        }

		private void FixedUpdate()
        {
            if (active)
            {
                foreach (Scythe scythe in scythes)
                {
                    scythe.DoHit();
                }
            }
        }

		public void UpdateCard()
        {
			int scytheCount = Math.Min(count, 4);
			if (scythes.Count() < scytheCount)
            {
				GameObject scythe = new GameObject("Scythe", typeof(Scythe));
				scythe.transform.SetParent(player.transform);
				player.transform.position = player.transform.position;
				scythes.Add(scythe.GetComponent<Scythe>());
            }
			else if (scythes.Count() > scytheCount)
            {
                Destroy(scythes[0]);
				scythes.Remove(scythes[0]);
            }

			foreach (Scythe scythe in scythes)
            {
				scythe.damage = 27.5f + (27.5f * count);
            }
		}

		IEnumerator BattleStart(IGameModeHandler gm)
		{
			active = true;
			yield break;
		}

		IEnumerator PointStart(IGameModeHandler gm)
		{
			rotation = 0f;
			angle = 0.0;
			yield break;
		}

        IEnumerator PointEnd(IGameModeHandler gm)
        {
            active = false;
            yield break;
        }

        private float radius = 2.5f;
		private double angle = 0;
		private float rotation = 0;
		public int count = 0;
		private bool active = false;
		List<Scythe> scythes = new List<Scythe>();
        Player player;
	}
}