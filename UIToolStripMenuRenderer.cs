using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeepMeBusy
{
	internal class UIToolStripMenuRenderer : ToolStripProfessionalRenderer
	{
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			if (e.Item.Enabled)
				base.OnRenderMenuItemBackground(e);
		}

		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
		{
			if (e.Item.Enabled)
				base.OnRenderMenuItemBackground(e);
		}
	}
}
