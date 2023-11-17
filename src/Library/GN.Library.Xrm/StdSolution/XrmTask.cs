using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm.StdSolution
{
    [EntityLogicalNameAttribute(Schema.LogicalName)]
    public class XrmTask : XrmActivity<XrmTask, XrmTask.Schema.StateCodes, XrmTask.Schema.StatusCodes>
    {
        public new class Schema : XrmActivity<XrmTask, DefaultStateCodes, DefaultStatusCodes>.Schema
        {
            public const string LogicalName = "task";

            /// <summary>
            ///  
            /// see also: https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/task?view=op-9-1#BKMK_StateCode
            /// </summary>
            public enum StateCodes
            {
                Open = 0,
                Completed = 1,
                Canceled = 2

            }
            /// <summary>
            /// Open: NotStarted, InProgress, WaitingOnSomeoneElse, Defered
            /// Completed : Completed
            /// Canceled: Canceled
            /// see also: https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/entities/task?view=op-9-1#BKMK_StateCode
            /// </summary>
            public enum StatusCodes
            {
                /// <summary>
                /// NotsAtsrted=1 (State=Open)
                /// </summary>
                NotStarted = 2,
                /// <summary>
                /// InProgress=3 (Valid only for State=Open)
                /// </summary>
                InProgress = 3,
                // Completed
                /// <summary>
                /// WaitingOnSomeOneElse=4 (State=Open)
                /// </summary>
                WaitingOnSomeoneElse = 4,

                /// <summary>
                /// Completed=5 (State=Completed)
                /// </summary>
                Completed = 5,
                /// <summary>
                /// Canceled=6 (State=Cance4led)
                /// </summary>
                Canceled = 6,
                /// <summary>
                /// Defered =7 (State=Open)
                /// </summary>
                Defered = 7,

               
            }
        }
        public XrmTask() : base(Schema.LogicalName)
        {
        }
    }
    public static class XrmTaskExtentions
    {
        public static void ChangeStatus(this XrmTask THIS , XrmTask.Schema.StatusCodes statusCode)
        {
            var stateCode = XrmTask.Schema.StateCodes.Open;
            switch (statusCode)
            {
                case XrmTask.Schema.StatusCodes.Completed:
                    stateCode = XrmTask.Schema.StateCodes.Completed;
                    break;
                case XrmTask.Schema.StatusCodes.Canceled:
                    stateCode = XrmTask.Schema.StateCodes.Canceled;
                    break;
                default:
                    stateCode = XrmTask.Schema.StateCodes.Open;
                    break;

            }
            THIS.Services.GetRepository().SetState(THIS, (int)stateCode, (int)statusCode);
        }
    }
}
