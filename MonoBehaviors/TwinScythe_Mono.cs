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
        public bool ableToHit = true;
        public bool active = true;
        public float damage = 35;
        private Player player;
        private Dictionary<int, float> recent = new Dictionary<int, float>();
        GameObject scythe;
        private void OnDestroy()
        {
            Destroy(scythe);
        }

		private void Start()
		{
			player = GetComponentInParent<Player>();

            scythe = Instantiate(RSCards.assets.LoadAsset<GameObject>("Scythe"), player.transform);
            scythe.SetActive(true);
        }

        public void DoHit()
        {
            if (player.data.view.IsMine)
            {
                var radius = transform.localScale.y;
                var hits = Physics2D.OverlapCircleAll(scythe.transform.position, radius);

                foreach (var hit in hits)
                {
                    var bullet = hit.gameObject.GetComponentInParent<ProjectileHit>();
                    var damageable = hit.gameObject.GetComponent<Damagable>();
                    var healthHandler = hit.gameObject.GetComponent<HealthHandler>();
                    if (healthHandler)
                    {
                        Player hitPlayer = ((Player)healthHandler.GetFieldValue("player"));
                        SoundManager.Instance.PlayAtPosition(healthHandler.soundBounce, this.transform, damageable.transform); // Play sfx
                        healthHandler.CallTakeForce(((Vector2)hitPlayer.transform.position - (Vector2)scythe.transform.position).normalized * 2500, ForceMode2D.Impulse, true); // Apply knockback
                        this.ableToHit = false; // Disable the scythe for the rest of the rotation
                        if (((Player)healthHandler.GetFieldValue("player")).GetComponent<Block>().blockedThisFrame) { continue; } // Skip everything else if they blocked
                    }
                    if (damageable)
                    {
                        damageable.CallTakeDamage(((Vector2)damageable.transform.position - (Vector2)transform.position).normalized * damage,
                            (Vector2)transform.position, gameObject, player);
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

        public void SetColor(Color color)
        {
            scythe.GetComponent<SpriteRenderer>().color = color; // set the color
        }
    }

    public class TwinScythe_Mono : MonoBehaviour
    {
        private double angle = 0;
        private float rotation = 0;
        private bool active = false;
        private Color color = new Color(0.5f, 0.5f, 0.5f);
        List<Scythe> scythes = new List<Scythe>();
        Player player;
        private void Start()
        {
            player = GetComponentInParent<Player>();
            var componentsInChildren = player.GetComponentsInChildren<TwinScythe_Mono>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (!(componentsInChildren[i] == this))
                {
                    Destroy(this);
                }
            }

            GameModeManager.AddHook(GameModeHooks.HookPointStart, PointStart);
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, PointEnd);
        }

        private void FixedUpdate()
        {
            angle += 200 * TimeHandler.deltaTime; // Update rotation
            if (angle > 360) // After a full rotation, re-enable all scythes
            {
                foreach (Scythe scythe in scythes)
                {
                    scythe.ableToHit = true;
                }
                angle -= 360;
            }
            rotation = (rotation - (1200 * TimeHandler.deltaTime)) % 360;

            int index = 0;
            foreach (Scythe scythe in scythes) // Tell each scythe where it belongs
            {
                double thisAngle = angle + (360f / scythes.Count() * index);
                scythe.UpdatePos(thisAngle, rotation, 2);
                if (active && scythe.ableToHit) // Trigger scythe hits
                {
                    scythe.DoHit();
                }
                if (scythe.ableToHit) // Update opacity to reflect whether the scythe is active or not
                {
                    Color setColor = color;
                    setColor.a = 1;
                    scythe.SetColor(setColor);
                }
                else
                {
                    Color setColor = color;
                    setColor.a = 0.5f;
                    scythe.SetColor(setColor);
                }
                index++;
            }
        }

        public void UpdateStats()
        {
            color = player.GetTeamColors().color;
            int scytheCount = 0;
            foreach(CardInfo card in player.data.currentCards)
                if (card == CardHolder.cards["Twin Scythe"]) scytheCount++;
            scytheCount = Math.Min(scytheCount, 4);
            while (scythes.Count() < scytheCount) // Create scythes as needed
            {
                GameObject scythe = new GameObject("Scythe", typeof(Scythe));
                scythe.transform.SetParent(player.transform);
                scythes.Add(scythe.GetComponent<Scythe>());
            }
            while (scythes.Count() > Math.Max(scytheCount, 0)) // Delete scythes as needed
            {
                Destroy(scythes[0]);
                scythes.Remove(scythes[0]);
            }
        }

        private void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPointStart, PointStart); // Remove hooks
            GameModeManager.RemoveHook(GameModeHooks.HookPointEnd, PointEnd);

            while (scythes.Count() > 0) // Destroy all the scythes
            {
                Destroy(scythes[0]);
                scythes.Remove(scythes[0]);
            }
        }

        IEnumerator PointStart(IGameModeHandler gm) // At the start of battle, reset rotations to help maintain sync and update stats while we're at it
        {
            active = true;
            rotation = 0f;
            angle = 0.0;
            UpdateStats();
            yield break;
        }

        IEnumerator PointEnd(IGameModeHandler gm) // Disable the scythes when not in a match
        {
            active = false;
            yield break;
        }
    }
}