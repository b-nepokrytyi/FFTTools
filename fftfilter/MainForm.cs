﻿using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Emgu.CV;
using Emgu.CV.Structure;
using FFTTools;

namespace fftfilter
{
    /// <summary>
    ///     Main application window
    /// </summary>
    public partial class MainForm : RibbonForm
    {
        private readonly FilterDialog _filterDialog = new FilterDialog(new Size(3, 3));

        public MainForm()
        {
            InitializeComponent();
            InitSkinGallery();
            openFileDialog1.Filter =
                saveFileDialog1.Filter =
                    @"Bitmap Images (*.bmp)|*.bmp|All Files (*.*)|*.*";
        }

        private void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }

        private void openFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                var bitmap = new Bitmap(openFileDialog1.FileName);
                pictureEdit1.Image = bitmap;
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void saveAsFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) throw new Exception("Нет изображения");
                if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                bitmap.Save(saveFileDialog1.FileName);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void blur_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) throw new Exception("Нет изображения");
                if (_filterDialog.ShowDialog() != DialogResult.OK) return;

                var filterSize = _filterDialog.FilterSize;

                using (var builder = new FilterBuilder(filterSize))
                    pictureEdit1.Image = builder.Blur(bitmap);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void iAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var aboutBox = new AboutBox())
                aboutBox.ShowDialog();
        }

        private void sharp_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) throw new Exception("Нет изображения");
                if (_filterDialog.ShowDialog() != DialogResult.OK) return;

                var filterSize = _filterDialog.FilterSize;

                using (var builder = new FilterBuilder(filterSize))
                    pictureEdit1.Image = builder.Sharp(bitmap);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void pictureEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) throw new Exception("Нет изображения");
                using (var image = new Image<Bgr, byte>(bitmap))
                {
                    var length = image.Data.Length;
                    var bytes = new byte[length];

                    var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                    Marshal.Copy(handle.AddrOfPinnedObject(), bytes, 0, bytes.Length);
                    handle.Free();

                    var average = bytes.Average(x => (double) x);
                    var delta = Math.Sqrt(bytes.Average(x => (double) x*x) - average*average);
                    var minValue = bytes.Min(x => (double) x);
                    var maxValue = bytes.Max(x => (double) x);
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Format("Length {0}", length));
                    sb.AppendLine(string.Format("Average {0}", average));
                    sb.AppendLine(string.Format("Delta {0}", delta));
                    sb.AppendLine(string.Format("minValue {0}", minValue));
                    sb.AppendLine(string.Format("maxValue {0}", maxValue));
                    siInfo.Caption = sb.ToString();
                }
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }

        private void vizualize_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var bitmap = pictureEdit1.Image as Bitmap;
                if (bitmap == null) throw new Exception("Нет изображения");
                if (_filterDialog.ShowDialog() != DialogResult.OK) return;

                var filterSize = _filterDialog.FilterSize;

                using (var builder = new FilterBuilder(filterSize))
                    pictureEdit1.Image = builder.ToBitmap(bitmap);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message);
            }
        }
    }
}