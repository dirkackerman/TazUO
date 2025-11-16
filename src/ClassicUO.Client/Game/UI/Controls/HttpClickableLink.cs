using ClassicUO.Assets;
using ClassicUO.Game.Scenes;
using ClassicUO.Input;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Controls;

public class HttpClickableLink : Control
{
    private string url;

    public ushort HighlightHue { get; set; } = 30;
    public HttpClickableLink(string title, string url, Color color, int fontsize = 18) : base()
    {
        this.url = url;
        AcceptMouseInput = true;
        CanMove = true;
        var tb = TextBox.GetOne("/tu" + title, TrueTypeLoader.EMBEDDED_FONT, fontsize, color, TextBox.RTLOptions.Default());
        Add(tb);
        SetTooltip(url);
        ForceSizeUpdate();
    }

    protected override void OnMouseUp(int x, int y, MouseButtonType button)
    {
        base.OnMouseUp(x, y, button);
        if(button == MouseButtonType.Left)
        {
            Utility.Platforms.PlatformHelper.LaunchBrowser(url);
        }
    }

    public override bool AddToRenderLists(RenderLists renderLists, int x, int y, ref float layerDepth)
    {
        if (MouseIsOver)
        {
            float depth = layerDepth;

            renderLists.AddGumpNoAtlas(batcher =>
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
                    ShaderHueTranslator.GetHueVector(HighlightHue, false, 0.3f),
                    depth
                );
                return true;
            });
        }

        return base.AddToRenderLists(renderLists, x, y, ref layerDepth);
    }
}
