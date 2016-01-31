using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectSacrifice : MonoBehaviour {

    public List<string> InputKeys;
   // private int CountPoolSacrifice = 0;
    private List<int> poolAnimals = new List<int>();

    public Animator bonfire1;
    public Animator bonfire2;
    public Animator bonfire3;

    public Animator button1;
    public Animator button2;
    public Animator button3;
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
         anim.SetBool("IsOnFire", true);
         StartCoroutine(AnimationUpdateCoroutine(anim, OnAnimationEnded));
    }

    void OnAnimationEnded(Animator anim)
    {
        anim.SetBool("IsOnFire", false);
        anim.SetBool("CatPressedSelect", false);
        anim.SetBool("GoatPressedSelect", false);
        anim.SetBool("ChickenPressedSelect", false);
    }

    public void RenderUpdateRestar(GameObject obj, bool result, string KeyNameBool)
    {
        Animator anim = obj.GetComponent<Animator>();
        AnimatorStateInfo animt = anim.GetCurrentAnimatorStateInfo(0);
        /*if (animt.nameHash == Animator.StringToHash(""))
        {
            
        }*/
        if (!anim.IsInTransition(0))

            //  if (anim.GetBool(KeyNameBool) != result)
            anim.SetBool(KeyNameBool, result);
    }

    IEnumerator AnimationUpdateCoroutine(Animator anim, System.Action<Animator> OnAnimationEnded = null)
    {
        //if (anim.GetBool())
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            yield return new WaitForEndOfFrame();
        }
        
        if (OnAnimationEnded != null)
            OnAnimationEnded(anim);
        
    }


    public void SacrificePool(int animalSacrifice)
    {
        poolAnimals.Add(animalSacrifice);
        if (poolAnimals.Count > 2)
        {
            //poolAnimals.Add(animalSacrifice);

            //Load Fire
           
            switch (poolAnimals[0])
            {
                case 1:
                    RenderUpdate(bonfire1, true, "CatPressedSelect");
                    break;
                case 2:
                    RenderUpdate(bonfire1, true, "GoatPressedSelect");
                    break;
                case 3:
                    RenderUpdate(bonfire1, true, "ChickenPressedSelect");
                    break;
            }
                       
            switch (poolAnimals[1])
            {
                case 1:
                    RenderUpdate(bonfire2, true, "CatPressedSelect");
                    break;
                case 2:
                    RenderUpdate(bonfire2, true, "GoatPressedSelect");
                    break;
                case 3:
                    RenderUpdate(bonfire2, true, "ChickenPressedSelect");
                    break;
            }
                      
            switch (poolAnimals[2])
            {
                case 1:
                    RenderUpdate(bonfire3, true, "CatPressedSelect");
                    break;
                case 2:
                    RenderUpdate(bonfire3, true, "GoatPressedSelect");
                    break;
                case 3:
                    RenderUpdate(bonfire3, true, "ChickenPressedSelect");
                    break;
            }

          

            poolAnimals.Clear();
        }

    }


    private void CheckInput()
    {
        
        if (Input.GetButtonDown(InputKeys[0]))
        {          
            button1.SetBool("Pressed", true);
            SacrificePool(1);

        }
        else if (Input.GetButtonUp(InputKeys[0]))
        {
            button1.SetBool("Pressed", false);
        }

        if (Input.GetButtonDown(InputKeys[1]))
        {
            button2.SetBool("Pressed", true);
            SacrificePool(2);
        }
        else if (Input.GetButtonUp(InputKeys[1]))
        {
            button2.SetBool("Pressed", false);
        }

        if (Input.GetButtonDown(InputKeys[2]))
        {
            button3.SetBool("Pressed", true);
            SacrificePool(3);

        }
        else if (Input.GetButtonUp(InputKeys[2]))
        {
            button3.SetBool("Pressed", false);
        }


    }

}
