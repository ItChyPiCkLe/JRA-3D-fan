using System;
using System.Drawing;
using System.IO;
using System.Text;

public class binCreator
{
	public binCreator()
	{
    }

    private void appendText(string iny, string file)
    {
        Encoding enc = Encoding.GetEncoding("iso-8859-8");
        Stream stream = new FileStream(file, FileMode.Create);
        StreamWriter swq = new StreamWriter(stream, enc);
        //using (StreamWriter sw = File.AppendText(file))
        {
            swq.WriteLine(iny);
        }
    }
    private void compress()
    {

    }

    public void makeFile(string file, string fileOut)
    {
        Bitmap img = new Bitmap(file);
        Bitmap resized = new Bitmap(img, img.Width/3, img.Height/3);     
        string runningLine = "";
        string rgbVal = "";


        for (int i = 0; i < resized.Height; i++)
        {
            for (int j = 0; j < resized.Width; j++)
            {
                Color pixel = resized.GetPixel(j, i);

                rgbVal = splitColor(pixel);
                runningLine += rgbVal;
                
             }
            //append info into file
            appendText(runningLine, fileOut);
            runningLine = "";
        }
    }

    private string splitColor(Color col)
    {
        //returns #FFFFFF
        String htmlColor1 = System.Drawing.ColorTranslator.ToHtml(col);
        //Console.Write(htmlColor1[1] + "," + htmlColor1[2]);
        //break it up.
        string r = htmlColor1[1].ToString() + htmlColor1[2].ToString();
        string g = htmlColor1[3].ToString() + htmlColor1[4].ToString();
        String b = htmlColor1[5].ToString() + htmlColor1[6].ToString();
        //Console.WriteLine(r);
        //Console.WriteLine(g);
        //Console.WriteLine(b);
        //convert hex to dec
        int numr = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
        int numg = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
        int numb = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
        //convert dec to symbol
        char R = Convert.ToChar(numr);
        char G = Convert.ToChar(numg);
        char B = Convert.ToChar(numb);
        //convert symbol back to string 
        string d = R.ToString();
        string e = G.ToString();
        string f = B.ToString();

        //Console.WriteLine(d + e + f);
        string outy = (d + e + f);

        return outy;
    }
}

