using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace BarcodeReader
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public SerialPort serial = new SerialPort();
        public TextControl textControl = new TextControl();
        public BarcodeDataModel barcode = new BarcodeDataModel();


        private List<BarcodeDataModel> barcodeDatas = new List<BarcodeDataModel>();
        private ObservableCollection<BarcodeDataModel> listViewCollection = new ObservableCollection<BarcodeDataModel>();

        private string[] port = SerialPort.GetPortNames();
        private List<string> baudRateList = new List<string>()
        {
            "110",
            "300",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600"
        };
        private List<int> dataBitsList = new List<int>()
        {
            5,
            6,
            7,
            8
        };
        private List<Parity> parityBitsList = new List<Parity>()
        {
            Parity.None,
            Parity.Even,
            Parity.Odd,
            Parity.Mark,
            Parity.Space
        };
        private List<StopBits> stopBitsList = new List<StopBits>()
        {
            StopBits.One,
            StopBits.Two,
            StopBits.OnePointFive
        };
        private string getPort;
        private string getBaudRate;
        private int getDataBits;
        private System.IO.Ports.Parity getparityBits;
        private System.IO.Ports.StopBits getStopBits;


        public List<BarcodeDataModel> BarcodeDatas { get => barcodeDatas; set { barcodeDatas = value; OnPropertyChangd(nameof(BarcodeDatas)); } }
        public ObservableCollection<BarcodeDataModel> ListViewCollection { get => listViewCollection; set { listViewCollection = value; OnPropertyChangd(nameof(ListViewCollection)); } }
        public string[] Port { get => port; set { port = value; OnPropertyChangd(nameof(Port)); } }
        public List<string> BaudRateList { get => baudRateList; set { baudRateList = value; OnPropertyChangd(nameof(BaudRateList)); } }
        public List<int> DataBitsList { get => dataBitsList; set { dataBitsList = value; OnPropertyChangd(nameof(DataBitsList)); } }
        public List<Parity> ParityBitsList { get => parityBitsList; set { parityBitsList = value; OnPropertyChangd(nameof(ParityBitsList)); } }
        public List<StopBits> StopBitsList { get => stopBitsList; set { stopBitsList = value; OnPropertyChangd(nameof(StopBitsList)); } }
        public string GetPort { get => getPort; set { getPort = value; OnPropertyChangd(nameof(GetPort)); } }
        public string GetBaudRate { get => getBaudRate; set { getBaudRate = value; OnPropertyChangd(nameof(GetBaudRate)); } }
        public int GetDataBits { get => getDataBits; set { getDataBits = value; OnPropertyChangd(nameof(GetDataBits)); } }
        public System.IO.Ports.Parity GetparityBits { get => getparityBits; set { getparityBits = value; OnPropertyChangd(nameof(GetparityBits)); } }
        public System.IO.Ports.StopBits GetStopBits { get => getStopBits; set { getStopBits = value; OnPropertyChangd(nameof(GetStopBits)); } }


        private string productBarcodeData;
        private string receivedData;


        public string ProductBarcodeData { get => productBarcodeData; set { productBarcodeData = value; OnPropertyChangd(nameof(ProductBarcodeData)); } }
        public string ReceivedData { get => receivedData; set { OnPropertyChangd(nameof(ReceivedData)); receivedData = value; DataSearch(value); } }


        public event PropertyChangedEventHandler PropertyChanged;

        public Window mainWin;
        public Window barcodeWin;

        public MainViewModel()
        {
            serial.NewLine = "\r\n";            
            serial.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            Thread thread = new Thread(delegate () { BarcodeDatas = textControl.TxtControl(); });
            thread.Start();            
        }

        public ICommand RunBtnClick { protected get; set; }

        protected void OnPropertyChangd(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void OpenComPort(string port, int baudRate, int dataBits, System.IO.Ports.Parity parityBits, System.IO.Ports.StopBits stopBits)
        {
            serial.PortName = port;
            serial.BaudRate = baudRate;
            serial.DataBits = dataBits;
            serial.Parity = parityBits;
            serial.StopBits = stopBits;
            serial.Open();
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                ReceivedData = serial.ReadLine();
            }
            catch (TimeoutException)
            {
                ReceivedData = string.Empty;
            }
        }

        private void DataSearch(string data)
        {
            int i = 0;
            foreach (var items in BarcodeDatas)
            {
                if ((BarcodeDatas[i].BarcodeData ) == data)
                {
                    barcode.BarcodeData = BarcodeDatas[i].BarcodeData;
                    barcode.ProductName = BarcodeDatas[i].ProductName;
                    barcode.ProductAmount = BarcodeDatas[i].ProductAmount;
                    barcode.ProductCost = BarcodeDatas[i].ProductCost;
                    barcode.ProductListCost = BarcodeDatas[i].ProductListCost;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ListViewCollection.Add(barcode.Deepcopy());
                    }));
                }
                i++;
            }
        }

        private DelegateCommand cmdOpenMainWindow;

        public DelegateCommand CmdOpenMainWindow => cmdOpenMainWindow ?? (cmdOpenMainWindow = new DelegateCommand()
        {
            CanExecuteFunc = (p) =>
            {
                if (GetPort != null && getBaudRate != null)
                {
                    return true;
                }
                return false;
                //return true;
            },
            ExecuteAct = (p) =>
            {
                OpenComPort(GetPort, Convert.ToInt32(getBaudRate), GetDataBits, GetparityBits, GetStopBits);
                mainWin = new MainWindow();
                mainWin.DataContext = this;
                mainWin.Show();
                barcodeWin?.Close();
                barcodeWin = null;
            }
        });

        private DelegateCommand cmdClosemainWindow;

        public DelegateCommand CmdClosemainWindow => cmdClosemainWindow ?? (cmdClosemainWindow = new DelegateCommand()
        {
            ExecuteAct = (p) =>
            {
                serial.Close();
                barcodeWin = new BarcodeReaderSet();
                barcodeWin.DataContext = this;
                barcodeWin.Show();
                mainWin?.Close();
                mainWin = null;
            }
        });

        private DelegateCommand cmdShutDown;

        public DelegateCommand CmdShutDown => cmdShutDown ?? (cmdShutDown = new DelegateCommand()
        {
            ExecuteAct = (p) =>
            {
                App.Current.Shutdown();
            }
        });
    }
}