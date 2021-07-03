export interface ModifyReasonQtyModel {
    reason_Code: string;
    reason_Name: string;
    totalModifyQty: number;
    qtyOfSizes: QtyOfSize[];
    qtyOfSizesLeft: QtyOfSize[];
    qtyOfSizesRight: QtyOfSize[];
    totalLeft: number;
    totalRight: number;
}
export interface QtyOfSize {
    tool_Size: string;
    order_Size: string;
    qty: number;
}