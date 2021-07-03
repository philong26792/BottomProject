export interface NSP_MISSING_REPORT_DETAIL {
    plan_Start_STF: string;
    pO_Batch: string;
    model_Name: string;
    model_No: string;
    article: string;
    part_Name: string;
    material_ID: string;
    material_Name: string;
    unit: string;
    plan_Qty: number | null;
    accumulated: number | null;
    tool_Size: string;
    missing_Qty: number | null;
    missing_Reason: string;
    t2_Supplier: string;
    t3_Supplier: string;
    download_count:number;
}