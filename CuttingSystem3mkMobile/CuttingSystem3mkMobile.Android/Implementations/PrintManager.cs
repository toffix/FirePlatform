﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using CuttingSystem3mkMobile.Models;
using CuttingSystem3mkMobile.Services;

namespace CuttingSystem3mkMobile.Droid.Implementations
{
    public class PrintManager : IPrintManager
    {
        protected static string ACTION_USB_PERMISSION = "it.mobi-ware.android.USB";
        private readonly Context _context;
        public PrintManager()
        {
            _context = Android.App.Application.Context;
        }
        public void ConnectAndSend(byte[] bytesToPrint, int productId, int vendorId)
        {
            if (_context.GetSystemService(Context.UsbService) is UsbManager usbManager)
            {
                var matchingDevice = usbManager.DeviceList.FirstOrDefault(item => item.Value.VendorId == vendorId);
                if (usbManager.DeviceList.Count == 0)
                    throw new Exception("usbManager.DeviceList.Count == 0");
                if (matchingDevice.Value == null)
                    throw new Exception("matchingDevice.Value == null");
                var usbPort = matchingDevice.Key;
                var usbDevice = matchingDevice.Value;

                if (!usbManager.HasPermission(usbDevice))
                {
                    try
                    {
                        PendingIntent pi = PendingIntent.GetBroadcast(_context, 0, new Intent(ACTION_USB_PERMISSION), 0);
                        usbManager.RequestPermission(usbDevice, pi);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                UsbDeviceConnection deviceConnection = null;
                try
                {
                    deviceConnection = usbManager.OpenDevice(usbDevice);
                    using (var usbInterface = usbDevice.GetInterface(0))
                    {
                        using (var usbEndpoint = usbInterface.GetEndpoint(0))
                        {
                            byte[] encodingSetting =
                                new byte[] { (byte)0x80, 0x25, 0x00, 0x00, 0x00, 0x00, 0x08 };
                            deviceConnection.ControlTransfer(
                                UsbAddressing.Out,
                                0x20,   //SET_LINE_CODING
                                0,      //value
                                0,      //index
                                encodingSetting,  //buffer
                                7,      //length
                                0);     //timeout
                            deviceConnection.BulkTransfer(usbEndpoint, bytesToPrint, bytesToPrint.Length, 0);

                        }
                    }
                }
                catch (Exception ex)
                {
                    // log or handle TODO need to change
                }
                finally
                {

                    // Close the connection
                    if (deviceConnection != null)
                    {
                        deviceConnection.Close();
                    }
                }
            }
        }

        public DeviceInfo[] Devices()
        {
            var devices = new List<DeviceInfo>();
            if (_context.GetSystemService(Context.UsbService) is UsbManager usbManager)
            {
                try
                {
                    if (usbManager.DeviceList.Count != 0)
                    {
                        usbManager.DeviceList.Values.ToList().ForEach(x => devices.Add(new DeviceInfo()
                        {
                            ProductId = x.ProductId,
                            VendorId = x.VendorId
                        }));
                    }
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.Message); TODO need to change
                }
            }
            return devices.ToArray();
        }
    }
}
