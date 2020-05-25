using GPSMap.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GPSMap.Parser
{
    public class NMFParser
    {
        public string filePath { get; set; }

        public MapAttributesModel ParseAttributes()
        {
            var returnValue = new MapAttributesModel();
            string[] Alllines = File.ReadAllLines(this.filePath);
            foreach (string line in Alllines)
            {
                string[] lineParts = line.Split(',');
                string lineHeader = lineParts[0];

                if (lineHeader == "#EI")
                {
                    // Entry 0 = #EI -> entry 3 = IMEI
                    returnValue.IMEI = lineParts[3];
                }

                if (lineHeader == "#SI") 
                {
                    // Entry 0 = #SI -> entry 3 = IMSI
                    returnValue.IMSI = lineParts[3];
                }

                if (lineHeader == "#DN")
                {
                    // Entry 0 = #DN -> entry 3 = device name
                    returnValue.Ue_Name = lineParts[3];
                }

                if (lineHeader == "#DL")
                {
                    // Entry 0 = #DL -> entry 3 = device label
                    returnValue.Ue_Label = lineParts[3];
                }

                if (!string.IsNullOrEmpty(returnValue.IMEI)  && !string.IsNullOrEmpty(returnValue.IMSI) && !string.IsNullOrEmpty(returnValue.Ue_Name) 
                    && !string.IsNullOrEmpty(returnValue.Ue_Label)) {
                    break;
                }
            }

            return returnValue;
        }
    }
}