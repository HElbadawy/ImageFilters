using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageFilters
{
    public class ImageOperations
    {
        /// <summary>
        /// Open an image, convert it to gray scale and load it into 2D array of size (Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of gray values</returns>
        public static byte[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            byte[,] Buffer = new byte[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x] = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x] = (byte)((int)(p[0] + p[1] + p[2]) / 3);
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(byte[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(byte[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(byte[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = p[1] = p[2] = ImageMatrix[i, j];
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }

        public static void MaxHeap(byte[] Array, int ArrayLength, int i)
        {
            int Left = 2 * i + 1;
            int Right = 2 * i + 2;
            int Largest;
            if (Left < ArrayLength && Array[Left] > Array[i])
                Largest = Left;
            else
                Largest = i;
            if (Right < ArrayLength && Array[Right] > Array[Largest])
                Largest = Right;
            if (Largest != i)
            {
                byte Temp = Array[i];
                Array[i] = Array[Largest];
                Array[Largest] = Temp;
                MaxHeap(Array, ArrayLength, Largest);
            }
        }
        public static void ConstructMaxHeap(byte[] Array, int ArrayLength)
        {
            for (int i = ArrayLength / 2 - 1; i >= 0; i--)
                MaxHeap(Array, ArrayLength, i);
        }
        public static byte[] HeapSort(byte[] Array, int ArrayLength)
        {
            int HeapSize = ArrayLength;
            ConstructMaxHeap(Array, ArrayLength);
            for (int i = ArrayLength - 1; i > 0; i--)
            {
                byte Temp = Array[0];
                Array[0] = Array[i];
                Array[i] = Temp;
                HeapSize--;
                MaxHeap(Array, HeapSize, 0);
            }
            return Array;
        }

        public static byte[] CountingSort(byte[] surroundPixels)
        {
            int k = 256;
            int size = surroundPixels.Length;
            int[] occurrences = new int[k];
            byte[] sortedArray = new byte[size];
            for (int i = 0; i < k; i++)
            {
                occurrences[i] = 0;
            }
            for (int i = 0; i < size; i++)
            {
                occurrences[surroundPixels[i]]++;
            }
            for (int i = 1; i < k; i++)
            {
                occurrences[i] += occurrences[i - 1];
            }
            for (int i = 0; i < size; i++)
            {
                sortedArray[occurrences[surroundPixels[i]] - 1] = surroundPixels[i];
                occurrences[surroundPixels[i]]--;
            }
            return sortedArray;
        }

        static int PIvot_Index(byte[] array, int left, int right)
        {

            int pivot = array[right];

            int leftIndex = (left - 1);

            while (left < right)
            {
                if (array[left] <= pivot)
                {
                    leftIndex++;

                    byte temp = array[leftIndex];
                    array[leftIndex] = array[left];
                    array[left] = temp;
                }
                left++;
            }

            byte temp1 = array[leftIndex + 1];
            array[leftIndex + 1] = array[right];
            array[right] = temp1;

            return leftIndex + 1;
        }

        static void QuickSort(byte[] array, int left, int right)
        {
            if (left < right)
            {
                int PivotIndex = PIvot_Index(array, left, right);

                QuickSort(array, left, PivotIndex - 1);
                QuickSort(array, PivotIndex + 1, right);
            }
        }

        public static byte[] getNeighbors(byte[,] Image, int N, int i, int j)
        {
            byte[] neighbors = new byte[N * N];
            int index = 0;
            for (int ii = -N / 2; ii <= N / 2; ii++)
            {
                for (int jj = -N / 2; jj <= N / 2; jj++)
                {
                    neighbors[index] = Image[i + ii, j + jj];
                    index++;
                }
            }
            return neighbors;
        }
        
        public static byte[] getNeighbors(byte[,] Image, int N, int i, int j, out int index)
        {
            byte[] neighbors = new byte[N * N];
            index = 0;
            for (int ii = -N / 2; ii <= N / 2; ii++)
            {
                for (int jj = -N / 2; jj <= N / 2; jj++)
                {
                    if (i + ii >= 0 && i + ii < GetHeight(Image) && j + jj >= 0 && j + jj < GetWidth(Image))
                    {
                        neighbors[index] = Image[i + ii, j + jj];
                        index++;
                    }
                }
            }

            byte[] final = new byte[index];
            for (int k = 0; k < index; k++)
            {
                final[k] = neighbors[k];
            }
            return final;
        }
        static byte[,] treatBorders(byte[,] Image, int offset, int H, int W, int N)
        {
            byte[,] newImage = new byte[H + N - 1, W + N - 1];

            for (int i = offset; i < W + offset; i++)
            {
                for (int j = offset; j < H + offset; j++)
                {
                    newImage[j, i] = Image[j - offset, i - offset];
                }              
            }

            //loop over row/col at i/j = offset, at i/j = H/W + offset
            //reflect left side
            int side1 = offset;
            int side2 = side1 - 1;
            for (int i = 0; i < offset; i++)
            {
                for (int j = offset; j < H + offset; j++)
                {
                    newImage[j, side2] = newImage[j, side1];
                }
                side1++;
                side2--;
            }

            //reflect right side
            side1 = offset + W;
            side2 = side1 - 1;
            for (int i = 0; i < offset; i++)
            {
                for (int j = offset; j < H + offset; j++)
                {
                    newImage[j, side1] = newImage[j, side2];
                }
                side1++;
                side2--;
            }

            //top side
            side1 = offset;
            side2 = side1 - 1;
            for (int i = 0; i < offset; i++)
            {
                for (int j = 0; j < W + N - 1; j++)
                {
                    newImage[side2, j] = newImage[side1, j];
                }
                side1++;
                side2--;
            }

            //lower side
            side1 = offset + H;
            side2 = side1 - 1;
            for (int i = 0; i < offset; i++)
            {
                for (int j = 0; j < W + N - 1; j++)
                {
                    newImage[side1, j] = newImage[side2, j];
                }
                side1++;
                side2--;
            }

            return newImage;
        }

        // Alpha trim
        public static byte[,] AlphaTrim(byte[,] Image, int N, int T, int sort) 
        {
            //loop over each direction 
            //array of surrounding pixels
            //counting sort
            //remove noise
            //calculate arithmetic mean
            //assign mean to middle pixel 
            int H = GetHeight(Image);
            int W = GetWidth(Image);
            int offset = N / 2;
            
            byte[,] newImage = treatBorders(Image, offset, H, W, N);


            byte[] sortedNeighbors;
            for (int i = offset; i < H + offset; i++)
            {
                for (int j = offset; j < W + offset; j++)
                {
                    if (sort == 0)
                    {
                        sortedNeighbors = CountingSort(getNeighbors(newImage, N, i, j));
                    }
                    else
                    {
                        sortedNeighbors = HeapSort(getNeighbors(newImage, N, i, j), N * N);
                    }

                    int sum = 0;
                    for (int n = T; n < N * N - T; n++)
                    {
                        sum += sortedNeighbors[n];
                    }
                    double mean = Math.Round((double)sum / (double)(N * N - 2 * T));
                    newImage[i, j] = (byte)mean;
                }
            }
            
            byte[,] final = new byte[H, W];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    final[j, i] = newImage[j + offset, i + offset];
                }
            }

            return final;
        }

        //Adaptive Median
        static int FindMedian(byte[] arr, int size)
        {
            int Z_median;
            if (size % 2 == 0)
            {
                Z_median = (arr[(size / 2) - 1] + arr[(size / 2)]) / 2;
            }
            else
            {
                Z_median = arr[(size / 2)];
            }
            return Z_median;
        }

        public static byte[,] AdaptiveMedianFilter(byte[,] Image, int N, int sort)
        {

            int H = GetHeight(Image);
            int W = GetWidth(Image);
            byte[,] newImage = Image;

            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    int neighborsLength;
                    byte[] sortedNeighbors;
                    int start = 3;
                    byte X = newImage[i, j];
                    byte newValue = X;

                    while (start <= N)
                    {
                        if (sort == 0)
                        {
                            sortedNeighbors = getNeighbors(newImage, start, i, j, out neighborsLength);
                            sortedNeighbors = CountingSort(sortedNeighbors);
                        }
                        else
                        {
                            sortedNeighbors = getNeighbors(newImage, start, i, j, out neighborsLength);
                            QuickSort(sortedNeighbors, 0, neighborsLength - 1);
                        }

                        int Z_Min = sortedNeighbors[0];
                        int Z_Max = sortedNeighbors[neighborsLength - 1];
                        int Z_Median = FindMedian(sortedNeighbors, neighborsLength);

                        int A1 = Z_Median - Z_Min;
                        int A2 = Z_Max - Z_Median;

                        if (A1 > 0 && A2 > 0)
                        {
                            int B1 = X - Z_Min;
                            int B2 = Z_Max - X;
                            if (B1 > 0 && B2 > 0)
                            {
                                newValue = X;
                            }
                            else
                            {
                                newValue = (byte)Z_Median;
                            }
                            break;
                        }

                        start += 2;     
                        if (start > N)
                        {
                            newValue = (byte)Z_Median;
                        }
                    }
                    newImage[i, j] = newValue;
                }
            }
            
            return newImage;
        }
    }
}
