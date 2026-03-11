using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForSessionValue
{
    public class SessionValue
    {
        HttpContext hp = HttpContext.Current;

        public string DOID
        {
            get { return (string)hp.Session["DOID"]; }
            set
            {
                hp.Session["DOID"] = value;
            }
        }

        public string DONo
        {
            get { return (string)hp.Session["DONo"]; }
            set
            {
                hp.Session["DONo"] = value;
            }
        }

        public string CustName
        {
            get { return (string)hp.Session["CustName"]; }
            set
            {
                hp.Session["CustName"] = value;
            }
        }

        public string InvoiceNo
        {
            get { return (string)hp.Session["InvoiceNo"]; }
            set
            {
                hp.Session["InvoiceNo"] = value;
            }
        }

        public string InvoiceID
        {
            get { return (string)hp.Session["InvID"]; }
            set
            {
                hp.Session["InvID"] = value;
            }
        }

        public string ReceivingID
        {
            get { return (string)hp.Session["ReceivingID"]; }
            set
            {
                hp.Session["ReceivingID"] = value;
            }
        }

        public string DeliveryANDCollection_DONo
        {
            get { return (string)hp.Session["DeliveryANDCollection_DONo"]; }
            set
            {
                hp.Session["DeliveryANDCollection_DONo"] = value;
            }
        }

        public string DeliveryANDCollection_Customer
        {
            get { return (string)hp.Session["DeliveryANDCollection_Customer"]; }
            set
            {
                hp.Session["DeliveryANDCollection_Customer"] = value;
            }
        }

        public string DeliveryANDCollection_VehicleNo
        {
            get { return (string)hp.Session["DeliveryANDCollection_VehicleNo"]; }
            set
            {
                hp.Session["DeliveryANDCollection_VehicleNo"] = value;
            }
        }

        public string DeliveryANDCollection_Status
        {
            get { return (string)hp.Session["DeliveryANDCollection_Status"]; }
            set
            {
                hp.Session["DeliveryANDCollection_Status"] = value;
            }
        }

        public string DeliveryANDCollection_Date
        {
            get { return (string)hp.Session["DeliveryANDCollection_Date"]; }
            set
            {
                hp.Session["DeliveryANDCollection_Date"] = value;
            }
        }

        public string Receiving_RCVNo
        {
            get { return (string)hp.Session["Receiving_RCVNo"]; }
            set
            {
                hp.Session["Receiving_RCVNo"] = value;
            }
        }

        public string Receiving_Customer
        {
            get { return (string)hp.Session["Receiving_Customer"]; }
            set
            {
                hp.Session["Receiving_Customer"] = value;
            }
        }

        public string Receiving_Status
        {
            get { return (string)hp.Session["Receiving_Status"]; }
            set
            {
                hp.Session["Receiving_Status"] = value;
            }
        }

        public string Receiving_Date
        {
            get { return (string)hp.Session["Receiving_Date"]; }
            set
            {
                hp.Session["Receiving_Date"] = value;
            }
        }

        public string Refilling_RFLNo
        {
            get { return (string)hp.Session["Refilling_RFLNo"]; }
            set
            {
                hp.Session["Refilling_RFLNo"] = value;
            }
        }

        public string Refilling_Customer
        {
            get { return (string)hp.Session["Refilling_Customer"]; }
            set
            {
                hp.Session["Refilling_Customer"] = value;
            }
        }

        public string Refilling_Status
        {
            get { return (string)hp.Session["Refilling_Status"]; }
            set
            {
                hp.Session["Refilling_Status"] = value;
            }
        }

        public string Refilling_Date
        {
            get { return (string)hp.Session["Refilling_Date"]; }
            set
            {
                hp.Session["Refilling_Date"] = value;
            }
        }

        public string Inspection_LotNo
        {
            get { return (string)hp.Session["Inspection_LotNo"]; }
            set
            {
                hp.Session["Inspection_LotNo"] = value;
            }
        }

        public string Inspection_GasType
        {
            get { return (string)hp.Session["Inspection_GasType"]; }
            set
            {
                hp.Session["Inspection_GasType"] = value;
            }
        }

        public string Inspection_Date
        {
            get { return (string)hp.Session["Inspection_Date"]; }
            set
            {
                hp.Session["Inspection_Date"] = value;
            }
        }

        public string BookingSummary_Status
        {
            get { return (string)hp.Session["BookingSummary_Status"]; }
            set
            {
                hp.Session["BookingSummary_Status"] = value;
            }
        }
        public string BookingSummary_BookingNo
        {
            get { return (string)hp.Session["BookingSummary_BookingNo"]; }
            set
            {
                hp.Session["BookingSummary_BookingNo"] = value;
            }
        }
        public string BookingSummary_BookingParty
        {
            get { return (string)hp.Session["BookingSummary_BookingParty"]; }
            set
            {
                hp.Session["BookingSummary_BookingParty"] = value;
            }
        }
        public string BookingSummary_POD
        {
            get { return (string)hp.Session["BookingSummary_POD"]; }
            set
            {
                hp.Session["BookingSummary_POD"] = value;
            }
        }
        public string BookingSummary_Vessel
        {
            get { return (string)hp.Session["BookingSummary_Vessel"]; }
            set
            {
                hp.Session["BookingSummary_Vessel"] = value;
            }
        }

        public string BookingSummary_Voyage
        {
            get { return (string)hp.Session["BookingSummary_Voyage"]; }
            set
            {
                hp.Session["BookingSummary_Voyage"] = value;
            }
        }

        public string EquipmentList_Equipment
        {
            get { return (string)hp.Session["EquipmentList_Equipment"]; }
            set
            {
                hp.Session["EquipmentList_Equipment"] = value;
            }
        }

        public string EquipmentList_Type
        {
            get { return (string)hp.Session["EquipmentList_Type"]; }
            set
            {
                hp.Session["EquipmentList_Type"] = value;
            }
        }


        public string TransportSubmittedLeave_FromDate
        {
            get { return (string)hp.Session["TransportSubmittedLeave_FromDate"]; }
            set
            {
                hp.Session["TransportSubmittedLeave_FromDate"] = value;
            }
        }

        public string TransportSubmittedLeave_Todate
        {
            get { return (string)hp.Session["TransportSubmittedLeave_Todate"]; }
            set
            {
                hp.Session["TransportSubmittedLeave_Todate"] = value;
            }
        }

        public string TransportSubmittedLeave_Project
        {
            get { return (string)hp.Session["TransportSubmittedLeave_Project"]; }
            set
            {
                hp.Session["TransportSubmittedLeave_Project"] = value;
            }
        }

        public string TransactionHistory_FromDate
        {
            get { return (string)hp.Session["TransactionHistory_FromDate"]; }
            set
            {
                hp.Session["TransactionHistory_FromDate"] = value;
            }
        }

        public string TransactionHistory_ToDate
        {
            get { return (string)hp.Session["TransactionHistory_ToDate"]; }
            set
            {
                hp.Session["TransactionHistory_ToDate"] = value;
            }
        }

        public string TransactionHistory_VehicleNo
        {
            get { return (string)hp.Session["TransactionHistory_VehicleNo"]; }
            set
            {
                hp.Session["TransactionHistory_VehicleNo"] = value;
            }
        }

        public string TransactionHistory_Project
        {
            get { return (string)hp.Session["TransactionHistory_Project"]; }
            set
            {
                hp.Session["TransactionHistory_Project"] = value;
            }
        }

        public string DriverApproval_Date
        {
            get { return (string)hp.Session["DriverApproval_Date"]; }
            set
            {
                hp.Session["DriverApproval_Date"] = value;
            }
        }
    }
}