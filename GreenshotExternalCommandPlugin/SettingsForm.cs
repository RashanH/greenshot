﻿/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2012  Thomas Braun, Jens Klingen, Robin Krom
 * 
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Greenshot.IniFile;
using GreenshotPlugin.Core;

namespace ExternalCommand {
	/// <summary>
	/// Description of SettingsForm.
	/// </summary>
	public partial class SettingsForm : ExternalCommandForm {
		private static readonly log4net.ILog LOG = log4net.LogManager.GetLogger(typeof(SettingsForm));
		private static ExternalCommandConfiguration config = IniConfig.GetIniSection<ExternalCommandConfiguration>();
		
		public SettingsForm() {
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.Icon = GreenshotPlugin.Core.GreenshotResources.getGreenshotIcon();
			UpdateView();
		}
		
		void ButtonOkClick(object sender, EventArgs e) {
			IniConfig.Save();
			DialogResult = DialogResult.OK;
		}
		
		void ButtonCancelClick(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}

		void ButtonAddClick(object sender, EventArgs e) {
			SettingsFormDetail form = new SettingsFormDetail(null);
			form.ShowDialog();
			
			UpdateView();
		}

		void ButtonDeleteClick(object sender, EventArgs e) {
			foreach ( ListViewItem item in listView1.SelectedItems ) {
				string commando = item.Tag as string;
				config.commands.Remove(commando);
				config.commandlines.Remove(commando);
				config.arguments.Remove(commando);
			}
			UpdateView();
		}

		void UpdateView() {
			listView1.Items.Clear();
			if (config.commands != null) {
				listView1.ListViewItemSorter = new ListviewComparer();
				ImageList imageList = new ImageList();
				listView1.SmallImageList = imageList;
				int imageNr = 0;
				foreach (string commando in config.commands) {
					ListViewItem item = null;
					Image iconForExe = IconCache.IconForExe(commando);
					if (iconForExe != null) {
						imageList.Images.Add(iconForExe);
						item = new ListViewItem(commando, imageNr++);
					} else {
						item = new ListViewItem(commando);
					}
					item.Tag = commando;
					listView1.Items.Add(item);
				}
			}
		}

		void ListView1ItemSelectionChanged(object sender, EventArgs e) {
			button4.Enabled = listView1.SelectedItems.Count > 0;
		}
		
		void Button4Click(object sender, EventArgs e)
		{
			ListView1DoubleClick(sender, e);
		}
		
		void ListView1DoubleClick(object sender, EventArgs e)
		{
			string commando = listView1.SelectedItems[0].Tag as string;
			
			SettingsFormDetail form = new SettingsFormDetail(commando);
			form.ShowDialog();
			
			UpdateView();
		}
	}
	public class ListviewComparer : System.Collections.IComparer {
		public int Compare(object x, object y) {
			if (!(x is ListViewItem)) {
				return (0);
			}
			if (!(y is ListViewItem)) {
				return (0);
			}

			ListViewItem l1 = (ListViewItem)x;
			ListViewItem l2 = (ListViewItem)y;
			if (l2 == null) {
				return 1;
			}
			return l1.Text.CompareTo(l2.Text);
		}
	}
}
