export interface ReasonDetail {
    order_size: string;
    tool_size: string;
    reason_code: string;
    left: number;
    right: number;
}
export interface BatchDetail {
    batch: string;
    left: number;
    right: number;
}
export interface LeftRightInBatchOfReason {
    batch: string;
    reason: string;
    order_size: string;
    tool_size: string;
    left: number;
    right: number;
}