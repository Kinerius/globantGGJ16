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

	public bool isAlive = true;
	public List<RitualToolType> values = null;

	private Vector3 direction = Vector3.zero;
	public float movementSpeed = 1.0f;

	public System.Action MonsterSpawned = null;
	public MonsterAlignment alignment = MonsterAlignment.PLAYER_1;
	private string MonsterTag = "Monster";
	private string SpawnTag = "Spawn";
	public float power = 0;

	public Animator anim;
	public SpriteRenderer renderer;

	private int MonsterType = 0;
	private int MonsterVariation = 0;
	
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

	public void AddPower()
	{
		power += 1;
	}

	public void AddValues( List<RitualToolType> tool ) 
	{
		int value1 = 0;
		int value2 = 0;
		int value3 = 0;

		values = new List<RitualToolType>();
		foreach ( RitualToolType t in tool )
		{

			values.Add(t);

			if ( t == RitualToolType.PIEDRA )
				value1++;

			if ( t == RitualToolType.PAPEL )
				value2++;

			if ( t == RitualToolType.TIJERA )
				value3++;

		}	

		if ( value1 == 3 )
			MonsterType = 0;

		if ( value2 == 3)
			MonsterType = 1;
		
		if ( value3 == 3)
			MonsterType = 2;

		if ( value1 == 2 )  // M1
		{
			MonsterType = 3;
			MonsterVariation = 0;
		}

		if ( value2 == 2 ) // M2
		{
			MonsterType = 3;
			MonsterVariation = 1;
		}
		
		if ( value3 == 2 ) // M3
		{
			MonsterType = 3;
			MonsterVariation = 2;
		}

		if ( value1 == 1 && value2 == 1 && value3 == 1 )
			MonsterType = 4;
	}

	public void Spawn( Vector3 position, Vector3 dir, MonsterAlignment align)
	{
		//gameObject.SetActive(true);
		alignment = align;
		direction = dir;
		this.transform.position = position;
		//isAlive = true;
		//Debug.Log(alignment);
		tag = MonsterTag + (int)alignment;
		if ( alignment == MonsterAlignment.PLAYER_2 )
		{
			renderer.flipX = true;
		}
	}

	public void Enable()
	{
		isAlive = true;
		//gameObject.SetActive(true);
		gameObject.SetActiveRecursively(true);
		
		anim.SetInteger("MonsterType", MonsterType);
		anim.SetInteger("MonsterVariation", MonsterVariation);
		Debug.Log("Type: " + MonsterType + " Variation: " + MonsterVariation);

		if (power > 0)
		{
			GameObject aura = ObjectPool.instance.GetObjectForType("powerAura", true);
			aura.GetComponent<Effect>().parent = this.transform;
		}
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

	FightResult Fight(List<RitualToolType> tools, float epower)
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
		if ( power > epower)
		{
			return FightResult.WIN;
		} else if (power < epower) {
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
			FightResult result = Fight(enemy.values, enemy.power);
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
			RenderUpdate(true, "StartAtack");
			enemy.RenderUpdate(true, "StartAtack");
		}
	}



    public void RenderUpdate(bool result, string KeyNameBool)
    {
        if (anim.GetBool("StartAtack"))
        {
            anim.SetBool("FinishAtack", true);
            anim.SetBool("StartAtack", false);
        }
        else
        {
            anim.SetBool("FinishAtack", false);
            anim.SetBool("StartAtack", true);
        }

        //anim.SetBool("FinishAtack", true);
        //anim.SetBool(KeyNameBool, result);
       // StartCoroutine(AnimationUpdateCoroutine(anim, OnAnimationEnded));
    }

    void OnAnimationEnded(Animator anim)
    {
        anim.SetBool("StartAtack", false);
        anim.SetBool("FinishAtack", true);
    }
    
    IEnumerator AnimationUpdateCoroutine(Animator anim, System.Action<Animator> OnAnimationEnded = null)
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Demon1Rojo"))
        {            
            yield return new WaitForEndOfFrame();
        }

        if (OnAnimationEnded != null)
            OnAnimationEnded(anim);
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
		
		SoundManager.Instance.Play("demon");
		//SoundManager.Instance.Play("sacrificio");
		
		isAlive = false;
		anim.SetInteger("MonsterType", -1);
		anim.SetInteger("MonsterVariation", -1);

		renderer.flipX = false;

		GameObject explosion = ObjectPool.instance.GetObjectForType("deathParticle", true);
		explosion.transform.position = transform.position;

		ObjectPool.instance.PoolObject(gameObject);
	}	

}
