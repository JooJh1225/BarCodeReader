using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace BarcodeReader
{
    internal class TextControl
    {
        public List<BarcodeDataModel> TxtControl()
        {
            List<BarcodeDataModel> masterCollection = new List<BarcodeDataModel>();
            BarcodeDataModel barcodeDataModel = new BarcodeDataModel();
            string path = AppDomain.CurrentDomain.BaseDirectory + @"마스터파일UTF8.csv";
            StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8);
            while (sr.Peek() != -1)
            {
                string[] modelData = sr.ReadLine().Split(',');
                barcodeDataModel.BarcodeData = modelData[0];
                barcodeDataModel.ProductName = modelData[1];
                barcodeDataModel.ProductAmount = modelData[2];
                barcodeDataModel.ProductListCost = modelData[3];
                barcodeDataModel.ProductCost = modelData[4];

                masterCollection.Add(barcodeDataModel.Deepcopy());
            }

            return masterCollection;
        }
    }
}