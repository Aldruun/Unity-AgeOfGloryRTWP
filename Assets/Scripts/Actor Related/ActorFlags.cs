[System.Flags]
public enum ActorFlags
{
    None = 0,
    ESSENTIAL = 1,
    PC = 1 << 1,
    FAMILIAR = 1 << 2,
    SUMMONED = 1 << 3,
    ALLY = 1 << 4,
    NEUTRAL = 1 << 5,
    PASSIVE = 1 << 6,
    HOSTILE = 1 << 7,
    BEAST = 1 << 8
}
