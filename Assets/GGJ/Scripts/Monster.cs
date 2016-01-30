using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MonsterAlignment
{
	NONE = 0,
	PLAYER_1 = 1,
	PLAYER_2 = 2,
}

public enum FightResult
{
	WIN = 0,
	DEFEAT,
	TIE,
}

public class Monster : MonoBehaviour {

	public bool isAlive = false;
	public List<RitualToolType> values = null;

	private Vector3 direction = Vector3.zero;
	public float movementSpeed = 1.0f;

	public System.Action MonsterSpawned = null;
	public MonsterAlignment alignment = MonsterAlignment.PLAYER_1;
	private string MonsterTag = "Monster";
	private string SpawnTag = "Spawn";

	private void OnMonsterSpawned()
	{
		if ( MonsterSpawned != null )
		{
			MonsterSpawned();
		}
	}
	
	// Use this for initialization
	void Start () {

	}

	public void AddValues( List<RitualToolType> tool ) 
	{
		values = new List<RitualToolType>();
		foreach ( RitualToolType t in tool )
		{
			values.Add(t);
		}	
	}

	public void Spawn( Vector3 position, Vector3 dir, MonsterAlignment align)
	{
		gameObject.SetActive(true);
		alignment = align;
		direction = dir;
		this.transform.position = position;
		isAlive = true;
		Debug.Log(alignment);
		tag = MonsterTag + (int)alignment;
	}

	void OnEnable()
	{
		OnMonsterSpawned();
	}

	void Update()
	{
		if ( !isAlive)
			return;

		//controller.Move();
		transform.position += direction * movementSpeed * Time.deltaTime;
	}

	FightResult Fight(List<RitualToolType> tools)
	{
		int value1 = 0;
		int value2 = 0;
		for ( int index = 0 ; index < 3 ; index ++ )
		{
			if ( values[index] == tools[index] )
				continue;

			if(values[index] == RitualToolType.PAPEL && tools[index] == RitualToolType.TIJERA)
			{
				value2++; // gana el
			}
			if(values[index] == RitualToolType.PAPEL && tools[index] == RitualToolType.PIEDRA)
			{
				value1++; // gano yo
			}

			if(values[index] == RitualToolType.TIJERA && tools[index] == RitualToolType.PAPEL)
			{
				value1++; // gano yo
			}
			if(values[index] == RitualToolType.TIJERA && tools[index] == RitualToolType.PIEDRA)
			{
				value2++; // gana el
			}

			if(values[index] == RitualToolType.PIEDRA && tools[index] == RitualToolType.TIJERA)
			{
				value1++; // gano yo
			}
			if(values[index] == RitualToolType.PIEDRA && tools[index] == RitualToolType.PAPEL)
			{
				value2++; // gana el
			}
		}
		Debug.Log("Fight result - You: " + value1 + " opponent: " + value2 );
		if ( value1 > value2 )
		{
			return FightResult.WIN;
		} else if (value1 < value2){
			return FightResult.DEFEAT;
		}
		return FightResult.TIE;
	}

	void OnCollisionEnter(Collision collision) 
	{
		if ( collision.gameObject.CompareTag("Untagged"))
			return;

		if ( collision.gameObject.CompareTag(gameObject.tag) || collision.gameObject.CompareTag(SpawnTag+(int)alignment) )
			return;

		
		Monster enemy = collision.gameObject.GetComponent<Monster>();
		if (enemy != null && enemy.alignment != alignment && enemy.isAlive && isAlive)
		{
			FightResult result = Fight(enemy.values);
			if ( result == FightResult.WIN )
			{
				enemy.Kill();
			} else if (result == FightResult.DEFEAT )
			{
				this.Kill();
			} else {
				enemy.Kill();
				this.Kill();
			}
		}

	}

	void OnTriggerEnter(Collider other) {
		if ( !other.gameObject.CompareTag(SpawnTag+(int)alignment ))
		{
			PlayerEntity enemy = other.gameObject.GetComponent<PlayerEntity>();
			enemy.KillCultist();
			this.Kill();
		}
		//Debug.Log(other.gameObject.tag + " " + gameObject.tag);

	}

	public void Kill()
	{
		isAlive = false;
		SoundManager.Instance.Play("sacrificio");
		Destroy(gameObject);
	}	

}
