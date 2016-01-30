using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RitualToolType
{
	PIEDRA = 0,
	PAPEL,
	TIJERA,
}

public class PlayerEntity : MonoBehaviour {

	public MonsterAlignment player;
	public Transform spawnPointTransform;
	public List<Cultist> cultistList;
	public List<string> InputKeys;
	private List<GameObject> followerList;
	private Vector3 spawnDirection;
	public GameMaster gm;

	public GameObject monsterTemp = null;
	public Transform followerSpawnPoint = null;
	

	List<RitualToolType> tools;

	// Use this for initialization

	private bool isCasting = false;
	private bool isOnCooldown = false;

	private float currentCooldown = 0;

	public PlayerData data;

	private float bonusCooldown = 0;
	private float currentPenalization = 0;
	

	void Start () {
		tools = new List<RitualToolType>();
		followerList = new List<GameObject>();
	}

	public void AddRitualTool (RitualToolType type)
	{
		if ( isOnCooldown ) 
			return;

		if ( isCasting )
		{
			SacrifyFollower();
			return;
		}

		if ( tools != null && tools.Count < 3 )
		{
			Debug.Log("add tool");
			tools.Add(type);
			if ( tools.Count >= 3 )
			{
				StartCoroutine(CastingCoroutine(SummonMinion));
			}
			//Debug.Log("Added ritual tool: " + type ); 
		}
	}

	public void SummonMinion()
	{
		if ( player == MonsterAlignment.PLAYER_1 )
		{
			spawnDirection = Vector3.right;
		} else {
			spawnDirection = -Vector3.right;
		}
		Debug.Log("Summoning minion");
		//GameObject tmpMonster = Instantiate(monsterTemp);
		GameObject tmpMonster = ObjectPool.instance.GetObjectForType("Monster", true);
		tmpMonster.SetActive(false);
		Monster tmpMonsterScript = tmpMonster.GetComponent<Monster>();
		tmpMonsterScript.Spawn(spawnPointTransform.position, spawnDirection, player);
		tmpMonsterScript.AddValues(tools);
		tools.Clear();
		StartCoroutine( CooldownCoroutine() );
	}

	public void KillCultist ( ) 
	{
		Debug.Log("Killing cultist");
		foreach ( Cultist cultist in cultistList ) 
		{
			if ( cultist.isAlive ) 
			{
				cultist.Kill();
				return;
			}
		}
	}
	

	private void CheckInput()
	{
		if ( Input.GetButtonDown(InputKeys[0]))
		{
			//Debug.Log("Piedra");
			AddRitualTool(RitualToolType.PIEDRA);
		}
		if ( Input.GetButtonDown(InputKeys[1]))
		{
			//Debug.Log("Papel");
			AddRitualTool(RitualToolType.PAPEL);
		}
		if ( Input.GetButtonDown(InputKeys[2]))
		{
			//Debug.Log("Tijera");
			AddRitualTool(RitualToolType.TIJERA);	
		}
	}

	IEnumerator CooldownCoroutine()
	{
		if ( isOnCooldown )  // esto no deberia pasar
			yield break;

		isOnCooldown = true;
		
		currentCooldown = bonusCooldown * followerList.Count;
		Debug.Log("Waiting: " + (data.cooldownTime - bonusCooldown));
		yield return new WaitForSeconds(data.cooldownTime - bonusCooldown);
		
		isOnCooldown = false;
	}

	IEnumerator CastingCoroutine(System.Action OnCastingComplete)
	{
		if ( isCasting )
			yield break;

		isCasting = true;

		// play fire animation here
		Debug.Log("Casting");
		yield return new WaitForSeconds(data.castTime);

		OnCastingComplete();

		isCasting = false;
	}

	IEnumerator FollowerUpdateCoroutine(System.Action OnFollowerAddition)
	{
		while ( cultistList.Count > 0 )
		{
			yield return new WaitForSeconds(data.followerTimer + data.followerPenalization * currentPenalization);
			if (OnFollowerAddition != null)
			{
				OnFollowerAddition();
			}
			currentPenalization = 0;
			yield return new WaitForEndOfFrame();
		}
	}

	public void OnFollowerAddition()
	{
		Debug.Log("Follower added");
		GameObject follower = ObjectPool.instance.GetObjectForType("Follower", true);
		follower.transform.position = followerSpawnPoint.position;
		followerList.Add ( follower );
	}

	public void SacrifyFollower()
	{
		Debug.Log("SacrifyFollower");
		currentPenalization += 1;
		KillOneFollower();
	}

	public void KillOneFollower()
	{
		foreach ( GameObject f in followerList )
		{
			if( f.activeInHierarchy )
			{
				ObjectPool.instance.PoolObject(f);
				return;
			}
		}
	}

	public void Update()
	{
		CheckInput();
	}
}
