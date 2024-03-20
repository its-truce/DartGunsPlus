using System.Collections.Generic;
using Terraria.UI;

namespace DartGunsPlus.Content.UI;

// thanks for verveine for all this
public abstract class ModUIState : UIState
{
    protected ModUIState()
    {
        Name = GetType().Name;
    }

    public string Name { get; }
    public virtual InterfaceScaleType ScaleType { get; set; } = InterfaceScaleType.UI;
    public virtual bool Visible { get; set; } = true;
    public virtual float Priority { get; set; } = 0f;
    public abstract int InsertionIndex(List<GameInterfaceLayer> layers);

    public virtual void Unload()
    {
    }

    public virtual void OnResolutionChanged(int width, int height)
    {
    }

    public virtual void OnUIScaleChanged()
    {
    }
}