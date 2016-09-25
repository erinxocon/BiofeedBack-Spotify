using System;
using System.Diagnostics;
using System.Linq;
using System.IO.Ports;
using SpotifyAPI.Local;
using System.Collections.Generic;
using System.Threading;

class PortDataReceived
{
    public static SpotifyLocalAPI _spotify;

    public static bool isPlaying1 = false;
    public static bool isPlaying2 = false;

    public static FixedSizedLinkedList<int> runningTotal = new FixedSizedLinkedList<int>(61);

    public static void Main()
    {
        _spotify = new SpotifyLocalAPI();

        _spotify.Connect();

        SerialPort serialPort = new SerialPort("COM1");

        serialPort.BaudRate = 115200;
        serialPort.Parity = Parity.None;
        serialPort.StopBits = StopBits.One;
        serialPort.DataBits = 8;
        serialPort.Handshake = Handshake.None;
        serialPort.RtsEnable = true;
        serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        serialPort.Open();

        var timer = new Timer(e => ChangePlaylist(), null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30));

        Console.WriteLine("Press any key to continue...");
        Console.WriteLine();
        Console.ReadKey();
        serialPort.Close();
    }

    public static void ChangePlaylist()
    {
        double average = runningTotal.Average();

        Console.WriteLine(average.ToString());
        Debug.Print(average.ToString());

        if (average < 95 && isPlaying1 == false)
        {

            //_spotify.PlayURL("spotify:album:6qb9MDR0lfsN9a2pw77uJy");
            Console.WriteLine("average bpm is below 95");
            isPlaying1 = true;
            isPlaying2 = false;

        }

        else if (average > 95 && isPlaying2 == false)
        {
            Console.WriteLine("average bpm is above 95");
            //_spotify.PlayURL("spotify:user:1232721484:playlist:3mTqGR2ENXSvXewhXTCWv0");
            isPlaying1 = false;
            isPlaying2 = true;
        }
    }

    public static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort serialPort = (SerialPort)sender;
        string indata = serialPort.ReadExisting();
        

        if (indata.Contains("B"))
        {
            Debug.WriteLine("Data Received:");
            Debug.WriteLine(indata);

            List<string> sub = new List<string>(indata.Split("SBQ".ToCharArray()));
            Debug.WriteLine(sub[2]);

            try
            {

                int bpm = Int32.Parse(sub[2]);

                if (bpm > 30 && bpm < 240)
                {
                    runningTotal.AddFirst(bpm);
                }

            }

            catch (System.FormatException exp)
            {
                Debug.Print(exp.ToString());
            }

        }
    }

    public class FixedSizedLinkedList<T> : LinkedList<T>
    {
        private readonly object syncObject = new object();

        public int Size { get; private set; }

        public FixedSizedLinkedList(int size)
        {
            Size = size;
        }

        public new void AddFirst(T obj)
        {
            base.AddFirst(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    base.RemoveLast();
                }
            }
        }

        public new void AddLast (T obj)
        {
            base.AddLast(obj);
            lock (syncObject)
            {
                while (base.Count > Size)
                {
                    base.RemoveFirst();
                }
            }
        }
    }
}
