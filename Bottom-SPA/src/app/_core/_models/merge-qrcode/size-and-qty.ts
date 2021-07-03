export interface SizeAndQty {
    tool_Size: string;
    order_Size: string;
    model_Size: string;
    instock_Qty: number | null;
    trans_Qty: number | null;
    act_Out_Qty: number | null;
    mO_Qty: number | null;
    purchase_Qty: number | null;
    instock_QtyDb: number | null;
    already_Offset_Qty: number | null;
    offset_Qty: number | null;
}