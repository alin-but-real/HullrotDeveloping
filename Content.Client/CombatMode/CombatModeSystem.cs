using Content.Client.Hands.Systems;
using Content.Client.NPC.HTN;
using Content.Shared.CCVar;
using Content.Shared.CombatMode;
using Content.Client.CombatMode;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Configuration;

namespace Content.Client.CombatMode;

public sealed class CombatModeSystem : SharedCombatModeSystem
{
    [Dependency] private readonly IOverlayManager _overlayManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IEyeManager _eye = default!;

    /// <summary>
    /// Raised whenever combat mode changes. 
    /// C# .NET 9 event, not SS14 event.
    /// use SharedCombatModeSystem.ToggleCombatActionEvent instead for anything else.
    /// </summary>
    public event Action<bool>? LocalPlayerCombatModeUpdated;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CombatModeComponent, AfterAutoHandleStateEvent>(OnHandleState);

        Subs.CVar(_cfg, CCVars.CombatModeIndicatorsPointShow, OnShowCombatIndicatorsChanged, true);
    }

    private void OnHandleState(EntityUid uid, CombatModeComponent component, ref AfterAutoHandleStateEvent args)
    {
        UpdateHud(uid);
    }

    public override void Shutdown()
    {
        _overlayManager.RemoveOverlay<CombatModeIndicatorsOverlay>();

        base.Shutdown();
    }

    public bool IsInCombatMode()
    {
        var entity = _playerManager.LocalEntity;

        if (entity == null)
            return false;

        return IsInCombatMode(entity.Value);
    }

    public override void SetInCombatMode(EntityUid entity, bool value, CombatModeComponent? component = null)
    {
        base.SetInCombatMode(entity, value, component);
        UpdateHud(entity);
    }

    protected override bool IsNpc(EntityUid uid)
    {
        return HasComp<HTNComponent>(uid);
    }

    private void UpdateHud(EntityUid entity)
    {
        if (entity != _playerManager.LocalEntity || !Timing.IsFirstTimePredicted)
        {
            return;
        }

        var inCombatMode = IsInCombatMode();
        LocalPlayerCombatModeUpdated?.Invoke(inCombatMode);
    }

    private void OnShowCombatIndicatorsChanged(bool isShow)
    {
        if (isShow)
        {
            _overlayManager.AddOverlay(new CombatModeIndicatorsOverlay(
                _inputManager,
                EntityManager,
                _eye,
                this,
                EntityManager.System<HandsSystem>()));
        }
        else
        {
            _overlayManager.RemoveOverlay<CombatModeIndicatorsOverlay>();
        }
    }
}
