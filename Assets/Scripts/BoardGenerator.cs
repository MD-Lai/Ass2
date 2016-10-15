using UnityEngine;
using System.Collections;

public class BoardGenerator : MonoBehaviour {
    private int UP = 0;
    private int RIGHT = 1;
    private int DOWN = 2;
    private int LEFT = 3;
    private int VISITED = 4;

    private int nVisited = 0;

    private GameObject pinPrefab;
    private GameObject wallPrefab;
    private GameObject floorPrefab;
    public int boardSizeX = 10;
    public int boardSizeZ = 10;
    public bool[,][] wallArray;
    Random rnd = new Random();

    // Use this for initialization
    void Start() {
        pinPrefab = (GameObject)Resources.Load("prefabs/Pin", typeof(GameObject));
        wallPrefab = (GameObject)Resources.Load("prefabs/Wall", typeof(GameObject));
        floorPrefab = (GameObject)Resources.Load("prefabs/Floor", typeof(GameObject));

        if (boardSizeX % 2 != 0) boardSizeX -= 1;
        if (boardSizeZ % 2 != 0) boardSizeZ -= 1;

        dfsMaze(boardSizeX, boardSizeZ);

        placePins(boardSizeX, boardSizeZ);
        placeWalls(boardSizeX, boardSizeZ);
        //placeFloor(boardSizeX, boardSizeZ);

        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R)) {
            dfsMaze(boardSizeX, boardSizeZ);
            placePins(boardSizeX, boardSizeZ);
            placeWalls(boardSizeX, boardSizeZ);
        }
    }

    // places pins based on size of board in number of squares desired
    // pins have to be at least 
    void placePins(int boardX, int boardZ) {
        
        for (int x = 0; x <= boardX; x ++) {
            for (int z = 0;  z <= boardZ; z ++) {
                GameObject pin = GameObject.Instantiate<GameObject>(pinPrefab);
                pin.transform.localPosition = new Vector3(x * 2 - 1, 0.75f, -z * 2 + 1);
            }
        }
    }

    /*
    void placeFloor(int boardX, int boardZ) {
        for(int x = 0; x < boardX; x++) {
            for (int z = 0; z < boardZ; z++) {
                GameObject floor = GameObject.Instantiate<GameObject>(floorPrefab);
                floor.transform.localPosition = new Vector3(x * 2 - 2, -0.25f, -z * 2 + 2);
            }
        }
    }
    */
    void placeWalls(int boardX, int boardZ) {
        // horizontal walls
        // top wall
        for(int x = 0; x < boardX; x++) {
            GameObject wall = GameObject.Instantiate<GameObject>(wallPrefab);
            wall.transform.localPosition = new Vector3(x * 2, 0.75f, 0.5f * 2);
        }

        // rest of horizontal walls
        for(int x = 0; x < boardX; x ++) {
            for (int z = 0; z < boardZ; z ++) {
                //Debug.Log(x + " " + z);
                //place walls in appropriate locations...
                //Debug.Log("placing " + x + " " + z);
                if (wallArray[x,z][DOWN] == true) {
                    GameObject wall = GameObject.Instantiate<GameObject>(wallPrefab);
                    wall.transform.localPosition = new Vector3(x*2, 0.75f, -2*z-1);
                }
            }
        }

        // vertical walls
        // left wall
        for (int z = 0; z < boardZ; z++) {
            GameObject wall = GameObject.Instantiate<GameObject>(wallPrefab);
            wall.transform.localPosition = new Vector3(-0.5f * 2, 0.75f, -z * 2);
            wall.transform.Rotate(new Vector3(0, 90, 0));
        }
        // rest of vertical walls
        for (int x = 0; x < boardX; x ++) {
            for (int z = 0; z < boardZ; z++) {
                //place walls in appropriate locations
                if (wallArray[x, z][RIGHT] == true) { 
                    GameObject wall = GameObject.Instantiate<GameObject>(wallPrefab);
                    wall.transform.localPosition = new Vector3(2*x + 1, 0.75f, -z * 2);
                    wall.transform.Rotate(new Vector3(0, 90, 0));
                }
            }
        }
    }

    
    void dfsMaze(int boardX, int boardZ) {
        wallArray = new bool[boardX, boardZ] [];
        Debug.Log(wallArray.Length);

        clearMaze(boardX, boardZ);

        explore(0, 0, boardX,boardZ);

    }

    void explore(int x, int y, int boardX, int boardZ) {

        wallArray[x, y][VISITED] = true;

        Debug.Log("boardX " + boardX + " boardZ " + boardZ);

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
                
                //nextVisits[i] = i;


            }

            for (int i = 0; i < nextVisits.Length; i++) {

                if (y > 0 && nextVisits[i] == UP && wallArray[x, y - 1][VISITED] == false) {
                    wallArray[x, y - 1][DOWN] = false;
                    wallArray[x, y][UP] = false;
                    Debug.Log(nVisited + "visiting " + x + " " + (y - 1));
                    nVisited++;
                    explore(x, y - 1, boardX, boardZ);
                    
                }

                if (y < boardZ-1 && nextVisits[i] == DOWN && wallArray[x, y + 1][VISITED] == false) {
                    wallArray[x, y + 1][UP] = false;
                    wallArray[x, y][DOWN] = false;
                    Debug.Log(nVisited + "visiting " + x + " " + (y + 1));
                    nVisited++;
                    explore(x, y + 1, boardX, boardZ);
                    
                }

                if (x > 0 && nextVisits[i] == LEFT && wallArray[x - 1, y][VISITED] == false) {
                    wallArray[x - 1, y][RIGHT] = false;
                    wallArray[x, y][LEFT] = false;
                    Debug.Log(nVisited + "visiting " + (x - 1) + " " + y);
                    nVisited++;
                    explore(x - 1, y, boardX, boardZ);

                }

                if (x < boardX-1 && nextVisits[i] == RIGHT && wallArray[x + 1, y][VISITED] == false) {
                    wallArray[x + 1, y][LEFT] = false;
                    wallArray[x, y][RIGHT] = false;
                    Debug.Log(nVisited + " visiting " + (x + 1) + " " + y);
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
                wallArray[i, j] = new bool[] { true, true, true, true, false };
            }
        }
        nVisited = 0;
    }

    bool nbrInIntArray(int[] checkArray, int nbr) {

        for(int i = 0; i < checkArray.Length; i++) {
            if (checkArray[i] == nbr)
                return true;
        }

        return false;

    }
    

}
