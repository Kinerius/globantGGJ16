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
	
	private GameObject _spawnedMonster = null;
	private Monster _currentSpawnMonster = null;

	void Start () {
		tools = new List<RitualToolType>();
		followerList = new List<GameObject>();

		for ( int f = 0 ; f < 5 ; f++)
		{
			OnFollowerAddition();
		}
		StartCoroutine(FollowerUpdateCoroutine(OnFollowerAddition));
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
				_spawnedMonster = ObjectPool.instance.GetObjectForType("Monster", true);
				_spawnedMonster.SetActive(false);
				_currentSpawnMonster = _spawnedMonster.GetComponent<Monster>();
				
				_currentSpawnMonster.AddValues(tools);
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
		_currentSpawnMonster.Spawn(spawnPointTransform.position, spawnDirection, player);
		_currentSpawnMonster.Enable();

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
		float totalPenalty = 0;
		while ( cultistList.Count > 0 )
		{
			totalPenalty = data.followerPenalization * currentPenalization;
			currentPenalization = 0;
			yield return new WaitForSeconds(data.followerTimer + totalPenalty);
			if (OnFollowerAddition != null)
			{
				OnFollowerAddition();
			}
			
			yield return new WaitForEndOfFrame();
		}
	}

	public void OnFollowerAddition()
	{
		//Debug.Log("Follower added");
		GameObject follower = ObjectPool.instance.GetObjectForType("Follower", true);
		Vector3 random = new Vector3(Random.Range(-2,2),0,Random.Range(-2,2));
		follower.transform.position = followerSpawnPoint.position+random;
		followerList.Add ( follower );
	}

	public void SacrifyFollower()
	{
		Debug.Log("SacrifyFollower");
		if (_currentSpawnMonster != null)
			_currentSpawnMonster.AddPower();
		currentPenalization += 1;
		KillOneFollower();
	}

	public void KillOneFollower()
	{
		foreach ( GameObject f in followerList )
		{
			if( f.activeInHierarchy )
			{
				SoundManager.Instance.Play("sacrificio");
				followerList.Remove(f);
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
