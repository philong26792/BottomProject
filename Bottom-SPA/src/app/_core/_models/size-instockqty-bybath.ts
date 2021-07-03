export interface SizeInstockQtyByBatch {
    mO_Seq: string;
    dataDetail: SizeInstockQty[];
    total: number;
}

export interface SizeInStockPlanQty {
    tool_Size: string;
    order_Size: string;
    totalInstockQty: number;
    planQty: number;
    disableInput: boolean;
}
export interface SizeInstockQty {
    tool_Size: string;
    order_Size: string;
    totalInstockQty: number;
    planQty: number;
    justReceivedQty: number;
    modifyQty: number;
    isChange: boolean;
    disableInput: boolean;
}