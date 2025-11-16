// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Renderer;
using ClassicUO.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ClassicUO.Game.Scenes;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Game.UI.Controls;

public class StaticPic : Control
{
    private ushort graphic;
    private Vector3 hueVector;
    private ushort hue;
    private bool isPartialHue;

    public StaticPic(ushort graphic, ushort hue)
    {
        Hue = hue;
        Graphic = graphic;
        CanMove = true;
        WantUpdateSize = false;
    }

    public StaticPic(List<string> parts)
        : this(
            UInt16Converter.Parse(parts[3]),
            parts.Count > 4 ? UInt16Converter.Parse(parts[4]) : (ushort)0
        )
    {
        X = int.Parse(parts[1]);
        Y = int.Parse(parts[2]);
        IsFromServer = true;
    }

    public ushort Hue
    {
        get => hue; set
        {
            hue = value;
            hueVector = ShaderHueTranslator.GetHueVector(value, IsPartialHue, 1);
        }
    }
    public bool IsPartialHue
    {
        get => isPartialHue; set
        {
            isPartialHue = value;
            hueVector = ShaderHueTranslator.GetHueVector(Hue, value, 1);
        }
    }

    public ushort Graphic
    {
        get => graphic;
        set
        {
            graphic = value;

            ref readonly SpriteInfo artInfo = ref Client.Game.UO.Arts.GetArt(value);

            if (artInfo.Texture == null)
            {
                Dispose();

                return;
            }

            Width = artInfo.UV.Width;
            Height = artInfo.UV.Height;

            IsPartialHue = Client.Game.UO.FileManager.TileData.StaticData[value].IsPartialHue;
        }
    }

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepthRef)
    {
        float layerDepth = layerDepthRef;

        if (hueVector == default) hueVector = ShaderHueTranslator.GetHueVector(Hue, IsPartialHue, 1);

        ref readonly SpriteInfo artInfo = ref Client.Game.UO.Arts.GetArt(Graphic);

        Texture2D texture = artInfo.Texture;
        if (texture != null)
        {
            Rectangle sourceRectangle = artInfo.UV;
            renderLists.AddGumpWithAtlas
            (
                (batcher) =>
                {
                    batcher.Draw(
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

    public override bool Contains(int x, int y) => Client.Game.UO.Arts.PixelCheck(Graphic, x - Offset.X, y - Offset.Y);
}
