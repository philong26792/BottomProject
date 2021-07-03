export class SettingT2Delivery {
    id: number;
    factory_ID: string;
    t2_Supplier_ID: string;
    t2_Supplier_Name: string;
    input_Delivery: string;
    reasons: ReasonInfo[];
    is_Valid: string;

}

export interface SettingT2Param {
    factory_id: string;
    reason_code: string;
    supplier_id: string;
    input_delivery: string;
}

export class ReasonInfo {
    reason_Code: string;
    reason_Name: string;
}