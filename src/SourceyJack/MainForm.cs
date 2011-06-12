/***********************************************************************

Sourcey Jack - A simple SOCKSifying application for Windows
Copyright (C) 2011 James Forshaw

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

***********************************************************************/ 
using System;
using System.Runtime.Remoting;
using System.Windows.Forms;
using EasyHook;
using System.Net;
using System.Diagnostics;
using System.Collections;

namespace SourceyJack
{
    public partial class MainForm : Form
    {        
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Executables (*.exe)|*.exe";
            dlg.ShowReadOnly = false;
            dlg.FileName = textBoxExe.Text;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxExe.Text = dlg.FileName;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string exe = textBoxExe.Text.Trim();
            IPAddress addr;            

            if (String.IsNullOrEmpty(exe))
            {
                MessageBox.Show(this, "Must specify an executable file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!IPAddress.TryParse(textBoxServer.Text, out addr))
            {
                MessageBox.Show(this, "Must specific a value IP address for the server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int pid;

                try
                {                    
                    RemoteHooking.CreateAndInject(exe, textBoxCmdLine.Text, 0, "JackInject.dll", "JackInject.dll", out pid, new IPEndPoint(addr, (int)numPort.Value));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        class ProcessComparer : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                int a = (int)((ListViewItem)x).Tag;
                int b = (int)((ListViewItem)y).Tag;

                if (a < b)
                {
                    return -1;
                }
                else if (a > b)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }                
            }
        }

        class ProcessNameComparer : IComparer
        {
            int IComparer.Compare(object x, object y)
            {
                string a = ((ListViewItem)x).SubItems[1].Text;
                string b = ((ListViewItem)y).SubItems[1].Text;

                return a.CompareTo(b);
            }
        }

        private void RefreshProcessList()
        {
            listViewProcesses.Items.Clear();

            Process[] procs = Process.GetProcesses();

            foreach (Process p in procs)
            {
                ListViewItem item = new ListViewItem(p.Id.ToString());

                item.SubItems.Add(p.ProcessName);
                item.Tag = p.Id;

                listViewProcesses.Items.Add(item);
            }

            listViewProcesses.ListViewItemSorter = new ProcessNameComparer();
            listViewProcesses.Sorting = SortOrder.Ascending;
            listViewProcesses.Sort();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void btnInject_Click(object sender, EventArgs e)
        {            
            IPAddress addr;

            if (listViewProcesses.SelectedItems.Count == 0)
            {
                MessageBox.Show(this, "Must select a process", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!IPAddress.TryParse(textBoxServer.Text, out addr))
            {
                MessageBox.Show(this, "Must specific a value IP address for the server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int pid = (int)listViewProcesses.SelectedItems[0].Tag;             

                try
                {
                    RemoteHooking.Inject(pid, "JackInject.dll", "JackInject.dll", new IPEndPoint(addr, (int)numPort.Value));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
