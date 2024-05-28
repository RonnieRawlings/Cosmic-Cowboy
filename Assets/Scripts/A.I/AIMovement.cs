// Author - Ronnie Rawlings.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class AIMovement : MonoBehaviour
{
    // Ref to gridManager script.
    [SerializeField] private Pathfinding findPath;

    // Node the AI currently occupies.
    [SerializeField] private Node currentNode;

    // Patrol points, start/end.
    [SerializeField] private GameObject startPoint, endPoint;

    // Gunshot (MuzzleFlash) VFX.
    [SerializeField] private VisualEffect muzzelFlash;

    // Shot SFX.
    [SerializeField] private AudioClip shotSFX;

    // Ref to the AIs patrol pathing.
    private List<Node> patrolPath;

    // Current turns decison is made.
    private bool decisonMade = false;

    // Grid the AI is currently on.
    [SerializeField] private int currentGrid = 0;
    
    #region Variable Properties

    public Node CurrentNode
    {
        get { return currentNode; }
        set { currentNode = value; }
    }

    public int CurrentGrid
    {
        get { return currentGrid; }
    }

    #endregion

    /// <summary> method <c>SetStartNode</c> sets currentNode to first node. </summary>
    public void SetStartNode()
    {
        // Sets start node if no node is present.
        if (currentNode == null)
        {
            // Sets AIs starting node in AIMovement.
            GetComponent<AIMovement>().CurrentNode = BattleInfo.gridManager.
                GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid);
        }
    }

    /// <summary> method <c>AttackPlayer</c> tries to damage the player when in range. </summary>
    public IEnumerator AttackPlayer()
    {
        GetComponent<Animator>().SetBool("isFireing", true);

        // Weapon attached to the AI obj.
        WeaponValues attachtedWeapon = GetComponentInChildren<WeaponValues>();

        yield return RotateToFacePlayer(BattleInfo.player);

        // Chance of AIs attack landing.
        int attackChance = attachtedWeapon.baseAccuracy;
        if (BattleInfo.playerInCover) 
        { 
            // Decreases hit chance by player evasion.
            attackChance -= BattleValues.coverEvasionChance; 
        }

        // Calcs enemies actually acc relevant to player skills.
        int baseAccuracy = Mathf.RoundToInt((float)GetComponentInChildren<EnemyStats>().perception / 
            SkillsAndClasses.playerStats["Fitness"] * attackChance);           

        // Waits before attacking.
        yield return new WaitForSeconds(0.75f);

        // Plays muzzle flash VFX.
        if (muzzelFlash != null) { muzzelFlash.Play(); }        
        GetComponent<AudioSource>().PlayOneShot(shotSFX);

        // Only remove health if hit attack chance.
        if (BattleInfo.CalculateAttackChance(baseAccuracy))
        {
            // Player's player hit anim + VFX.
            BattleInfo.player.GetComponent<Animator>().SetBool("isHit", true);
            SetHitEffect(); 

            BattleInfo.player.GetComponent<AudioSource>().PlayOneShot(BattleInfo.player.GetComponent<PlayerMovement>().
                playerAnimationSFX[0]);

            // Checks if attack is a crit.
            int attackDamage = BattleInfo.CalculateDamage(attachtedWeapon.baseDamage, GetComponentInChildren<EnemyStats>().toughness, 
                SkillsAndClasses.playerStats["Toughness"]);

            int critChance = Mathf.RoundToInt((float)GetComponentInChildren<EnemyStats>().luck / 
                SkillsAndClasses.playerStats["Luck"] * attachtedWeapon.critChance);

            if (BattleInfo.CalculateAttackChance(critChance))
            {
                // Increases damage by multiplyer.
                attackDamage = BattleInfo.ApplyCritMultiplyer(attackDamage, attachtedWeapon.critMultiplyer);

                Debug.Log("AI Attack Crit!");
                StartCoroutine(CritEffect());
            }
            else
            {
                StartCoroutine(HitEffect());
            }

            // Applyies damage to player.
            BattleInfo.currentPlayerHealth -= attackDamage;
            Debug.Log("AI Attack Success!");

        }
        else
        {
            Debug.Log("AI Attack Missed!");
            StartCoroutine(MissEffect());
        }

        // Waits, ends turn.
        yield return new WaitForSeconds(1f);

        GetComponent<Animator>().SetBool("isFireing", false);

        BattleInfo.player.GetComponent<Animator>().SetBool("isHit", false);

        // Ends this AIs turn.       
        BattleInfo.levelEnemyTurns[gameObject.name] = false;
        BattleInfo.hasLockedEnemy = false;

        if (squadScript != null) { squadScript.TakenTurn = false; }

        // Makes sure node is still set to occupied.
        BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid).
            Occupied = this.gameObject;
    }

    /// <summary> method <c>SetHitEffect</c> moves the player's hit VFX to pos relevant to enemies hit. </summary>
    public void SetHitEffect()
    {
        // Determines hit direction/position, uses raycast.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (BattleInfo.player.transform.position - transform.position).normalized, out hit, 
            Mathf.Infinity, LayerMask.GetMask("Player")))
        {
            // Moves hitVFX to player hit point.
            if (hit.collider.gameObject == BattleInfo.player)
            {
                // Set new VFX position to hit point.
                BattleInfo.player.transform.Find("hit").transform.position = new Vector3(hit.point.x, BattleInfo.player.transform.Find("hit").
                    transform.position.y, hit.point.z);
            }
        }

        // Plays VFX.
        BattleInfo.player.transform.Find("hit").GetComponent<VisualEffect>().Play();
    }

    /// <summary> coroutine <c>RotateToFacePlayer</c> has the enemy rotate to face the player before attacking. </summary>
    public IEnumerator RotateToFacePlayer(GameObject player)
    {
        // Calculate direction & target rot.
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Vector3 targetDirection = new Vector3(direction.x, 0, direction.z);

        // Calculate total angle & time for rotation
        float totalAngle = Vector3.Angle(transform.forward, targetDirection);
        float totalTime = totalAngle / 5;

        // Time since move/rotation began.
        float elapsedTime = 0;

        // Rotate enemy to face the player.
        while (Vector3.Angle(transform.forward, targetDirection) > 3f)
        {
            elapsedTime += Time.deltaTime;
            float maxAngleChange = elapsedTime / totalTime * totalAngle;
            transform.forward = Vector3.RotateTowards(transform.forward, targetDirection, maxAngleChange * Mathf.Deg2Rad, 0.0f);
            yield return null;
        }
    }

    /// <summary> interface <c>MoveToNextNode</c> moves AI enemy to next node on AI turn. </summary>
    public IEnumerator MoveToNextNode(List<Node> nodePath)
    {
        GetComponent<Animator>().SetBool("isWalking", true);

        // Moves AI by enemyTileAmount tiles.
        int tilesMoved = 0;
        while (tilesMoved < BattleInfo.levelEnemyStats[this.gameObject].MovementRange)
        {
            // Prevents multiple enemies from being on the same node.
            if (nodePath[tilesMoved].Occupied) 
            { 
                break; 
            }
            else { nodePath[tilesMoved].Occupied = this.gameObject; }

            // Next node pos.
            Vector3 targetPosition = new Vector3(nodePath[tilesMoved].WorldPos.x, transform.position.y, nodePath[tilesMoved].WorldPos.z - 0.75f);

            // Calculate direction & target rot.
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Calculate total angle & time for rotation
            float totalAngle = Quaternion.Angle(transform.rotation, targetRotation);
            float totalTime = totalAngle / 20;

            // Time since move/rotation began.
            float elapsedTime = 0;

            // Resets occupied status of current node.
            currentNode.Occupied = null;

            // Moves AI towards next node.
            while (transform.position != targetPosition)
            {
                // Rotate AI to face the target position
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / totalTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

                // Move towards target pos.
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 2 * Time.deltaTime);
                yield return null;
            }

            // Sets occupied status to obj ref.
            nodePath[tilesMoved].Occupied = this.gameObject;
            currentNode = nodePath[tilesMoved];

            // When AI is within a 2 node radius, attack + end turn.
            if (tilesMoved + GetComponentInChildren<WeaponValues>().range >= nodePath.Count - 1)
            {
                GetComponent<Animator>().SetBool("isWalking", false);

                // Attacks player.
                StartCoroutine(AttackPlayer());

                // Exits the routine.
                yield break;
            }

            // Incremements moved.
            tilesMoved++;

            // Checks if enemy is in range of player.
            GetComponent<EnemyHoverInfo>().CheckForCompromised();
        }

        // Waits before ending turn.
        yield return new WaitForSeconds(0.75f);

        GetComponent<Animator>().SetBool("isWalking", false);

        // Ends this AIs turn.      
        BattleInfo.levelEnemyTurns[gameObject.name] = false;
        BattleInfo.hasLockedEnemy = false;

        if (squadScript != null) { squadScript.TakenTurn = false; }

        // Makes sure occupied node is still set.
        BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid).
            Occupied = this.gameObject;
    }

    // Current patrol node.
    int currentTargetNode = 0;

    /// <summary> interface <c>PatrolArea</c> when enemy isn't in combat with player, move around set area. </summary>
    public IEnumerator PatrolArea()
    {
        // Prevents patrol paths that aren't accessible from breaking AI movement.
        if (patrolPath.Count <= 0) 
        {
            GetComponent<Animator>().SetBool("isWalking", false);

            // Waits before ending turn.
            yield return new WaitForSeconds(0.5f);

            // Ends this AIs turn.            
            BattleInfo.levelEnemyTurns[gameObject.name] = false;
            BattleInfo.hasLockedEnemy = false;

            // Makes sure current node is still set to occupied.
            BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid).
                Occupied = this.gameObject;

            if (squadScript != null) { squadScript.TakenTurn = false; }

            yield break;
        }

        GetComponent<Animator>().SetBool("isWalking", true);

        // Tiles moved this turn.
        int tilesMoved = 0;

        // Finds the way back to startNode.
        List<Node> toStartPoint;
        if (transform.position != patrolPath[0].WorldPos && currentTargetNode == 0)
        {           
            // Finds path from pos to startNode.            
            toStartPoint = findPath.FindPath(transform.position, BattleInfo.gridManager.GetComponent<GridManager>().
                FindNodeFromWorldPoint(startPoint.transform.position, currentGrid).WorldPos, currentGrid);

            // Moves along each node.
            foreach (Node n in toStartPoint)
            {
                // Prevents broken walk cycles.
                if (n.Occupied || !n.Walkable) 
                { 
                    break; 
                }
                else { n.Occupied = this.gameObject; }

                // Next node pos.
                Vector3 targetPosition = new Vector3(n.WorldPos.x, transform.position.y, n.WorldPos.z - 0.75f);

                // Calculate direction & target rot.
                Vector3 direction = (targetPosition - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Calculate total angle & time for rotation
                float totalAngle = Quaternion.Angle(transform.rotation, targetRotation);
                float totalTime = totalAngle / 20;

                // Time since move/rotation began.
                float elapsedTime = 0;

                // Resets occupied status of current node.
                currentNode.Occupied = null;

                // Move AI towards next node.
                while (transform.position != targetPosition)
                {
                    // Rotate AI to face the target position
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / totalTime;
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

                    // Move towards target pos.
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, 2 * Time.deltaTime);
                    yield return null;
                }

                // Sets occupied status to obj ref.
                n.Occupied = this.gameObject;
                currentNode = n;

                tilesMoved++;

                // Checks if enemy is in range of player.
                GetComponent<EnemyHoverInfo>().CheckForCompromised();

                if (tilesMoved >= BattleInfo.levelEnemyStats[this.gameObject].MovementRange)
                {
                    break;
                }
            }
        }

        // Prevents over tile movement.
        if (tilesMoved < BattleInfo.levelEnemyStats[this.gameObject].MovementRange)
        {
            // Moves AI Enemy along the patrol path.
            while (currentTargetNode < patrolPath.Count)
            {
                List<Node> toNextPatrol = findPath.FindPath(transform.position, patrolPath[currentTargetNode].WorldPos, currentGrid);

                foreach (Node n in toNextPatrol)
                {
                    // Prevents broken walk cycles.
                    if (n.Occupied || !n.Walkable)
                    {
                        break;
                    }
                    else { n.Occupied = this.gameObject; }

                    // Next node pos.
                    Vector3 targetPosition = new Vector3(patrolPath[currentTargetNode].WorldPos.x, transform.position.y,
                        patrolPath[currentTargetNode].WorldPos.z - 0.75f);

                    // Calculate direction & target rot.
                    Vector3 direction = (targetPosition - transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    // Calculate total angle & time for rotation
                    float totalAngle = Quaternion.Angle(transform.rotation, targetRotation);
                    float totalTime = totalAngle / 20;

                    // Time since move/rotation began.
                    float elapsedTime = 0;

                    // Resets occupied status of current node.
                    currentNode.Occupied = null;

                    // Move AI towards next node.
                    while (transform.position != targetPosition)
                    {
                        // Rotate AI to face the target position
                        elapsedTime += Time.deltaTime;
                        float t = elapsedTime / totalTime;
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

                        // Move towards target pos.
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 2 * Time.deltaTime);
                        yield return null;
                    }

                    // Sets occupied status to obj ref.
                    n.Occupied = this.gameObject;
                    currentNode = n;

                    tilesMoved++;

                    // Checks if enemy is in range of player.
                    GetComponent<EnemyHoverInfo>().CheckForCompromised();

                    if (tilesMoved >= BattleInfo.levelEnemyStats[this.gameObject].MovementRange)
                    {
                        break;
                    }
                }
             
                // Increment to next node & increase moved tiles.
                currentTargetNode++;

                if (tilesMoved >= BattleInfo.levelEnemyStats[this.gameObject].MovementRange)
                {
                    break;
                }
            }
        }

        // If the end of the patrol path is reached, reset currentTargetNode to 0.
        if (currentTargetNode >= patrolPath.Count)
        {
            currentTargetNode = 0;
        }

        GetComponent<Animator>().SetBool("isWalking", false);

        // Waits before ending turn.
        yield return new WaitForSeconds(0.5f);

        // Ends this AIs turn.        
        BattleInfo.levelEnemyTurns[gameObject.name] = false;
        BattleInfo.hasLockedEnemy = false;

        if (squadScript != null) { squadScript.TakenTurn = false; }

        // Makes sure current node is still set to occupied.
        BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid).
            Occupied = this.gameObject;
    }

    /// <summary> method <c>DecideAction</c> uses nodePath (distance to target) to decide relevant action. </summary>
    public void DecideAction()
    {     
        // Exits if already run.
        if (decisonMade) { return; }

        // SquadMember taken their turn.
        if (squadScript != null)
        {
            if (!squadScript.TakenTurn)
            {
                squadScript.TakenTurn = true;

                foreach (SquadMember agent in SquadSystem.levelSquads[squadScript.SquadNumber])
                {
                    agent.TakenTurn = true;
                }
            }
        }

        // Checks if start node should be set.
        SetStartNode();

        // Finds best path to target.        
        List<Node> nodePath = findPath.FindPath(transform.position, BattleInfo.player.transform.position, currentGrid);

        // Decide which action to take.
        if (nodePath.Count <= 2 && BattleInfo.currentPlayerGrid == currentGrid && nodePath.Count != 0)
        {
            // Attacks player when close.
            StartCoroutine(AttackPlayer());
        }
        else if ((nodePath.Count > BattleInfo.levelEnemyStats[gameObject].DetectionRange || BattleInfo.currentPlayerGrid != currentGrid || 
            nodePath.Count == 0) && endPoint != null)
        {
            // Patrols area when far.
            StartCoroutine(PatrolArea());
        }
        else if (endPoint == null)
        {
            // Ends this AIs turn.
            BattleInfo.levelEnemyTurns[gameObject.name] = false;
            BattleInfo.hasLockedEnemy = false;

            if (squadScript != null) { squadScript.TakenTurn = false; }

            // Makes sure current node is still set to occupied.
            BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid).
                Occupied = this.gameObject;
        }
        else
        {          
            // Moves towards player.
            StartCoroutine(MoveToNextNode(nodePath));
        }
    }

    /// <summary> method <c>CheckIfTurnOver</c> checks weather all turns have been taken, if switches turn to player. </summary>
    public void CheckIfTurnOver()
    {
        // Loops over all AI turn values.
        foreach (KeyValuePair<string, bool> turn in BattleInfo.levelEnemyTurns)
        {
            // Exits if turn still ongoing.
            if (turn.Value)
            {
                return;
            }
        }
        
        // Switches to player turn.
        BattleInfo.playerTurn = true;
        BattleInfo.aiTurn = false;

        // Resets player's AP.
        BattleInfo.currentActionPoints = 2;

        // Resets decison making var.
        decisonMade = false;       
    }

    public IEnumerator SetFirstOccupied()
    {
        yield return new WaitForEndOfFrame();

        // Sets occupied status of start node.
        BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid).
            Occupied = this.gameObject;

        // Push AI unit to start node middle.
        Node startNode = BattleInfo.gridManager.GetComponent<GridManager>().FindNodeFromWorldPoint(transform.position, currentGrid);
        transform.position = new Vector3(startNode.WorldPos.x, transform.position.y, startNode.WorldPos.z - 0.75f);
    }

    /// <summary> coroutine <c>CritEffect</c> allows crit effect to play, disables canvas again once finished. </summary>
    public IEnumerator CritEffect()
    {
        yield return new WaitForSeconds(0.3f);

        // Enables, sets hitEnemy.
        BattleInfo.critCanvas.GetComponent<ShotEffects>().PlayerHit = true;
        BattleInfo.critCanvas.SetActive(true);

        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.8f);
        BattleInfo.critCanvas.SetActive(false);
        BattleInfo.critCanvas.GetComponent<ShotEffects>().PlayerHit = false;
    }

    /// <summary> coroutine <c>HitEffect</c> allows hit effect to play, disables canvas again once finished. </summary>
    public IEnumerator HitEffect()
    {
        yield return new WaitForSeconds(0.3f);

        // Enables, sets hitEnemy.
        BattleInfo.hitCanvas.GetComponent<ShotEffects>().PlayerHit = true;
        BattleInfo.hitCanvas.SetActive(true);

        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.6f);
        BattleInfo.hitCanvas.SetActive(false);
        BattleInfo.hitCanvas.GetComponent<ShotEffects>().PlayerHit = false;
    }

    /// <summary> coroutine <c>MissEffect</c> allows miss effect to play, disables canvas again once finished. </summary>
    public IEnumerator MissEffect()
    {
        yield return new WaitForSeconds(0.3f);

        // Enables, sets hitEnemy.
        BattleInfo.missCanvas.GetComponent<ShotEffects>().PlayerHit = true;
        BattleInfo.missCanvas.SetActive(true);

        // Waits for anim to complete, disables again.
        yield return new WaitForSeconds(0.6f);
        BattleInfo.missCanvas.SetActive(false);
        BattleInfo.missCanvas.GetComponent<ShotEffects>().PlayerHit = false;
    }

    // Script with all squad member info.
    private SquadMember squadScript;

    private void Start()
    {
        StartCoroutine(SetFirstOccupied());

        // Gets squad comp if part of one.
        if (GetComponent<SquadMember>() != null) { squadScript = GetComponent<SquadMember>(); }
    }

    // Called once every frame.
    void Update()
    {
        // Prevents path setting before visual create.
        if (BattleInfo.nodeObjects.Count == 0) { return; }

        // Creates patrolPath if enemy has points.
        if (patrolPath == null && endPoint != null)
        {            
            // Sets AIs patrol path.
            patrolPath = findPath.FindPath(startPoint.transform.position, BattleInfo.gridManager.GetComponent<GridManager>().
                FindNodeFromWorldPoint(endPoint.transform.position, currentGrid).WorldPos, currentGrid);
        }

        // If the enemy is far away, turn is ended automatially.
        if (Vector3.Distance(transform.position, BattleInfo.player.transform.position) > 35 && BattleInfo.levelEnemyTurns[gameObject.name])
        {
            Debug.Log("Too far");
            
            BattleInfo.levelEnemyTurns[gameObject.name] = false;
            BattleInfo.hasLockedEnemy = false;

            if (squadScript != null) { squadScript.TakenTurn = false; }

            CheckIfTurnOver();
            return;
        }

        if (BattleInfo.currentPlayerGrid != currentGrid && BattleInfo.levelEnemyTurns[gameObject.name])
        {
            Debug.Log("Not on same grid");

            BattleInfo.levelEnemyTurns[gameObject.name] = false;
            BattleInfo.hasLockedEnemy = false;

            if (squadScript != null) { squadScript.TakenTurn = false; }

            CheckIfTurnOver();
            return;
        }

        // Finds enemy index.
        int currentIndex = BattleInfo.levelEnemiesList.IndexOf(gameObject.name);

        // Have previous enemies finished their turns.
        bool allPreviousFinished = true;      
        for (int i = 0; i < currentIndex; i++)
        {            
            if (BattleInfo.levelEnemyTurns[BattleInfo.levelEnemiesList[i]])
            {
                allPreviousFinished = false;
                break;
            }                      
        }

        if (squadScript != null)
        {
            if (squadScript.TakenTurn) { allPreviousFinished = true; }
        }

        // Begins AI turn if aiTurn & all enemies before finished.
        if (allPreviousFinished)
        {            
            if (BattleInfo.levelEnemyTurns[gameObject.name] && !BattleInfo.playerTurn && BattleInfo.hasLockedEnemy)
            {
                // Decides which action to take.
                DecideAction();
                decisonMade = true;
            }
            else if (!BattleInfo.levelEnemyTurns[gameObject.name] && !BattleInfo.playerTurn && BattleInfo.aiTurn)
            {
                // Check for turn over.
                CheckIfTurnOver();
            }
            else if (BattleInfo.playerTurn && !BattleInfo.aiTurn)
            {
                // Prevents AI becoming stuck.
                decisonMade = false;
            }
        }
    }
}