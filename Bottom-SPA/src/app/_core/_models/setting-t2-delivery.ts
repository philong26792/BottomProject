export interface Setting_T2Delivery {
    id: number;
    factory_ID: string;
    t2_Supplier_ID: string;
    t2_Supplier_Name: string;
    input_Delivery: string;
    reason_Code: string;
    reason_Name: string;
    is_Valid: string;
    invalid_Date: string | null;
    updated_By: string;
    updated_Time: string | null;
}