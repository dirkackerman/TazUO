using System;
using ClassicUO.Assets;
using ClassicUO.Game.Scenes;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Controls;

public class AnimationDisplay : Control
{
    private ushort _graphic;
    public ushort Graphic => _graphic;

    private readonly uint _playSpeedMs;

    private ulong _nextFrame = 0;

    private readonly int _mWidth;
    private readonly int _mHeight;

    private byte _animGroup;

    private ushort _lastFrame = 0;

    private Vector3 _hueVector;
    public bool DrawBorder { get; set; }

    public AnimationDisplay(ushort graphic, int width = 100, int height = 100, uint playSpeedMs = 650)
    {
        _mWidth = width;
        _mHeight = height;
        UpdateGraphic(graphic);
        _playSpeedMs = playSpeedMs;
        Width = width;
        Height = height;
    }

    public void UpdateGraphic(ushort graphic)
    {
        if (graphic >= Client.Game.UO.Animations.MaxAnimationCount)
            graphic = 0;

        _graphic = graphic;
        _animGroup = GetAnimGroup(graphic);

        Client.Game.UO.Animations.GetAnimationFrames(graphic, _animGroup, 1, out ushort hue2, out _, true);
        _hueVector = ShaderHueTranslator.GetHueVector(hue2, Client.Game.UO.FileManager.TileData.StaticData[_graphic].IsPartialHue, 1f);
    }

    private static byte GetAnimGroup(ushort graphic)
    {
        AnimationGroupsType groupType = Client.Game.UO.Animations.GetAnimType(graphic);

        switch (Client.Game.UO.FileManager.Animations.GetGroupIndex(graphic, groupType))
        {
            case AnimationGroups.Low: return (byte)LowAnimationGroup.Stand;

            case AnimationGroups.High: return (byte)HighAnimationGroup.Stand;

            case AnimationGroups.People: return (byte)PeopleAnimationGroup.Stand;
        }

        return 0;
    }

    public override void PreDraw()
    {
        base.PreDraw();

        if (_nextFrame <= Time.Ticks)
        {
            _nextFrame = Time.Ticks + _playSpeedMs;
            _lastFrame++;
        }
    }

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepth)
    {
        base.AddToRenderLists(renderLists, x, y, ref layerDepth);

        Span<SpriteInfo> frames = Client.Game.UO.Animations.GetAnimationFrames(_graphic, _animGroup, 1, out ushort hue2, out _, true);

        if (frames.Length == 0)
            return true;

        if (_lastFrame >= frames.Length)
            _lastFrame = 0;

        SpriteInfo spriteInfo = frames[_lastFrame];

        float depth = layerDepth;

        renderLists.AddGumpNoAtlas(batcher =>
        {
            if (spriteInfo.Texture != null)
                batcher.Draw(spriteInfo.Texture, new Rectangle(x, y, Math.Min(spriteInfo.UV.Width, _mWidth), Math.Min(spriteInfo.UV.Height, _mHeight)), spriteInfo.UV, _hueVector, depth);

            if (DrawBorder)
                batcher.DrawRectangle(SolidColorTextureCache.GetTexture(Color.Gray), x, y, Width - 1, Height - 1, ShaderHueTranslator.GetHueVector(0, false, Alpha), depth);

            return true;
        });

        return true;
    }
}
