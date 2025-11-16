// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Renderer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ClassicUO.Game.Scenes;

namespace ClassicUO.Game.UI.Controls;

public class CheckerTrans : Control
{
    private Vector3 hueVector = ShaderHueTranslator.GetHueVector(0, false, 0.5f);

    public CheckerTrans(List<string> parts)
    {
        X = int.Parse(parts[1]);
        Y = int.Parse(parts[2]);
        Width = int.Parse(parts[3]);
        Height = int.Parse(parts[4]);
        AcceptMouseInput = false;
        IsFromServer = true;
    }

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepthRef)
    {
        float layerDepth = layerDepthRef;
        renderLists.AddGumpNoAtlas
        (
            (batcher) =>
            {
                batcher.Draw
                (
                    SolidColorTextureCache.GetTexture(Color.Black),
                    new Rectangle
                    (
                        x,
                        y,
                        Width,
                        Height
                    ),
                    hueVector,
                    layerDepth
                );

                return true;
            }
        );

        return true;
    }
}