
export class SettingReason {
    kind: number = 0;
    reason_Code: string = "";
    kind_Name: string = "";
    hP_Reason_Code: string = "";
    reason_Cname: string = "";
    reason_Ename: string = "";
    reason_Lname: string = "";
    trans_toHP: string = "";
    is_Shortage: string = "";
    updated_Time: Date;
    updated_By: string = "";
}
export interface ReasonOfQty {
    reason: string;
    qty: number;
}