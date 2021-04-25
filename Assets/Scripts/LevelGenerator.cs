using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance { get; set; }

    // Configuration
    public GridObject[] BlockPrefabs;
    public GridObject NonDrillableBlockPrefab;
    
    public int Width = 9;
    public int Height = 30;
    public int MaxNonDrillablePerRow = 2;
    public float NonDrillableChance = .1f;

    // Runtime
    public int Difficulty = 4;

    void Awake() {
        if (Instance) {
            Destroy(this);
        }
        else Instance = this;
    }

    void Start() {
        Generate(0);
    }

    void Update() {
        
    }

    public void Generate(int atY) {
        for (int y = 0; y > -Height; y--) {
            int nonDrillableThisRow = 0;
            for (int x = 0; x < Width; x++) {
                int i = Random.Range(0, Difficulty + 1);
                var pos = new Vector3(x, y + atY, 0);
                
                GridObject box;
                if (Random.Range(0f, 1f) <= NonDrillableChance && nonDrillableThisRow < MaxNonDrillablePerRow) {
                    nonDrillableThisRow++;
                    box = Instantiate(NonDrillableBlockPrefab, pos, Quaternion.identity);
                }
                else {
                    box = Instantiate(BlockPrefabs[i], pos, Quaternion.identity);
                }
                if (y == -Height+1) {
                    box.CanFall = false;
                }
            }
        }
    }
}
