namespace ImageMerger
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;

    public class TileMerger
    {
        private int mergedImageCount = 0;

        public static Bitmap MergeBitmaps(List<Bitmap> listOfTileBitmaps, int columnCount, bool horizontalTiling)
        {
            int width = 0;
            int height = 0;
            if (columnCount > listOfTileBitmaps.Count)
            {
                columnCount = listOfTileBitmaps.Count;
            }
            int rowCount = (int)Math.Ceiling((double)(((double)listOfTileBitmaps.Count) / ((double)columnCount)));
            if (!horizontalTiling)
            {
                int temp = rowCount;
                rowCount = columnCount;
                columnCount = temp;
            }

            int count = 1;
            foreach (Bitmap sourceTile in listOfTileBitmaps)
            {
                if (count <= rowCount)
                {
                    height += sourceTile.Height;
                }
                if (count % rowCount == 0)
                {
                    width += sourceTile.Width;
                }
                count++;
            }

            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(image);
            SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0xff, 240, 200));
            graphics.FillRectangle(brush, 0, 0, image.Width, image.Height);
            int n = 0;
            int rowHeight = 0;
            int colWidth = 0;
            int prevcol = 0;
            int prevrow = 0;
            int prevHeight = 0;
            int prevWidth = 0;
            if (horizontalTiling)
            {
                foreach (Bitmap tileBitmap in listOfTileBitmaps)
                {
                    int col = (int)Math.Floor((double)(((double)n) / ((double)columnCount)));
                    int row = n % columnCount;
                    if (col == 0)
                    {
                        rowHeight = 0;
                        prevcol = col;
                    }
                    if (col != prevcol)
                    {
                        rowHeight += prevHeight;
                        prevcol = col;
                    }
                    if (row != prevrow)
                    {
                        colWidth += prevWidth;
                        prevrow = row;
                    }
                    Point location = new Point(colWidth, rowHeight);
                    Size size = new Size(tileBitmap.Width, tileBitmap.Height);
                    Graphics.FromImage(image).DrawImage(tileBitmap, new Rectangle(location, size));
                    n++;
                }
            }
            else
            {
                foreach (Bitmap tileBitmap in listOfTileBitmaps)
                {
                    int row = (int)Math.Floor((double)(((double)n) / ((double)rowCount)));
                    int col = n % rowCount;
                    if (col == 0)
                    {
                        rowHeight = 0;
                        prevcol = col;
                    }
                    if (col != prevcol)
                    {
                        rowHeight += prevHeight;
                        prevcol = col;
                    }
                    if (row != prevrow)
                    {
                        colWidth += prevWidth;
                        prevrow = row;
                    }
                    Point location = new Point(colWidth, rowHeight);
                    Size size = new Size(tileBitmap.Width, tileBitmap.Height);
                    Graphics.FromImage(image).DrawImage(tileBitmap, new Rectangle(location, size));
                    n++;
                    prevWidth = tileBitmap.Width;
                    prevHeight = tileBitmap.Height;
                }
            }
            return image;
        }

        public bool ProcessDirectoryToFile(Form parentForm, string directory, string fileTarget, int columnCount, string filter, bool horizontalTiling=true)
        {
            Bitmap bitmap;
            this.mergedImageCount = 0;
            if (!Directory.Exists(directory))
            {
                MessageBox.Show(parentForm, "The specified source directory " + directory + " does not exist.", "Folder not found error", MessageBoxButtons.OK);
                return false;
            }
            if (fileTarget == "")
            {
                MessageBox.Show(parentForm, "An invalid target file was set.", "Operation stopped", MessageBoxButtons.OK);
                return false;
            }
            List<Bitmap> bitmaps = ImageLoader.LoadImages(ImageLoader.ListFiles(directory, filter));
            if (bitmaps.Count == 0)
            {
                MessageBox.Show(parentForm, "No images were found, could not create merged image.", "Operation stopped", MessageBoxButtons.OK);
                return false;
            }
            try
            {
                bitmap = MergeBitmaps(bitmaps, columnCount, horizontalTiling);
            }
            catch (Exception exception)
            {
                MessageBox.Show(parentForm, exception.Message, "Merge Bitmaps Exception", MessageBoxButtons.OK);
                ImageLoader.DisposeImages(bitmaps);
                return false;
            }
            try
            {
                SaveImage(bitmap, fileTarget);
            }
            catch (Exception exception2)
            {
                MessageBox.Show(parentForm, exception2.Message, "Save Image Exception", MessageBoxButtons.OK);
                return false;
            }
            this.mergedImageCount = bitmaps.Count;
            ImageLoader.DisposeImages(bitmaps);
            return true;
        }

        public static void SaveImage(Bitmap image, string fileTarget)
        {
            image.Save(fileTarget, ImageFormat.Png);
        }

        public int MergedImageCount
        {
            get
            {
                return this.mergedImageCount;
            }
        }
    }
}

