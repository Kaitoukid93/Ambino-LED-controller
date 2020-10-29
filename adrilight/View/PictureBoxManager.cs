﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Rectangle = System.Windows.Shapes.Rectangle;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using MaterialDesignThemes.Wpf;

namespace adrilight.View
{
    public class PictureBoxManager
    {

        public PictureBoxManager(int width, int height)
        {
            loadedWidth = width;
            loadedHeight = height;
        }

        public bool isDrawingMode = false;

        Gifloader form;
       // public Image [,] boxes;
        public ColorZone[,] boxes;


        int[][] newOrder;
        int loadedWidth, loadedHeight;

        int drawIndex = 0;

        public void CreateBoxes(int width, int height)
        {
            int index = 0;
            loadedWidth = width;
            loadedHeight = height;

            DestroyBoxes();

            boxes = new ColorZone [width,height] ;

            int pixelSize = 640 / height;
            float pixelSpacing = 0f;

            //if matrix is too long and pixel size limited by the x width
            if (pixelSize * width > 1344)
            {
                pixelSize = 1344 / width;
                pixelSpacing = (width / 2.1f);
            }
            //if matrix is not too long and pixel size is limited by height
            else
            {
                pixelSpacing = height;
            }

            for (int y = 0; y < loadedHeight; y++)
            {
                for (int x = 0; x < loadedWidth; x++)
                {

                     boxes[x,y] = new ColorZone {
                        Width = pixelSize - 1,
                        Height = pixelSize - 1,
                        Tag = index.ToString(),
                        Background=Brushes.Black,




                    };





                    Canvas.SetTop(boxes[x,y], (int)((x * 640) / pixelSpacing) + 3);
                    Canvas.SetLeft(boxes[x, y], (int)((y * 640) / pixelSpacing) + 3);
                form.matrixContainer.Children.Add(boxes[x, y]);
                    index++;


                }

            }
        }
        
        public void DestroyBoxes()
        {
            if (form == null)
                form = (Gifloader)Application.OpenForms;
            if (boxes != null)
            {
                foreach (var b in form.matrixContainer.Children.OfType<Rectangle>())
                {
                    form.matrixContainer.Children.Remove(b);
                }
            }
            boxes = null;
        }

        public void ChangeDrawEnable(bool state)
        {
            isDrawingMode = state;
            for (int y = 0; y < loadedHeight; y++)
            {
                for (int x = 0; x < loadedWidth; x++)
                {
                    boxes[x,y].IsEnabled = !state;
                }
            }
        }

        public void FrameToBoxes(byte[] data)
        {
            int byteIndex = 0;
            for (int y = 0; y < loadedHeight; y++)
            {
                for (int x = 0; x < loadedWidth; x++)
                {
                    boxes[x, y].Background = new SolidColorBrush(Color.FromArgb(255, data[byteIndex + 2], data[byteIndex + 1], data[byteIndex]));
                    byteIndex += 3;
                }
            }
        }


        public void SendPixel(int x, int y, byte[] data)
        {
            byte[] pixelData = new byte[5];

            int rawIndex = x * 16 + y;
            int orderedIndex = form.sm.frameByteOrder[rawIndex];

            boxes[x, y].Background = new SolidColorBrush(Color.FromArgb(255, data[0], data[1], data[2]));
        }


        public void ClearFrame()
        {
            for (int y = 0; y < loadedHeight; y++)
                for (int x = 0; x < loadedWidth; x++)
                    boxes[x, y].Background =Brushes.Black;
        }

        public void Edit()
        {
            newOrder = new int[loadedWidth * loadedHeight][];

            for (int y = 0; y < loadedHeight; y++)
            {
                for (int x = 0; x < loadedWidth; x++)
                {
                    boxes[x, y].Background = new SolidColorBrush(Color.FromArgb(255, 63, 63, 63));

                    boxes[x, y].MouseMove += pb_MouseMove;
                    boxes[x, y].AllowDrop = true;
                    boxes[x, y].DragEnter += pb_DragEnter;
                    boxes[x, y].DragDrop += pb_DragDrop;
                    boxes[x, y].IsEnabled = true;
                }
            }
        }



        

        private void pb_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var pb = (ColorZone)sender;
              //  pb.DragLeave(pb, DragDropEffects.Move);
            }
        }

        private void pb_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void pb_DragDrop(object sender, DragEventArgs e)
        {
            ColorZone target = (ColorZone)sender;

            ColorZone source = (ColorZone)e.Data.GetData(typeof(ColorZone));

            if (source != target)
            {
                OrderManager(int.Parse(source.Tag.ToString()), int.Parse(target.Tag.ToString()));
            }
        }




        public void OrderManager(int startIndex, int endIndex)
        {
            int totalPixels = loadedWidth * loadedHeight;

            int startx = startIndex % loadedWidth;
            int starty = startIndex / loadedWidth;
            int endx = endIndex % loadedWidth;
            int endy = endIndex / loadedWidth;


            if (startx == endx)
            {
                if (endy > starty)
                {
                    for (int i = starty; i <= endy; i++)
                        DrawOnBox(startx, i, totalPixels);
                }
                else
                {
                    for (int i = starty; i >= endy; i--)
                        DrawOnBox(startx, i, totalPixels);
                }
            }
            if (starty == endy)
            {
                if (endx > startx)
                {
                    for (int i = startx; i <= endx; i++)
                        DrawOnBox(i, starty, totalPixels);
                }
                else
                {
                    for (int i = startx; i >= endx; i--)
                        DrawOnBox(i, starty, totalPixels);
                }
            }

        }


        void DrawOnBox(int x, int y, int totalPixels)
        {
            boxes[x, y].Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, (byte)(((float)drawIndex / (float)totalPixels) * 64 + 64)));

            newOrder[drawIndex] = new int[] { x, y };
            WritePixelData(x, y, drawIndex);

            boxes[x, y].AllowDrop = false;
            boxes[x, y].MouseMove -= pb_MouseMove;
            boxes[x, y].DragEnter -= pb_DragEnter;
            boxes[x, y].DragDrop -= pb_DragDrop;

            drawIndex++;

        }

        void WritePixelData(int x, int y, int di)
        {

            Bitmap bmp = new Bitmap(32, 32);
            RectangleF rectf = new RectangleF(0, 0, 32, 32);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString(di.ToString(), new Font("Tahoma", 8), Brushes.Yellow, rectf);
                g.Flush();
            }
            boxes[x, y].Content = bmp;
        }


        public void SaveOrder()
        {
            int[] order = new int[loadedHeight * loadedWidth];

            if (drawIndex == order.Length)
            {
                int index = 0;
                for (int y = 0; y < loadedHeight; y++)
                {
                    for (int x = 0; x < loadedWidth; x++)
                    {
                        order[index] = int.Parse(boxes[newOrder[index][0], newOrder[index][1]].Tag.ToString());
                        index++;

                        boxes[x, y].MouseMove -= pb_MouseMove;
                        boxes[x, y].AllowDrop = false;
                        boxes[x, y].DragEnter -= pb_DragEnter;
                        boxes[x, y].DragDrop -= pb_DragDrop;
                    }
                }
                SaveFileDialog sf = new SaveFileDialog();

                sf.Title = "Save Pixel Order";
                sf.Filter = "Pixel Order|*.pxlod";
                sf.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                sf.ShowDialog();

                if (sf.FileName != "")
                {

                    string[] fileOrderData = order.Select(x => x.ToString()).ToArray();
                    File.WriteAllLines(sf.FileName, fileOrderData);

                    Properties.Settings.Default.previousPixelOrderFile = sf.FileName;
                    form.sm.frameByteOrder = order;

                    int[] pixelOrder = new int[order.Length];
                    for (int i = 0; i < order.Length; i++)
                    {
                        //  int keyIndex = Array.FindIndex(words, w => w.IsKey);
                        pixelOrder[i] = Array.IndexOf(order, i);
                    }
                    form.sm.pixelByteOrder = pixelOrder;

                    DestroyBoxes();
                    CreateBoxes(loadedWidth, loadedHeight);
                }
                sf.Dispose();
            }
            else
            {
                MessageBox.Show("Not all pixels have an order index");
            }
        }


        public void DoneEditing()
        {
            drawIndex = 0;
            DestroyBoxes();
            CreateBoxes(loadedWidth, loadedHeight);
        }

        public void ResetOrder()
        {
            drawIndex = 0;
            DestroyBoxes();
            CreateBoxes(loadedWidth, loadedHeight);
            Edit();
        }

        public byte[] CreateFrameData()
        {
            byte[] data = new byte[2];

            return data;
        }

    }
}
