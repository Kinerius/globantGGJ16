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
	public Animator circleAnimator = null;

	List<RitualToolType> tools;

	// Use this for initialization

	public bool isCasting = false;
	public bool isOnCooldown = false;

	private float currentCooldown = 0;

	public PlayerData data;

	private float bonusCooldown = 0;
	private float currentPenalization = 0;
	
	private GameObject _spawnedMonster = null;
	private Monster _currentSpawnMonster = null;

	void Start () {
		tools = new List<RitualToolType>();
		followerList = new List<GameObject>();
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
			//Debug.Log("add tool");
			tools.Add(type);
			if ( tools.Count >= 3 )
			{
				if ( player == MonsterAlignment.PLAYER_1 )
				{
					spawnDirection = Vector3.right;
				} else {
					spawnDirection = -Vector3.right;
				}
				_spawnedMonster = ObjectPool.instance.GetObjectForType("Monster", true);
				_spawnedMonster.SetActive(false);
				_currentSpawnMonster = _spawnedMonster.GetComponent<Monster>();
				_currentSpawnMonster.Spawn(spawnPointTransform.position, spawnDirection, player);
				_currentSpawnMonster.AddValues(tools);

				StartCoroutine(CastingCoroutine(SummonMinion));
			}
			//Debug.Log("Added ritual tool: " + type ); 
		}
		
		_spawnedMonster = null;
	}

	public void SummonMinion()
	{
		_currentSpawnMonster.Enable();

		tools.Clear();

		int random = Random.Range(1,3);
		SoundManager.Instance.Play("invocacion" + random, 0.8f, Random.Range(0.75f,1.24f));


		StartCoroutine( CooldownCoroutine() );
	}

	public void KillCultist ( ) 
	{
		//Debug.Log("Killing cultist");
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
		circleAnimator.SetBool("isOnCooldown",true);
		currentCooldown = data.cooldownBonusPerFollower * followerList.Count;
		Debug.Log("Waiting: " + (data.cooldownTime - bonusCooldown));
		yield return new WaitForSeconds(data.cooldownTime - currentCooldown);
		circleAnimator.SetBool("isOnCooldown",false);
		isOnCooldown = false;
	}

	IEnumerator CastingCoroutine(System.Action OnCastingComplete)
	{
		if ( isCasting )
			yield break;

		isCasting = true;
		circleAnimator.SetBool("isCasting",true);

		GameObject particle = ObjectPool.instance.GetObjectForType("sumonParticle",true);
		particle.transform.position = spawnPointTransform.position;
		particle.GetComponent<Effect>().disableTime = data.castTime;
		// play fire animation here
		//Debug.Log("Casting");
		yield return new WaitForSeconds(data.castTime);

		OnCastingComplete();
		circleAnimator.SetBool("isCasting",false);

		isCasting = false;
	}

	IEnumerator FollowerUpdateCoroutine(System.Action OnFollowerAddition)
	{
		yield return new WaitForSeconds(1);


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
		string followerName = "Follower";
		if ( player == MonsterAlignment.PLAYER_2 ) followerName = "FollowerRed";

		GameObject follower = ObjectPool.instance.GetObjectForType(followerName, true);
		float random1 = Random.Range(-2,2);
		float random2 = Random.Range(-2,2);
		Vector3 random = new Vector3(random1,0,random2);
		if (follower != null)
		{
			follower.transform.position = followerSpawnPoint.position+random;
			followerList.Add ( follower );
		}
		
	}

	public void SacrifyFollower()
	{
		//Debug.Log("SacrifyFollower");
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
				SoundManager.Instance.Play("sacrificio", 0.85f , Random.Range(0.75f, 1.25f));
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
