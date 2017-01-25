using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : MonoBehaviour {

	[SerializeField] private float attackDelay;
	[SerializeField] private float attackRadius;
	[SerializeField] private Projectile projectile;
	private Enemy targetEnemy = null;
	private float attackCounter;
	private bool isAttacking = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		attackCounter -= Time.deltaTime;
		if (targetEnemy == null || targetEnemy.IsDead){
			Enemy nearestEnemy = GetNearestEnemyInRange();
			if (nearestEnemy != null && Vector2.Distance(transform.localPosition, nearestEnemy.transform.localPosition) <= attackRadius){
				targetEnemy = nearestEnemy;
			}
		} else {
			if (attackCounter <= 0){
				isAttacking = true;
				//reset attackCounter
				attackCounter = attackDelay;
			} else {
				isAttacking = false;
			}
			if (Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius){
				targetEnemy = null;
			}
		}
	}

	void FixedUpdate(){
		if (isAttacking)
			Attack();
	}

	public void Attack(){
		isAttacking = false;
		Projectile newProjectile = Instantiate(projectile) as Projectile;
		newProjectile.transform.localPosition = transform.localPosition;
		if (newProjectile.ProjectileType == projectileType.arrow){
			GameManager.instance.AudioSource.PlayOneShot(SoundManager.instance.Arrow);
		} else if (newProjectile.ProjectileType == projectileType.fireball){
			GameManager.instance.AudioSource.PlayOneShot(SoundManager.instance.Fireball);
		} else if (newProjectile.ProjectileType == projectileType.rock){
			GameManager.instance.AudioSource.PlayOneShot(SoundManager.instance.Rock);
		}
		if (targetEnemy == null){
			Destroy(newProjectile);
		} else {
			StartCoroutine(ShootProjectile(newProjectile));
		}
	}

	IEnumerator ShootProjectile(Projectile newProjectile){
		while(GetTargetDistance(targetEnemy) > 0.20f && newProjectile != null && targetEnemy != null){
			var dir = targetEnemy.transform.localPosition - transform.localPosition;
			var angleDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			newProjectile.transform.rotation = Quaternion.AngleAxis(angleDir, Vector3.forward);
			newProjectile.transform.localPosition = Vector2.MoveTowards(newProjectile.transform.localPosition, targetEnemy.transform.localPosition, 5f * Time.deltaTime);
			yield return null;
		}
		if (newProjectile != null || targetEnemy == null){
			Destroy(newProjectile);
		}

	}

	private float GetTargetDistance(Enemy enemy){
		if (enemy == null){
			enemy = GetNearestEnemyInRange();
			if (enemy == null){
				return 0f;
			}
		}
		return Mathf.Abs(Vector2.Distance(transform.localPosition, enemy.transform.localPosition));
	}

	private List<Enemy> GetAllEnemiesInRange(){
		List<Enemy> enemiesInRange = new List<Enemy>();
		foreach(Enemy enemy in GameManager.instance.EnemyList){
			if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius){
				enemiesInRange.Add(enemy);
			}
		}
		
		return enemiesInRange;
	}

	private Enemy GetNearestEnemyInRange(){
		Enemy nearestEnemy = null;
		float smallestDistance = float.PositiveInfinity;
		foreach(Enemy enemy in GetAllEnemiesInRange()){
			if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) < smallestDistance){
				nearestEnemy = enemy;
				smallestDistance = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
			}
		}
		return nearestEnemy;
	}




}
