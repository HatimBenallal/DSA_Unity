using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Boss : MovingObject
{
    public int playerDamage=15;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    public Sprite dmgSprite;
    public int vidaenemy = 40;
    

    
    // Start is called before the first frame update
    protected override void Start ()
    {
        GameManager.instance.AddBossToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag ("Player").transform;
        base.Start();        
    }


    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove <T> (xDir,yDir);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        if (GameManager.instance.back == false)
        {
            int xDir = 0;
            int yDir = 0;
            //If the difference in positions is approximately zero (Epsilon) do the following:
            // if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)

            //     //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            //     yDir = target.position.y > transform.position.y ? 1 : -1;

            //If the difference in positions is not approximately zero (Epsilon) do the following:
            //if (Mathf.Abs (target.position.y - transform.position.y) < float.Epsilon)
                //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
            if(Mathf.Abs (target.position.y - transform.position.y) < float.Epsilon)    
                xDir = target.position.x > transform.position.x ? 1 : -1;

            else
                yDir = target.position.y > transform.position.y ? 1 : -1;

            //Call the AttemptMove function and pass in the generic parameter Player, because Enemy is moving and expecting to potentially encounter a Player
            AttemptMoveEnemy <Player> (xDir, yDir);
        }
    }

    protected override void OnCantMove <T> (T component)
    {
        if (atack!=false){
            Player hitPlayer = component as Player;
            animator.SetTrigger("bossAttack");
            hitPlayer.Loselife(playerDamage);
        }
    }
    
    private bool atack=true; 

    public void DamageEnemy (int loss)
    {
        vidaenemy -= loss;
        if  (vidaenemy<=loss)
        {
            GameManager.instance.RemoveBoss(this);
            
            gameObject.SetActive(false);
            atack=false;
            
        }
        
    }
}
