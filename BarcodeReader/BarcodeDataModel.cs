using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BarcodeReader
{
    class BarcodeDataModel
    {
        string barcodeData;
        string productName;
        string productAmount;
        string productCost;
        string productListCost;

        public string BarcodeData { get => barcodeData; set => barcodeData = value; }
        public string ProductName { get => productName; set => productName = value; }
        public string ProductAmount { get => productAmount; set => productAmount = value; }
        public string ProductCost { get => productCost; set => productCost = value; }
        public string ProductListCost { get => productListCost; set => productListCost = value; }
    

        public BarcodeDataModel Deepcopy()
        {
            BarcodeDataModel model = new BarcodeDataModel();
            model.BarcodeData = this.BarcodeData;
            model.ProductName = this.ProductName;
            model.ProductAmount = this.ProductAmount;
            model.ProductCost = this.ProductCost;
            model.ProductListCost = this.ProductListCost; 
            return model;
        }
    }
}
