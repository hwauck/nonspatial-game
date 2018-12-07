using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerStartRoom : MonoBehaviour {

    public float boardScalingFactor = 1;

    private DataCollector dataCollector;
    public Vector2 predictedSquare;
    private Vector2 square;
    public Vector2 startingSquare;
    private Vector3 direction;

    private Sprite upSprite;
    private Sprite downSprite;
    private Sprite leftSprite;
    private Sprite rightSprite;
    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start () {
        dataCollector = GameObject.Find("DataCollector").GetComponent<DataCollector>();

        direction = Vector3.right;
        square = startingSquare;

        upSprite = Resources.Load<Sprite>("player_astronaut/player-up");
        downSprite = Resources.Load<Sprite>("player_astronaut/player-down");
        leftSprite = Resources.Load<Sprite>("player_astronaut/player-left");
        rightSprite = Resources.Load<Sprite>("player_astronaut/player-right");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private bool offScreen()
    {
        if (GameObject.Find("Block" + coordinatesToSquare(predictedSquare)) == null)
        {
            // can't move - offscreen
            return true;
        }
        return false;
    }

    private string coordinatesToSquare(Vector2 coordinates)
    {
        return coordinates.x.ToString() + coordinates.y.ToString();
    }

    public void turnDown()
    {
        direction = Vector3.down;
        predictedSquare.x = square.x + 1;
        predictedSquare.y = square.y;
        spriteRenderer.sprite = downSprite;

    }

    public void turnUp()
    {
        direction = Vector3.up;
        predictedSquare.x = square.x - 1;
        predictedSquare.y = square.y;
        spriteRenderer.sprite = upSprite;

    }

    public void turnLeft()
    {
        direction = Vector3.left;
        predictedSquare.x = square.x;
        predictedSquare.y = square.y - 1;
        spriteRenderer.sprite = leftSprite;

    }

    public void turnRight()
    {
        direction = Vector3.right;
        predictedSquare.x = square.x;
        predictedSquare.y = square.y + 1;
        spriteRenderer.sprite = rightSprite;

    }

    private void tryMove()
    {
        if (!offScreen())
        {
            // player moves physically in the direction they are turned
            Debug.Log("PLAYER MOVED");

            string newLoc = move();

        }
 
    }

    public string move()
    {
        dataCollector.AddMove();

        transform.Translate(direction * 2f * boardScalingFactor, Space.World);
        string predictedSquareName = coordinatesToSquare(predictedSquare);
        Debug.Log("MOVED TO " + predictedSquareName);

        Vector3 oldSquare = square;
        square = predictedSquare;

        predictedSquare.x = 2f * square.x - oldSquare.x;
        predictedSquare.y = 2f * square.y - oldSquare.y;
        return predictedSquareName;

    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.name.Equals("ToIce"))
        {
            SceneManager.LoadScene("ice");
        } else if (otherCollider.gameObject.name.Equals("ToIce2"))
        {
            SceneManager.LoadScene("ice_2");
        } else if (otherCollider.gameObject.name.Equals("ToIce3"))
        {
            SceneManager.LoadScene("ice_3");
        } else if (otherCollider.gameObject.name.Equals("ToIce4"))
        {
            SceneManager.LoadScene("ice_4");
        } else if (otherCollider.gameObject.name.Equals("ToIce5"))
        {
            SceneManager.LoadScene("ice_5");
        } else if (otherCollider.gameObject.name.Equals("ToTimedIce"))
        {
            SceneManager.LoadScene("ice_timed");
        } else if (otherCollider.gameObject.name.Equals("ToTile1"))
        {
            SceneManager.LoadScene("tile");
        } else if (otherCollider.gameObject.name.Equals("ToTile2"))
        {
            SceneManager.LoadScene("tile2");
        } else if (otherCollider.gameObject.name.Equals("ToTile3"))
        {
            SceneManager.LoadScene("tile3");
        } else if (otherCollider.gameObject.name.Equals("ToTileHard"))
        {
            SceneManager.LoadScene("tileHard");
        }
    }

    // Update is called once per frame
    void Update () {
		//This part is for moving
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
            turnDown();
            tryMove();
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
            turnUp();
            tryMove();
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
            turnRight();
            tryMove();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
            turnLeft();
            tryMove();
		}
		//This part is for going to other scenes
		//if(this.transform.position.x==-2.5&&this.transform.position.y==-8)  SceneManager.LoadScene("ice");
		//else if(this.transform.position.x==1.5&&this.transform.position.y==-8) SceneManager.LoadScene("ice_2");
		//else if(this.transform.position.x==5.5&&this.transform.position.y==-8) SceneManager.LoadScene("ice_3");
		//else if(this.transform.position.x==11.5&&this.transform.position.y==-4) SceneManager.LoadScene("ice_4");
		//else if(this.transform.position.x==11.5&&this.transform.position.y==2) SceneManager.LoadScene("ice_5");
		//else if(this.transform.position.x==-6.5&&this.transform.position.y==-4) SceneManager.LoadScene("tile");
		//else if(this.transform.position.x==-6.5&&this.transform.position.y==2) SceneManager.LoadScene("tile2");
		//else if(this.transform.position.x==-0.5&&this.transform.position.y==6) SceneManager.LoadScene("tile3");
		//else if(this.transform.position.x==5.5&&this.transform.position.y==6) SceneManager.LoadScene("tileHard");

	}
}
