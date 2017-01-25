using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MouseSwitchr
{
	internal class Switcher : ApplicationContext
	{
		[DllImport("user32.dll")]
		private static extern int SwapMouseButton(int bSwap);

		private readonly NotifyIcon icon;

		public Switcher()
		{
			icon = new NotifyIcon
			{
				Icon = ChooseIcon(),
				Visible = true
			};
			icon.MouseClick += (o, e) =>
			{
				if (e.Button != MouseButtons.Left)
				{
					return;
				}
				Toggle();
			};
			var toggle = new MenuItem("&Toggle", (o, e) => Toggle());
			var makePermanent = new MenuItem("Make &permanent", (o, e) => WriteRegistry());
			var visitWeb = new MenuItem("Visit website", (o, e) => VisitWebsite());
			var exit = new MenuItem("E&xit", (o, e) => Exit(icon));
			icon.ContextMenu = new ContextMenu(new[] {toggle, makePermanent, visitWeb, exit});
		}

		private static Icon ChooseIcon()
		{
			return SystemInformation.MouseButtonsSwapped ? Resources.mouse_right : Resources.mouse_left;
		}

		private static void WriteRegistry()
		{
			var key = Registry.CurrentUser.CreateSubKey(@"Control Panel\Mouse");
			key?.SetValue("SwapMouseButtons", SystemInformation.MouseButtonsSwapped ? "1" : "0");
		}

		private static void VisitWebsite()
		{
			Process.Start("https://github.com/jmrein/MouseSwitchr");
		}

		private void Toggle()
		{
			SwapMouseButton(SystemInformation.MouseButtonsSwapped ? 0 : 1);
			icon.Icon = ChooseIcon();
		}

		private static void Exit(NotifyIcon icon)
		{
			icon.Visible = false;
			Application.Exit();
		}

		[STAThread]
		private static void Main()
		{
			Application.Run(new Switcher());
		}
	}
}