// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Renderer;
using ClassicUO.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ClassicUO.Game.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Game.UI.Controls;

public class GumpPicTiled : Control
{
    private ushort _graphic;
    private ushort hue;
    Vector3 hueVector;

    public GumpPicTiled(ushort graphic)
    {
        CanMove = true;
        AcceptMouseInput = true;
        Graphic = graphic;
    }

    public GumpPicTiled(int x, int y, int width, int heigth, ushort graphic) : this(graphic)
    {
        X = x;
        Y = y;

        if (width > 0) Width = width;

        if (heigth > 0) Height = heigth;
    }

    public GumpPicTiled(List<string> parts) : this(UInt16Converter.Parse(parts[5]))
    {
        X = int.Parse(parts[1]);
        Y = int.Parse(parts[2]);
        Width = int.Parse(parts[3]);
        Height = int.Parse(parts[4]);
        IsFromServer = true;
    }

    public ushort Graphic
    {
        get => _graphic;
        set
        {
            if (_graphic != value && value != 0xFFFF)
            {
                _graphic = value;

                ref readonly SpriteInfo gumpInfo = ref Client.Game.UO.Gumps.GetGump(_graphic);

                if (gumpInfo.Texture == null)
                {
                    Dispose();

                    return;
                }

                Width = gumpInfo.UV.Width;
                Height = gumpInfo.UV.Height;
            }
        }
    }

    public ushort Hue
    {
        get => hue; set
        {
            hue = value;
            hueVector = ShaderHueTranslator.GetHueVector(value, false, Alpha, true);
        }
    }

    public override void AlphaChanged(float oldValue, float newValue)
    {
        base.AlphaChanged(oldValue, newValue);
        hueVector = ShaderHueTranslator.GetHueVector(Hue, false, newValue, true);
    }

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepthRef)
    {
        float layerDepth = layerDepthRef;

        if (hueVector == default) hueVector = ShaderHueTranslator.GetHueVector(Hue, false, Alpha, true);

        ref readonly SpriteInfo gumpInfo = ref Client.Game.UO.Gumps.GetGump(Graphic);

        Texture2D texture = gumpInfo.Texture;
        if (texture != null)
        {
            Rectangle sourceRectangle = gumpInfo.UV;
            renderLists.AddGumpWithAtlas
            (
                (batcher) =>
                {
                    batcher.DrawTiled(
                        texture,
                        new Rectangle(x, y, Width, Height),
                        sourceRectangle,
                        hueVector,
                        layerDepth
                    );
                    return true;
                }
            );
        }

        return base.AddToRenderLists(renderLists, x, y, ref layerDepthRef);
    }

    public override bool Contains(int x, int y)
    {
        int width = Width;
        int height = Height;

        x -= Offset.X;
        y -= Offset.Y;

        ref readonly SpriteInfo gumpInfo = ref Client.Game.UO.Gumps.GetGump(Graphic);

        if (gumpInfo.Texture == null) return false;

        if (width == 0) width = gumpInfo.UV.Width;

        if (height == 0) height = gumpInfo.UV.Height;

        while (x > gumpInfo.UV.Width && width > gumpInfo.UV.Width)
        {
            x -= gumpInfo.UV.Width;
            width -= gumpInfo.UV.Width;
        }

        while (y > gumpInfo.UV.Height && height > gumpInfo.UV.Height)
        {
            y -= gumpInfo.UV.Height;
            height -= gumpInfo.UV.Height;
        }

        if (x > width || y > height) return false;

        return Client.Game.UO.Gumps.PixelCheck(Graphic, x, y);
    }
}
