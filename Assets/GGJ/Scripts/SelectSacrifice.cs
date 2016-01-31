using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimalType
{
	CAT = 0,
	GOAT,
	SLAVE,
}


public class SelectSacrifice : MonoBehaviour {

    public List<string> InputKeys;
   	// private int CountPoolSacrifice = 0;
    private List<int> poolAnimals = new List<int>();

    public Animator button1;
    public Animator button2;
    public Animator button3;

	public List<Animator> bonfires;

	public PlayerEntity player;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    public void RenderUpdate(Animator anim, bool result, string KeyNameBool)
    {
         anim.SetBool(KeyNameBool, result);         
    }

    void OnAnimationEnded(Animator anim)
    {
        anim.SetInteger("AnimalType", -1);
		
    }

    public void RenderUpdateRestar(GameObject obj, bool result, string KeyNameBool)
    {
        Animator anim = obj.GetComponent<Animator>();

        if (!anim.IsInTransition(0))
            anim.SetBool(KeyNameBool, result);
    }

    IEnumerator AnimationUpdateCoroutine(Animator anim, System.Action<Animator> OnAnimationEnded = null)
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            yield return new WaitForEndOfFrame();
        }
        
        if (OnAnimationEnded != null)
            OnAnimationEnded(anim);        
    }


    public void SacrificePool(AnimalType animalSacrifice)
    {
		if (player.isOnCooldown || player.isCasting)
			return;

        AddAnimalToBonfire(animalSacrifice);
        if (poolAnimals.Count > 2)
        {
			foreach ( Animator bonfire in bonfires)
			{
				bonfire.SetTrigger("StartFire");
				SoundManager.Instance.Play("fire1", 1.0f , Random.Range(0.75f, 1.25f));
				StartCoroutine(AnimationUpdateCoroutine(bonfire, OnAnimationEnded));
			}
          
            poolAnimals.Clear();
        }

    }

	public void AddAnimalToBonfire( AnimalType animal )
	{
		if ( poolAnimals == null )
			poolAnimals = new List<int>();

		poolAnimals.Add((int)animal);
		UpdateBonfire(poolAnimals.Count-1, animal);
	}

	public void UpdateBonfire(int index, AnimalType animal)
	{
		bonfires[index].SetTrigger("Spawn");
	    bonfires[index].SetInteger("AnimalType", (int)animal);
	}


    private void CheckInput()
    {
        
        if (Input.GetButtonDown(InputKeys[0]))
        {          
            button1.SetBool("Pressed", true);
            SacrificePool(AnimalType.CAT);
        }
        else if (Input.GetButtonUp(InputKeys[0]))
        {
            button1.SetBool("Pressed", false);
        }

        if (Input.GetButtonDown(InputKeys[1]))
        {
            button2.SetBool("Pressed", true);
			SacrificePool(AnimalType.GOAT);
        }
        else if (Input.GetButtonUp(InputKeys[1]))
        {
            button2.SetBool("Pressed", false);
        }

        if (Input.GetButtonDown(InputKeys[2]))
        {
            button3.SetBool("Pressed", true);
			SacrificePool(AnimalType.SLAVE);

        }
        else if (Input.GetButtonUp(InputKeys[2]))
        {
            button3.SetBool("Pressed", false);
        }


    }

}
