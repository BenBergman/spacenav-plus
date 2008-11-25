/*
   HidLibrary.Net   http://sourceforge.net/projects/hidlibrary/
 
   Copyright (C)    Roman Reichel 2006
					Bauhaus University of Weimar

   This library is free software; you can redistribute it and/or
   modify it under the terms of the GNU Lesser General Public
   License as published by the Free Software Foundation; either
   version 2.1 of the License, or (at your option) any later version.

   This library is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
   Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public
   License along with this library; if not, see http://www.gnu.org/licenses .

*/

using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Windows.Forms;

namespace HidLibrary
{
    delegate void ReceivedReportDelegate(byte[] report);

    class DeviceReadThread : IDisposable
    {
        DeviceAccess dev;
        DeviceReadState state;
        private int bufferSize;
        
        private volatile bool shouldStop = false;
        private bool running = false;

        public ReceivedReportDelegate receivedReport;

        public DeviceReadThread(DeviceAccess Dev, ReceivedReportDelegate handler)
        {
            dev = Dev;
            InitializeFileStream();
            InitializeDelegates(handler);
        }

        private void InitializeDelegates(ReceivedReportDelegate handler)
        {
            receivedReport = handler;
        }

        private void InitializeFileStream()
        {

            if (dev.OpenDeviceForIoctl())
            {
                using (HidPreparsedData preparsedData = new HidPreparsedData(dev.Handle))
                {
                    NativeApi.HIDP_CAPS Capabilities = new NativeApi.HIDP_CAPS();
                    NativeApi.HidP_GetCaps(preparsedData.Handle, ref Capabilities);
                    bufferSize = Capabilities.InputReportByteLength;
                }

                if (dev.OpenDeviceForRead())
                {
                    state = new DeviceReadState(new FileStream(dev.Handle, FileAccess.Read), new byte[bufferSize]);
                }
                else
                {
                    throw new Exception("Unable to open device for Read");
                }
            }
        }

        private void ReadAsync(IAsyncResult asyncResult)
        {
            if (asyncResult != null)
            {
                state = (DeviceReadState)asyncResult.AsyncState;
                //FileStream deviceStream = state.DeviceStream;
                try
                {
                    state.DeviceStream.EndRead(asyncResult);
                }
                catch (IOException)
                {
                    state.DeviceStream.Close();
                    return;
                }
                catch (InvalidOperationException)
                {
                    // fStream.Close();
                    //if (!shouldStop) MessageBox.Show("EndRead failed...InvalidOperationEx!");
                }
            }

            if (!shouldStop)
            {
                try
                {
                    asyncResult = state.DeviceStream.BeginRead(
                    state.ReadArray, 0, state.ReadArray.Length,
                    new AsyncCallback(ReadAsync), state);
                }
                catch
                {
                    shouldStop = true;
                }

                if (asyncResult != null)
                {
                    byte[] report = state.ReadArray;//new byte[state.ReadArray.Length];
                    /*
                    for (int i = 0; i < state.ReadArray.Length; i++)
                    {
                        report[i] = state.ReadArray[i];
                    }
                    */
                    receivedReport.Invoke(report);
                }
            }
        }

        public void Start()
        {
            if (!running)
            {
                running = true;
                ReadAsync(null);
            }
        }

        public void Stop()
        {
            if (running)
            {
                running = false;
                shouldStop = true;
            }
        }

        ~DeviceReadThread()
        {
            Dispose(false);
        }

        #region IDisposable Member

        protected bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Stop();
                if (state.DeviceStream != null) state.DeviceStream.Close();

                if (disposing)
                { state.DeviceStream.Dispose(); }

                dev.CloseDevice();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private class DeviceReadState
        {
            // fStream is used to read and write to the file.
            private FileStream deviceStream;

            // readArray stores data that is read from the file.
            private byte[] readArray;

            public DeviceReadState(FileStream deviceStream, byte[] readArray)
            {
                this.deviceStream = deviceStream;
                this.readArray = readArray;
            }

            public FileStream DeviceStream
            { get { return deviceStream; } }

            public byte[] ReadArray
            { get { return readArray; } }
        }
    }
}