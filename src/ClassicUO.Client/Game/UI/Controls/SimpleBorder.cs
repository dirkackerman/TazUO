using ClassicUO.Game.Scenes;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Controls;

public class SimpleBorder : Control
{
    public ushort Hue
    {
        get => hue;
        set
        {
            hue = value;
            hueVector = ShaderHueTranslator.GetHueVector(value, false, Alpha);
        }
    }

    private int _width = 0, _height = 0;
    private Vector3 hueVector;
    private ushort hue = 0;

    //Return 0 so this control has a 0, 0 size to not interfere with hitboxes
    public new int Width { get { return 0; } set { _width = value; } }
    public new int Height { get { return 0; } set { _height = value; } }

    public override void AlphaChanged(float oldValue, float newValue)
    {
        base.AlphaChanged(oldValue, newValue);
        hueVector = ShaderHueTranslator.GetHueVector(Hue, false, newValue);
    }

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepth)
    {
        if (IsDisposed)
        {
            return false;
        }

        base.AddToRenderLists(renderLists, x, y, ref layerDepth);

        if (hueVector == default)
        {
            hueVector = ShaderHueTranslator.GetHueVector(Hue, false, Alpha);
        }

        float depth = layerDepth;

        renderLists.AddGumpNoAtlas(batcher =>
        {
            batcher.DrawRectangle(
                SolidColorTextureCache.GetTexture(Color.White),
                x, y,
                _width, _height,
                hueVector,
                depth
            );
            return true;
        });

        return true;
    }
}
