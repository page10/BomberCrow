using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public MapManager mapManager;  // Assign this in the Unity Inspector
    public Camera mainCamera;      // Assign the main camera in the Unity Inspector
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject fireBallPrefab;
    [FormerlySerializedAs("enemyPrefab")] [SerializeField] private GameObject enemyPrefabFox; 
    [FormerlySerializedAs("enemyPrefab2")] [SerializeField] private GameObject enemyPrefabOwl;
    public GameObject explosionCenterPrefab;
    public GameObject explosionLinePrefab;  
    public GameObject explosionEndPrefab;
    public Text timerText;  
    
    private float gameDuration = 120f;  // Game duration in seconds
    private float timeRemaining;        // Time remaining in the game
    
    private int snowPileBlastedCount = 0;  // Number of snow piles blasted by the player
    private int enemySlaughteredCount = 0; // Number of enemies killed by the player
    
    private Character _character;
    private Vector2 CrowGridPos => _character ? mapManager.PositionInGrid(_character.transform.position) : Vector2.zero;
    
    //public int maxFireballs = 1;        // Max fireballs that can be placed
    //private int currentFireballs = 0;   // Currently placed fireballs

    // List of fireballs. You may detonate all bombs by controller while PowerUp has been taken.
    private List<Fireball> _currentFireBalls = new List<Fireball>();

    // 当前正在进行的爆炸
    private List<Explosion> _explosions = new List<Explosion>();
    private List<Explosion> _toRemoveExplosions = new List<Explosion>(); //要删除的

    private List<Enemy> _enemies = new List<Enemy>();


    private enum BattleState
    {
        Playing,
        Over
    }

    private BattleState _state = BattleState.Playing;
    
    
    void Start() {
        // Generate the map
        mapManager.GenerateMap(out Vector2Int playerStartPosition);
        PlacePlayerCharacter(playerStartPosition);
        CreateEnemies(6);

        // Center the camera
        CenterCamera();
        
        // Initialize the timer when the game starts
        timeRemaining = gameDuration;
    }

    void CenterCamera() {
        // Calculate the center point of the map
        float mapCenterX = mapManager.width / 2f - 0.5f;
        float mapCenterY = mapManager.height / 2f - 0.5f;

        // Set the camera's position to the center of the map
        mainCamera.transform.position = new Vector3(mapCenterX, mapCenterY, mainCamera.transform.position.z);
    }
    
    private void PlacePlayerCharacter(Vector2Int position) {
        GameObject go = Instantiate(playerPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
        _character = go.GetComponent<Character>();
        _character.TryMove(MoveDirection.Down, 0);
        _character.transform.SetParent(transform);
    }

    private void CreateEnemies(int enemyCount)
    {
        //筛选出格子
        List<Vector2Int> eg = mapManager.EmptyGrids();
        if (_character)
        {
            Vector2Int cg = mapManager.PositionInGrid(_character.transform.position);
            for (int i = -3; i <= 3; i++)
            for (int j = -3; j <= 3; j++)
            {
                Vector2Int g = new Vector2Int(i + cg.x, j + cg.y);
                eg.Remove(g);  //玩家周围3格内不刷怪
            }
        }
        while (eg.Count > enemyCount) eg.RemoveAt(Random.Range(0, eg.Count));
        //刷怪
        foreach (Vector2Int g in eg) 
        {
            EnemyType type = (Random.value > 0.5f) ? EnemyType.Fox: EnemyType.Owl;
            PlaceEnemyCharacter(g, type);
        }
    }
    
    private void PlaceEnemyCharacter(Vector2Int position, EnemyType type) {
        GameObject enemyPrefab = type == EnemyType.Fox ? enemyPrefabFox : enemyPrefabOwl;
        GameObject go = Instantiate(enemyPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
        Enemy e = go.GetComponent<Enemy>();
        e.TryMove(MoveDirection.Down, 0);
        List<MoveDirection> md = new List<MoveDirection> { MoveDirection.Up ,MoveDirection.Down,MoveDirection.Left, MoveDirection.Right};
        e.movingDir = md[Random.Range(0, md.Count)];
        e.transform.SetParent(transform);
        _enemies.Add(e);
    }

    private void Update()
    {
        float delta = Time.deltaTime;

        // Countdown logic
        if (timeRemaining > 0)
        {
            timeRemaining -= delta;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                OnTimerEnd();
            }
        }
        timerText.text = GetFormattedTime();
        
        if (_state == BattleState.Playing)
        {
            //所有list线对于所在的格子继续造成伤害
            foreach (Explosion explosion in _explosions)
            {
                if (!explosion) continue;
                ExplosionEffect(explosion);
                explosion.DoUpdate(delta);
            }
            //统一处理要删除的爆破线，这是list的缺陷，只能这么做
            foreach (Explosion explosion in _toRemoveExplosions)
            {
                _explosions.Remove(explosion);
                if (explosion && explosion.gameObject) Destroy(explosion.gameObject);
            }
        
            //所有的敌人行动
            foreach (Enemy enemy in _enemies)
            {
                if (enemy.Dead) continue;
                //尝试移动
                if (!MoveCharacter(enemy, enemy.movingDir, delta))
                {
                    //todo 如果敌人移动失败，就会运行ai，现在先随机换个方向
                    List<MoveDirection> md = new List<MoveDirection>
                        { MoveDirection.Up, MoveDirection.Down, MoveDirection.Left, MoveDirection.Right };
                    md.Remove(enemy.movingDir);
                    enemy.movingDir = md[Random.Range(0, md.Count)];
                }
                //尝试杀死玩家
                if (Mathf.Abs(Vector2.Distance(enemy.transform.position, _character.transform.position)) < enemy.killRange)
                {
                    EndGame();
                }
            }

            HandleInput();
        }else if (_state == BattleState.Over)
        {
            //todo 真的结束了，判断角色是否删除了自己， 危险，但临时有效，之后要改的
            if (!_character || !_character.gameObject)
            {
                ScenesManager.Instance.LoadGameOver();
                
            }
        }
    }
    
    private void OnTimerEnd()
    {
        Debug.Log("Time's up! You lose.");
        // Trigger the lose condition, maybe stop player movement, and display a loss screen
        EndGame(false);
    }
    
    // Function to display the remaining time
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    private void HandleInput()
    {
        float delta = Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) {  // Move up
            MoveCharacter(_character, MoveDirection.Up, delta);
        } else if (Input.GetKey(KeyCode.S)) {  // Move down
            MoveCharacter(_character, MoveDirection.Down, delta);
        } else if (Input.GetKey(KeyCode.A)) {  // Move left
            MoveCharacter(_character, MoveDirection.Left, delta);
        } else if (Input.GetKey(KeyCode.D)) {  // Move right
            MoveCharacter(_character, MoveDirection.Right, delta);
        }
        
        // Fireball placement input
        if (Input.GetKeyDown(KeyCode.Space) && _currentFireBalls.Count < _character.BombCount)
        {
            PlaceFireball();
        }
    }
    
    private void PlaceFireball() {
        // Check if the tile is passable and if we can place the fireball
        if (mapManager.IsMoveValid(CrowGridPos))
        {
            Vector2 ballPos = mapManager.CenterOfPosition(CrowGridPos); //炸弹放在乌鸦所在单元格中心
            GameObject go = Instantiate(fireBallPrefab, new Vector3(ballPos.x, ballPos.y, -1), Quaternion.identity);
            Fireball fb = go.GetComponent<Fireball>();
            fb.Set(FireballExploded);
            Debug.Log("fireball placed at " + ballPos);
            _currentFireBalls.Add(fb);
            //currentFireballs++; // Increment the active fireball count
        }
    }
    
    // Call this when a fireball explodes and is removed
    public void FireballExploded(Fireball bomb)
    {
        _currentFireBalls.Remove(bomb);
        // Check if the tile is within bounds and if it can be destroyed
        int bombRange = _character.BombRange;
        Vector2Int[] dir = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down};
        
        List<Vector2Int> toBeGround = new List<Vector2Int> { bomb.GridPos };
        foreach (Vector2Int d in dir)
        {
            int r = 1;
            while (r <= bombRange)
            {
                Vector2Int grid = bomb.GridPos + d * r;
                bool beObstucled = false;
                if (mapManager.IsInBounds(grid))
                {
                    if (mapManager.IsDestructible(grid))
                    {
                        toBeGround.Add(grid);
                        
                    }
                    else if (mapManager.GetTileType(grid) == TileType.Rock)
                    {
                        beObstucled = true;
                    }
                    // todo Handle damage to the player or enemies DONT do this here!
                    // // Check if the crow is within the explosion range
                    // Vector2Int crowPosition = new Vector2Int(Mathf.RoundToInt(_character.transform.position.x), Mathf.RoundToInt(_character.transform.position.y));
                    // if (crowPosition == grid)
                    // {
                    //     EndGame();
                    // }
                }
                else beObstucled = true;

                if (beObstucled) break;
                r++;
            }
        }
        
        // Instantiate the explosion center prefab at the bomb's position
        CreateExplosion(explosionCenterPrefab, bomb.GridPos, Vector2Int.zero);
        
        // Instantiate the explosion line and end prefabs in each direction
        Vector2Int[] directions = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int direction in directions)
        {
            for (int i = 1; i <= _character.BombRange; i++)
            {
                Vector2Int gridPos = bomb.GridPos + direction * i;
                //Vector3 worldPos = new Vector3(gridPos.x, gridPos.y, -1);

                // Check if the grid position is within the map bounds
                if (!mapManager.IsInBounds(gridPos))
                {
                    // The grid position is outside the map bounds, skip this iteration
                    continue;
                }         
    
                // Check if the tile is a rock
                if (mapManager.GetTileType(gridPos) == TileType.Rock)
                {
                    // Stop the explosion from spreading further in this direction
                    break;
                }
                
                if (i == _character.BombRange)
                {
                    CreateExplosion(explosionEndPrefab, gridPos, direction);
                }
                else
                {
                    CreateExplosion(explosionLinePrefab, gridPos, direction);
                }
            }
        }
        
        // Check if the crow is within the explosion range
        
        foreach (Vector2Int g in toBeGround)
        {
            // Check if the tile is a snow pile before replacing it with ground
            if (mapManager.GetTileType(g) == TileType.SnowPile)
            {
                snowPileBlastedCount++;
                ScoreManager.Instance.AddSnowPileBlasted(1);
            }
            
            mapManager.ReplaceWithGround(g.x, g.y); // Destroy the tile (replace with ground)
        }
        //remove from list
        _currentFireBalls.Remove(bomb);
    }

    /// <summary>
    /// 创建爆炸
    /// </summary>
    /// <param name="model"></param>
    /// <param name="grid"></param>
    /// <param name="direction"></param>
    private void CreateExplosion(GameObject model, Vector2Int grid, Vector2Int direction)
    {
        Vector3 pos = mapManager.CenterOfPosition(grid);
        GameObject explosionLine = Instantiate(model, pos, Quaternion.identity);

        Explosion exp = explosionLine.GetComponent<Explosion>();
        exp.Set(grid, e =>
        {
            _toRemoveExplosions.Add(e);
        });
        // Rotate the explosion line based on its direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        explosionLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle ));
        
        _explosions.Add(exp);
    }

    /// <summary>
    /// 爆炸的效果
    /// </summary>
    /// <param name="explosion"></param>
    private void ExplosionEffect(Explosion explosion)
    {
        if (HitByExplosion(_character, explosion))
        {
            EndGame();
        }
        
        bool hasBomb = IsBombHere(explosion.CoverGrid, out Fireball bombHere);
        if (hasBomb)
        {
            FireballExploded(bombHere);
        }
        
        //如果有敌人，就杀死了
        List<Enemy> toRemove = new List<Enemy>();
        foreach (Enemy enemy in _enemies)
        {
            if (enemy.Dead) continue;
            if (HitByExplosion(enemy, explosion))
            {
                // Increment the enemy slaughtered count when an enemy is killed
                enemySlaughteredCount++;
                ScoreManager.Instance.AddEnemySlaughtered(1);
                
                //todo enemy 挂了
                enemy.Kill();
                toRemove.Add(enemy);
            }
        }
        foreach (Enemy enemy in toRemove) _enemies.Remove(enemy);
        
    }

    private bool HitByExplosion(Character cha, Explosion exp)
    {
        Vector2 ePos = mapManager.CenterOfPosition(exp.CoverGrid);
        Vector2 cPos = cha.transform.position;
        float hitRangeX = (cha == _character ? 0.4f : 0.5f) * MapManager.TileSize.x + cha.bodySize.x / 2.00f;
        if (Mathf.Abs(cPos.x - ePos.x) > hitRangeX) return false;
        float hitRangeY = (cha == _character ? 0.4f : 0.5f) * MapManager.TileSize.y + cha.bodySize.y / 2.00f;
        if (Mathf.Abs(cPos.y - ePos.y) > hitRangeY) return false;
        return true;
    }
    
    private void EndGame(bool win = false)
    {
        if (win)
        {
            Debug.Log("You win!");
            ScenesManager.Instance.LoadWinning();
        }
        else
        {
            _character.Kill();
            _state = BattleState.Over;
        }
    }
    
    /// <summary>
    /// 返回是否移动成功
    /// </summary>
    /// <param name="mover"></param>
    /// <param name="dir"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    private bool MoveCharacter(Character mover, MoveDirection dir, float delta)
    {
        if (!mover) return false;
        if (dir == MoveDirection.None) return true;

        const float squeezeRate = 0.1f;
        float checkOffsetX = squeezeRate * MapManager.TileSize.x * 0.5f;
        float checkOffsetY = squeezeRate * MapManager.TileSize.y * 0.5f;
        float bodyX = mover.bodySize.x * 0.5f;
        float bodyY = mover.bodySize.y * 0.5f;
        Vector3 dest = mover.TryMove(dir, delta);
        //根据方向获得具体的要检查的点，如果2个点都可过，则移动生效，这里的squeezeRate是一个挤过去的倍率，是为了手感
        Vector2[] checkPoints = new Vector2[] { Vector2.zero ,Vector2.zero};
        switch (dir)
        {
            case MoveDirection.Up:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.up * bodyY + Vector3.left * checkOffsetX,
                    dest + Vector3.up * bodyY + Vector3.right * checkOffsetX
                };
                dest = new Vector3(mapManager.CenterOfPosition(dest).x, dest.y, dest.z);
                break;
            case MoveDirection.Down:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.down * bodyY + Vector3.left * checkOffsetX,
                    dest + Vector3.down * bodyY + Vector3.right * checkOffsetX
                };
                dest = new Vector3(mapManager.CenterOfPosition(dest).x, dest.y, dest.z);
                break;
            case MoveDirection.Left:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.left * bodyX + Vector3.up * checkOffsetY,
                    dest + Vector3.left * bodyX + Vector3.down * checkOffsetY
                };
                dest = new Vector3(dest.x, mapManager.CenterOfPosition(dest).y, dest.z);
                break;
            case MoveDirection.Right:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.right * bodyX + Vector3.up * checkOffsetY,
                    dest + Vector3.right * bodyX + Vector3.down * checkOffsetY
                };
                dest = new Vector3(dest.x, mapManager.CenterOfPosition(dest).y, dest.z);
                break;
        }

        Vector2Int targetGrid = mapManager.PositionInGrid(dest);
        bool canMove = !ObstacleByFireball(mover.transform.position, targetGrid, dir); //向着炸弹不能走
        //没有向炸弹，那就看看地形让不让走
        if (canMove)
            foreach (Vector2 point in checkPoints)
            {
                if (!mapManager.IsMoveValid(point))
                {
                    canMove = false;
                    break;
                }
            }

        //能走才走，否则不鸟
        if (canMove)
        {
            mover.transform.position = dest;
            //只有玩家的角色能走进去
            if (mover == _character && mapManager.CheckWinCondition(dest))
            {
                //Debug.Log("Player picked up the food! Game won.");
                //ScenesManager.Instance.LoadWinning();
                EndGame(true);
            }
            
            // pick up item
            if (mover == _character)
            {
                mapManager.CheckPickUpItem(_character);
            }

            return true;
        }
        return false;
        
    }

    /// <summary>
    /// 以enterDirection向checkGrid行动，是否会被其中的炸弹阻挡而不能走
    /// </summary>
    /// <param name="guyPos">行动的角色的当前世界位置</param>
    /// <param name="checkGrid">炸弹的单元格</param>
    /// <param name="enterDirection">行动的角色想要行动的方向</param>
    /// <returns>true有炸弹不能走；false能走</returns>
    private bool ObstacleByFireball(Vector2 guyPos, Vector2Int checkGrid, MoveDirection enterDirection)
    {
        bool has = IsBombHere(checkGrid, out Fireball fireball);
        if (!has) return false;
        Vector2 bc = mapManager.CenterOfPosition(checkGrid);
        Vector2 dir = (bc - guyPos);
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return (dir.x > 0.5f && enterDirection == MoveDirection.Right) ||
                   (dir.x < -0.5f && enterDirection == MoveDirection.Left);
        }
        else
        {
            return (dir.y > 0.5f && enterDirection == MoveDirection.Up) ||
                   (dir.y < -0.5f && enterDirection == MoveDirection.Down);
        }
        // Check if an explosion is happening at the grid position
        if (IsExplosionHere(checkGrid))
        {
            return true;
        }
    }
    
    private bool IsExplosionHere(Vector2Int grid)
    {
        foreach (Explosion explosion in _explosions)
        {
            if (explosion.CoverGrid == grid)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// pos所在的单元格是否是个炸弹
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool IsBombHere(Vector2 pos, out Fireball fireball)
    {
        fireball = null;
        Vector2Int g = mapManager.PositionInGrid(pos);
        foreach (Fireball fb in _currentFireBalls)
        {
            if (g == fb.GridPos)
            {
                fireball = fb;
                return true;
            }
        }

        return false;
    }
}
