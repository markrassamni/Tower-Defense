using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TowerManager : Singleton<TowerManager> {

	private TowerBtn towerBtnPressed;
	private SpriteRenderer spriteRenderer;
	private List <Tower> towerList = new List<Tower>();
	private List<Collider2D> buildList = new List<Collider2D>();
	private Collider2D buildTile;



	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		buildTile = GetComponent<Collider2D>();
		spriteRenderer.enabled = false;
	}

	public void RegisterBuildSite(Collider2D buildTag){
		buildList.Add(buildTag);
	}

	public void RegisterTower(Tower tower){
		towerList.Add(tower);
	}

	public void RenameTagsBuildSites(){
		foreach(Collider2D buildTag in buildList){
			buildTag.tag = "BuildSite";
		}
		buildList.Clear();
	}

	public void DestroyAllTowers(){
		foreach(Tower tower in towerList){
			Destroy(tower.gameObject);
		}	
		towerList.Clear();
	}


	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)){
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
			if(hit.collider.tag == "BuildSite"){
				buildTile = hit.collider;
				buildTile.tag = "UsedBuildSite";
				RegisterBuildSite(buildTile);
				PlaceTower(hit);
			}
		}
		if (spriteRenderer.enabled){
			FollowMouse();
		}
	}

	public void PlaceTower(RaycastHit2D hit){
		if(!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null){
			Tower newTower = Instantiate(towerBtnPressed.TowerObject);
			newTower.transform.position = hit.transform.position;
			BuyTower(towerBtnPressed.TowerPrice);
			RegisterTower(newTower);
			towerBtnPressed = null;
			DisableDragSprite();
			GameManager.instance.AudioSource.PlayOneShot(SoundManager.instance.TowerBuilt);
		}
	}

	public void BuyTower(int price){
		GameManager.instance.SubtractMoney(price);
	}

	public void SelectedTower(TowerBtn towerSelected){
		if (GameManager.instance.TotalMoney >= towerSelected.TowerPrice){
			towerBtnPressed = towerSelected;
			EnableDragSprite(towerBtnPressed.DragSprite);
		}
	}

	public void FollowMouse(){
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);	
		transform.position = new Vector2(transform.position.x, transform.position.y);
	}

	public void EnableDragSprite(Sprite sprite){
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

		public void DisableDragSprite(){
			spriteRenderer.enabled = false;
			towerBtnPressed = null;
		}

}
