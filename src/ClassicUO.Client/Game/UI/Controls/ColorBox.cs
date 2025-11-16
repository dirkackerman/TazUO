// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Game.Scenes;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Controls;

public class ColorBox : Control
{
    protected Vector3 HueVector;

    public ColorBox(int width, int height, ushort hue)
    {
        CanMove = false;
        Width = width;
        Height = height;
        Hue = hue;
        WantUpdateSize = false;
    }

    public ushort Hue
    {
        get;
        set
        {
            field = value;
            HueVector = ShaderHueTranslator.GetHueVector(value, false, Alpha);
        }
    }

    public override void AlphaChanged(float oldValue, float newValue)
    {
        base.AlphaChanged(oldValue, newValue);
        HueVector = ShaderHueTranslator.GetHueVector(Hue, false, Alpha);
    }

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepthRef)
    {
        float layerDepth = layerDepthRef;

        renderLists.AddGumpNoAtlas
        ((batcher) =>
            {
                batcher.Draw
                (
                    SolidColorTextureCache.GetTexture(Color.White),
                    new Rectangle
                    (
                        x,
                        y,
                        Width,
                        Height
                    ),
                    HueVector,
                    layerDepth
                );

                return true;
            }
        );

        return true;
    }
}
