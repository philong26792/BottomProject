import { PackingListDetail } from './packingList-detail';

export interface IntegrationInputModel {
    rack_Location: string;
    receive_No: string;
    qrCode_ID: string;
    qrCode_Version: number;
    mO_No: string;
    mO_Seq: string;
    material_ID: string;
    material_Name: string;
    supplier_ID: string;
    supplier_Name: string;
    receive_Qty: number;
    packingListDetailItem: PackingListDetail[];
}