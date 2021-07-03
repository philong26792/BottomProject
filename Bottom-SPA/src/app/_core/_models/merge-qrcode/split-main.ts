export interface SplitMain {
    splitPlanNoParent: SplitDetail;
    splitPlanNoChild: SplitDetail[];
}

export interface SplitDetail {
    transac_No: string;
    transac_Type: string;
    qrCode_ID: string;
    qrCode_Version: number;
    mO_No: string;
    mO_Seq: string;
    material_ID: string;
    material_Name: string;
    rack_Location: string;
    split_Time: string | null;
    preBuy_MO_No: string;
    updated_By: string;
    stock_Qty: number | null;
}