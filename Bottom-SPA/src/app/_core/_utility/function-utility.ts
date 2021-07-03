import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../environments/environment";
import {
  BatchDetail,
  LeftRightInBatchOfReason,
  ReasonDetail,
} from "../_viewmodels/modify-store/reason-detail";

@Injectable({
  providedIn: "root",
})
export class FunctionUtility {
  baseUrl = environment.apiUrl;
  /**
   *Hàm tiện ích
   */

  constructor(private http: HttpClient) {}

  /**
   *Trả ra ngày hiện tại, chỉ lấy năm tháng ngày: yyyy/MM/dd
   */
  getToDay() {
    const toDay =
      new Date().getFullYear().toString() +
      "/" +
      (new Date().getMonth() + 1).toString() +
      "/" +
      new Date().getDate().toString();
    return toDay;
  }

  /**
   *Trả ra ngày với tham số truyền vào là ngày muốn format, chỉ lấy năm tháng ngày: yyyy/MM/dd
   */
  getDateFormat(day: Date) {
    const dateFormat =
      day.getFullYear().toString() +
      "/" +
      (day.getMonth() + 1).toString() +
      "/" +
      day.getDate().toString();
    return dateFormat;
  }

  /**
   *Trả ra transferNo mới theo yêu cầu: TB(ngày thực hiện yyyymmdd) 3 mã số random number. (VD: TB20200310001)
   */
  async getTransferNo() {
    let transferNo;
    do {
      transferNo =
        "TB" +
        new Date().getFullYear().toString() +
        (new Date().getMonth() + 1 > 9
          ? (new Date().getMonth() + 1).toString()
          : "0" + (new Date().getMonth() + 1).toString()) +
        (new Date().getDate() > 9
          ? new Date().getDate().toString()
          : "0" + new Date().getDate().toString()) +
        Math.floor(Math.random() * (999 - 100 + 1) + 100);
    } while (await this.checkTransacNoDuplicate(transferNo));
    return transferNo;
  }

  /**
   *Trả ra outputSheetNo mới theo yêu cầu: BO+(Plan No)+ 3 số random (001,002,003…). VD: BO0124696503001
   */
  async getOutSheetNo(planNo: string) {
    let outputSheetNo;
    do {
      outputSheetNo =
        "BO" + planNo + Math.floor(Math.random() * (999 - 100 + 1) + 100);
    } while (await this.checkTransacNoDuplicate(outputSheetNo));
    return outputSheetNo;
  }

  /**
   *Trả ra inputNo mới theo yêu cầu: BI+(Plan No)+ 3 số random (001,002,003…). VD: BI0124696503001
   */
  async getInputNo(planNo: string, arr: string[]) {
    let inputNo;
    debugger;
    do {
      inputNo =
        "BI" + planNo + Math.floor(Math.random() * (999 - 100 + 1) + 100);
    } while (
      (await this.checkTransacNoDuplicate(inputNo)) ||
      arr.findIndex((x) => x === inputNo) >= 0
    );
    return inputNo;
  }

  async checkTransacNoDuplicate(transacNo: string) {
    const result = await this.http
      .get<boolean>(this.baseUrl + "TransferLocation/CheckTransacNoDuplicate", {
        params: { transacNo: transacNo },
      })
      .toPromise();
    return result;
  }

  /**
   * Nhập vào kiểu chuỗi hoặc số dạng 123456789 sẽ trả về 123,456,789
   */
  convertNumber(amount) {
    return String(amount).replace(
      /(?<!\..*)(\d)(?=(?:\d{3})+(?:\.|$))/g,
      "$1,"
    );
  }

  /**
   * Check 1 string có phải empty hoặc null hoặc undefined ko.
   */
  checkEmpty(str: string) {
    return !str || /^\s*$/.test(str);
  }

  checkQrCodeOfLength(str: string) {
    if (str.length < 14) {
      return false;
    } else if (str.length === 14) {
      let arrayStr = str.split("");
      if (arrayStr[0] === "B") {
        return true;
      } else {
        return false;
      }
    } else if (str.length >= 15) {
      return true;
    }
  }

  getLeftRight(reasons: ReasonDetail[], batchs: BatchDetail[]): LeftRightInBatchOfReason[] {
    // reasons = [
    //   { reason_code: "124", order_size:'05',tool_size:'05', left: 10, right: 50 },
    //   { reason_code: "647", order_size:'06',tool_size:'06', left: 170, right: 150 },
    // ];
    // batchs = [
    //   { batch: "-3", left: 20, right: 20 },
    //   { batch: "-2", left: 160, right: 160 },
    //   { batch: "-1", left: 180, right: 180 },
    // ];
    // ----------------------------------------------------------
    // Chỉ dừng trong vòng for reason khi reason L và R đã hết,không thể phân chia đc nữa
    // Chỉ dừng trong vòng for batch khi reason L và R đã hết,không thể phân chia đc nữa
    let reasult: LeftRightInBatchOfReason[] = [];
    for (let x = 0; x < batchs.length; x++) {
      for (let y = 0; y < reasons.length; y++) {
        if (reasons[y].left != 0 || reasons[y].right != 0) {
          if (batchs[x].left == reasons[y].left) {
            let left = batchs[x].left;
            batchs[x].left = 0; // không thể nhận đc nữa
            reasons[y].left = 0; // Ko thể chia được nữa
            if (batchs[x].right >= reasons[y].right) {
              let right = reasons[y].right;
              reasons[y].right = 0; // không thể chia đc nữa
              // Nếu nó là reason cuối cùng mà có qty < batch thì khi chia hết qty còn 0 => ko thể chia đc nữa
              if (y == reasons.length - 1) {
                batchs[x].right = 0; // không thể nhận đc nữa
              } else {
                batchs[x].right = batchs[x].right - right;
              }
              let item: LeftRightInBatchOfReason = {
                    batch: batchs[x].batch,
                    reason: reasons[y].reason_code,
                    order_size: reasons[y].order_size,
                    tool_size: reasons[y].tool_size,
                    left: left,
                    right: right,
              };
              reasult.push(item);
            } else {
              let right = batchs[x].right;
              batchs[x].right = 0;
              reasons[y].right = reasons[y].right - right;
              let item: LeftRightInBatchOfReason = {
                    batch: batchs[x].batch,
                    reason: reasons[y].reason_code,
                    order_size: reasons[y].order_size,
                    tool_size: reasons[y].tool_size,
                    left: left,
                    right: right,
              };
              reasult.push(item);
            }
          } else if (batchs[x].left < reasons[y].left) {
            let left = batchs[x].left;
            reasons[y].left = reasons[y].left - left;
            batchs[x].left = 0;
            if (batchs[x].right >= reasons[y].right) {
              let right = reasons[y].right;
              reasons[y].right = 0; // không thể chia đc nữa
              // Nếu nó là reason cuối cùng mà có qty < batch thì khi chia hết qty còn 0 => ko thể chia đc nữa
              if (y == reasons.length - 1) {
                batchs[x].right = 0; // không thể nhận đc nữa
              } else {
                batchs[x].right = batchs[x].right - right;
              }
              let item: LeftRightInBatchOfReason = {
                      batch: batchs[x].batch,
                      reason: reasons[y].reason_code,
                      order_size: reasons[y].order_size,
                      tool_size: reasons[y].tool_size,
                      left: left,
                      right: right,
              };
              reasult.push(item);
            } else {
              let right = batchs[x].right;
              batchs[x].right = 0;
              reasons[y].right = reasons[y].right - right;
              let item: LeftRightInBatchOfReason = {
                      batch: batchs[x].batch,
                      reason: reasons[y].reason_code,
                      order_size: reasons[y].order_size,
                      tool_size: reasons[y].tool_size,
                      left: left,
                      right: right,
              };
              reasult.push(item);
            }
          } else if (batchs[x].left > reasons[y].left) {
            let left = reasons[y].left;
            reasons[y].left = 0;
            if (y == reasons.length - 1 && x == batchs.length - 1) {
              batchs[x].left = 0;
            } else {
              batchs[x].left = batchs[x].left - left; // Lượng có thể nhận được nữa
            }
            if (batchs[x].right >= reasons[y].right) {
              let right = reasons[y].right;
              reasons[y].right = 0; // không thể chia đc nữa
              // Nếu nó là reason cuối cùng mà có qty < batch thì khi chia hết qty còn 0 => ko thể chia đc nữa
              if (y == reasons.length - 1) {
                batchs[x].right = 0; // không thể nhận đc nữa
              } else {
                batchs[x].right = batchs[x].right - right;
              }
              let item: LeftRightInBatchOfReason = {
                    batch: batchs[x].batch,
                    reason: reasons[y].reason_code,
                    order_size: reasons[y].order_size,
                    tool_size: reasons[y].tool_size,
                    left: left,
                    right: right,
              };
              reasult.push(item);
            } else {
              let right = batchs[x].right;
              batchs[x].right = 0;
              reasons[y].right = reasons[y].right - right;
              let item: LeftRightInBatchOfReason = {
                    batch: batchs[x].batch,
                    reason: reasons[y].reason_code,
                    order_size: reasons[y].order_size,
                    tool_size: reasons[y].tool_size,
                    left: left,right: right};
              reasult.push(item);
            }
          }
        }
      }
    }
    reasult = reasult.filter(x => !(x.left == 0 && x.right == 0));
    return reasult;
  }
}
