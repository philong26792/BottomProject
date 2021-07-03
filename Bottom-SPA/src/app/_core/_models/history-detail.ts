import { TransactionMain } from './transaction-main';
import { TransferDetail } from './transfer-detail';

export interface HistotyDetail {
    transactionMain: TransactionMain;
    listTransactionDetail: TransferDetail[];
}