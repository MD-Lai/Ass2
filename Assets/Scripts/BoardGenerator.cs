using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoardGenerator : MonoBehaviour {
    private int TILERIGHT = 0;
    private int TILEDOWN = 1;
    private int TILEVISITED = 2;
    private int UP = 0;
    private int RIGHT = 1;
    private int DOWN = 2;
    private int LEFT = 3;
    
    private int nVisited = 0;

    private GameObject pinPrefab;
    private GameObject wallPrefab;
    private GameObject floorPrefab;

    private ArrayList components = new ArrayList();

    public Text countText;
    public int boardSizeX = 5;
    public int boardSizeZ = 5;
    public float noSpawnRate = 0.0f;
    public BallControl ballInfo;
    public Shader shader;
    public PointLight pointLight;
    

    private GameObject ball;

    private bool[,][] wallArray;
    private Vector3 ballPos;
    private bool complete = false;

    // Use this for initialization
    void Start() {
        pinPrefab = (GameObject)Resources.Load("prefabs/Pin", typeof(GameObject));
        wallPrefab = (GameObject)Resources.Load("prefabs/Wall", typeof(GameObject));
        floorPrefab = (GameObject)Resources.Load("prefabs/Floor", typeof(GameObject));
        ball = GameObject.Find("MainBall");
        ballPos = ball.transform.localPosition;

        wallArray = new bool[boardSizeX, boardSizeZ][];

        updateScore();
        generateMaze(boardSizeX, boardSizeZ);
        
    }
	
	// Update is called once per frame
	void Update () {

        if (completedMaze() || Input.GetKeyDown(KeyCode.R)) {
            if (complete) {
                ballInfo.respawn();
                ballInfo.addScore((boardSizeX+boardSizeZ)/2);
                boardSizeX++;
                boardSizeZ++;
            }
            
            generateMaze(boardSizeX, boardSizeZ);

        }
        updateScore();

        foreach (GameObject component in components) {
            MeshRenderer componentRenderer = component.gameObject.GetComponent<MeshRenderer>();

            // Pass updated light positions to shader
            componentRenderer.material.SetColor("_PointLightColor", this.pointLight.color);
            componentRenderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());
        }
    }

    void updateScore() {
        countText.text = "Score: " + ballInfo.getScore().ToString();
    }

    bool completedMaze() {
        ballPos = ball.transform.localPosition;
        if (ballPos.x > boardSizeX * 2 - 2 && ballPos.z < -boardSizeZ * 2 + 2) {
            complete = true;
            return true;
        }
        else {
            complete = false;
            return false;
        }
    }

    void generateMaze(int boardX, int boardZ) {
        dfsMaze(boardX, boardZ);

        placeFloor(boardX, boardZ);
        placePins(boardX, boardZ);
        placeWalls(boardX, boardZ);

    }
    // places pins based on size of board in number of squares desired
    // pins have to be at least 
    void placePins(int boardX, int boardZ) {
        
        for (int x = 0; x <= boardX; x ++) {
            for (int z = 0;  z <= boardZ; z ++) {
                GameObject pin = GameObject.Instantiate<GameObject>(pinPrefab);
                pin.transform.localPosition = new Vector3(x * 2 - 1, 0.75f, -z * 2 + 1);
                
                components.Add(pin);
            }
        }
    }

    
    void placeFloor(int boardX, int boardZ) {
        for(int x = 0; x < boardX; x++) {
            for (int z = 0; z < boardZ; z++) {
                if (Random.value < (1-noSpawnRate) || x+z == 0) {
                    GameObject floor = GameObject.Instantiate<GameObject>(floorPrefab);
                    floor.transform.localPosition = new Vector3(x * 2, -0.25f, -z * 2);

                    components.Add(floor);
                }
            }
        }
    }
    
    void placeWalls(int boardX, int boardZ) {
        // horizontal walls
        // top wall
        for(int x = 0; x < boardX; x++) {
            GameObject wall = GameObject.Instantiate<GameObject>(wallPrefab);
            wall.transform.localPosition = new Vector3(x * 2, 0.75f, 0.5f * 2);

            components.Add(wall);
        }

        // rest of horizontal walls
        for(int x = 0; x < boardX; x ++) {
            for (int z = 0; z < boardZ; z ++) {
                //UnityEngine.Debug.Log(x + " " + z);
                //place walls in appropriate locations...
                if (wallArray[x,z][TILEDOWN] == true) {
                    GameObject wall = GameObject.Instantiate<GameObject>(wallPrefab);
                    wall.transform.localPosition = new Vector3(x*2, 0.75f, -2*z-1);

                    components.Add(wall);
                }
            }
        }

        // vertical walls
        // left wall
        for (int z = 0; z < boardZ; z++) {
            GameObject wall = GameObject.Instantiate<GameObject>(wallPrefab);
            wall.transform.localPosition = new Vector3(-0.5f * 2, 0.75f, -z * 2);
            wall.transform.Rotate(new Vector3(0, 90, 0));

            components.Add(wall);
        }
        // rest of vertical walls
        for (int x = 0; x < boardX; x ++) {
            for (int z = 0; z < boardZ; z++) {
                //place walls in appropriate locations
                if (wallArray[x, z][TILERIGHT] == true) { 
                    GameObject wall = GameObject.Instantiate<GameObject>(wallPrefab);
                    wall.transform.localPosition = new Vector3(2*x + 1, 0.75f, -z * 2);
                    wall.transform.Rotate(new Vector3(0, 90, 0));

                    components.Add(wall);
                }
            }
        }
    }

    
    void dfsMaze(int boardX, int boardZ) {

        wallArray = new bool[boardSizeX, boardSizeZ][];
        clearMaze(boardX, boardZ);
        
        explore(0, 0, boardX,boardZ);

    }

    void explore(int x, int y, int boardX, int boardZ) {

        wallArray[x, y][TILEVISITED] = true;

        if (nVisited <= boardX * boardZ) {
            int[] nextVisits = new int[4] { -1, -1, -1, -1 };
            int nbr;
            // make random order of adjacent squares to visit 
            // (UP, DOWN, LEFT, RIGHT) as defined above
            for (int i = 0; i < nextVisits.Length; i++) {

                do {
                    nbr = Random.Range(0, nextVisits.Length);
                } while (nbrInIntArray(nextVisits, nbr));
                
                nextVisits[i] = nbr;
                
            }

            for (int i = 0; i < nextVisits.Length; i++) {

                if (y > 0 && nextVisits[i] == UP && wallArray[x, y - 1][TILEVISITED] == false) {
                    wallArray[x, y - 1][TILEDOWN] = false;
                    nVisited++;
                    explore(x, y - 1, boardX, boardZ);
                    
                }
                

                if (y < boardZ-1 && nextVisits[i] == DOWN && wallArray[x, y + 1][TILEVISITED] == false) {
                    wallArray[x, y][TILEDOWN] = false;
                    nVisited++;
                    explore(x, y + 1, boardX, boardZ);
                    
                }
                
                if (x > 0 && nextVisits[i] == LEFT && wallArray[x - 1, y][TILEVISITED] == false) {
                    wallArray[x - 1, y][TILERIGHT] = false;
                    nVisited++;
                    explore(x - 1, y, boardX, boardZ);
                }

                if (x < boardX-1 && nextVisits[i] == RIGHT && wallArray[x + 1, y][TILEVISITED] == false) {
                    wallArray[x, y][TILERIGHT] = false;
                    nVisited++;
                    explore(x + 1, y, boardX, boardZ);
                }
            }
        }
    }

    void clearMaze(int boardX, int boardZ) {
        for (int i = 0; i < boardX; i++) {
            for (int j = 0; j < boardZ; j++) {
                // true if a wall should be present
                wallArray[i, j] = new bool[] { true, true, false };
            }
        }

        foreach (GameObject component in components) {
            Destroy(component);
        }
        components = new ArrayList();
        
        nVisited = 0;
    }

    bool nbrInIntArray(int[] checkArray, int nbr) {

        for(int i = 0; i < checkArray.Length; i++) {
            if (checkArray[i] == nbr)
                return true;
        }

        return false;

    }

    public int getBoardX() {
        return boardSizeX;
    }

    public int getBoardZ() {
        return boardSizeZ;
    }
}
