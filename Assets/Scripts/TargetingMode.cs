/// <summary>
/// BABOMOBA Slice 21: Targeting mode enum for player characters.
/// Determines which targeting filter is active.
/// <list type="bullet">
/// <item><see cref="Harvester"/> — targets minions (standard wave-clear mode).</item>
/// <item><see cref="Demolition"/> — targets structures (towers and cores).</item>
/// </list>
/// Serialize this on the player prefab and pass to the appropriate filter component.
/// </summary>
public enum TargetingMode
{
    /// <summary>Targets enemy minions. Used by HarvesterFilter.</summary>
    Harvester,

    /// <summary>Targets enemy structures (tower → core). Used by DemolitionMode.</summary>
    Demolition
}
