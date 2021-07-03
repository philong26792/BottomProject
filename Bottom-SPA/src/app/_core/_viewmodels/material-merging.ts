import { BatchQtyItem } from './batch-qty-item';
export interface MaterialMergingViewModel {
    tool_Size: string;
    order_Size: string;
    purchase_Qty: number;
    accumlated_In_Qty: number;
    delivery_Qty: number;
    delivery_Qty_Batches: number;
    purchase_Qty_Item: BatchQtyItem[]
}