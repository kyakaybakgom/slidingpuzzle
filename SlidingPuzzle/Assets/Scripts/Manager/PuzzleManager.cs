using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public int boardSize = 3; // 3x3, 4x4, 5x5 등
    public GameObject tilePrefab;
    public Transform boardParent;
    public float tileSpacing = 5f;

    private int[,] board;
    private Vector2Int emptyTile;
    private Dictionary<Vector2Int, GameObject> tileObjects = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        GenerateBoard(boardSize);
        ShuffleBoard();
    }

    // 퍼즐 보드 생성
    void GenerateBoard(int size)
    {
        board = new int[size, size];
        tileObjects.Clear();

        // 타일 크기와 전체 보드 크기 계산
        float tileSize = tilePrefab.GetComponent<RectTransform>().rect.width;
        float boardWidth = (tileSize + tileSpacing) * size - tileSpacing;
        float boardHeight = (tileSize + tileSpacing) * size - tileSpacing;

        // x, y축 모두 -방향으로 전체 길이의 절반만큼 이동
        Vector3 offset = new Vector3(-boardWidth / 2, -boardHeight / 2, 0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int num = y * size + x + 1;
                if (num == size * size)
                {
                    board[x, y] = 0; // 빈 칸
                    emptyTile = new Vector2Int(x, y);
                    continue;
                }
                board[x, y] = num;

                GameObject tile = Instantiate(tilePrefab, boardParent);
                tile.GetComponentInChildren<TextMeshProUGUI>().text = num.ToString();
                tile.GetComponent<RectTransform>().anchoredPosition = GetTilePosition(x, y);
                tileObjects[new Vector2Int(x + (int)offset.x, y + (int)offset.y)] = tile;

                // 클릭 이벤트 연결
                int cx = x, cy = y;
                tile.GetComponent<Button>().onClick.AddListener(() => OnTileClicked(cx, cy));
            }
        }
    }

    // 타일 위치 계산
    Vector3 GetTilePosition(int x, int y)
    {
        float size = tilePrefab.GetComponent<RectTransform>().rect.width + tileSpacing;
        return new Vector3(x * size, -y * size, 0);
    }

    // 무작위 초기화 (항상 풀 수 있는 셔플)
    void ShuffleBoard()
    {
        int shuffleCount = boardSize * boardSize * 20;
        for (int i = 0; i < shuffleCount; i++)
        {
            List<Vector2Int> neighbors = GetMovableTiles();
            Vector2Int moveTile = neighbors[Random.Range(0, neighbors.Count)];
            MoveTile(moveTile.x, moveTile.y, false);
        }
    }

    // 이동 가능한 타일 리스트 반환
    List<Vector2Int> GetMovableTiles()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in dirs)
        {
            Vector2Int n = emptyTile + dir;
            if (n.x >= 0 && n.x < boardSize && n.y >= 0 && n.y < boardSize)
                list.Add(n);
        }
        return list;
    }

    // 타일 클릭 시 이동
    void OnTileClicked(int x, int y)
    {
        if (IsMovable(x, y))
        {
            MoveTile(x, y, true);
            if (IsSolved())
            {
                Debug.Log("퍼즐 완료!");
                // 타이머 정지, 점수 계산 등
            }
        }
    }

    // 타일 이동 가능 여부
    bool IsMovable(int x, int y)
    {
        return (Mathf.Abs(emptyTile.x - x) + Mathf.Abs(emptyTile.y - y)) == 1;
    }

    // 타일 이동
    void MoveTile(int x, int y, bool animate)
    {
        if (!tileObjects.ContainsKey(new Vector2Int(x, y))) return;

        // 데이터 교환
        board[emptyTile.x, emptyTile.y] = board[x, y];
        board[x, y] = 0;

        // 오브젝트 이동
        GameObject tile = tileObjects[new Vector2Int(x, y)];
        if (animate)
            StartCoroutine(MoveAnimation(tile, GetTilePosition(emptyTile.x, emptyTile.y)));
        else
            tile.GetComponent<RectTransform>().anchoredPosition = GetTilePosition(emptyTile.x, emptyTile.y);

        tileObjects[new Vector2Int(emptyTile.x, emptyTile.y)] = tile;
        tileObjects.Remove(new Vector2Int(x, y));
        emptyTile = new Vector2Int(x, y);
    }

    // 부드러운 이동 애니메이션
    System.Collections.IEnumerator MoveAnimation(GameObject tile, Vector3 target)
    {
        float t = 0;
        Vector3 start = tile.GetComponent<RectTransform>().anchoredPosition;
        while (t < 1)
        {
            t += Time.deltaTime * 8f;
            tile.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }
        tile.GetComponent<RectTransform>().anchoredPosition = target;
    }

    // 정답 판별
    bool IsSolved()
    {
        int num = 1;
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (x == boardSize - 1 && y == boardSize - 1)
                    return board[x, y] == 0;
                if (board[x, y] != num++)
                    return false;
            }
        }
        return true;
    }
} 