using UnityEngine;

public class Enemy : MonoBehaviour {
	
	[SerializeField] private Transform exitPoint; 
	[SerializeField] private Transform[] checkpoints;
	[SerializeField] private float navigationUpdate;
	[SerializeField] private int healthPoints;
	[SerializeField] private int rewardAmount;
	private bool isDead = false;

	private int target = 0;
	private Transform enemy;
	private Collider2D enemyCollider;
	private Animator enemyAnimator;
	private float navigationTime = 0f;

	public bool IsDead{
		get {
			return isDead;
		}
	}


	// Use this for initialization
	void Start () {
		enemy = GetComponent<Transform> ();
		enemyCollider = GetComponent<Collider2D>();
		enemyAnimator = GetComponent<Animator>();
		GameManager.instance.RegisterEnemy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (checkpoints != null && !isDead){
			navigationTime += Time.deltaTime;
			if (navigationTime > navigationUpdate){
				if (target < checkpoints.Length){
					enemy.position = Vector2.MoveTowards(enemy.position, checkpoints[target].position, navigationTime);

				} else {
					enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);
				}
				navigationTime = 0f;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		//print("Collided");
		if (other.tag == "Checkpoint"){
			target += 1;
		} else if (other.tag == "Finish"){
			GameManager.instance.EnemyEscaped();
			GameManager.instance.UnregisterEnemy(this);
			GameManager.instance.IsWaveOver();
		} else if (other.tag == "Projectile"){
			Projectile newProjectile = other.gameObject.GetComponent<Projectile>();
			EnemyHit(newProjectile.AttackStrength);
			Destroy(other.gameObject);
		}
	}

	public void EnemyHit(int damage){
		if (healthPoints - damage > 0){
			healthPoints -= damage;
			GameManager.instance.AudioSource.PlayOneShot(SoundManager.instance.Hit);
			enemyAnimator.Play("Hurt");
		} else {
			Die();
		}
	}

	public void Die(){
		isDead = true;
		enemyCollider.enabled = false;
		enemyAnimator.SetTrigger("didDie");
		GameManager.instance.AudioSource.PlayOneShot(SoundManager.instance.Death);
		GameManager.instance.AddMoney(rewardAmount);
		GameManager.instance.EnemyKilled();
	}



}
