public static class FactionExentions
{
    public static bool IsFriend(this ActorStats agentToCheck, Faction faction)
    {
        return FactionManager.GetRelations(agentToCheck.Faction, faction) > 0;
    }

    public static bool IsFriend(this ActorStats agentToCheck, ActorStats agent)
    {
        return FactionManager.GetRelations(agentToCheck.Faction, agent.Faction) > 0;
    }

    public static bool IsNeutral(this ActorStats agentToCheck, ActorStats agent)
    {
        return FactionManager.GetRelations(agentToCheck.Faction, agent.Faction) == 0;
    }

    public static bool IsEnemy(this ActorStats agentToCheck, ActorStats agent)
    {
        return FactionManager.GetRelations(agentToCheck.Faction, agent.Faction) < 0;
    }

    public static bool IsEnemy(ActorStats agent, Faction faction)
    {
        return FactionManager.GetRelations(faction, agent.Faction) < 0;
    }

    public static bool IsNeutral(ActorStats agent, Faction faction)
    {
        return FactionManager.GetRelations(faction, agent.Faction) == 0;
    }
}