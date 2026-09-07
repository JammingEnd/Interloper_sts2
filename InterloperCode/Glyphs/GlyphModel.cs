using BaseLib.Abstracts;
using Interloper.InterloperCode.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Interloper.InterloperCode.Glyphs;

public abstract class GlyphModel : AbstractModel, ICustomModel
{
    public abstract GlyphType Type { get; }

    public virtual decimal Value { get; set; } = 1;

    public bool HasBeenRemovedFromState { get; private set; }

    private Player? _owner;

    public Player Owner
    {
        get
        {
            AssertMutable();
            return _owner ?? throw new Exception($"Glyph {Id.Entry} does not have an owner.");
        }
        set
        {
            AssertMutable();
            if (_owner != null && value != null && value != _owner)
                throw new InvalidOperationException($"Glyph {Id.Entry} already has an owner.");
            _owner = value;
        }
    }

    private GlyphModel? _canonicalInstance;

    public GlyphModel? CanonicalInstance
    {
        get => !IsMutable ? this : _canonicalInstance;
        set
        {
            AssertMutable();
            _canonicalInstance = value;
        }
    }

    public override bool ShouldReceiveCombatHooks => true;

    public GlyphModel ToMutable()
    {
        AssertCanonical();
        var model = (GlyphModel)MutableClone();
        model.CanonicalInstance = this;
        return model;
    }

    public GlyphModel CreateClone()
    {
        AssertMutable();
        return (GlyphModel)ClonePreservingMutability();
    }

    public void RemoveInternal()
    {
        HasBeenRemovedFromState = true;
    }
}