using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Database
{
    public class Operation
    {
        public int OperationId { get; set; }
        public int OpType { get; set; }
        public short OpAllowed { get; set; }
        public string ChargePointId { get; set; }
    }
}
