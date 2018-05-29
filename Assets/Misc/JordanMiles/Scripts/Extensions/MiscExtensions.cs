namespace JMiles42 {
    public static class MiscExtensions
    {
        public static bool IsAlive(this Health health) { return health.Amount > 0; }
        public static bool IsDead(this Health health) { return health.Amount <= 0; }

        public static bool CanAffordAttack(this Energy energy, float attackAmount) {
            return (energy.Amount - attackAmount) >= 0;
        }
    }
}