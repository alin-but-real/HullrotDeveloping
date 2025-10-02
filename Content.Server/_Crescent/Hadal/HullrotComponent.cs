namespace Content.Server._Crescent.Hadal;

/// <summary>
/// Added and removed from players, when a player moves in/out of hadal deadspace. Used as a marker to fire invisible projectiles centered on the player.
/// </summary>

[RegisterComponent]
public sealed partial class HullrotComponent : Component
{
    /// <summary>
    /// Increases linearly the more time you spend in Hadal. Set to 0 every time you enter Hadal.
    /// </summary>
    [DataField]
    public float Intensity = 0f;
}
