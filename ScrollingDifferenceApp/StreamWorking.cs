using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public static class StreamWorking
{

    unsafe public static Bitmap WskaźnikNaObraz(byte* ws, int x, int y)
    {
        int lb = x * y;
        Bitmap zw = new Bitmap(x, y);
        BitmapData bd = zw.LockBits(new Rectangle(0, 0, x, y), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
        RGB[] LisKol = new RGB[256];
        LisKol[0] = new RGB() { R = 0, G = 0, B = 0 };
        for (byte i = 1; i != 0; i++)
        {
            LisKol[i] = new RGB() { R = i, G = i, B = i };
        }
        int[] l = new int[256];
        for (int i = 0; i < y; i++)
        {
            RGB* tb = (RGB*)(byte*)(bd.Scan0 + i * bd.Stride);
            for (int j = 0; j < x; j++, tb++, ws++)
            {
                *tb = LisKol[*ws];
            }
        }

        zw.UnlockBits(bd);
        return zw;
    }
    unsafe public static void Clear(bool* Sprawdzana, int Dłógość)
    {
        int ilośćDecimal = Dłógość - 16;
        long* CzyśćDecimal = (long*)Sprawdzana;
        int i = 0;
        for (; i < ilośćDecimal; i += 16)
        {
            *CzyśćDecimal = 0;
            CzyśćDecimal[1] = 0;
            i += 2;
        }
        bool* CzyśćBool = (bool*)CzyśćDecimal;

        bool* KonecBool = (bool*)Sprawdzana + Dłógość;
        while (CzyśćBool < KonecBool)
        {
            *CzyśćBool = false;
            CzyśćBool++;
        }
    }

    public static unsafe byte* LoadMonohrome(Bitmap Obraz)
    {
        IntPtr mr = Marshal.AllocHGlobal(Obraz.Width * Obraz.Height);
        StreamWorking.Clear((bool*)mr, Obraz.Width * Obraz.Height);
        byte* obsugiwana = (byte*)mr;

        int j = 0;
        BitmapData bp = Obraz.LockBits(new Rectangle(0, 0, Obraz.Width, Obraz.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

        for (int y = 0; y < Obraz.Height; y++)
        {

            rgb* kr = (rgb*)((byte*)(bp.Scan0 + y * bp.Stride));
            for (int x = 0; x < Obraz.Width; x++, kr++, obsugiwana++)
            {
                j = (*kr).r;
                j += (*kr).g;
                j += (*kr).b;
                byte zw = ((byte)(j / 3));
                *obsugiwana = zw;
            }
        }
        Obraz.UnlockBits(bp);
        return (byte*)mr;
    }
    public static unsafe void OtshuMethod( int WielkośćObrazu, byte* Obraz)
    {
        int[] his = PobierzHistogram(new Size(1, WielkośćObrazu), Obraz);
        Progój(WielkośćObrazu, MetodaOtsu(his), Obraz);

    }
    private static unsafe int[] PobierzHistogram(Size wielkość, byte* obsugiwana)
    {
        int[] n = new int[256];
        PobierzHistogram(wielkość, obsugiwana, n);
        return n;
    }

    private static int MetodaOtsu(int[] hist)
    {
        byte t = 0;
        float[] vet = new float[256];
        vet.Initialize();
        float PrA, PrB, p12;
        int k;


        PrA = hist[0];
        PrB = Px(1, 255, hist);
        int SrA = hist[0];
        int SrB = ŚredniaI(1, 255, hist);
        for (k = 1; k != 255; k++)
        {
            int W = hist[k];
            PrA += W; PrB -= W;
            SrA += W * k; SrB -= W * k;
            p12 = PrA * PrB;
            if (p12 < 0.0001f) p12 = 1;
            float diff = (SrA * PrB) - (SrB * PrA);
            vet[k] = (float)diff * diff / p12;
        }
        t = (byte)findMax(vet, 256);
        return t;
    }

    private static float Px(int init, int end, int[] hist)
    {
        int sum = 0;
        int i;
        for (i = init; i <= end; i++)
            sum += hist[i];

        return (float)sum;
    }

    private static int findMax(float[] vec, int n)
    {
        float maxVec = 0;
        int idx = 0;
        int i;

        for (i = 1; i < n - 1; i++)
        {
            if (vec[i] > maxVec)
            {
                maxVec = vec[i];
                idx = i;
            }
        }
        return idx;
    }
    private static int ŚredniaI(int init, int end, int[] hist)
    {
        int sum = 0;
        for (int i = init; i <= end; i++)
            sum += i * hist[i];
        return sum;
    }
    public static unsafe void PobierzHistogram(Size wielkość, byte* obsugiwana, int[] histogram)
    {
        int l = wielkość.Width * wielkość.Height;
        while (l > 0)
        {
            histogram[*obsugiwana]++;
            obsugiwana++;
            l--;
        }
    }
    private static unsafe bool* Progój(int WielkośćObrazu, int WartośćProgu, byte* ObrazZprogowany)
    {
        byte* K = ObrazZprogowany;
        byte Prawda = 255;
        byte Fałsz = 0;
        for (int i = 0; i < WielkośćObrazu; i++, ObrazZprogowany++)
        {
            *ObrazZprogowany = (byte)((*ObrazZprogowany) > WartośćProgu ? Prawda : Fałsz);
        }

        return (bool*)K;
    }

    struct rgb
    {
        public byte r, g, b;
    }
    public struct RGB
    {
        public byte R, G, B;
    }
}

