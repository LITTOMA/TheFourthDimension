﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace The4Dimension
{
    public partial class FrmObjEditor : Form
    {
        public LevelObj Value { get; set; }

        public FrmObjEditor(Dictionary<string, object> Lev)
        {
            InitializeComponent();
            Value = new LevelObj();
            Value.Prop = Lev;
        }

        private void FrmObjEditor_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = new DictionaryPropertyGridAdapter(Value.Prop);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Value.Prop.Remove(propertyGrid1.SelectedGridItem.Label);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FrmAddValue v = new FrmAddValue(Value);
            v.ShowDialog();
            if (v.resName != null && v.resName != "") Value.Prop.Add(v.resName, v.result);
            propertyGrid1.Refresh();
        }

        private void pasteValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteValue(clipboard[clipboard.Count - 1]);
            ClipBoardMenu.Close();
        }

        private void copyPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyValue("pos_");
        }

        private void copyRotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyValue("dir_");
        }

        private void copyScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyValue("scale_");
        }

        private void ClipBoardMenu_CopyArgs_Click(object sender, EventArgs e)
        {
            CopyValue("Arg");
        }

        void CopyValue(string value)
        {
            ClipBoardItem cl = new ClipBoardItem();
            if (value == "pos_" || value == "dir_" || value == "scale_")
            {
                if (value == "pos_") cl.Type = ClipBoardItem.ClipboardType.Position;
                else if (value == "dir_") cl.Type = ClipBoardItem.ClipboardType.Rotation;
                else cl.Type = ClipBoardItem.ClipboardType.Scale;
                if (Value.Prop.ContainsKey(value + "x") && Value.Prop.ContainsKey(value + "y") && Value.Prop.ContainsKey(value + "z"))
                {
                    cl.X = Single.Parse(((Node)Value.Prop[value + "x"]).StringValue);
                    cl.Y = Single.Parse(((Node)Value.Prop[value + "y"]).StringValue);
                    cl.Z = Single.Parse(((Node)Value.Prop[value + "z"]).StringValue);
                }
                else MessageBox.Show("You can't copy this value from this object");
            }
            else if (value == "Arg")
            {
                cl.Type = ClipBoardItem.ClipboardType.IntArray;
                if (Value.Prop.ContainsKey("Arg"))
                {
                    cl.Args = (int[])((int[])Value.Prop["Arg"]).Clone(); //This looks strange but (int[])Value.Prop["Arg"] doesn't work
                }
                else MessageBox.Show("You can't copy this value from this object");
            }
            clipboard.Add(cl);
            if (clipboard.Count > 5) clipboard.RemoveAt(0);
            ClipBoardMenu_Paste.DropDownItems.Clear();
            List<ToolStripMenuItem> Items = new List<ToolStripMenuItem>();
            for (int i = 0; i < clipboard.Count; i++)
            {
                ToolStripMenuItem btn = new ToolStripMenuItem();
                btn.Name = "ClipboardN" + i.ToString();
                btn.Text = clipboard[i].ToString();
                btn.Click += QuickClipboardItem_Click;
                Items.Add(btn);
            }
            Items.Reverse();
            ClipBoardMenu_Paste.DropDownItems.AddRange(Items.ToArray());
        }

        private void QuickClipboardItem_Click(object sender, EventArgs e)
        {
            string SenderName = ((ToolStripMenuItem)sender).Name;
            int index = int.Parse(SenderName.Substring("ClipboardN".Length));
            PasteValue(clipboard[index]);
        }

        void PasteValue(ClipBoardItem itm)
        {
            if (itm.Type == ClipBoardItem.ClipboardType.Position)
            {
                if (Value.Prop.ContainsKey("pos_x")) ((Node)Value.Prop["pos_x"]).StringValue = itm.X.ToString();
                if (Value.Prop.ContainsKey("pos_y")) ((Node)Value.Prop["pos_y"]).StringValue = itm.Y.ToString();
                if (Value.Prop.ContainsKey("pos_z")) ((Node)Value.Prop["pos_z"]).StringValue = itm.Z.ToString();
            }
            else if (itm.Type == ClipBoardItem.ClipboardType.Rotation)
            {
                if (Value.Prop.ContainsKey("dir_x")) ((Node)Value.Prop["dir_x"]).StringValue = itm.X.ToString();
                if (Value.Prop.ContainsKey("dir_y")) ((Node)Value.Prop["dir_y"]).StringValue = itm.Y.ToString();
                if (Value.Prop.ContainsKey("dir_z")) ((Node)Value.Prop["dir_z"]).StringValue = itm.Z.ToString();
            }
            else if (itm.Type == ClipBoardItem.ClipboardType.Scale)
            {
                if (Value.Prop.ContainsKey("scale_x")) ((Node)Value.Prop["scale_x"]).StringValue = itm.X.ToString();
                if (Value.Prop.ContainsKey("scale_y")) ((Node)Value.Prop["scale_y"]).StringValue = itm.Y.ToString();
                if (Value.Prop.ContainsKey("scale_z")) ((Node)Value.Prop["scale_z"]).StringValue = itm.Z.ToString();
            }
            else if (itm.Type == ClipBoardItem.ClipboardType.IntArray)
            {
                if (Value.Prop.ContainsKey("Arg")) Value.Prop["Arg"] = itm.Args;
                else Value.Prop.Add("Arg", itm.Args);
            }
            propertyGrid1.Refresh();
        }
    }
}