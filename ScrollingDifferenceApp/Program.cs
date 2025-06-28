//#define difrence
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
public class ScrollingDifferenceApp
{
#if difrence

    static int indexDifrenceImage = 0;
    static nint ToTestDifrencesnInt;
#endif
    static int Width;
    static int Height;
    class RowResult
    {
        public string path;
        public int Number, Gap;
    }
    static string textName = "frame_";

    const string parameterForBinaryMask = "-binaryMask";
    const string parameterFormaxMove = "-maxMove";
    public unsafe static void Main(string[] args)
    {
        int NumberRowToProces;
        List<RowResult> rowResults = new List<RowResult>();
        Console.WriteLine($"arrey path frames, {parameterForBinaryMask} ");
        if (args == null || args.Length == 0)
        {
            textName = "FrameLoadFromFile_";
            args = File.ReadAllLines("args.txt");
        }

        if (args == null || args.Length == 0)
        {
            Console.WriteLine("give proper parameters or files named args.txt");
        }
        else
        {
            string maskPath = null;
            string[] imagesPath;

            imagesPath = LoadParameters(args, ref maskPath, out float MaxMove, out rowResults);
            //variables
            bool* maskImageBool = (bool*)0;
            byte* OldImageByte, NewImageByte;
            bool UseMask = false;


            //loading images
            if (maskPath != null)
            {
                Bitmap MaskImage = (Bitmap)Bitmap.FromFile(maskPath);
                maskImageBool = (bool*)StreamWorking.LoadMonohrome(MaskImage);
                MaskImage.Dispose();
                UseMask = true;
            }

            {
                Bitmap OldImage = (Bitmap)Bitmap.FromFile(imagesPath[0]);
                Width = OldImage.Width;
                Height = OldImage.Height;

#if difrence
                ToTestDifrencesnInt = Marshal.AllocHGlobal(Width * Height);
#endif
                OldImageByte = StreamWorking.LoadMonohrome(OldImage);
                StreamWorking.OtshuMethod(Width * Height, OldImageByte);
                OldImage.Dispose();
                NumberRowToProces = (int)(MaxMove * Height);

            }
            ///
            int Posytion = 0;
            for (int i = 1; i < imagesPath.Length; i++)
            {
                NewImageByte = LoadPointer(imagesPath, i);

                decimal[] results = new decimal[NumberRowToProces];


#if difrence 
for (int item = 1; item < NumberRowToProces; item++)
                {
#else
                Parallel.ForEach(Enumerable.Range(0, NumberRowToProces ), item =>{
#endif
                    int numberRowToProces = (Height - item);
                    int sift = (item * Width);
                    results[item] = GetDifrent(OldImageByte + sift, NewImageByte, UseMask, maskImageBool + sift, maskImageBool, (Width * numberRowToProces));


                }

                );
                rowResults[i].Gap = Posytion += MinIndex(results);
                Marshal.FreeHGlobal((nint)OldImageByte);
                OldImageByte = NewImageByte;
            }
        }
        SaveResult(rowResults);

    }

    private static void SaveResult(List<RowResult> rowResults)
    {
        List<string> strings = new List<string>(rowResults.Count + 1);
        strings.Add("index,path,gap,mask");
        foreach (var item in rowResults)
        {
            strings.Add($"{item.Number},{item.path},{item.Gap},");
        }
        File.WriteAllLines("output.csv", strings.ToArray());
    }

    public static int MinIndex(decimal[] table)
    {
        int minIndex = 0;
        decimal minValue = decimal.MaxValue;
        for (int i = 1; i < table.Length; i++)
        {
            decimal current = table[i];
            if (minValue > current)
            {
                minIndex = i;
                minValue = current;
            }
        }
        return minIndex;
    }

    private static unsafe string[] LoadParameters(string[] args, ref string maskPath, out float maxMove, out List<RowResult> rowResults)
    {
        rowResults = new List<RowResult>(args.Length);
        string[] imagesPath;
        int indexMask = Array.IndexOf(args, parameterForBinaryMask);
        int indexMove = Array.IndexOf(args, parameterFormaxMove);
        if (indexMask != -1)
        {
            maskPath = args.Skip(indexMask + 1).First();
        }
        if (indexMove != -1)
        {
            maxMove = float.Parse(args.Skip(indexMove + 1).First(), CultureInfo.InvariantCulture);
        }
        else
        {
            maxMove = 1;
        }

        int numberOfPaths = new int[] { indexMove, indexMask }.Min();

        if (numberOfPaths == -1)
        {
            imagesPath = args;
        }
        else
        {
            imagesPath = args.Take(indexMask).ToArray();
        }

        for (int i = 0; i < numberOfPaths; i++)
        {
            rowResults.Add(new RowResult() { path = args[i], Number = i });
        }
        return imagesPath;
    }

    private static unsafe byte* LoadPointer(string[] imagesPath, int i)
    {
        byte* NewImageByte;
        Bitmap Image = (Bitmap)Bitmap.FromFile(imagesPath[i]);
        NewImageByte = StreamWorking.LoadMonohrome(Image);

        StreamWorking.OtshuMethod(Width * Height, NewImageByte);
        Image.Dispose();


        return NewImageByte;
    }

    public static unsafe decimal GetDifrent(byte* imageOne, byte* imageTwo, bool UseMask, bool* mask, bool* mask2, int lenght)
    {
#if difrence
        byte* testPointer = (byte*)ToTestDifrencesnInt;
#endif
        int pixelToCount = 0;
        long result = 0;
        for (int i = 0; i < lenght; i++)
        {
            if ((*mask) && (*mask2) && UseMask)
            {
                int value = (*imageOne) - (*imageTwo);
                result += Math.Abs(value);
                pixelToCount++;
#if difrence
                *testPointer = (byte)value;

            }
            else
            {
                *testPointer = 0;
            }
            testPointer++;
#endif
            }
            imageOne++;
            imageTwo++;
            mask++;
            mask2++;
        }

        if (pixelToCount == 0)
        {
            return decimal.MaxValue;
        }
        decimal resultDouble = result;
        resultDouble /= pixelToCount;

        return resultDouble;

#if difrence
            SaveDifrenceImage();

        unsafe static void SaveDifrenceImage()
        {
            Bitmap bp = StreamWorking.WskaźnikNaObraz((byte*)ToTestDifrencesnInt, Width, Height);
            bp.Save($"Framedifrence{indexDifrenceImage++}.png");
            bp.Dispose();
        }
#endif
    }
}