using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm
{
    public class HajirCrmConstants
    {
        public static int[,] CabinetRowCapacityTable = {
            {07,16,12 },
            {09,16,12 },
            {12,10,08 },
            {18,12,10 },
            {28,06,04 },
            {40,06,04 },
            {65,03,02 },
            {100,03,02}
        };
        public const string RahsamSolutionPerfix = "rhs_";
        public const string DefaultLegacyCrmConnectionString = "Url=http://192.168.20.15:5555/hajircrm;UserName=CRMIMPU01;Password=%H@ZH!r_1402&$;Domain=hsco";
    }
}
