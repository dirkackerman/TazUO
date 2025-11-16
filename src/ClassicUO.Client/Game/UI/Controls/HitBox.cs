// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Game.Scenes;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Game.UI.Controls;

public class HitBox : Control
{
    public HitBox
    (
        int x,
        int y,
        int w,
        int h,
        string tooltip = null,
        float alpha = 0.25f
    )
    {
        CanMove = false;
        AcceptMouseInput = true;
        Alpha = alpha;
        Texture = SolidColorTextureCache.GetTexture(Color.White);

        X = x;
        Y = y;
        Width = w;
        Height = h;
        WantUpdateSize = false;

        SetTooltip(tooltip);
    }

    public ushort Hue { get; set; }
    public override ClickPriority Priority { get; set; } = ClickPriority.High;
    protected readonly Texture2D Texture;

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepthRef)
    {
        if (IsDisposed) return false;

        float layerDepth = layerDepthRef;

        if (!MouseIsOver) return base.AddToRenderLists(renderLists, x, y, ref layerDepthRef);

        Vector3 hueVector = ShaderHueTranslator.GetHueVector
        (
            0,
            false,
            Alpha,
            true
        );

        renderLists.AddGumpNoAtlas(
            batcher =>
            {
                batcher.Draw
                (
                    Texture,
                    new Vector2(x, y),
                    new Rectangle(0, 0, Width, Height),
                    hueVector,
                    layerDepth
                );
                return true;
            }
        );

        return base.AddToRenderLists(renderLists, x, y, ref layerDepthRef);
    }
}
