import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public services: ServicesStatus;
  public telegramServicesWorkingText: string;
  public autoRunnerText: string;
  public webServiceConnectionText: string;
  public httpClient: HttpClient;
  public baseUrl: string;
  public numberToCheck: string;
  public numbersToCheck: string;
  public checkedPhone: ILead[];
  public resultTableCreated: boolean;
  public rowLast: number;
  public downloadFile: boolean;
  public savePath: string;
  public resultToFile: string;

  public progress: number;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this.httpClient = http;
    this.progress = 0;
    this.baseUrl = baseUrl;
    this.resultTableCreated = false;
    this.downloadFile = false;
    this.checkedPhone = [
      { id: 0, phone: "", isTelegram: false, telegramUser: "", isViber: false, isWhatsApp: false }
    ];
    this.rowLast = 10000;
    http.get<ServicesStatus>(baseUrl + 'Connection')
      .subscribe((result: ServicesStatus) => {
        var jsonResult = JSON.parse(JSON.stringify(result));
        this.autoRunnerText = jsonResult.autoRunner ? "Running" : "Offline";
        this.webServiceConnectionText = jsonResult.webServiceConnection ? "Web Service Online" : "Offline";
        this.telegramServicesWorkingText = jsonResult.telegramServicesWorking ? "Online" : "Offline";
      }, error => console.error(error));
  }

  public onKey(event) {
    if (event.target.id === "phone") {
      if (event.keyCode === 13) {
        this.checkNumber();
      } else {
        this.numberToCheck = event.target.value;
        return;
      }
    }

    if (event.target.id === "phones") {
      if (event.keyCode === 13 && event.ctrlKey) {
        this.checkNumbers();
        return;
      }
      this.numbersToCheck = event.target.value.replace('+', '').replace('-', '').replace('(', '').replace(')', '').replace(' ', '').replace(',', '').replace('.', '').replace(';', '').replace('|', '');
      event.target.value = this.numbersToCheck;
    }
  }

  public saveFile() {
    if (this.downloadFile && this.savePath !== null && this.savePath.length > 0) {
      //a.click();
      this.downloadFile = false;
      this.savePath = "";


      var exportedFilenmae = 'result.csv';
      if (this.resultToFile !== null && this.resultToFile.length > 0) {

        var blob = new Blob([this.resultToFile], { type: 'text/csv;charset=utf-8;' });
        if (navigator.msSaveBlob) { // IE 10+
          navigator.msSaveBlob(blob, exportedFilenmae);
        } else {
          var link = document.createElement("a");
          if (link.download !== undefined) { // feature detection
            // Browsers that support HTML5 download attribute
            var url = URL.createObjectURL(blob);

            this.savePath = url;
            link.setAttribute("href", url);
            link.setAttribute("download", exportedFilenmae);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
          }
        }
      }
    }
  }

  public checkNumbers() {
    if (this.numbersToCheck != null && this.numbersToCheck.length > 0) {
      var initLength = this.numbersToCheck.length;
      var numbersText = this.numbersToCheck.replace("/(\r\n|\n|\r)/gm", "");
      numbersText = numbersText.replace("\r", "").replace("\r\n", "").replace("\n", "");
      //var newCount = numbersText.length;
      if (numbersText.length % 2 !== 0) {
        //alert("Неправильный формат телефонов. Укажите телефоны в формате: 380935505050");
      }
      var request = "[";
      for (var i = 0; i < numbersText.length; i += 13) {
        var number = this.numbersToCheck.slice(i, i + 12);
        request += " {\"id\": " + i.toString() + ",\"phone\": \"" + number + "\"},";
      }
      request = request.substring(0, request.length - 1);
      request += "]";
      this.move();
      this.httpClient.post(this.baseUrl + 'Check', request, { headers: { 'Content-Type': 'application/json' } }).subscribe(
        response => {
          var result = JSON.parse(JSON.stringify(response));
          if (result != null) {
            var resultCode = response['resultCode'];
            var isException = response['isException'];
            var messageText = response['messageText'];
            if (resultCode !== null && (resultCode === 401 || resultCode === 402 || resultCode === 403)) {
              alert("Служба TelegramService отключена, пожалуйста проверьте настройки и авторизируйтесь");
              window.location.href = document.baseURI + '/settingspage';
            }

            if (result != null && result.length > 0) {
              if (document.getElementById('table') === null) {
                var table = document.createElement('table');
                table.className = 'table table-striped';
                table.id = 'table';
                table.style.color = 'white;';

                var header = document.createElement("tr");
                var PhoneNumber = document.createElement("th");
                PhoneNumber.innerHTML = 'PhoneNumber';

                var Telegram = document.createElement("th");

                Telegram.innerHTML = 'Telegram ';

                var Viber = document.createElement("th");
                Viber.innerHTML = 'Viber';

                var WhatsApp = document.createElement("th");
                WhatsApp.innerHTML = 'WhatsApp';

                header.appendChild(PhoneNumber);
                header.appendChild(Telegram);
                header.appendChild(Viber);
                header.appendChild(WhatsApp);
                table.appendChild(header);
                this.resultTableCreated = true;
                var div = document.createElement('div');
                div.className = 'token';
                div.appendChild(table);
                document.getElementById('tableplace').appendChild(div);
              }



              for (var i = 0; i < result.length; i++) {
                var tr = document.createElement("tr");
                tr.id = "0";
                //var id = document.createElement("td");
                var phone = document.createElement("td");
                var isTelegram = document.createElement("td");
                var isViber = document.createElement("td");
                var isWhatsApp = document.createElement("td");

                // id.innerHTML = result[0].id;
                phone.innerHTML = result[i].phone;
                isTelegram.innerHTML = result[i].isTelegram ?
                  'Есть <a href="tg://resolve?domain=' + result[i].telegramUser + ' "> <img src="assets/img/telegram.png" width="30px" height="30px"></a>' : 'Отсутствует';
                isViber.innerHTML = result[i].isViber ? 'Есть' : 'Отсутствует';
                isWhatsApp.innerHTML = result[i].isWhatsApp ? 'Есть' : 'Отсутствует';

                // tr.appendChild(id);
                tr.appendChild(phone);
                tr.appendChild(isTelegram);
                tr.appendChild(isViber);
                tr.appendChild(isWhatsApp);
                document.getElementById('table').appendChild(tr);

              }



              var text = JSON.stringify(response);

              var a = document.createElement("a");
              var file = new Blob([text], { type: 'text/plain' });
              a.href = URL.createObjectURL(file);
              this.downloadFile = true;
              this.savePath = a.href;

              this.exportCSVFile(result);
            }
          }
        });
    }
  }

  public checkNumber() {
    var self = this;
    if (this.numberToCheck != null && this.numberToCheck.length > 0) {
      this.httpClient.post(this.baseUrl + 'Check',
        "[   {\"id\": 559,\"phone\": \"" + this.numberToCheck + "\"}]",
        { headers: { 'Content-Type': 'application/json' } }
      ).subscribe(function (response) {

        var searchResult: any[];
        var result = JSON.parse(JSON.stringify(response));

        self.move();
        if (result != null) {
          var resultCode = response['resultCode'];
          var isException = response['isException'];
          var messageText = response['messageText'];
          if (resultCode !== null && (resultCode === 401 || resultCode === 402 || resultCode === 403)) {
            alert("Служба TelegramService отключена, пожалуйста проверьте настройки и авторизируйтесь");
            window.location.href = document.baseURI + '/settingspage';
          }
          if (result.length > 0) {
            this.checkedPhone = [
              {
                id: result[0].id,
                phone: result[0].phone,
                isTelegram: result[0].isTelegram,
                isViber: result[0].isViber,
                isWhatsApp: result[0].isWhatsApp
              }
            ];
            var tr = document.createElement("tr");
            tr.id = "0";
            //var id = document.createElement("td");
            var phone = document.createElement("td");
            var isTelegram = document.createElement("td");
            var isViber = document.createElement("td");
            var isWhatsApp = document.createElement("td");

            // id.innerHTML = result[0].id;
            phone.innerHTML = result[0].phone;
            isTelegram.innerHTML = result[0].isTelegram ?
              'Есть <a href="tg://resolve?domain=' + result[0].telegramUser + ' "> <img src="assets/img/telegram.png" width="30px" height="30px"></a>' : 'Отсутствует';
            isViber.innerHTML = result[0].isViber ? 'Есть' : 'Отсутствует';
            isWhatsApp.innerHTML = result[0].isWhatsApp ? 'Есть' : 'Отсутствует';

            // tr.appendChild(id);
            tr.appendChild(phone);
            tr.appendChild(isTelegram);
            tr.appendChild(isViber);
            tr.appendChild(isWhatsApp);

            if (document.getElementById('table') === null) {
              var table = document.createElement('table');
              table.className = 'table table-striped';
              table.id = 'table';
              table.style.color = 'white;';

              var header = document.createElement("tr");
              var PhoneNumber = document.createElement("th");
              PhoneNumber.innerHTML = 'PhoneNumber';

              var Telegram = document.createElement("th");

              Telegram.innerHTML = 'Telegram ';

              var Viber = document.createElement("th");
              Viber.innerHTML = 'Viber';

              var WhatsApp = document.createElement("th");
              WhatsApp.innerHTML = 'WhatsApp';

              header.appendChild(PhoneNumber);
              header.appendChild(Telegram);
              header.appendChild(Viber);
              header.appendChild(WhatsApp);
              table.appendChild(header);
              this.resultTableCreated = true;
              var div = document.createElement('div');
              div.className = 'token';
              div.appendChild(table);
              document.getElementById('tableplace').appendChild(div);
            }
            document.getElementById('table').appendChild(tr);
          }
        }


        // parse result
      });
    }
  }

  public move() {
    this.progress = 1;
    var elem = document.getElementById("myBar");
    var width = 0;
    elem.style.width = '0%';
    elem.style.visibility = 'visible';
    var id = setInterval(frame, 10);
    function frame() {
      if (width >= 100) {
        clearInterval(id);
        this.progress = 0;
      } else {
        width++;
        elem.style.width = width + "%";
        elem.innerHTML = width + "%";
      }
    }

  }

  public convertToCSV(objArray) {
    var array = typeof objArray != 'object' ? JSON.parse(objArray) : objArray;
    var str = '';

    for (var i = 0; i < array.length; i++) {
      var line = '';
      for (var index in array[i]) {
        if (line != '') line += ';'

        line += array[i][index];
      }

      str += line + '\r\n';
    }

    return str;
  }

  public exportCSVFile(items) {
    var fileTitle = 'result'; // or 'my-unique-title'
    var headers = {
      id: 'Temp id'.replace(/,/g, ''), // remove commas to avoid errors
      phone: 'Phone number'.replace(/,/g, ''), // remove commas to avoid errors
      isTelegram: "Telegram",
      telegramUser: "Telegram username",
    };
    var itemsFormatted = [];
    items.forEach((item) => {
      itemsFormatted.push({
        id: item.id.toString(),
        phone: item.phone.replace(/,/g, ''), // remove commas to avoid errors,
        isTelegram: item.isTelegram,
        telegramUser: item.telegramUser
      });
    });
    if (headers) {
      itemsFormatted.unshift(headers);
    }


    // format the data
    
    // Convert Object to JSON
    var jsonObject = JSON.stringify(itemsFormatted);

    var csv = this.convertToCSV(jsonObject);
    this.resultToFile = csv;

  }
}

interface ServicesStatus {
  TelegramServicesWorking: boolean;
  AutoRunner: boolean;
  WebServiceConnection: boolean;
}

interface IAuthResponce {
  ResultCode: number,
  IsException: boolean,
  MessageText: string;
}

interface ILead {
  id: number;
  phone: string;
  isTelegram: boolean;
  telegramUser: string;
  isViber: boolean;
  isWhatsApp: boolean;
}
