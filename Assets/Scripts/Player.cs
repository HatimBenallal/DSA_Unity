using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;        //Allows us to use SceneManager

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
    //Delay time in seconds to restart level.
    public float restartLevelDelay = 1f;
    //Number of points to add to player life points when picking up a life object.
    public int pointsPerLife = 5;
    //Number of points to add to player life points whne picking up a coins object.
    public int pointsPerCoins = 2;
    //How much damage a player does to a wall whne chopping it.
    public int wallDamage = 1;

    public int enemyDamage = 10;

    public Text lifeText;
    public Text coinsText;

    //Used to store a refrence to the Player's animator component
    private Animator animator;
    //Used to store player life points total during level.
    private int life;
    private int maxlife = 100;
    private int coins;
    private int escene;

    private Vector2 touchOrigin = -Vector2.one;
    public List<Enemy> enemies;

    
    //Start overrides the Start function of MovingObject
    protected override void Start ()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        //Get the current life point total stored in GameManager.instance between levels.
        life = GameManager.instance.playerLifePoints;
        //coins = GameManager.instance.playerCoinsPoints;

        lifeText.text = "life: " + life;
        //coinsText.text = "coins: "+coins;

        //Call the Start function of the MovingObject base class.
        base.Start ();
    }


    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable ()
    {
        //When Player object is disabled, store the current local life total in the GameManager so it can be re-loaded in next level.
        GameManager.instance.playerLifePoints = life;
        //GameManager.instance.playerCoinsPoints = coins;
    }


    void Update ()
    {
        //If it's not the player's turn, exit the function.
        if(!GameManager.instance.playersTurn) return;

        int horizontal = 0;      //Used to store the horizontal move direction.
        int vertical = 0;        //Used to store the vertical move direction.

        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER 
    
        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int) (Input.GetAxisRaw ("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if(horizontal != 0)
        {
            vertical = 0;
        }
    
    //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
        #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
 

        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            if (myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }

        #endif
        //Check if we have a non-zero value for horizontal or vertical
        if(horizontal != 0 || vertical != 0)
        {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove<Wall> (horizontal, vertical);
            AttemptMove<Enemy> (horizontal, vertical);
        }
        
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove <T> (xDir, yDir);

        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;

        //Since the player has moved and lost life points, check if the game has ended.
        CheckIfGameOver ();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;
    }

    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove <T> (T component)
    {
        if (component as Wall){
            //Set hitWall to equal the component passed in as a parameter.
            
            Wall hitWall = component as Wall;

            //Call the DamageWall function of the Wall we are hitting.
            hitWall.DamageWall (wallDamage);
            animator.SetTrigger("playerChop");
        }
        if (component as Enemy){
            Enemy hitEnemy = component as Enemy;

            hitEnemy.DamageEnemy (enemyDamage);

            animator.SetTrigger("playerChop");
        }        
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D (Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if(other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke ("Restart", restartLevelDelay);

            //Disable the player object since level is over.
            enabled = false;
        }
        else if(other.tag == "Exit1")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke ("Back", restartLevelDelay);

            //Disable the player object since level is over.
            enabled = false;
        }

        //Check if the tag of the trigger collided with is life.
        else if(other.tag == "Life")
        {
            //Add pointsPerlife to the players current life total.
            if (life<maxlife)
                life += pointsPerLife;
            lifeText.text = "+" + pointsPerLife + " life: " + life;

            //Disable the life object the player collided with.
            other.gameObject.SetActive (false);
        }

        //Check if the tag of the trigger collided with is coins.
        else if(other.tag == "Coins")
        {
            //Add pointsPercoins to players life points total
            coins = GameManager.instance.playerCoinsPoints;
            coins += pointsPerCoins;
            coinsText.text = "+" + pointsPerCoins + " coins: " + coins;
            GameManager.instance.playerCoinsPoints = coins;
            //Disable the coins object the player collided with.
            other.gameObject.SetActive (false);
        }
    }


    //Restart reloads the scene when called.
    private void Restart ()
    {
        escene = GameManager.instance.scene;
        if (escene < 5)
        {
            SceneManager.LoadScene(escene+1);
            GameManager.instance.scene = escene+1;        
        }

        if (escene == 5){
            SceneManager.LoadScene(0);
            GameManager.instance.scene = 0;
        }

        GameManager.instance.back = false;
    }

    private void Back ()
    {
        escene = GameManager.instance.scene;
        if (escene > 1)
        {
            SceneManager.LoadScene(escene-1);
            GameManager.instance.scene = escene-1;
            
            GameManager.instance.back = true;

        }
    }


    //Loselife is called when an enemy attacks the player.
    //It takes a parameter loss which specifies how many points to lose.
    public void Loselife (int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger ("playerHit");

        //Subtract lost life points from the players total.
        life -= loss;
        lifeText.text = "-" + loss + " life: " + life;

        //Check to see if game has ended.
        CheckIfGameOver ();
    }

    //CheckIfGameOver checks if the player is out of life points and if so, ends the game.
    private void CheckIfGameOver ()
    {
        //Check if life point total is less than or equal to zero.
        if (life <= 0) 
        {
            GameManager.instance.GameOver();
        }
    }
}