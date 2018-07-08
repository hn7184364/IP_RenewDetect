using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IP_RenewDetect
{
    public partial class IP_RenewDetect : ServiceBase
    {
        string currentIP = "";

        public IP_RenewDetect()
        {
            InitializeComponent();
            
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("IP_RenewDetect"))
            {
                EventLog.CreateEventSource(
                    "IP_RenewDetect",null);
            }
            eventLog1.Source = "IP_RenewDetect";
            //eventLog1.Log = "IP_Log";
        }

        protected override void OnStart(string[] args)
        {
            // Set up a timer to trigger every minute.
            Timer timer1 = new Timer();
            timer1.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer1.Interval = 60000; // 60 seconds  
            timer1.Start();
        }

        private int eventId = 1;

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            string sURL;
            sURL = "http://ifconfig.io/ip/";

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);
            String ResIP = objReader.ReadLine();
            // Console.WriteLine("***" + ResIP + "***");
            if (!ResIP.Equals(this.currentIP))
            {
                // Console.WriteLine("IP updated");
                currentIP = ResIP;
                eventLog1.WriteEntry("Current IP is "+currentIP, EventLogEntryType.Information, eventId++);
            }
            
        }

        protected override void OnStop()
        {
            timer1.Stop();
            eventLog1.WriteEntry("Stop IP_RenewDetect service");
        }
    }
}
