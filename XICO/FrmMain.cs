﻿using System;
using System.Drawing;
using System.Windows.Forms;
using NewLife.IO;
using System.Collections.Generic;
using System.IO;

namespace XICO
{
    public partial class FrmMain : Form
    {
        #region 窗口初始化
        public FrmMain()
        {
            InitializeComponent();

            //AllowDrop = true;
            picSrc.AllowDrop = true;
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            var ms = FileSource.GetFileResource(null, "XCoder.XICO.leaf.png");
            if (ms != null) picSrc.Image = new Bitmap(ms);

            var ft = lbFont.Font;
            lbFont.Tag = new Font(ft.Name, 96, ft.Style);

            MakeWater();
        }
        #endregion

        #region 水印
        private void label3_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = (Font)lbFont.Tag;
            if (fontDialog1.ShowDialog() != DialogResult.OK) return;

            var ft = fontDialog1.Font;
            lbFont.Tag = ft;
            lbFont.Font = new Font(ft.Name, lbFont.Font.Size, ft.Style);

            MakeWater();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = lbFont.ForeColor;
            if (colorDialog1.ShowDialog() != DialogResult.OK) return;

            lbFont.ForeColor = colorDialog1.Color;
            label4.ForeColor = colorDialog1.Color;

            MakeWater();
        }

        private void btnWater_Click(object sender, EventArgs e)
        {
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            //var str = txt.Text;
            //if (String.IsNullOrEmpty(str)) str = "字体样板";
            //lbFont.Text = str;

            MakeWater();
        }

        private void numX_ValueChanged(object sender, EventArgs e)
        {
            MakeWater();
        }

        void MakeWater(Boolean save = false)
        {
            var brush = new SolidBrush(lbFont.ForeColor);

            var bmp = picSrc.Image;
            if (!save && bmp.Width > picDes.Width)
                bmp = new Bitmap(bmp, picDes.Width, picDes.Height);
            else
                bmp = new Bitmap(bmp);

            if (!String.IsNullOrEmpty(txt.Text))
            {
                var ft = (Font)lbFont.Tag;

                var g = Graphics.FromImage(bmp);
                g.DrawString(txt.Text, ft, brush, (Int32)numX.Value, (Int32)numY.Value);
                g.Dispose();
            }

            if (!save)
            {
                picDes.Image = bmp;
                picDes.Refresh();
            }
            else
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    bmp.Save(saveFileDialog1.FileName, picSrc.Image.RawFormat);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MakeWater(true);
        }
        #endregion

        #region 图标
        private void btnMakeICO_Click(object sender, EventArgs e)
        {
            var list = new List<Int16>();
            foreach (var item in groupBox2.Controls)
            {
                var chk = item as CheckBox;
                if (chk != null && chk.Checked) list.Add(Int16.Parse(chk.Name.Substring(3)));
            }

            if (list.Count < 1)
            {
                MessageBox.Show("请选择大小！");
                return;
            }

            var ms = new MemoryStream();
            IconFile.Convert(picSrc.Image, ms, list.ToArray());

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, ms.ToArray());
            }
        }
        #endregion

        #region 图片加载
        private void picSrc_DragDrop(object sender, DragEventArgs e)
        {
            var fs = (String[])e.Data.GetData(DataFormats.FileDrop);
            if (fs != null && fs.Length > 0)
            {
                try
                {
                    saveFileDialog1.FileName = fs[0];
                    picSrc.Load(fs[0]);
                }
                catch { }
            }
        }

        private void picSrc_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;
        }
        #endregion
    }
}