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
	private Vector3 spawnDirection;
	public GameMaster gm;

	public GameObject monsterTemp = null;

	

	List<RitualToolType> tools;

	// Use this for initialization

	private bool isCasting = false;
	private bool isOnCooldown = false;
	private float currentCooldown = 0;

	void Start () {
		tools = new List<RitualToolType>();
	}

	public void AddRitualTool (RitualToolType type)
	{
		if ( tools != null && tools.Count < 3 )
		{
			tools.Add(type);
			if ( tools.Count >= 3 )
			{
				SummonMinion();
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
		GameObject tmpMonster = Instantiate(monsterTemp);
		tmpMonster.SetActive(false);
		Monster tmpMonsterScript = tmpMonster.GetComponent<Monster>();
		tmpMonsterScript.Spawn(spawnPointTransform.position, spawnDirection, player);
		tmpMonsterScript.AddValues(tools);
		tools.Clear();
	}

	public void KillCultist ( ) 
	{
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

	public void Update()
	{
		CheckInput();
	}
}
