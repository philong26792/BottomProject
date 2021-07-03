import { OutputM } from './outputM';
export class Output {
    outputs: OutputM[];
    outputTotalNeedQty: OutputTotalNeedQty[];
    message: string;
}

export interface OutputTotalNeedQty {
    order_Size: number;
    model_Size: number;
    tool_Size: number;
    qty: number;
}
