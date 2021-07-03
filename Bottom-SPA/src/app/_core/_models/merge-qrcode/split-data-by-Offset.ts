import { SizeAndQty } from "./size-and-qty";

export interface SplitDataByOffset {
    mO_No: string;
    dMO_No: string;
    mO_Seq: string;
    material_ID: string;
    rack_Location: string;
    listSizeAndQty: SizeAndQty[];
    checked: boolean;
    sumInstockQty: number;
    plan_Start_STF: Date;
    crd: Date;
    sumMOQty: number;
    sumAlreadyOffsetQty: number;
    sumOffsetQty: number;
}