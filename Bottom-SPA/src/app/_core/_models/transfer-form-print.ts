export interface TransferFormPrint {
    rack_Location: string;
    collect_Trans_No: string;
    mO_No: string;
    mO_Seq: string;
    article: string;
    plan_Qty: number;
    model_Name: string;
    model_No: string;
    subcon_ID: string;
    subcon_Name: string;
    supplier_ID: string;
    supplier_Name: string;
    t3_Supplier: string;
    t3_Supplier_Name: string;
    custmoer_Part: string;
    custmoer_Name: string;
    line_ASY: string;
    line_STF: string;
    material_ID: string;
    material_Name: string;
    transferFormQty: TransferFormPrintQty[];
}

export interface TransferFormPrintQty {
    tool_Size: string;
    order_Size: string;
    mO_Qty: number;
    trans_Qty: number;
    act_Qty: number;
    act_Trans_Qty: number;
    sumMOQty: number;
    sumTransQty: number;
}
