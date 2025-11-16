using ClassicUO.Game.Scenes;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Renderer;

namespace ClassicUO.Game.UI.Gumps
{
    public class MultipleToolTipGump : Gump
    {
        private readonly CustomToolTip[] toolTips;
        private readonly Control hoverReference;

        public static bool SSIsEnabled = false;

        public static int SSX, SSY;
        public static int SSWidth, SSHeight;

        public MultipleToolTipGump(World world, int x, int y, CustomToolTip[] toolTips, Controls.Control hoverReference) : base(world, 0, 0)
        {
            this.toolTips = toolTips;
            this.hoverReference = hoverReference;
            BuildGump();
            WantUpdateSize = true;

            X = x;
            Y = y;

            SSIsEnabled = true;
        }

        private void BuildGump()
        {
            for (int i = 0; i < toolTips.Length; i++)
            {
                if (toolTips[i] == null)
                    continue;
                toolTips[i].OnOPLLoaded += () => { RepositionTooltips(); };
                Add(toolTips[i]);
            }
            RepositionTooltips();
        }

        private void RepositionTooltips()
        {
            int x = 0, totalWidth = 0, totalHeight = 0;
            for (int i = 0; i < toolTips.Length; i++)
            {
                if (toolTips[i] == null)
                    continue;
                toolTips[i].X = x;
                toolTips[i].Y = 0;
                toolTips[i].RemoveHoverReference();
                totalWidth += toolTips[i].Width;

                x += toolTips[i].Width + 14;

                if (totalHeight < toolTips[i].Height)
                    totalHeight = toolTips[i].Height;
            }
            ForceSizeUpdate();
            SSWidth = Width + 9;
            SSHeight = Height + 9;

            SetInScreen();

            SSX = ScreenCoordinateX - 4;
            SSY = ScreenCoordinateY - 2;
        }

        public override void PreDraw()
        {
            base.PreDraw();

            if (!hoverReference.MouseIsOver)
                Dispose();
        }

        public override void Dispose()
        {
            base.Dispose();
            SSIsEnabled = false;
        }
    }
}
