using ClassicUO.Configuration;
using ClassicUO.Game.UI.Controls;
using Microsoft.Xna.Framework;
using System;

namespace ClassicUO.Game.UI.Gumps;

public class SimpleTimedTextGump : Gump
{
    private readonly DateTime expireAt;

    public SimpleTimedTextGump(World world, string text, Color color, TimeSpan duration) : base(world, 0, 0)
    {
        expireAt = DateTime.Now.Add(duration);
        TextBox t;
        Add(t = TextBox.GetOne(text, ProfileManager.CurrentProfile.OverheadChatFont, ProfileManager.CurrentProfile.OverheadChatFontSize, color, TextBox.RTLOptions.DefaultCentered()));
        Height = t.MeasuredSize.Y;
        Width = t.MeasuredSize.X;
        WantUpdateSize = true;
    }

    public SimpleTimedTextGump(World world, string text, uint hue, TimeSpan duration, int width) : base(world, 0, 0)
    {
        expireAt = DateTime.Now.Add(duration);
        TextBox t;
        Add(t = TextBox.GetOne(text, ProfileManager.CurrentProfile.OverheadChatFont, ProfileManager.CurrentProfile.OverheadChatFontSize, (int)hue, TextBox.RTLOptions.DefaultCentered(width)));
        Height = t.MeasuredSize.Y;
        Width = t.MeasuredSize.X;
        WantUpdateSize = true;
    }

    public override void PreDraw()
    {
        base.PreDraw();

        if (DateTime.Now >= expireAt)
            Dispose();
    }
}
