import { MergeQrCodeModel } from "../../_viewmodels/merge-qrcode/merge-qrcode-model";
import { SizeAndQty } from "./size-and-qty";

export interface SplitProcess {
    transacMainMergeQrCode: MergeQrCodeModel;
    listSizeAndQty: SizeAndQty[];
    listOffsetNo: string[];
    sumInstockQty: number;
    sumAccQty: number;
    sumRemainingInstockQty: number;
}