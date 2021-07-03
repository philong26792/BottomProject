export interface QRCodeMainModel {
    qrCode_ID: string;
    mO_No: string;
    qrCode_Version: number;
    receive_No: string;
    receive_Date: Date;
    t3_Supplier: string;
    t3_Supplier_Name: string;
    supplier_ID: string;
    supplier_Name: string;
    subcon_ID: string;
    subcon_Name: string;
    model_No: string;
    model_Name: string;
    article: string;
    mO_Seq: string;
    material_ID: string;
    material_Name: string;
    stockfiting_Date: Date;
    assembly_Date: Date;
    crd: Date;
    line_ASY: string;
    custmoer_Part: string;
    custmoer_Name: string;
    checkInput: boolean;
    qrCode_Type: string;
}
