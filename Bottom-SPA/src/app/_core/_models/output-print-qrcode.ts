import { PackingListDetailModel } from '../_viewmodels/packing-list-detail-model';
import { QRCodeMainModel } from '../_viewmodels/qrcode-main-model';

export interface OutputPrintQrCode {
    packingListDetail: PackingListDetailModel[];
    qrCodeModel: QRCodeMainModel;
    rackLocation: string;
    print1: Array<any>; // tách ra mảng đầu có 8 phần tử để in
    print2: Array<any>; // mảng có 8 phần tử tiếp theo để in
    print3: Array<any>; // mảng có 8 phần tử tiếp theo để in
    print4: Array<any>; // mảng có 8 phần tử tiếp theo để in
    totalPQty: number;
    totalRQty: number;
}