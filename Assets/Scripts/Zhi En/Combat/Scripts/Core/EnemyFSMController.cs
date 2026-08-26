namespace Game.Combat
{
    /// <summary>
    /// fake fsm lol just loops thru EnemyDataSO.moveOrder list
    /// </summary>
    public static class EnemyFSMController
    {
        public static EnemyMove GetNextMove(CombatantRuntime enemy)
        {
            var data = enemy.enemySource;
            var moves = data.moveOrder;

            if (moves == null || moves.Length == 0)
                return default; // TODO: fall back to a basic attack if the SO has no moves configured

            int index = enemy.enemyMoveIndex;
            if (index >= moves.Length)
            {
                index = data.loopMoveOrder ? 0 : moves.Length - 1;
            }

            EnemyMove move = moves[index];

            enemy.enemyMoveIndex = System.Math.Min(enemy.enemyMoveIndex + 1, moves.Length);
            // just a move counter it doesn't affect the looping

            return move;
        }
    }
}
