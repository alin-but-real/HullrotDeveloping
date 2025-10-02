using Content.Server._Crescent.Hadal;
using Content.Shared._Crescent.SpaceBiomes;

namespace Content.Shared.Sound.Systems;

public sealed class HullrotSystem : EntitySystem
{
    private ISawmill _sawmill = default!;

    [Dependency] private readonly IEntityManager _entityManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = IoCManager.Resolve<ILogManager>().GetSawmill("hullrotSystem.cs");
        SubscribeLocalEvent<SpaceBiomeTrackerComponent, SpaceBiomeSwapMessage>(OnBiomeSwap);
    }


    //THIS DOESNT WORK - INSTEAD APPLY THE COMPONENT/REMOVAL OF COMPONENT IN SPACEBIOMESYSTEM IN SERVER
    // then, on update, use the gunsystem's shoot function (and a dummy hadal gun i guess) to shoot at the players position
    // from 100 tiles away
    // probelms with countsman-wide ships but it should be fine for now
    // bullets can make cool noises
    private void OnBiomeSwap(EntityUid uid, SpaceBiomeTrackerComponent comp, SpaceBiomeSwapMessage args)
    {
        if (args.Biome == "Hadal Deadspace")
        {
            EnsureComp<HullrotComponent>(uid); //add hullrot component to player
            _sawmill.Debug("ENTERED HADAL");
        }
        else
        {
            _sawmill.Debug("ENTERED NOT HADAL");
            if (TryComp<HullrotComponent>(uid, out var dontNeedThis)) //if the player has the hullrot component and they moved to another biome (not hadal)
                _entityManager.RemoveComponent<HullrotComponent>(uid); //then remove it.
        }

    }

}