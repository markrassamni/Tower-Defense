using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum gameStatus{
	next, play, gameOver, win
}

public class GameManager : Singleton<GameManager> {

	[SerializeField] private int totalWaves = 10;
	[SerializeField] private Text totalMoneyLbl;
	[SerializeField] private Text currentWaveLbl;
	[SerializeField] private Text playBtnLbl;
	[SerializeField] private Button playBtn;
	[SerializeField] private Text totalEscapedLbl;
	[SerializeField] private GameObject spawnPoint;
	[SerializeField] private Enemy[] enemies;
	[SerializeField] private int totalEnemies = 3;
	[SerializeField] private int enemiesPerSpawn;
	[SerializeField] private int escapedEnemiesEndGame = 10;
	[SerializeField] private Text bannerLbl;
	[SerializeField] private GameObject banner;

	private int waveNumber = 0;
	private int totalMoney = 10;
	private int totalEscaped = 0;
	private int roundEscaped = 0;
	private int totalKilled = 0;
	private int enemyDifficulty = 0;
	private gameStatus currentState = gameStatus.play;
	private AudioSource audioSource;

	public AudioSource AudioSource {
		get {
			return audioSource;
		}
	}
	
	
	public int TotalMoney {
		get {
			return totalMoney;
		} set {
			totalMoney = value;
			totalMoneyLbl.text = totalMoney.ToString();
		}
	}

	public int TotalEscaped {
		get {
			return totalEscaped;
		}
	}

	public int RoundEscaped {
		get {
			return roundEscaped;
		}
	}

	public int TotalKilled {
		get {
			return totalKilled;
		}
	}


	

	public List<Enemy> EnemyList = new List<Enemy>();
	
	const float spawnDelay = .5f;



	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		// playBtn.gameObject.SetActive(false);
		// banner.gameObject.SetActive(false);
		ShowMenu();
	}

	void Update(){
		handleEscape();
	}

 	IEnumerator SpawnEnemy(){
		if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies){
			for (int i = 0; i<enemiesPerSpawn; i++){
				if (EnemyList.Count < totalEnemies){
					Enemy newEnemy = Instantiate(enemies[Random.Range(0,enemyDifficulty)]);
					newEnemy.transform.position = spawnPoint.transform.position;
				}
			}
			yield return new WaitForSeconds(spawnDelay);
			StartCoroutine(SpawnEnemy());
		}
	}


	public void RegisterEnemy(Enemy enemy){
		EnemyList.Add(enemy);

	}

	public void UnregisterEnemy(Enemy enemy){
		EnemyList.Remove(enemy);
		Destroy(enemy.gameObject);
	}

	public void DestroyAllEnemies(){
		foreach(Enemy enemy in EnemyList){
			//EnemyList.Remove(enemy);
			Destroy(enemy.gameObject);
		}
		
		EnemyList.Clear();
	}

	public void AddMoney(int amount){
		totalMoney += amount;
		totalMoneyLbl.text = totalMoney.ToString();
	}

	public void SubtractMoney(int amount){
		totalMoney -= amount;
		totalMoneyLbl.text = totalMoney.ToString();
	}

	public void IsWaveOver(){
		totalEscapedLbl.text = "Escaped " + totalEscaped + "/" + escapedEnemiesEndGame;
		//SetCurrentGameState();
		if (roundEscaped + totalKilled >= totalEnemies) {
			if (waveNumber <= enemies.Length){
				enemyDifficulty = waveNumber;
			}
			SetCurrentGameState();
			ShowMenu();
		}
	}

	public void SetCurrentGameState(){
		if (totalEscaped >= escapedEnemiesEndGame){
			currentState = gameStatus.gameOver;
		} else if (waveNumber == 0 && totalKilled + roundEscaped == 0){
			currentState = gameStatus.play;
		} else if (waveNumber+1 >= totalWaves){ 
			currentState = gameStatus.win;
		} else {
			currentState = gameStatus.next;
		}
	}

	public void ShowMenu(){
		switch(currentState){
			case gameStatus.gameOver:
				playBtnLbl.text = "Play Again!";
				bannerLbl.text = "Game Over";
				audioSource.PlayOneShot(SoundManager.instance.GameOver);
				break;
			case gameStatus.next:
				playBtnLbl.text = "Next Wave!";
				bannerLbl.text = "Wave " + (waveNumber+2) + " next";
				//bannerLbl.text = "Game Over"; //wave _ next
				break;
			case gameStatus.play:
				bannerLbl.text = "Press Play to Start";
				playBtnLbl.text = "Play";
				break;
			case gameStatus.win:
				bannerLbl.text = "You Won!";
				playBtnLbl.text = "Play Again";
				break;
		}
		playBtn.gameObject.SetActive(true);
		banner.gameObject.SetActive(true);
	}

	public void ButtonPressed(){
		switch (currentState){
			case gameStatus.next:
				waveNumber +=1;
				totalEnemies += waveNumber;
				break;
			default:
				totalEnemies = 3;
				totalEscaped = 0;
				enemyDifficulty = 0;
				waveNumber = 0;
				totalMoney = 10;
				totalMoneyLbl.text = totalMoney.ToString();
				TowerManager.instance.DestroyAllTowers();
				TowerManager.instance.RenameTagsBuildSites();
				totalEscapedLbl.text = "Escaped " + totalEscaped + "/" + escapedEnemiesEndGame;
				audioSource.PlayOneShot(SoundManager.instance.NewGame);
				break;
		}
		DestroyAllEnemies();
		roundEscaped = 0;
		totalKilled = 0;
		currentWaveLbl.text = "Wave " + (waveNumber+1);
		StartCoroutine(SpawnEnemy());
		playBtn.gameObject.SetActive(false);
		banner.gameObject.SetActive(false);
	}

	private void handleEscape(){
		if (Input.GetKeyDown(KeyCode.Escape)){
			TowerManager.instance.DisableDragSprite();
		}
	}

	public void EnemyEscaped(){
		roundEscaped += 1;
		totalEscaped += 1;
	}

	public void EnemyKilled(){
		totalKilled += 1;
		IsWaveOver();
	}


}
