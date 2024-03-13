/*
 * OCPP.Core - https://github.com/dallmann-consulting/OCPP.Core
 * Copyright (C) 2020-2021 dallmann consulting GmbH.
 *All Rights Reserved.
 *
 *This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OCPP.Core.Database;
using OCPP.Core.Server.Messages_OCPP16;

namespace OCPP.Core.Server
{
    public partial class ControllerOCPP16
    {

        public string HandleRemotes(OCPPMessage msgIn, OCPPMessage msgOut)
        {
            string errorCode = "NoAction";

            try
            {
                using (OCPPCoreContext dbContext = new OCPPCoreContext(Configuration))
                {
                    Console.WriteLine(ChargePointStatus.Id);

                    var lastOpt = dbContext.Operations.Where(m => m.ChargePointId == ChargePointStatus.Id && m.OpAllowed == 0).OrderBy(m => m.OperationId).LastOrDefault();
                    if (lastOpt != null)
                    {
                        if (lastOpt.OpType == 1)
                        {
                            errorCode = HandleRemoteStartTransaction(msgIn, msgOut);
                        }
                        else if (lastOpt.OpType == 2)
                        {
                            errorCode = HandleRemoteStopTransaction(msgIn, msgOut, dbContext);
                        }

                        if (string.IsNullOrEmpty(errorCode))
                        {
                            lastOpt.OpAllowed = 1;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
            }

            return errorCode;
        }

        public string HandleRemoteStartTransaction(OCPPMessage msgIn, OCPPMessage msgOut)
        {
            string errorCode = null;

            Console.WriteLine("Send remote start");

            msgOut.MessageType = "2";
            msgOut.UniqueId = Guid.NewGuid().ToString("N");
            msgOut.Action = "RemoteStartTransaction";

            Logger.LogTrace("Processing Remote transaction start...");
            RemoteStartTransactionRequest remoteTransactionRequest = new RemoteStartTransactionRequest();
            remoteTransactionRequest.IdTag = CleanChargeTagId("1", Logger);

            msgOut.JsonPayload = JsonConvert.SerializeObject(remoteTransactionRequest);
            Logger.LogTrace("RemoteTransactionStart => Response serialized");

            return errorCode;
        }

        public string HandleRemoteStopTransaction(OCPPMessage msgIn, OCPPMessage msgOut, OCPPCoreContext dbContext)
        {
            string errorCode = null;

            msgOut.MessageType = "2";
            msgOut.UniqueId = Guid.NewGuid().ToString("N");
            msgOut.Action = "RemoteStopTransaction";

            try
            {
                var lastTrans = dbContext.Transactions.Where(m => m.ChargePointId == ChargePointStatus.Id).OrderBy(m => m.TransactionId).LastOrDefault();
                if (lastTrans != null)
                {
                    Logger.LogTrace("Processing Remote transaction start...");
                    RemoteStopTransactionRequest remoteTransactionRequest = new RemoteStopTransactionRequest();
                    remoteTransactionRequest.TransactionId = lastTrans.TransactionId;

                    msgOut.JsonPayload = JsonConvert.SerializeObject(remoteTransactionRequest);
                    Logger.LogTrace("RemoteTransactionStart => Response serialized");
                }
            }
            catch (Exception ex)
            {
                errorCode = ex.Message;
            }

            return errorCode;
        }
    }
}
