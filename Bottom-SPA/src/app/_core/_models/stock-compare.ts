export interface StockCompare {
    mO_No: string;
    model_No: string;
    model_Name: string;
    article: string;
    material_ID: string;
    material_Name: string;
    hP_WH: string;
    received_Qty: number;
    a_WMS_Rec_Date: Date;
    b_HP_Rec_Date: Date;
    freeze_Date: Date;
    coverage: number;
    c_WMS_Accu_Rec_Qty: number;
    d_HP_Accu_Rec_Qty: number;
    e_Balance: number;
    accuracy: number;
    supplier_ID: string;
    t2_Supplier_Name: string;
    supplier: string;
}