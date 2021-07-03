import { SizeAndQty } from "./size-and-qty";

export interface QrCodeSplitDetail {
    mO_No: string;
    model_No: string;
    model_Name: string;
    article: string;
    material_ID: string;
    material_Name: string;
    listSizeAndQty: SizeAndQty[];
}
