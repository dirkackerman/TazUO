// SPDX-License-Identifier: BSD-2-Clause

using System;
using ClassicUO.Game.Scenes;
using ClassicUO.Input;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Controls;

public class ScrollBar : ScrollBarBase
{
    private readonly Rectangle _rectSlider;

    private Rectangle _emptySpace;

    private const ushort BUTTON_UP_0 = 251;
    private const ushort BUTTON_UP_1 = 250;
    private const ushort BUTTON_DOWN_0 = 253;
    private const ushort BUTTON_DOWN_1 = 252;
    private const ushort BACKGROUND_0 = 257;
    private const ushort BACKGROUND_1 = 256;
    private const ushort BACKGROUND_2 = 255;
    private const ushort SLIDER = 254;

    private Vector3 _hueVector = ShaderHueTranslator.GetHueVector(0);

    public ScrollBar(int x, int y, int height)
    {
        Height = height;
        Location = new Point(x, y);
        AcceptMouseInput = true;

        ref readonly SpriteInfo gumpInfoUp = ref Client.Game.UO.Gumps.GetGump(BUTTON_UP_0);
        ref readonly SpriteInfo gumpInfoDown = ref Client.Game.UO.Gumps.GetGump(BUTTON_DOWN_0);
        ref readonly SpriteInfo gumpInfoBackground = ref Client.Game.UO.Gumps.GetGump(BACKGROUND_0);
        ref readonly SpriteInfo gumpInfoSlider = ref Client.Game.UO.Gumps.GetGump(SLIDER);

        Width = gumpInfoBackground.UV.Width;

        _rectDownButton = new Rectangle(
            0,
            Height - gumpInfoDown.UV.Height,
            gumpInfoDown.UV.Width,
            gumpInfoDown.UV.Height
        );
        _rectUpButton = new Rectangle(0, 0, gumpInfoUp.UV.Width, gumpInfoUp.UV.Height);
        _rectSlider = new Rectangle(
            (gumpInfoBackground.UV.Width - gumpInfoSlider.UV.Width) >> 1,
            gumpInfoUp.UV.Height + _sliderPosition,
            gumpInfoSlider.UV.Width,
            gumpInfoSlider.UV.Height
        );
        _emptySpace.X = 0;
        _emptySpace.Y = gumpInfoUp.UV.Height;
        _emptySpace.Width = gumpInfoSlider.UV.Width;
        _emptySpace.Height = Height - (gumpInfoDown.UV.Height + gumpInfoUp.UV.Height);
    }

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepthRef)
    {
        if (Height <= 0 || !IsVisible || IsDisposed) return false;

        float layerDepth = layerDepthRef;

        renderLists.AddGumpWithAtlas(batcher =>
        {
            ref readonly SpriteInfo gumpInfoUp0 = ref Client.Game.UO.Gumps.GetGump(BUTTON_UP_0);
            ref readonly SpriteInfo gumpInfoUp1 = ref Client.Game.UO.Gumps.GetGump(BUTTON_UP_1);
            ref readonly SpriteInfo gumpInfoDown0 = ref Client.Game.UO.Gumps.GetGump(BUTTON_DOWN_0);
            ref readonly SpriteInfo gumpInfoDown1 = ref Client.Game.UO.Gumps.GetGump(BUTTON_DOWN_1);
            ref readonly SpriteInfo gumpInfoBackground0 = ref Client.Game.UO.Gumps.GetGump(BACKGROUND_0);
            ref readonly SpriteInfo gumpInfoBackground1 = ref Client.Game.UO.Gumps.GetGump(BACKGROUND_1);
            ref readonly SpriteInfo gumpInfoBackground2 = ref Client.Game.UO.Gumps.GetGump(BACKGROUND_2);
            ref readonly SpriteInfo gumpInfoSlider = ref Client.Game.UO.Gumps.GetGump(SLIDER);

            // draw scrollbar background
            int middleHeight =
                Height
                - gumpInfoUp0.UV.Height
                - gumpInfoDown0.UV.Height
                - gumpInfoBackground0.UV.Height
                - gumpInfoBackground2.UV.Height;

            if (middleHeight > 0)
            {
                batcher.Draw(
                    gumpInfoBackground0.Texture,
                    new Vector2(x, y + gumpInfoUp0.UV.Height),
                    gumpInfoBackground0.UV,
                    _hueVector,
                    layerDepth
                );

                batcher.DrawTiled(
                    gumpInfoBackground1.Texture,
                    new Rectangle(
                        x,
                        y + gumpInfoUp1.UV.Height + gumpInfoBackground0.UV.Height,
                        gumpInfoBackground0.UV.Width,
                        middleHeight
                    ),
                    gumpInfoBackground1.UV,
                    _hueVector,
                    layerDepth
                );

                batcher.Draw(
                    gumpInfoBackground2.Texture,
                    new Vector2(
                        x,
                        y + Height - gumpInfoDown0.UV.Height - gumpInfoBackground2.UV.Height
                    ),
                    gumpInfoBackground2.UV,
                    _hueVector,
                    layerDepth
                );
            }
            else
            {
                middleHeight = Height - gumpInfoUp0.UV.Height - gumpInfoDown0.UV.Height;

                batcher.DrawTiled(
                    gumpInfoBackground1.Texture,
                    new Rectangle(
                        x,
                        y + gumpInfoUp0.UV.Height,
                        gumpInfoBackground0.UV.Width,
                        middleHeight
                    ),
                    gumpInfoBackground1.UV,
                    _hueVector,
                    layerDepth
                );
            }

            // draw up button
            if (_btUpClicked)
                batcher.Draw(gumpInfoUp1.Texture, new Vector2(x, y), gumpInfoUp1.UV, _hueVector, layerDepth);
            else
                batcher.Draw(gumpInfoUp0.Texture, new Vector2(x, y), gumpInfoUp0.UV, _hueVector, layerDepth);

            // draw down button
            if (_btDownClicked)
                batcher.Draw(
                    gumpInfoDown1.Texture,
                    new Vector2(x, y + Height - gumpInfoDown0.UV.Height),
                    gumpInfoDown1.UV,
                    _hueVector,
                    layerDepth
                );
            else
                batcher.Draw(
                    gumpInfoDown0.Texture,
                    new Vector2(x, y + Height - gumpInfoDown0.UV.Height),
                    gumpInfoDown0.UV,
                    _hueVector,
                    layerDepth
                );

            // draw slider
            if (MaxValue > MinValue && middleHeight > 0)
                batcher.Draw(
                    gumpInfoSlider.Texture,
                    new Vector2(
                        x + ((gumpInfoBackground0.UV.Width - gumpInfoSlider.UV.Width) >> 1),
                        y + gumpInfoUp0.UV.Height + _sliderPosition
                    ),
                    gumpInfoSlider.UV,
                    _hueVector,
                    layerDepth
                );
            return true;
        });

        return base.AddToRenderLists(renderLists, x, y, ref layerDepthRef);
    }

    protected override int GetScrollableArea()
    {
        ref readonly SpriteInfo gumpInfoUp = ref Client.Game.UO.Gumps.GetGump(BUTTON_UP_0);
        ref readonly SpriteInfo gumpInfoDown = ref Client.Game.UO.Gumps.GetGump(BUTTON_DOWN_0);
        ref readonly SpriteInfo gumpInfoSlider = ref Client.Game.UO.Gumps.GetGump(SLIDER);

        return Height
               - gumpInfoUp.UV.Height
               - gumpInfoDown.UV.Height
               - gumpInfoSlider.UV.Height;
    }

    protected override void OnMouseDown(int x, int y, MouseButtonType button)
    {
        base.OnMouseDown(x, y, button);

        if (_btnSliderClicked && _emptySpace.Contains(x, y)) CalculateByPosition(x, y);
    }

    protected override void CalculateByPosition(int x, int y)
    {
        if (y != _clickPosition.Y)
        {
            y -= _emptySpace.Y + (_rectSlider.Height >> 1);

            if (y < 0) y = 0;

            int scrollableArea = GetScrollableArea();

            if (y > scrollableArea) y = scrollableArea;

            _sliderPosition = y;
            _clickPosition.X = x;
            _clickPosition.Y = y;

            ref readonly SpriteInfo gumpInfoUp = ref Client.Game.UO.Gumps.GetGump(BUTTON_UP_0);
            ref readonly SpriteInfo gumpInfoDown = ref Client.Game.UO.Gumps.GetGump(BUTTON_DOWN_0);
            ref readonly SpriteInfo gumpInfoSlider = ref Client.Game.UO.Gumps.GetGump(SLIDER);

            if (
                y == 0
                && _clickPosition.Y < gumpInfoUp.UV.Height + (gumpInfoSlider.UV.Height >> 1)
            )
                _clickPosition.Y = gumpInfoUp.UV.Height + (gumpInfoSlider.UV.Height >> 1);
            else if (
                y == scrollableArea
                && _clickPosition.Y
                > Height - gumpInfoDown.UV.Height - (gumpInfoSlider.UV.Height >> 1)
            )
                _clickPosition.Y =
                    Height - gumpInfoDown.UV.Height - (gumpInfoSlider.UV.Height >> 1);

            _value = (int)
                Math.Round(y / (float)scrollableArea * (MaxValue - MinValue) + MinValue);
        }
    }

    public override bool Contains(int x, int y) => x >= 0 && x <= Width && y >= 0 && y <= Height;
}
