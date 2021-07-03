export interface MissingAgain {
    type: string;
    missing_No: string;
    mO_No: string;
    mO_Seq: string;
    material_ID: string;
    material_Name: string;
    model_No: string;
    model_Name: string;
    article: string;
    custmoer_Part: string;
    custmoer_Name: string;
    rack_Location: string;
    supplier_ID: string;
    missing_Qty: number | null;
    updated_Time: string | null;
    checked: boolean;
    download_count:number|null;
}
